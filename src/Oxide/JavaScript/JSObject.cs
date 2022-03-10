// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using Oxide.Javascript.Interop;

namespace Oxide.Javascript
{
    public unsafe class JSObject : DynamicObject, IDisposable
    {
        internal IntPtr Handle
        {
            get
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().Name);

                if (Value == IntPtr.Zero)
                    throw new InvalidOperationException(@"This object has yet to be initialized.");

                lock (sync)
                    return Value;
            }
        }

        /// <summary>
        /// Gets whether this Javascript object is a wrapped .NET Object.
        /// </summary>
        public bool IsHostObject => JSCore.JSValueIsObjectOfClass(Context.Handle, Handle, HostObjectProxy.Handle);

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

        /// <summary>
        /// Gets this Javascript object's array length.
        /// </summary>
        public int Length
        {
            get
            {
                if (!IsArray)
                    throw new InvalidOperationException(@"Object is not an array.");


                int value = (int)JSCore.JSObjectGetTypedArrayLength(Context.Handle, Handle, out var error);
                throwOnError(@"An error has occured.", error);

                return value;
            }
        }

        /// <summary>
        /// Gets this Javascript object's array byte length.
        /// </summary>
        public int ByteLength
        {
            get
            {
                if (!IsArray && ArrayType == JSTypedArrayType.None)
                    throw new InvalidOperationException(@"Object is not an array.");


                int value = (int)JSCore.JSObjectGetTypedArrayByteLength(Context.Handle, Handle, out var error);
                throwOnError(@"An error has occured.", error);

                return value;
            }
        }

        /// <summary>
        /// Gets this Javascript object's array byte offset.
        /// </summary>
        public int ByteOffset
        {
            get
            {
                if (!IsArray && ArrayType == JSTypedArrayType.None)
                    throw new InvalidOperationException(@"Object is not an array.");


                int value = (int)JSCore.JSObjectGetTypedArrayByteOffset(Context.Handle, Handle, out var error);
                throwOnError(@"An error has occured.", error);

                return value;
            }
        }

        internal readonly JSContext Context;
        internal readonly JSValue Value;
        private readonly bool protect;
        private bool isDisposed;
        private readonly object sync = new object();

        internal JSObject(JSValue value, bool protect = true)
        {
            Value = value;
            Context = value.Context;
            this.protect = protect;

            if (protect)
                JSCore.JSValueProtect(Context.Handle, Handle);
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            result = Undefined.Value;

            if (indexes.Length > 1)
                return false;

            IntPtr error = IntPtr.Zero;
            JSValue value = default;

            if (IsArray && uint.TryParse(indexes[0].ToString(), out uint num))
            {
                value = new JSValue(Context.Handle, JSCore.JSObjectGetPropertyAtIndex(Context.Handle, Handle, num, out error));
            }

            string prop = indexes[0].ToString();

            if (JSCore.JSObjectHasProperty(Context.Handle, Handle, prop))
            {
                value = new JSValue(Context.Handle, JSCore.JSObjectGetProperty(Context.Handle, Handle, prop, out error));
            }

            result = value.GetValue();

            throwOnError(@"An error has occured while trying to get an object's property.", error);

            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (JSCore.JSObjectHasProperty(Context.Handle, Handle, binder.Name))
            {
                result = new JSValue(Context.Handle, JSCore.JSObjectGetProperty(Context.Handle, Handle, binder.Name, out var error)).GetValue();
                throwOnError(@"An error has occured while trying to get an object's property.", error);
            }
            else
            {
                result = Undefined.Value;
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
                JSCore.JSObjectSetPropertyAtIndex(Context.Handle, Handle, num, JSValue.From(Context.Handle, value), out error);
            }
            else
            {
                JSCore.JSObjectSetProperty(Context.Handle, Handle, (string)indexes[0], JSValue.From(Context.Handle, value));
            }

            throwOnError(@"An error has occured while setting the object's value.", error);

            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
            => JSCore.JSObjectSetProperty(Context.Handle, Handle, binder.Name, JSValue.From(Context.Handle, value));

        public override bool TryDeleteMember(DeleteMemberBinder binder)
        {
            bool result = JSCore.JSObjectDeleteProperty(Context.Handle, Handle, binder.Name, out var error);
            throwOnError(@"Failed to delete member.", error);
            return result;
        }

        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            if (!IsFunction)
                return base.TryInvoke(binder, args, out result);

            fixed (void* arr = args.Select(v => (IntPtr)JSValue.From(Context.Handle, v)).ToArray())
            {
                result = new JSValue(Context.Handle,
                    JSCore.JSObjectCallAsFunction(
                        Context.Handle, Handle, Handle, (uint)binder.CallInfo.ArgumentCount, (IntPtr)arr, out var error
                    )
                ).GetValue();

                throwOnError(@"An error has occured while trying to invoke the function.", error);
            }

            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            if (!IsFunction)
                return base.TryInvokeMember(binder, args, out result);

            var function = JSCore.JSObjectGetProperty(Context.Handle, Handle, binder.Name, out var error);

            fixed (void* arr = new Span<IntPtr>(args.Select(v => (IntPtr)JSValue.From(Context.Handle, v)).ToArray()))
            {
                result = new JSValue(Context.Handle,
                    JSCore.JSObjectCallAsFunction(
                        Context.Handle, function, Handle, (uint)binder.CallInfo.ArgumentCount, (IntPtr)arr, out error
                    )
                ).GetValue();

                throwOnError(@"An error has occured while trying to invoke the function.", error);
            }

            return true;
        }

        public object GetHostObject()
        {
            if (!IsHostObject)
                return null;

            return (GCHandle.FromIntPtr(JSCore.JSObjectGetPrivate(Handle)).Target as HostObject).Target;
        }

        private void throwOnError(string reason, IntPtr error)
        {
            if (error != IntPtr.Zero)
                throw new JavascriptException(reason, new JSValue(Context.Handle, error));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed)
                return;

            if (!Context.IsDisposed && protect)
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
