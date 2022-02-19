// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System;
using System.Runtime.InteropServices;
using Oxide.Graphics;
using Oxide.Graphics.Bitmaps;
using Oxide.Graphics.Drivers;
using Oxide.Graphics.Fonts;
using Oxide.Input;
using Oxide.Interop;
using Oxide.Platforms;

namespace Oxide
{
    public class Ultralight
    {

#pragma warning disable CA2101

        internal const string LIB_ULTRALIGHT = @"Ultralight";

        public static readonly Version Version = new Version((int)ulVersionMajor(), (int)ulVersionMinor(), (int)ulVersionPatch());

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        private static extern uint ulVersionMajor();

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        private static extern uint ulVersionMinor();

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        private static extern uint ulVersionPatch();

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern IntPtr ulCreateEmptyBitmap();

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern IntPtr ulCreateBitmap(uint width, uint height, BitmapFormat format);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern IntPtr ulCreateBitmapFromPixels(uint width, uint height, BitmapFormat format, uint rowBytes, IntPtr pixels, uint size, [MarshalAs(UnmanagedType.I1)] bool shouldCopy);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern IntPtr ulCreateBitmapFromCopy(IntPtr existingBitmap);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulDestroyBitmap(IntPtr bitmap);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern uint ulBitmapGetWidth(IntPtr bitmap);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern uint ulBitmapGetHeight(IntPtr bitmap);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern BitmapFormat ulBitmapGetFormat(IntPtr bitmap);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern uint ulBitmapGetBpp(IntPtr bitmap);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern uint ulBitmapGetRowBytes(IntPtr bitmap);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern uint ulBitmapGetSize(IntPtr bitmap);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool ulBitmapOwnsPixels(IntPtr bitmap);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern IntPtr ulBitmapLockPixels(IntPtr bitmap);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulBitmapUnlockPixels(IntPtr bitmap);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern IntPtr ulBitmapRawPixels(IntPtr bitmap);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool ulBitmapIsEmpty(IntPtr bitmap);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulBitmapErase(IntPtr bitmap);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true, CharSet = CharSet.Ansi, BestFitMapping = true, ThrowOnUnmappableChar = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool ulBitmapWritePNG(IntPtr bitmap, [MarshalAs(UnmanagedType.LPStr)] string path);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulBitmapSwapRedBlueChannels(IntPtr bitmap);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern Matrix4x4 ulApplyProjection(Matrix4x4 transform, float viewportWidth, float viewportHeight, [MarshalAs(UnmanagedType.I1)] bool flipY);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool ulRectIsEmpty(Rect rect);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool ulRectIIsEmpty(RectI rect);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern uint ulSurfaceGetWidth(IntPtr surface);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern uint ulSurfaceGetHeight(IntPtr surface);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern uint ulSurfaceGetRowBytes(IntPtr surface);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern uint ulSurfaceGetSize(IntPtr surface);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern IntPtr ulSurfaceLockPixels(IntPtr surface);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulSurfaceUnlockPixels(IntPtr surface);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulSurfaceResize(IntPtr surface, uint width, uint height);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulSurfaceSetDirtyBounds(IntPtr surface, RectI bounds);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern RectI ulSurfaceGetDirtyBounds(IntPtr surface);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulSurfaceClearDirtyBounds(IntPtr surface);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern IntPtr ulSurfaceGetUserData(IntPtr surface);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern IntPtr ulBitmapSurfaceGetBitmap(IntPtr surface);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern IntPtr ulCreateKeyEvent(
            KeyEventType type,
            uint modifiers,
            int virtualKeyCode,
            int nativeKeyCode,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string text,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string unmodifiedText,
            [MarshalAs(UnmanagedType.I1)] bool isKeypad,
            [MarshalAs(UnmanagedType.I1)] bool isAutoRepeat,
            [MarshalAs(UnmanagedType.I1)] bool isSystemKey);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern IntPtr ulCreateKeyEventWindows(
            KeyEventType type,
            UIntPtr wparam,
            UIntPtr lparam,
            [MarshalAs(UnmanagedType.I1)] bool isSystemKey);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern IntPtr ulCreateKeyEventMacOS(IntPtr evt);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern IntPtr ulCreateMouseEvent(MouseEventType type, int x, int y, MouseButton button);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulDestroyMouseEvent(IntPtr evt);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern IntPtr ulCreateScrollEvent(ScrollEventType type, int xDelta, int yDelta);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulDestroyScrollEvent(IntPtr evt);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern IntPtr ulCreateStringUTF8([MarshalAs(UnmanagedType.LPUTF8Str)] string ptr, uint length);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulDestroyString(IntPtr str);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true, CharSet = CharSet.Unicode)]
        internal static extern IntPtr ulStringGetData(IntPtr str);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulPlatformSetSurfaceDefinition(SurfaceDefinition surfaceDefinition);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulPlatformSetLogger(Logger logger);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulPlatformSetFileSystem(FileSystem fileSystem);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulPlatformSetClipboard(Clipboard clipboard);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulPlatformSetGPUDriver(GPUDriver driver);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern IntPtr ulCreateConfig();

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulDestroyConfig(IntPtr handle);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulConfigSetCachePath(IntPtr config, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string cachePath);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulConfigSetFaceWinding(IntPtr config, FaceWinding winding);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulConfigSetFontHinting(IntPtr config, FontHinting fontHinting);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulConfigSetFontGamma(IntPtr config, double gamma);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulConfigSetUserStyleSheet(IntPtr config, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string cssString);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulConfigSetForceRepaint(IntPtr config, [MarshalAs(UnmanagedType.I1)] bool enabled);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulConfigSetAnimationTimerDelay(IntPtr config, double delay);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulConfigSetScrollTimerDelay(IntPtr config, double delay);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulConfigSetRecycleDelay(IntPtr config, double delay);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulConfigSetMemoryCacheSize(IntPtr config, uint size);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulConfigSetPageCacheSize(IntPtr config, uint size);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulConfigSetOverrideRAMSize(IntPtr config, uint size);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulConfigSetMinLargeHeapSize(IntPtr config, uint size);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulConfigSetMinSmallHeapSize(IntPtr config, uint size);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern IntPtr ulCreateRenderer(IntPtr config);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulDestroyRenderer(IntPtr renderer);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulUpdate(IntPtr renderer);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulRender(IntPtr renderer);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulPurgeMemory(IntPtr renderer);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulLogMemoryUsage(IntPtr renderer);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern IntPtr ulDefaultSession(IntPtr renderer);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern IntPtr ulCreateSession(
            IntPtr renderer,
            [MarshalAs(UnmanagedType.I1)] bool isPersistent,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string name);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulDestroySession(IntPtr session);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool ulSessionIsPersistent(IntPtr session);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))]
        internal static extern string ulSessionGetName(IntPtr session);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern uint ulSessionGetId(IntPtr session);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))]
        internal static extern string ulSessionGetDiskPath(IntPtr session);

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

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern IntPtr ulCreateViewConfig();

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulDestroyViewConfig(IntPtr config);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulViewConfigSetIsAccelerated(IntPtr config, [MarshalAs(UnmanagedType.I1)] bool isAccelerated);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulViewConfigSetIsTransparent(IntPtr config, [MarshalAs(UnmanagedType.I1)] bool isTransparent);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulViewConfigSetInitialDeviceScale(IntPtr config, double initialDeviceScale);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulViewConfigSetInitialFocus(IntPtr config, [MarshalAs(UnmanagedType.I1)] bool isFocussed);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulViewConfigSetEnableImages(IntPtr config, [MarshalAs(UnmanagedType.I1)] bool enabled);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulViewConfigSetEnableJavascript(IntPtr config, [MarshalAs(UnmanagedType.I1)] bool enabled);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulViewConfigSetFontFamilyStandard(
            IntPtr config,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string fontName
        );

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulViewConfigSetFontFamilyFixed(
            IntPtr config,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string fontName
        );

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulViewConfigSetFontFamilySerif(
            IntPtr config,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string fontName
        );

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulViewConfigSetFontFamilySansSerif(
            IntPtr config,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string fontName
        );

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulViewConfigSetFontFamilyUserAgent(
            IntPtr config,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string fontName
        );

#pragma warning restore CA2101

    }
}
