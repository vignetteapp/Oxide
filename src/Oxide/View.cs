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
        private readonly bool isAccelerated;

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
        /// Get the surface for this view.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when attempted to access when <see cref="ViewConfig"/>.IsAccelerated = true.
        /// </exception>
        public Surface Surface
        {
            get
            {
                if (isAccelerated)
                    throw new InvalidOperationException($"{typeof(ViewConfig)} must have IsAccelerated = false.");

                return new Surface(Ultralight.ulViewGetSurface(Handle));
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
        /// Called whenever the JavaScript Window object is ready.
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

        internal View(IntPtr handle)
            : base(handle, true)
        {
            Ultralight.ulViewSetUpdateHistoryCallback(handle, historyUpdate += handleHistoryUpdate, IntPtr.Zero);
            Ultralight.ulViewSetBeginLoadingCallback(handle, beginLoad += handleLoadingBegin, IntPtr.Zero);
            Ultralight.ulViewSetFinishLoadingCallback(handle, finishLoad += handleLoadingFinish, IntPtr.Zero);
            Ultralight.ulViewSetFailLoadingCallback(handle, failLoad += handleLoadingFail, IntPtr.Zero);
            Ultralight.ulViewSetWindowObjectReadyCallback(handle, windowObjectReady += handleReadyWindowObject, IntPtr.Zero);
            Ultralight.ulViewSetDOMReadyCallback(handle, domReady += handleReadyDOM, IntPtr.Zero);
            Ultralight.ulViewSetChangeTitleCallback(handle, titleChanged += handleTitleChange, IntPtr.Zero);
            Ultralight.ulViewSetChangeURLCallback(handle, urlChanged += handleURLChange, IntPtr.Zero);
            Ultralight.ulViewSetChangeTooltipCallback(handle, tooltipChanged += handleTooltipChange, IntPtr.Zero);
            Ultralight.ulViewSetChangeCursorCallback(handle, cursorChanged += handleCursorChange, IntPtr.Zero);
            Ultralight.ulViewSetAddConsoleMessageCallback(handle, consoleLogged += handleConsoleMessage, IntPtr.Zero);
            Ultralight.ulViewSetCreateChildViewCallback(handle, childCreated += handleChildViewCreated, IntPtr.Zero);
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
            historyUpdate -= handleHistoryUpdate;
            beginLoad -= handleLoadingBegin;
            finishLoad -= handleLoadingFinish;
            failLoad -= handleLoadingFail;
            windowObjectReady -= handleReadyWindowObject;
            domReady -= handleReadyDOM;
            titleChanged -= handleTitleChange;
            tooltipChanged -= handleTooltipChange;
            urlChanged -= handleURLChange;
            cursorChanged -= handleCursorChange;
            consoleLogged -= handleConsoleMessage;
            childCreated -= handleChildViewCreated;
        }

        protected override void DisposeUnmanaged()
            => Ultralight.ulDestroyView(Handle);

        /// <summary>
        /// Acquire the page's JSContext for use with JavaScriptCore API.
        /// </summary>
        /// <returns>The JavaScript context.</returns>
        public JSContext LockJSContext()
            => new JSContext(Ultralight.ulViewLockJSContext(Handle));

        /// <summary>
        /// Unlock the page's JSContext after a previous call to <see cref="LockJSContext"/>.
        /// </summary>
        public void UnlockJSContext()
            => Ultralight.ulViewUnlockJSContext(Handle);

        /// <summary>
        /// Helper function to evaluate a raw string of JavaScript and return the
        /// result as a String.
        /// </summary>
        /// <param name="script">A string of JavaScript to evaluate in the main frame.</param>
        /// <param name="exception">The resulting string when an exception has occured.</param>
        /// <returns>The JavaScript result as a string.</returns>
        public string EvaluateScript(string script, out string exception)
            => Ultralight.ulViewEvaluateScript(Handle, script, out exception);

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

        private void handleLoadingFail(IntPtr userData, IntPtr caller, ulong frameId, bool isMainFrame, string url, int errorCode)
            => OnFailLoading?.Invoke(this, new ViewLoadFailEventArgs(frameId, isMainFrame, url, errorCode));

        private void handleHistoryUpdate(IntPtr userData, IntPtr caller)
            => OnHistoryUpdated?.Invoke(this, EventArgs.Empty);
    }

    public static class ViewExtensions
    {
        /// <summary>
        /// Helper method to perform actions within the <see cref="JSContext"/> ensuring that it is
        /// unlocked after use.
        /// </summary>
        /// <param name="view">This view.</param>
        /// <param name="action">Action invoked while the <see cref="JSContext"/> is locked.</param>
        public static void GetJSContext(this View view, Action<JSContext> action)
        {
            action(view.LockJSContext());
            view.UnlockJSContext();
        }
    }

    public enum ViewMessageSource
    {
        XML = 0,
        JS,
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

        public ViewLoadFailEventArgs(ulong frameID, bool isMainFrame, string url, int status)
            : base(frameID, isMainFrame, url)
        {
            Status = status;
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
        [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string errorDomain,
        int errorCode
    );

    internal delegate void ChangeCursorCallback(IntPtr userData, IntPtr caller, CursorType cursor);
    internal delegate void UpdateHistoryCallback(IntPtr userData, IntPtr caller);

#pragma warning disable CA2101 // Custom marshaler is used

    public partial class Ultralight
    {
        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern IntPtr ulCreateView(IntPtr renderer, uint width, uint height, IntPtr viewConfig, IntPtr session);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulDestroyView(IntPtr view);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler), MarshalCookie = "DoNotDestroy")]
        internal static extern string ulViewGetURL(IntPtr view);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler), MarshalCookie = "DoNotDestroy")]
        internal static extern string ulViewGetTitle(IntPtr view);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern uint ulViewGetWidth(IntPtr view);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern uint ulViewGetHeight(IntPtr view);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool ulViewIsLoading(IntPtr view);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern RenderTarget ulViewGetRenderTarget(IntPtr view);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern IntPtr ulViewGetSurface(IntPtr view);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulViewLoadHTML(
            IntPtr view,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string html
        );

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulViewLoadURL(
            IntPtr view,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string url
        );

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulViewResize(IntPtr view, uint width, uint height);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern IntPtr ulViewLockJSContext(IntPtr view);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulViewUnlockJSContext(IntPtr view);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))]
        internal static extern string ulViewEvaluateScript(
            IntPtr view,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string jsString,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] out string exception);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool ulViewCanGoBack(IntPtr view);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool ulViewCanGoForward(IntPtr view);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulViewGoBack(IntPtr view);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulViewGoForward(IntPtr view);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulViewGoToHistoryOffset(IntPtr view, int offset);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulViewReload(IntPtr view);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulViewStop(IntPtr view);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulViewFocus(IntPtr view);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulViewUnfocus(IntPtr view);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool ulViewHasFocus(IntPtr view);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool ulViewHasInputFocus(IntPtr view);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulViewFireKeyEvent(IntPtr view, IntPtr keyEvent);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulViewFireMouseEvent(IntPtr view, IntPtr mouseEvent);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulViewFireScrollEvent(IntPtr view, IntPtr scrollEvent);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ulViewSetChangeTitleCallback(IntPtr view, ChangePropertyCallback callback, IntPtr userData);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ulViewSetChangeURLCallback(IntPtr view, ChangePropertyCallback callback, IntPtr userData);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ulViewSetChangeTooltipCallback(IntPtr view, ChangePropertyCallback callback, IntPtr userData);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ulViewSetChangeCursorCallback(IntPtr view, ChangeCursorCallback callback, IntPtr userData);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ulViewSetCreateChildViewCallback(IntPtr view, CreateChildViewCallback callback, IntPtr userData);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ulViewSetAddConsoleMessageCallback(IntPtr view, AddConsoleMessageCallback callback, IntPtr userData);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ulViewSetBeginLoadingCallback(IntPtr view, LoadingCallback callback, IntPtr userData);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ulViewSetFinishLoadingCallback(IntPtr view, LoadingCallback callback, IntPtr userData);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ulViewSetFailLoadingCallback(IntPtr view, FailLoadingCallback callback, IntPtr userData);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ulViewSetWindowObjectReadyCallback(IntPtr view, LoadingCallback callback, IntPtr userData);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ulViewSetDOMReadyCallback(IntPtr view, LoadingCallback callback, IntPtr userData);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ulViewSetUpdateHistoryCallback(IntPtr view, UpdateHistoryCallback callback, IntPtr userData);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulViewSetNeedsPaint(IntPtr view, [MarshalAs(UnmanagedType.I1)] bool needsPaint);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool ulViewGetNeedsPaint(IntPtr view);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern IntPtr ulViewCreateInspectorView(IntPtr view);
    }

#pragma warning restore CA2101

}
