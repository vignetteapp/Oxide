using System.Runtime.InteropServices;

namespace Mikomi.Graphics.Drivers.Buffers
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RenderBuffer
    {
        public uint TextureId;
        public uint Width;
        public uint Height;

        [MarshalAs(UnmanagedType.I1)]
        public bool HasStencilBuffer;

        [MarshalAs(UnmanagedType.I1)]
        public bool HasDepthBuffer;
    }
}
