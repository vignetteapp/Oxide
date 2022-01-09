using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Mikomi.Input;

namespace Mikomi.Input
{
    public class MouseEvent : ManagedObject
    {
        public readonly MouseEventType Type;
        public readonly MouseButton Button;
        public readonly Vector2 Position;

        /// <summary>
        /// Creates a mouse event.
        /// </summary>
        public MouseEvent(MouseEventType type, MouseButton button, Vector2 position)
            : this(type, button, (int)position.X, (int)position.Y)
        {
        }

        /// <summary>
        /// Creates a mouse event.
        /// </summary>
        public MouseEvent(MouseEventType type, MouseButton button, int x, int y)
            : base(Ultralight.ulCreateMouseEvent(type, x, y, button))
        {
            Type = type;
            Button = button;
            Position = new Vector2(x, y);
        }

        protected override void DisposeUnmanaged()
            => Ultralight.ulDestroyMouseEvent(Handle);
    }

    public enum MouseEventType
    {
        MouseMoved,
        MouseDown,
        MouseUp,
    }

    public enum MouseButton
    {
        None,
        Left,
        Middle,
        Right,
    }
}

namespace Mikomi
{
    public partial class Ultralight
    {
        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern IntPtr ulCreateMouseEvent(MouseEventType type, int x, int y, MouseButton button);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulDestroyMouseEvent(IntPtr evt);
    }
}
