// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Oxide.Javascript.Interop;
using UnmanageUtility;

namespace Oxide.Javascript.Objects
{
    public class JSObject : DynamicObject, IDisposable
    {
        internal IntPtr Handle
        {
            get
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().Name);

                if (handle == IntPtr.Zero)
                    throw new InvalidOperationException(@"This object has yet to be initialized.");

                return handle;
            }
        }

        /// <summary>
        /// Gets whether this Javascript object is a wrapped .NET Object.
        /// </summary>
        public bool IsHostObject => JSCore.JSValueIsObjectOfClass(Context, Handle, HostObjectProxy.Handle);

        /// <summary>
        /// Gets whether this Javascript object is an array.
        /// </summary>
        public bool IsArray => JSCore.JSValueIsArray(Context, Handle);

        /// <summary>
        /// Gets whther this Javascript object is a function.
        /// </summary>
        public bool IsFunction => JSCore.JSObjectIsFunction(Context, Handle);

        /// <summary>
        /// Gets whether this Javascript object is a constructor.
        /// </summary>
        public bool IsConstructor => JSCore.JSObjectIsConstructor(Context, Handle);

        /// <summary>
        /// Gets this Javascript object's array type.
        /// </summary>
        public JSTypedArrayType ArrayType => JSCore.JSValueGetTypedArrayType(Context, Handle, out _);

        protected readonly IntPtr Context;
        private volatile int disposeSignal = 0;
        private readonly bool protect;
        private readonly IntPtr handle;
        private bool isDisposed;

        internal JSObject(IntPtr context, IntPtr handle, bool protect = true)
        {
            Context = context;
            this.handle = handle;
            this.protect = protect;

            if (this.protect)
                JSCore.JSValueProtect(context, Handle);
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            var members = new List<string>();
            var accumulator = JSCore.JSObjectCopyPropertyNames(Context, Handle);
            uint length = JSCore.JSPropertyNameArrayGetCount(accumulator);

            for (uint i = 0; i < length; i++)
            {
                string item = JSCore.JSPropertyNameArrayGetNameAtIndex(accumulator, i);
                members.Add(item);
            }

            JSCore.JSPropertyNameArrayRelease(accumulator);

            return members;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            result = Undefined.Value;

            if (indexes.Length > 1 && indexes[0] is not string)
                return false;

            if (!JSCore.JSObjectHasProperty(Context, Handle, (string)indexes[0]))
                return false;

            IntPtr error;
            IntPtr value;

            if (uint.TryParse((string)indexes[0], out uint num))
            {
                value = JSCore.JSObjectGetPropertyAtIndex(Context, Handle, num, out error);
            }
            else
            {
                value = JSCore.JSObjectGetProperty(Context, Handle, (string)indexes[0], out error);
            }

            result = JSValueRefConverter.ConvertJSValue(Context, value);

            throwOnError(error);

            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (JSCore.JSObjectHasProperty(Context, Handle, binder.Name))
            {
                var value = JSCore.JSObjectGetProperty(Context, Handle, binder.Name, out var error);
                result = JSValueRefConverter.ConvertJSValue(Context, value);
                throwOnError(error);
            }
            else
            {
                result = JSValueRefConverter.ConvertHostObject(Context, Undefined.Value);
            }

            return true;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            if (indexes.Length > 1 && indexes[0] is not string)
                return false;

            var error = IntPtr.Zero;

            if (uint.TryParse((string)indexes[0], out uint num))
            {
                JSCore.JSObjectSetPropertyAtIndex(Context, Handle, num, JSValueRefConverter.ConvertHostObject(Context, value), out error);
            }
            else
            {
                JSCore.JSObjectSetProperty(Context, Handle, (string)indexes[0], JSValueRefConverter.ConvertHostObject(Context, value));
            }

            throwOnError(error);

            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
            => JSCore.JSObjectSetProperty(Context, Handle, binder.Name, JSValueRefConverter.ConvertHostObject(Context, value));

        public override bool TryDeleteMember(DeleteMemberBinder binder)
        {
            bool result = JSCore.JSObjectDeleteProperty(Context, Handle, binder.Name, out var error);
            throwOnError(error);
            return result;
        }

        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            using (var arr = new UnmanagedArray<IntPtr>(new Span<IntPtr>(args.Select(v => JSValueRefConverter.ConvertHostObject(Context, v)).ToArray())))
            {
                result = JSValueRefConverter.ConvertJSValue(Context,
                    JSCore.JSObjectCallAsFunction(
                        Context, Handle, Handle, (uint)args.Length, arr.Ptr, out var error
                    )
                );

                throwOnError(error);
            }

            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            var function = JSCore.JSObjectGetProperty(Context, Handle, binder.Name, out var error);

            using (var arr = new UnmanagedArray<IntPtr>(new Span<IntPtr>(args.Select(v => JSValueRefConverter.ConvertHostObject(Context, v)).ToArray())))
            {
                result = JSValueRefConverter.ConvertJSValue(Context,
                    JSCore.JSObjectCallAsFunction(
                        Context, function, Handle, (uint)args.Length, arr.Ptr, out error
                    )
                );

                throwOnError(error);
            }

            return true;
        }

        public object GetHostObject()
        {
            if (!IsHostObject)
                return null;

            return (GCHandle.FromIntPtr(JSCore.JSObjectGetPrivate(Handle)).Target as HostObject).Target;
        }

        private void throwOnError(IntPtr error)
        {
            if (error != IntPtr.Zero)
                throw JavascriptException.GetException(Context, error);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed || Interlocked.Exchange(ref disposeSignal, 1) != 0)
                return;

            if (protect)
                JSCore.JSValueUnprotect(Context, Handle);

            isDisposed = true;
        }

        ~JSObject()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
