using System.Runtime.InteropServices;
using Mikomi.Graphics;

namespace Mikomi.Graphics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        public float Left;
        public float Top;
        public float Right;
        public float Bottom;
    }

    public static class RectExtensions
    {
        public static bool IsEmpty(this Rect rect)
            => Ultralight.ulRectIsEmpty(rect);
    }
}

namespace Mikomi
{
    public partial class Ultralight
    {
        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool ulRectIsEmpty(Rect rect);
    }
}
