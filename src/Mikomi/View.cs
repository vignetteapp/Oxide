using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Mikomi.Graphics;
using Mikomi.Input;
using Mikomi.Javascript;

namespace Mikomi
{
    public class View : DisposableObject
    {
        private readonly bool isAccelerated;

        /// <summary>
        /// Get current URL. Set loads a new URL.
        /// </summary>
        public string URL
        {
            get => Ultralight.ulViewGetUrl(Handle);
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

        internal View(IntPtr handle)
            : base(handle, true)
        {
        }

        public View(Renderer renderer, uint width, uint height, ViewConfig config, Session session)
            : base(Ultralight.ulCreateView(renderer.Handle, width, height, config.Handle, session.Handle))
        {
            isAccelerated = config.IsAccelerated;
        }

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
        {
            Ultralight.ulViewLoadHTML(Handle, html);
        }

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
        /// <br/>
        /// Only <see cref="KeyEventType.Char"/> events actually generate text in input fields.
        /// </summary>
        public void FireMouseEvent(MouseEvent evt)
        {
            Ultralight.ulViewFireMouseEvent(Handle, evt.Handle);
            evt.Dispose();
        }

        /// <summary>
        /// Fire a keyboard event. The event is disposed after calling.
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

    internal delegate void ChangeTitleCallback(IntPtr userData, IntPtr caller, IntPtr title);
    internal delegate void ChangeTooltipCallback(IntPtr userData, IntPtr caller, IntPtr tooltip);
    internal delegate void ChangeURLCallback(IntPtr userData, IntPtr caller, IntPtr url);
    internal delegate void ChangeCursorCallback(IntPtr userData, IntPtr caller, CursorType cursor);
    internal delegate void AddConsoleMessageCallback(IntPtr userData, IntPtr caller, ViewMessageSource source, ViewMessageLevel level, IntPtr message, uint lineNUmber, uint columnNumber, IntPtr sourceId);
    internal delegate void CreateChildViewCallback(IntPtr userData, IntPtr caller, IntPtr openerUrl, IntPtr targetUrl, [MarshalAs(UnmanagedType.I1)] bool isPopup, RectI popupRect);
    internal delegate void BeginLoadingCallback(IntPtr userData, IntPtr caller, ulong frameId, [MarshalAs(UnmanagedType.I1)] bool isMainFrame, IntPtr url);
    internal delegate void FinishLoadingCallback(IntPtr userData, IntPtr caller, ulong frameId, [MarshalAs(UnmanagedType.I1)] bool isMainFrame, IntPtr url);
    internal delegate void FailLoadingCallback(IntPtr userData, IntPtr caller, ulong frameId, [MarshalAs(UnmanagedType.I1)] bool isMainFrame, IntPtr errorDomain, int errorCode);
    internal delegate void WindowObjectReadyCallback(IntPtr userData, IntPtr caller, ulong frameId, [MarshalAs(UnmanagedType.I1)] bool isMainFrame, IntPtr url);
    internal delegate void DOMReadyCallback(IntPtr userData, IntPtr caller, ulong frameId, [MarshalAs(UnmanagedType.I1)] bool isMainFrame, IntPtr url);
    internal delegate void UpdateHistoryCallback(IntPtr userData, IntPtr caller);

    public partial class Ultralight
    {
        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern IntPtr ulCreateView(IntPtr renderer, uint width, uint height, IntPtr viewConfig, IntPtr session);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulDestroyView(IntPtr view);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler), MarshalCookie = "DoNotDestroy")]
        internal static extern string ulViewGetUrl(IntPtr view);

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
        internal static extern void ulViewSetChangeTitleCallBack(IntPtr view, ChangeTitleCallback callback, IntPtr userData);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ulViewSetChangeTooltipChangeTooltipCallback(IntPtr view, ChangeTooltipCallback callback, IntPtr userData);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ulViewSetChangeCursorCallBack(IntPtr view, ChangeCursorCallback callback, IntPtr userData);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ulViewSetAddConsoleMessageCallback(IntPtr view, AddConsoleMessageCallback callback, IntPtr userData);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ulViewSetBeginLoadingCallBack(IntPtr view, BeginLoadingCallback callback, IntPtr userData);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ulViewSetFinishLoadingCallBack(IntPtr view, FinishLoadingCallback callback, IntPtr userData);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ulViewSetFailLoadingCallBack(IntPtr view, FailLoadingCallback callback, IntPtr userData);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ulViewSetWindowObjectReadyCallBack(IntPtr view, WindowObjectReadyCallback callback, IntPtr userData);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ulViewSetDOMReadyCallBack(IntPtr view, DOMReadyCallback callback, IntPtr userData);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ulViewSetUpdateHistoryCallBack(IntPtr view, UpdateHistoryCallback callback, IntPtr userData);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulViewSetNeedsPaint(IntPtr view, [MarshalAs(UnmanagedType.I1)] bool needsPaint);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool ulViewGetNeedsPaint(IntPtr view);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern IntPtr ulViewCreateInspectorView(IntPtr view);
    }
}
