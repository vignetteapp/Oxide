using System.Runtime.InteropServices;

namespace Oxide.Graphics.Drivers
{
    [StructLayout(LayoutKind.Sequential)]
    public struct GPUState
    {
        public uint ViewportWidth;
        public uint ViewportHeight;
        public Matrix4x4 Transform;
        public bool EnableTexturing;
        public bool EnableBlend;

        [MarshalAs(UnmanagedType.U1)]
        public ShaderType ShaderType;

        public uint RenderBufferId;
        public uint Texture1Id;
        public uint Texture2Id;
        public uint Texture3Id;

        [MarshalAs(UnmanagedType.LPArray, SizeConst = 8)]
        public float[] UniformScalar;

        [MarshalAs(UnmanagedType.LPArray, SizeConst = 8)]
        public Vector4[] UniformVector;

        public byte ClipSize;

        [MarshalAs(UnmanagedType.LPArray, SizeConst = 8)]
        public Matrix4x4[] Clip;

        public bool EnableScissor;
        public RectI ScissorRect;
    }
}
