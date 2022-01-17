// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System.Numerics;

namespace Oxide.Input
{
    public class MouseEvent : DisposableObject
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
}
