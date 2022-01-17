// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System.Numerics;

namespace Oxide.Input
{
    public class ScrollEvent : DisposableObject
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
}
