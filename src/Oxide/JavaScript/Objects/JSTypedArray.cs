using System;
using System.Runtime.InteropServices;

namespace Oxide.JavaScript.Objects
{
    public class JSTypedArray : JSObject
    {
        public int Length => (int)JSCore.JSObjectGetTyepdArrayLength(Context.Handle, Handle, IntPtr.Zero);
        public int ByteLength => (int)JSCore.JSObjectGetTypedArrayByteLength(Context.Handle, Handle, IntPtr.Zero);
        public int ByteOffset => (int)JSCore.JSObjectGetTypedArrayByteOffset(Context.Handle, Handle, IntPtr.Zero);

        internal JSTypedArray(JSContext ctx, IntPtr handle)
            : base(ctx, handle)
        {
        }

        internal JSTypedArray(JSContext ctx, JSTypedArrayType arrayType, uint length)
            : base(ctx, JSCore.JSObjectMakeTypedArray(ctx.Handle, arrayType, length, IntPtr.Zero))
        {
        }
    }
}

namespace Oxide.JavaScript
{
    public partial class JSCore
    {
        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectMakeTypedArray(IntPtr ctx, JSTypedArrayType arrayType, uint length, IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectMakeTypedArrayWithBytesNoCopy(IntPtr ctx, JSTypedArrayType arrayType, IntPtr bytes, uint byteLength, IntPtr bytesDeallocator, IntPtr deallocatorContext, IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectMakeTypedArrayWithArrayBufferAndOffset(IntPtr ctx, JSTypedArrayType arrayType, IntPtr buffer, uint byteOffset, uint length, IntPtr excetpion);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectGetTypedArrayBytesPtr(IntPtr ctx, IntPtr obj, IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern uint JSObjectGetTyepdArrayLength(IntPtr ctx, IntPtr obj, IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern uint JSObjectGetTypedArrayByteLength(IntPtr ctx, IntPtr obj, IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern uint JSObjectGetTypedArrayByteOffset(IntPtr ctx, IntPtr obj, IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectGetTypedArrayBuffer(IntPtr ctx, IntPtr obj, IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectMakeArrayBufferWithBytesNoCopy(IntPtr ctx, IntPtr bytes, uint byteLength, IntPtr bytesDeallocator, IntPtr deallocatorContext, IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectGetArrayBufferBytesPtr(IntPtr ctx, IntPtr obj, IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern uint JSObjectGetArrayBufferByteLength(IntPtr ctx, IntPtr obj, IntPtr exception);
    }
}
