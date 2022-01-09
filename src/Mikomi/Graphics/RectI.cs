using System.Runtime.InteropServices;

namespace Mikomi.Graphics.Fonts
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RectI
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
}
