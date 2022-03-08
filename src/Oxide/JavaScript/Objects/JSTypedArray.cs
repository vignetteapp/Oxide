// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System;
using Oxide.Javascript.Interop;

namespace Oxide.Javascript.Objects
{
    public class JSTypedArray : JSObject
    {
        public int Length => (int)JSCore.JSObjectGetTypedArrayLength(Context, Handle, out _);
        public int ByteLength => (int)JSCore.JSObjectGetTypedArrayByteLength(Context, Handle, out _);
        public int ByteOffset => (int)JSCore.JSObjectGetTypedArrayByteOffset(Context, Handle, out _);

        internal JSTypedArray(IntPtr context, IntPtr handle)
            : base(context, handle)
        {
        }

        internal JSTypedArray(IntPtr context, JSTypedArrayType arrayType, uint length)
            : base(context, makeTypedArray(context, arrayType, length))
        {
        }

        private static IntPtr makeTypedArray(IntPtr context, JSTypedArrayType arrayType, uint length)
        {
            var value = JSCore.JSObjectMakeTypedArray(context, arrayType, length, out var error);

            if (error != IntPtr.Zero)
            {
                dynamic errorObj = JSValueRefConverter.ConvertJSValue(context, error);
                throw new Exception($"[{errorObj.name}]: {errorObj.message}");
            }

            return value;
        }
    }
}
