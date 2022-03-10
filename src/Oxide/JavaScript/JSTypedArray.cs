// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System;

namespace Oxide.Javascript
{
    public class JSTypedArray : JSObject
    {
        public int Length => (int)JSCore.JSObjectGetTypedArrayLength(Context.Handle, Handle, out _);
        public int ByteLength => (int)JSCore.JSObjectGetTypedArrayByteLength(Context.Handle, Handle, out _);
        public int ByteOffset => (int)JSCore.JSObjectGetTypedArrayByteOffset(Context.Handle, Handle, out _);

        internal JSTypedArray(JSContext context, IntPtr handle)
            : base(context, handle)
        {
        }

        internal JSTypedArray(JSContext context, JSTypedArrayType arrayType, uint length)
            : base(context, makeTypedArray(context, arrayType, length))
        {
        }

        private static IntPtr makeTypedArray(JSContext context, JSTypedArrayType arrayType, uint length)
        {
            var value = JSCore.JSObjectMakeTypedArray(context.Handle, arrayType, length, out var error);

            if (error != IntPtr.Zero)
                throw JavascriptException.Throw(context, error);

            return value;
        }
    }
}
