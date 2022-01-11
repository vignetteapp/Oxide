using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Mikomi.Input;

namespace Mikomi.Apps
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

        internal Window(Monitor monitor, int width, int height, bool isFullScreen, WindowFlags flags)
            : base(AppCore.ulCreateWindow(monitor.Handle, (uint)width, (uint)height, isFullScreen, flags))
        {
            Monitor = monitor;
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
        public Overlay CreateOverlay(ViewConfig config, int width, int height, int x, int y)
            => new Overlay(this, new View(Monitor.App.Renderer, width, height, config, Monitor.App.Renderer.Session), x, y);

        /// <summary>
        /// Creates a new overlay from this window inheriting its size.
        /// </summary>
        public Overlay CreateOverlay(ViewConfig config)
            => CreateOverlay(config, Width, Height, 0, 0);

        protected override void DisposeUnmanaged()
            => AppCore.ulDestroyWindow(Handle);
    }

    internal delegate void CloseCallback(IntPtr userData, IntPtr window);
    internal delegate void ResizeCallback(IntPtr userData, IntPtr window, uint width, uint height);

#pragma warning disable CA2101

    public partial class AppCore
    {
        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern IntPtr ulCreateWindow(IntPtr monitor, uint width, uint height, [MarshalAs(UnmanagedType.I1)] bool fullscreen, WindowFlags windowFlags);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern IntPtr ulDestroyWindow(IntPtr window);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern void ulWindowSetCloseCallback(IntPtr window, CloseCallback callback, IntPtr userData);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern void ulWindowSetResizeCallback(IntPtr window, ResizeCallback callback, IntPtr userData);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern uint ulWindowGetScreenWidth(IntPtr window);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern uint ulWindowGetWidth(IntPtr window);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern uint ulWindowGetScreenHeight(IntPtr window);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern uint ulWindowGetHeight(IntPtr window);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern void ulWindowMoveTo(IntPtr window, int x, int y);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern void ulWindowMoveToCenter(IntPtr window);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern int ulWindowGetPositionX(IntPtr window);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern int ulWindowGetPositionY(IntPtr window);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool ulWindowIsFullScreen(IntPtr window);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern double ulWindowGetScale(IntPtr window);

        [DllImport(LIB_APPCORE, ExactSpelling = true, CharSet = CharSet.Ansi, ThrowOnUnmappableChar = true, BestFitMapping = true)]
        internal static extern void ulWindowSetTitle(IntPtr window, [MarshalAs(UnmanagedType.LPStr)] string title);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern void ulWindowSetCursor(IntPtr window, CursorType cursor);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern void ulWindowShow(IntPtr window);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern void ulWindowHide(IntPtr window);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool ulWindowIsVisible(IntPtr window);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern void ulWindowClose(IntPtr window);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern int ulWindowScreenToPixels(IntPtr window, int val);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern int ulWindowPixelsToScreen(IntPtr window, int val);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern IntPtr ulWindowGetNativeHandle(IntPtr window);
    }

#pragma warning restore CA2101

}
