using System.Runtime.InteropServices;

namespace Mikomi.Graphics.Drivers
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct GPUState
    {
        public uint ViewportWidth;
        public uint ViewportHeight;
        public Matrix4x4 Transform;
        public bool EnableTexturing;
        public bool EnableBlend;
        public byte ShaderType;
        public uint RenderBufferId;
        public uint Texture1Id;
        public uint Texture2Id;
        public uint Texture3Id;
        public float[] UniformScalar;
        public Vector4[] UniformVector;
        public byte ClipSize;
        public Matrix4x4[] Clip;
        public bool EnableScissor;
        public RectI ScissorRect;
    }
}
