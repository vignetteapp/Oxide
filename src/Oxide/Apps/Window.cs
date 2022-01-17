// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System;
using System.Numerics;
using Oxide.Input;

namespace Oxide.Apps
{
    public sealed class Window : DisposableObject
    {
        /// <summary>
        /// The monitor that instantiated this window.
        /// </summary>
        public readonly Monitor Monitor;

        /// <summary>
        /// The window size (in screen coordinates).
        /// </summary>
        public Vector2 ScreenSize => new Vector2(ScreenWidth, ScreenHeight);

        /// <summary>
        /// The window width (in screen coordinates).
        /// </summary>
        public int ScreenWidth => (int)AppCore.ulWindowGetScreenWidth(Handle);

        /// <summary>
        /// The window height (in screen coordinates).
        /// </summary>
        public int ScreenHeight => (int)AppCore.ulWindowGetScreenHeight(Handle);

        /// <summary>
        /// The window size (in pixels).
        /// </summary>
        public Vector2 Size => new Vector2(Width, Height);

        /// <summary>
        /// The window width (in pixels).
        /// </summary>
        public int Width => (int)AppCore.ulWindowGetWidth(Handle);

        /// <summary>
        /// The window height (in pixels).
        /// </summary>
        public int Height => (int)AppCore.ulWindowGetHeight(Handle);

        /// <summary>
        /// The window position (in screen coordinates).
        /// <br/>
        /// Setting this value will move the window to a new position relative
        /// to the top-left of the monitor area.
        /// </summary>
        public Vector2 Position
        {
            get => new Vector2(X, Y);
            set => AppCore.ulWindowMoveTo(Handle, (int)value.X, (int)value.Y);
        }

        /// <summary>
        /// The window x-position (in screen coordinates).
        /// <br/>
        /// Setting this value will move the window to a new position relative
        /// to the top-left of the monitor area.
        /// </summary>
        public int X
        {
            get => AppCore.ulWindowGetPositionX(Handle);
            set => AppCore.ulWindowMoveTo(Handle, value, Y);
        }

        /// <summary>
        /// The window y-position (in screen coordinates).
        /// <br/>
        /// Setting this value will move the window to a new position relative
        /// to the top-left of the monitor area.
        /// </summary>
        public int Y
        {
            get => AppCore.ulWindowGetPositionY(Handle);
            set => AppCore.ulWindowMoveTo(Handle, X, value);
        }

        /// <summary>
        /// The DPI scale of a window.
        /// </summary>
        public double Scale => AppCore.ulWindowGetScale(Handle);

        /// <summary>
        /// Whether this window is full screen or not.
        /// </summary>
        public bool IsFullScreen => AppCore.ulWindowIsFullScreen(Handle);

        /// <summary>
        /// Whether this window is visible or not.
        /// </summary>
        public bool IsVisible => AppCore.ulWindowIsVisible(Handle);

        /// <summary>
        /// The underlying native window handle.
        /// <br/>
        /// This is:
        /// <br/>
        /// - HWND on Windows
        /// <br/>
        /// - NSWindow on macOS
        /// <br/>
        /// - GLFWwindow on Linux
        /// </summary>
        public IntPtr WindowHandle => AppCore.ulWindowGetNativeHandle(Handle);

        private string title;

        /// <summary>
        /// Gets or sets the window title.
        /// </summary>
        public string Title
        {
            get => title;
            set => AppCore.ulWindowSetTitle(Handle, title = value);
        }

        /// <summary>
        /// Invoked when the window is closing.
        /// </summary>
        public event EventHandler OnClose;

        /// <summary>
        /// Invoked when the window is resizing.
        /// </summary>
        public event EventHandler<ResizeEventArgs> OnResize;

        private ResizeCallback resize;
        private CloseCallback close;

        internal Window(Monitor monitor, int width, int height, bool isFullScreen, WindowFlags flags)
            : base(AppCore.ulCreateWindow(monitor.Handle, (uint)width, (uint)height, isFullScreen, flags))
        {
            Monitor = monitor;
            AppCore.ulWindowSetCloseCallback(Handle, close += handleClose, IntPtr.Zero);
            AppCore.ulWindowSetResizeCallback(Handle, resize += handleResize, IntPtr.Zero);
        }

        private void handleClose(IntPtr data, IntPtr window)
        {
            OnClose?.Invoke(this, EventArgs.Empty);
        }

        private void handleResize(IntPtr data, IntPtr window, uint width, uint height)
        {
            OnResize?.Invoke(this, new ResizeEventArgs(width, height));
        }

        /// <summary>
        /// Sets the cursor for this window.
        /// </summary>
        public void SetCursor(CursorType cursor) => AppCore.ulWindowSetCursor(Handle, cursor);

        /// <summary>
        /// Show this window.
        /// </summary>
        public void Show() => AppCore.ulWindowShow(Handle);

        /// <summary>
        /// Hide this window.
        /// </summary>
        public void Hide() => AppCore.ulWindowHide(Handle);

        /// <summary>
        /// Close this window.
        /// </summary>
        public void Close() => AppCore.ulWindowClose(Handle);

        /// <summary>
        /// Moves this window to the center of the screen.
        /// </summary>
        public void MoveToCenter() => AppCore.ulWindowMoveToCenter(Handle);

        /// <summary>
        /// Convert screen coordinates to pixels using the current DPI scale.
        /// </summary>
        public int ScreenToPixels(int val) => AppCore.ulWindowScreenToPixels(Handle, val);

        /// <summary>
        /// Convert pixels to screen coordinates using the current DPI scale.
        /// </summary>
        public int PixelsToScreen(int val) => AppCore.ulWindowPixelsToScreen(Handle, val);

        /// <summary>
        /// Creates a new overlay from this window.
        /// </summary>
        public Overlay CreateOverlay(int width, int height, int x, int y)
            => new Overlay(this, width, height, x, y);

        /// <summary>
        /// Creates a new overlay from this window inheriting its size.
        /// </summary>
        public Overlay CreateOverlay()
            => CreateOverlay(Width, Height, 0, 0);

        /// <summary>
        /// Creates a new overlay from this window with a given view.
        /// </summary>
        public Overlay CreateOverlay(View view)
            => new Overlay(this, view, 0, 0);

        protected override void DisposeManaged()
        {
            resize -= handleResize;
            close -= handleClose;
        }

        protected override void DisposeUnmanaged()
            => AppCore.ulDestroyWindow(Handle);
    }

    public class ResizeEventArgs : EventArgs
    {
        public int Width { get; }

        public int Height { get; }

        internal ResizeEventArgs(uint width, uint height)
        {
            Width = (int)width;
            Height = (int)height;
        }
    }

    internal delegate void CloseCallback(IntPtr userData, IntPtr window);
    internal delegate void ResizeCallback(IntPtr userData, IntPtr window, uint width, uint height);
}
