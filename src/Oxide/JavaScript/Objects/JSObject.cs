// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
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
        public bool IsHostObject => JSCore.JSValueIsObjectOfClass(Context.Handle, Handle, Context.Proxy.Handle);

        /// <summary>
        /// Gets whether this Javascript object is an array.
        /// </summary>
        public bool IsArray => JSCore.JSValueIsArray(Context.Handle, Handle);

        /// <summary>
        /// Gets whther this Javascript object is a function.
        /// </summary>
        public bool IsFunction => JSCore.JSObjectIsFunction(Context.Handle, Handle);

        /// <summary>
        /// Gets whether this Javascript object is a constructor.
        /// </summary>
        public bool IsConstructor => JSCore.JSObjectIsConstructor(Context.Handle, Handle);

        /// <summary>
        /// Gets this Javascript object's array type.
        /// </summary>
        public JSTypedArrayType ArrayType => JSCore.JSValueGetTypedArrayType(Context.Handle, Handle, out _);

        protected readonly JSContext Context;
        private readonly IntPtr handle;
        private bool isDisposed;

        internal JSObject(JSContext context, IntPtr handle, bool protect = true)
        {
            Context = context;
            this.handle = handle;

            if (!protect)
                return;

            JSCore.JSValueProtect(context.Handle, Handle);
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            var members = new List<string>();
            var accumulator = JSCore.JSObjectCopyPropertyNames(Context.Handle, Handle);
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

            if (indexes.Length > 1)
                return false;

            if (!JSCore.JSObjectHasProperty(Context.Handle, Handle, indexes[0].ToString()))
                return false;

            IntPtr error;
            IntPtr value;

            if (uint.TryParse(indexes[0].ToString(), out uint num))
            {
                value = JSCore.JSObjectGetPropertyAtIndex(Context.Handle, Handle, num, out error);
            }
            else
            {
                value = JSCore.JSObjectGetProperty(Context.Handle, Handle, indexes[0].ToString(), out error);
            }

            result = Context.Converter.ConvertJSValue(value);

            throwOnError(error);

            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (JSCore.JSObjectHasProperty(Context.Handle, Handle, binder.Name))
            {
                var value = JSCore.JSObjectGetProperty(Context.Handle, Handle, binder.Name, out var error);
                result = Context.Converter.ConvertJSValue(value);
                throwOnError(error);
            }
            else
            {
                result = Context.Converter.ConvertHostObject(Undefined.Value);
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
                JSCore.JSObjectSetPropertyAtIndex(Context.Handle, Handle, num, Context.Converter.ConvertHostObject(value), out error);
            }
            else
            {
                JSCore.JSObjectSetProperty(Context.Handle, Handle, (string)indexes[0], Context.Converter.ConvertHostObject(value));
            }

            throwOnError(error);

            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
            => JSCore.JSObjectSetProperty(Context.Handle, Handle, binder.Name, Context.Converter.ConvertHostObject(value));

        public override bool TryDeleteMember(DeleteMemberBinder binder)
        {
            bool result = JSCore.JSObjectDeleteProperty(Context.Handle, Handle, binder.Name, out var error);
            throwOnError(error);
            return result;
        }

        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            using (var arr = new UnmanagedArray<IntPtr>(new Span<IntPtr>(args.Select(v => Context.Converter.ConvertHostObject(v)).ToArray())))
            {
                result = Context.Converter.ConvertJSValue(
                    JSCore.JSObjectCallAsFunction(
                        Context.Handle, Handle, Handle, (uint)args.Length, arr.Ptr, out var error
                    )
                );

                throwOnError(error);
            }

            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            var function = JSCore.JSObjectGetProperty(Context.Handle, Handle, binder.Name, out var error);

            using (var arr = new UnmanagedArray<IntPtr>(new Span<IntPtr>(args.Select(v => Context.Converter.ConvertHostObject(v)).ToArray())))
            {
                result = Context.Converter.ConvertJSValue(
                    JSCore.JSObjectCallAsFunction(
                        Context.Handle, function, Handle, (uint)args.Length, arr.Ptr, out error
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
                throw JavascriptException.Throw(Context, error);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed)
                return;

            if (!Context.IsDisposed)
                JSCore.JSValueUnprotect(Context.Handle, Handle);

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
