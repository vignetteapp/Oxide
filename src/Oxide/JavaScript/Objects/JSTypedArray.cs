using System;

namespace Oxide.JavaScript.Objects
{
    public class JSTypedArray : JSObject
    {
        public int Length => (int)JSCore.JSObjectGetTyepdArrayLength(Context.Handle, Handle, out _);

        public int ByteLength => (int)JSCore.JSObjectGetTypedArrayByteLength(Context.Handle, Handle, out _);

        public int ByteOffset => (int)JSCore.JSObjectGetTypedArrayByteOffset(Context.Handle, Handle, out _);

        internal JSTypedArray(JSContext ctx, IntPtr handle)
            : base(ctx, handle)
        {
        }

        internal JSTypedArray(JSContext ctx, JSTypedArrayType arrayType, uint length)
            : base(ctx, makeTypedArray(ctx, arrayType, length))
        {
        }

        private static IntPtr makeTypedArray(JSContext ctx, JSTypedArrayType arrayType, uint length)
        {
            var value = JSCore.JSObjectMakeTypedArray(ctx.Handle, arrayType, length, out var error);

            if (error != IntPtr.Zero)
            {
                dynamic errorObj = ctx.Converter.ConvertJSValue(error);
                throw new Exception($"[{errorObj.name}]: {errorObj.message}");
            }

            return value;
        }
    }
}
