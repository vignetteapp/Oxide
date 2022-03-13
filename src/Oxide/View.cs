// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Oxide.Graphics;
using Oxide.Graphics.Drivers;
using Oxide.Input;
using Oxide.Interop;
using Oxide.Javascript;

namespace Oxide
{
    public class View : DisposableObject
    {
        /// <summary>
        /// Get current loaded URL. Set loads a new URL.
        /// </summary>
        public string URL
        {
            get => Ultralight.ulViewGetURL(Handle);
            set => Ultralight.ulViewLoadURL(Handle, value);
        }

        /// <summary>
        /// Get current title.
        /// </summary>
        public string Title => Ultralight.ulViewGetTitle(Handle);

        /// <summary>
        /// Get the width, in pixels. Set resizes the width.
        /// </summary>
        public int Width
        {
            get => (int)Ultralight.ulViewGetWidth(Handle);
            set => Ultralight.ulViewResize(Handle, (uint)value, (uint)Height);
        }

        /// <summary>
        /// Get the height, in pixels. Set resizes the height.
        /// </summary>
        public int Height
        {
            get => (int)Ultralight.ulViewGetHeight(Handle);
            set => Ultralight.ulViewResize(Handle, (uint)Width, (uint)value);
        }

        /// <summary>
        /// Gets the size, in pixels. Set resizes this <see cref="View"/>
        /// </summary>
        public Vector2 Size
        {
            get => new Vector2(Width, Height);
            set => Ultralight.ulViewResize(Handle, (uint)value.X, (uint)value.Y);
        }

        /// <summary>
        /// Check if main frame is loading.
        /// </summary>
        public bool IsLoading => Ultralight.ulViewIsLoading(Handle);

        /// <summary>
        /// Get the render target for this view.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when attempted to access when <see cref="ViewConfig"/>.IsAccelerated = false.
        /// </exception>
        public RenderTarget RenderTarget
        {
            get
            {
                if (!isAccelerated)
                    throw new InvalidOperationException($"{typeof(ViewConfig)} must have IsAccelerated = true.");

                return Ultralight.ulViewGetRenderTarget(Handle);
            }
        }

        /// <summary>
        /// Gets or sets whether or not a view should be painted during the next call to <see cref="Renderer.Render"/>.
        /// Setting this to true forces a repaint.
        /// </summary>
        public bool NeedsPaint
        {
            get => Ultralight.ulViewGetNeedsPaint(Handle);
            set => Ultralight.ulViewSetNeedsPaint(Handle, value);
        }

        /// <summary>
        /// Whether or not the View has focus.
        /// </summary>
        public bool HasFocus => Ultralight.ulViewHasFocus(Handle);

        /// <summary>
        /// Whether or not we can navigate forwards in history
        /// </summary>
        public bool CanGoForward => Ultralight.ulViewCanGoForward(Handle);

        /// <summary>
        /// Whether or not we can navigate backwards in history
        /// </summary>
        public bool CanGoBack => Ultralight.ulViewCanGoBack(Handle);

        /// <summary>
        /// Called whenever the <see cref="Title"/> is changed.
        /// </summary>
        public event EventHandler<ViewPropertyChangedEventArgs> OnTitleChanged;

        /// <summary>
        /// Called whenever the <see cref="URL"/> is changed.
        /// </summary>
        public event EventHandler<ViewPropertyChangedEventArgs> OnURLChanged;

        /// <summary>
        /// Called whenever the tooltip is changed.
        /// </summary>
        public event EventHandler<ViewPropertyChangedEventArgs> OnTooltipChanged;

        /// <summary>
        /// Called whenever the cursor is changed.
        /// </summary>
        public event EventHandler<ViewCursorChangedEventArgs> OnCursorChanged;

        /// <summary>
        /// Called whenever a new console message has been logged.
        /// </summary>
        public event EventHandler<ViewAddConsoleMessageEventArgs> OnConsoleMessageLogged;

        /// <summary>
        /// Called whenever a child view is created (i.e. popups).
        /// </summary>
        public event EventHandler OnChildViewCreated;

        /// <summary>
        /// Called whenever loading has started.
        /// </summary>
        public event EventHandler<ViewLoadEventArgs> OnBeginLoading;

        /// <summary>
        /// Called whenever a page has successfully been loaded.
        /// </summary>
        public event EventHandler<ViewLoadEventArgs> OnFinishLoading;

        /// <summary>
        /// Called whenever a page has failed loading.
        /// </summary>
        public event EventHandler<ViewLoadFailEventArgs> OnFailLoading;

        /// <summary>
        /// Called whenever the Javascript Window object is ready.
        /// </summary>
        public event EventHandler<ViewLoadEventArgs> OnWindowObjectReady;

        /// <summary>
        /// Called whenever the page's DOM is ready.
        /// </summary>
        public event EventHandler<ViewLoadEventArgs> OnDOMReady;

        /// <summary>
        /// Called whenever the history is changed. Not to confuse with <see cref="OnURLChanged"/>.
        /// </summary>
        public event EventHandler OnHistoryUpdated;

#pragma warning disable IDE0052 // References are kept to avoid garbage collection

        private ChangePropertyCallback titleChanged;
        private ChangePropertyCallback tooltipChanged;
        private ChangePropertyCallback urlChanged;
        private ChangeCursorCallback cursorChanged;
        private AddConsoleMessageCallback consoleLogged;
        private CreateChildViewCallback childCreated;
        private LoadingCallback beginLoad;
        private LoadingCallback finishLoad;
        private LoadingCallback windowObjectReady;
        private LoadingCallback domReady;
        private FailLoadingCallback failLoad;
        private UpdateHistoryCallback historyUpdate;

#pragma warning restore IDE0052

        private readonly bool isAccelerated;

        internal View(IntPtr handle)
            : base(handle, true)
        {
            Ultralight.ulViewSetUpdateHistoryCallback(handle, historyUpdate = handleHistoryUpdate, IntPtr.Zero);
            Ultralight.ulViewSetBeginLoadingCallback(handle, beginLoad = handleLoadingBegin, IntPtr.Zero);
            Ultralight.ulViewSetFinishLoadingCallback(handle, finishLoad = handleLoadingFinish, IntPtr.Zero);
            Ultralight.ulViewSetFailLoadingCallback(handle, failLoad = handleLoadingFail, IntPtr.Zero);
            Ultralight.ulViewSetWindowObjectReadyCallback(handle, windowObjectReady = handleReadyWindowObject, IntPtr.Zero);
            Ultralight.ulViewSetDOMReadyCallback(handle, domReady = handleReadyDOM, IntPtr.Zero);
            Ultralight.ulViewSetChangeTitleCallback(handle, titleChanged = handleTitleChange, IntPtr.Zero);
            Ultralight.ulViewSetChangeURLCallback(handle, urlChanged = handleURLChange, IntPtr.Zero);
            Ultralight.ulViewSetChangeTooltipCallback(handle, tooltipChanged = handleTooltipChange, IntPtr.Zero);
            Ultralight.ulViewSetChangeCursorCallback(handle, cursorChanged = handleCursorChange, IntPtr.Zero);
            Ultralight.ulViewSetAddConsoleMessageCallback(handle, consoleLogged = handleConsoleMessage, IntPtr.Zero);
            Ultralight.ulViewSetCreateChildViewCallback(handle, childCreated = handleChildViewCreated, IntPtr.Zero);
        }

        /// <summary>
        /// Creates a view.
        /// </summary>
        /// <param name="renderer">The renderer to use.</param>
        /// <param name="width">The width of this view.</param>
        /// <param name="height">The height of this view.</param>
        /// <param name="config">The config to use for this view.</param>
        /// <param name="session">The session to use for this view. Leave null to use <see cref="Renderer.Session"/></param>
        public View(Renderer renderer, int width, int height, ViewConfig config, Session session = null)
            : this(Ultralight.ulCreateView(renderer.Handle, (uint)width, (uint)height, config.Handle, session?.Handle ?? IntPtr.Zero))
        {
            isAccelerated = config.IsAccelerated;
        }

        protected override void DisposeManaged()
        {
            historyUpdate = null;
            beginLoad = null;
            finishLoad = null;
            failLoad = null;
            windowObjectReady = null;
            domReady = null;
            titleChanged = null;
            tooltipChanged = null;
            urlChanged = null;
            cursorChanged = null;
            consoleLogged = null;
            childCreated = null;
        }

        protected override void DisposeUnmanaged()
            => Ultralight.ulDestroyView(Handle);

        /// <summary>
        /// Get the surface for this view.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when attempted to access when <see cref="ViewConfig"/>.IsAccelerated = true.
        /// </exception>
        public Surface GetSurface()
        {
            if (isAccelerated)
                throw new InvalidOperationException($"{typeof(ViewConfig)} must have IsAccelerated = false.");

            return new Surface(Ultralight.ulViewGetSurface(Handle));
        }

        /// <summary>
        /// Perform actions while the view's <see cref="JSContext"/> is locked.
        /// </summary>
        /// <param name="action">Action invoked while the <see cref="JSContext"/> is locked.</param>
        public void GetJSContext(Action<JSContext> action)
        {
            using (var context = new JSContext(Ultralight.ulViewLockJSContext(Handle), false))
                action.Invoke(context);

            Ultralight.ulViewUnlockJSContext(Handle);
        }

        /// <summary>
        /// Load a raw string of HTML.
        /// </summary>
        /// <param name="html">The html string to load.</param>
        public void LoadHTML(string html)
            => Ultralight.ulViewLoadHTML(Handle, html);

        /// <summary>
        /// Navigate backwards in history
        /// </summary>
        public void GoBack()
            => Ultralight.ulViewGoBack(Handle);

        /// <summary>
        /// Navigate forwards in history
        /// </summary>
        public void GoForward()
            => Ultralight.ulViewGoForward(Handle);

        /// <summary>
        /// Navigate to an arbitrary offset in history
        /// </summary>
        public void GoToHistoryOffset(int offset)
            => Ultralight.ulViewGoToHistoryOffset(Handle, offset);

        /// <summary>
        /// Reload current page
        /// </summary>
        public void Reload()
            => Ultralight.ulViewReload(Handle);

        /// <summary>
        ///  Stop all page loads
        /// </summary>
        public void Stop()
            => Ultralight.ulViewStop(Handle);


        /// <summary>
        /// Give focus to the View.
        /// <br/>
        /// You should call this to give visual indication that the View has input
        /// focus (changes active text selection colors, for example).
        /// </summary>
        public void Focus()
            => Ultralight.ulViewFocus(Handle);


        /// <summary>
        /// Remove focus from the View and unfocus any focused input elements.
        /// <br/>
        /// You should call this to give visual indication that the View has lost
        /// input focus.
        /// </summary>
        public void Unfocus()
            => Ultralight.ulViewUnfocus(Handle);

        /// <summary>
        /// Create an inspector for this View, this is useful for debugging and
        /// inspecting pages locally. This will only succeed if you have the
        /// inspector assets in your filesystem-- the inspector will look for
        /// file:///inspector/Main.html when it loads.
        /// </summary>
        /// <returns>The inspector view.</returns>
        public View CreateInspectorView()
            => new View(Ultralight.ulViewCreateInspectorView(Handle));

        /// <summary>
        /// Fire a mouse event. The event is disposed after calling.
        /// </summary>
        public void FireMouseEvent(MouseEvent evt)
        {
            Ultralight.ulViewFireMouseEvent(Handle, evt.Handle);
            evt.Dispose();
        }

        /// <summary>
        /// Fire a keyboard event. The event is disposed after calling.
        /// <br/>
        /// Only <see cref="KeyEventType.Char"/> events actually generate text in input fields.
        /// </summary>
        public void FireKeyEvent(KeyEvent evt)
        {
            Ultralight.ulViewFireKeyEvent(Handle, evt.Handle);
            evt.Dispose();
        }

        /// <summary>
        /// Fire a scroll event. The event is disposed after calling.
        /// </summary>
        public void FireScrollEvent(ScrollEvent evt)
        {
            Ultralight.ulViewFireScrollEvent(Handle, evt.Handle);
            evt.Dispose();
        }

        private void handleTitleChange(IntPtr userData, IntPtr caller, string title)
            => OnTitleChanged?.Invoke(this, new ViewPropertyChangedEventArgs(title));

        private void handleTooltipChange(IntPtr userData, IntPtr caller, string tooltip)
            => OnTooltipChanged?.Invoke(this, new ViewPropertyChangedEventArgs(tooltip));

        private void handleURLChange(IntPtr userData, IntPtr caller, string url)
            => OnURLChanged?.Invoke(this, new ViewPropertyChangedEventArgs(url));

        private void handleCursorChange(IntPtr userData, IntPtr caller, CursorType cursor)
            => OnCursorChanged?.Invoke(this, new ViewCursorChangedEventArgs(cursor));

        private void handleConsoleMessage(IntPtr userData, IntPtr caller, ViewMessageSource source, ViewMessageLevel level, string message, uint line, uint column, string file)
            => OnConsoleMessageLogged?.Invoke(this, new ViewAddConsoleMessageEventArgs(source, level, message, file, (int)line, (int)column));

        private void handleChildViewCreated(IntPtr userData, IntPtr caller, string source, string destination, bool isPopup, RectI popupRect)
            => OnChildViewCreated?.Invoke(this, new ViewChildCreatedEventArgs(source, destination, isPopup, popupRect));

        private void handleLoadingBegin(IntPtr userData, IntPtr caller, ulong frameId, bool isMainFrame, string url)
            => OnBeginLoading?.Invoke(this, new ViewLoadEventArgs(frameId, isMainFrame, url));

        private void handleLoadingFinish(IntPtr userData, IntPtr caller, ulong frameId, bool isMainFrame, string url)
            => OnFinishLoading?.Invoke(this, new ViewLoadEventArgs(frameId, isMainFrame, url));

        private void handleReadyWindowObject(IntPtr userData, IntPtr caller, ulong frameId, bool isMainFrame, string url)
            => OnWindowObjectReady?.Invoke(this, new ViewLoadEventArgs(frameId, isMainFrame, url));

        private void handleReadyDOM(IntPtr userData, IntPtr caller, ulong frameId, bool isMainFrame, string url)
            => OnDOMReady?.Invoke(this, new ViewLoadEventArgs(frameId, isMainFrame, url));

        private void handleLoadingFail(IntPtr userData, IntPtr caller, ulong frameId, bool isMainFrame, string url, string description, string errorDomain, int errorCode)
            => OnFailLoading?.Invoke(this, new ViewLoadFailEventArgs(frameId, isMainFrame, url, description, errorDomain, errorCode));

        private void handleHistoryUpdate(IntPtr userData, IntPtr caller)
            => OnHistoryUpdated?.Invoke(this, EventArgs.Empty);
    }

    public enum ViewMessageSource
    {
        XML = 0,
        Javascript,
        Network,
        ConsoleAPI,
        Storage,
        AppCache,
        Rendering,
        CSS,
        Security,
        ContentBlocker,
        Other,
    }

    public enum ViewMessageLevel
    {
        Log = 1,
        Warning,
        Error,
        Debug,
        Info,
    }

    public class ViewLoadEventArgs : EventArgs
    {
        public ulong FrameID { get; }
        public bool IsMainFrame { get; }
        public string URL { get; }

        public ViewLoadEventArgs(ulong frameID, bool isMainFrame, string url)
        {
            FrameID = frameID;
            IsMainFrame = isMainFrame;
            URL = url;
        }
    }

    public class ViewLoadFailEventArgs : ViewLoadEventArgs
    {
        public int Status { get; }
        public string Description { get; }
        public string ErrorDomain { get; }

        public ViewLoadFailEventArgs(ulong frameID, bool isMainFrame, string url, string description, string errorDomain, int status)
            : base(frameID, isMainFrame, url)
        {
            Status = status;
            Description = description;
            ErrorDomain = errorDomain;
        }
    }

    public class ViewAddConsoleMessageEventArgs : EventArgs
    {
        public ViewMessageSource Source { get; }
        public ViewMessageLevel Level { get; }
        public string Message { get; }
        public string File { get; }
        public int Line { get; }
        public int Column { get; }

        public ViewAddConsoleMessageEventArgs(ViewMessageSource source, ViewMessageLevel level, string message, string file, int line, int column)
        {
            Source = source;
            Level = level;
            Message = message;
            File = file;
            Line = line;
            Column = column;
        }
    }

    public class ViewPropertyChangedEventArgs : EventArgs
    {
        public string Value { get; }

        public ViewPropertyChangedEventArgs(string value)
        {
            Value = value;
        }
    }

    public class ViewCursorChangedEventArgs : EventArgs
    {
        public CursorType Cursor { get; }

        public ViewCursorChangedEventArgs(CursorType cursor)
        {
            Cursor = cursor;
        }
    }

    public class ViewChildCreatedEventArgs : EventArgs
    {
        public string Source { get; }
        public string Destination { get; }
        public bool IsPopup { get; }
        public RectI PopupRect { get; }

        public ViewChildCreatedEventArgs(string source, string destination, bool isPopup, RectI popupRect)
        {
            Source = source;
            Destination = destination;
            IsPopup = isPopup;
            PopupRect = popupRect;
        }
    }

    internal delegate void ChangePropertyCallback(
        IntPtr userData, IntPtr caller,
        [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string title
    );

    internal delegate void AddConsoleMessageCallback(
        IntPtr userData, IntPtr caller, ViewMessageSource source, ViewMessageLevel level,
        [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string message, uint lineNUmber, uint columnNumber,
        [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string sourceId
    );

    internal delegate void CreateChildViewCallback(
        IntPtr userData, IntPtr caller,
        [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string openerUrl,
        [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string targetUrl,
        [MarshalAs(UnmanagedType.I1)] bool isPopup, RectI popupRect
    );

    internal delegate void LoadingCallback(
        IntPtr userData, IntPtr caller, ulong frameId,
        [MarshalAs(UnmanagedType.I1)] bool isMainFrame,
        [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string url
    );

    internal delegate void FailLoadingCallback(
        IntPtr userData, IntPtr caller, ulong frameId,
        [MarshalAs(UnmanagedType.I1)] bool isMainFrame,
        [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string url,
        [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string description,
        [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string errorDomain,
        int errorCode
    );

    internal delegate void ChangeCursorCallback(IntPtr userData, IntPtr caller, CursorType cursor);
    internal delegate void UpdateHistoryCallback(IntPtr userData, IntPtr caller);
}
