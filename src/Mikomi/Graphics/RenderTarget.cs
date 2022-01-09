using System.Runtime.InteropServices;
using Mikomi.Graphics.Bitmaps;

namespace Mikomi.Graphics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RenderTarget
    {
        public bool IsEmpty;
        public uint Width;
        public uint Height;
        public uint TextureId;
        public uint TextureWidth;
        public uint TextureHeight;
        public BitmapFormat TextureFormat;
        public Rect UVCoords;
        public uint RenderBufferId;
    }
}
