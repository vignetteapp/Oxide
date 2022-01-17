using System.Numerics;

namespace Oxide.Apps
{
    public sealed class Overlay : DisposableObject
    {
        /// <summary>
        /// The window that instantiated this overlay.
        /// </summary>
        public readonly Window Window;

        /// <summary>
        /// The view it owns.
        /// </summary>
        public readonly View View;

        /// <summary>
        /// The overlay size (in pixels).
        /// </summary>
        public Vector2 Size
        {
            get => new Vector2(Width, Height);
            set => AppCore.ulOverlayResize(Handle, (uint)value.X, (uint)value.Y);
        }

        /// <summary>
        /// The overlay width (in pixels).
        /// </summary>
        public int Width
        {
            get => (int)AppCore.ulOverlayGetWidth(Handle);
            set => AppCore.ulOverlayResize(Handle, (uint)value, (uint)Height);
        }

        /// <summary>
        /// The overlay height (in pixels).
        /// </summary>
        public int Height
        {
            get => (int)AppCore.ulOverlayGetHeight(Handle);
            set => AppCore.ulOverlayResize(Handle, (uint)Width, (uint)value);
        }

        /// <summary>
        /// Gets the position of this overlay relative the top-left corner of the window.
        /// <br/>
        /// Setting this value will offset the overlay relative to the window.
        /// </summary>
        public Vector2 Position
        {
            get => new Vector2(X, Y);
            set => AppCore.ulOverlayMoveTo(Handle, (int)value.X, (int)value.Y);
        }

        /// <summary>
        /// Gets the x-position of this overlay relative the top-left corner of the window.
        /// <br/>
        /// Setting this value will offset the overlay relative to the window.
        /// </summary>
        public int X
        {
            get => AppCore.ulOverlayGetX(Handle);
            set => AppCore.ulOverlayMoveTo(Handle, value, Y);
        }

        /// <summary>
        /// Gets the y-position of this overlay relative the top-left corner of the window.
        /// <br/>
        /// Setting this value will offset the overlay relative to the window.
        /// </summary>
        public int Y
        {
            get => AppCore.ulOverlayGetY(Handle);
            set => AppCore.ulOverlayMoveTo(Handle, X, value);
        }

        /// <summary>
        /// Whether this overlay is hidden or not.
        /// </summary>
        public bool IsHidden => AppCore.ulOverlayIsHidden(Handle);

        /// <summary>
        /// Whether this overlay has focus or not.
        /// </summary>
        public bool HasFocus => AppCore.ulOverlayHasFocus(Handle);

        internal Overlay(Window window, int width, int height, int x, int y)
            : base(AppCore.ulCreateOverlay(window.Handle, (uint)width, (uint)height, x, y))
        {
            View = new View(AppCore.ulOverlayGetView(Handle));
            Window = window;
        }

        internal Overlay(Window window, View view, int x, int y)
            : base(AppCore.ulCreateOverlayWithView(window.Handle, view.Handle, x, y))
        {
            View = view;
            Window = window;
        }

        /// <summary>
        /// Show the overlay.
        /// </summary>
        public void Show() => AppCore.ulOverlayShow(Handle);

        /// <summary>
        /// Hide the overlay (will no longer be drawn).
        /// </summary>
        public void Hide() => AppCore.ulOverlayHide(Handle);

        /// <summary>
        /// Grant this overlay exclusive keyboard focus.
        /// </summary>
        public void Focus() => AppCore.ulOverlayFocus(Handle);

        /// <summary>
        /// Remove keyboard focus.
        /// </summary>
        public void Unfocus() => AppCore.ulOverlayUnfocus(Handle);

        protected override void DisposeUnmanaged()
            => AppCore.ulDestroyOverlay(Handle);
    }
}
