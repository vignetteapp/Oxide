using System.Runtime.InteropServices;
using Oxide.Graphics;

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

namespace Oxide
{
    public partial class Ultralight
    {
        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool ulRectIIsEmpty(RectI rect);
    }
}
