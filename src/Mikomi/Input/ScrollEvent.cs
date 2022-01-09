using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Mikomi.Input;

namespace Mikomi.Input
{
    public class ScrollEvent : ManagedObject
    {
        public readonly ScrollEventType Type;
        public readonly Vector2 Delta;

        /// <summary>
        /// Creates a scroll event.
        /// </summary>
        public ScrollEvent(ScrollEventType type, Vector2 delta)
            : this(type, (int)delta.X, (int)delta.Y)
        {
        }

        /// <summary>
        /// Creates a scroll event.
        /// </summary>
        public ScrollEvent(ScrollEventType type, int xDelta, int yDelta)
            : base(Ultralight.ulCreateScrollEvent(type, xDelta, yDelta))
        {
            Type = type;
            Delta = new Vector2(xDelta, yDelta);
        }

        protected override void DisposeUnmanaged()
            => Ultralight.ulDestroyScrollEvent(Handle);
    }

    public enum ScrollEventType
    {
        ScrollByPixel,
        ScrollByPage,
    }
}

namespace Mikomi
{
    public partial class Ultralight
    {
        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern IntPtr ulCreateScrollEvent(ScrollEventType type, int xDelta, int yDelta);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulDestroyScrollEvent(IntPtr evt);
    }
}
