using System.Runtime.InteropServices;

namespace Oxide.Graphics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RectI
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    public static class RectIExtensions
    {
        public static bool IsEmpty(this RectI rect)
            => Ultralight.ulRectIIsEmpty(rect);
    }
}
