// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System;
using System.Runtime.InteropServices;
using Oxide.Input;
using Oxide.Interop;

namespace Oxide.Apps
{
    public class AppCore
    {
        internal const string LIB_APPCORE = @"AppCore";

        /// <summary>
        /// This is only needed if you are not creating your own app.
        /// <br/>
        /// Initializes the platform font loader and sets it as the current FontLoader.
        /// </summary>
        public static void EnablePlatformFontLoader() => ulEnablePlatformFontLoader();

        /// <summary>
        /// This is only needed if you are not creating your own app.
        /// <br/>
        /// Initializes the platform file system (needed for loading file:/// URLs) and
        /// sets it as the current FileSystem.
        /// <br/>
        /// You can specify a base directory path to resolve relative paths against.
        /// </summary>
        public static void EnablePlatformFileSystem(string basePath) => ulEnablePlatformFileSystem(basePath);

        /// <summary>
        /// This is only needed if you are not creating your own app.
        /// <br/>
        /// Initializes the default logger (writes the log to a file).
        /// <br/>
        /// You should specify a writable log path to write the log to
        /// for example "./ultralight.log".
        /// </summary>
        public static void EnableDefaultLogger(string logPath) => ulEnableDefaultLogger(logPath);

#pragma warning disable CA2101

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern void ulEnablePlatformFontLoader();

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern void ulEnablePlatformFileSystem(
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string basePath
        );

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern void ulEnableDefaultLogger(
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string logPath
        );

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern IntPtr ulCreateApp(IntPtr settings, IntPtr config);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern IntPtr ulDestroyApp(IntPtr app);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern void ulAppSetUpdateCallback(IntPtr app, AppUpdateCallback callback, IntPtr userData);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool ulAppisRunning(IntPtr app);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern IntPtr ulAppGetMainMonitor(IntPtr app);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern IntPtr ulAppGetRenderer(IntPtr app);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern void ulAppRun(IntPtr app);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern void ulAppQuit(IntPtr app);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern double ulMonitorGetScale(IntPtr monitor);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern uint ulMonitorGetWidth(IntPtr monitor);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern uint ulMonitorGetHeight(IntPtr monitor);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern IntPtr ulCreateOverlay(IntPtr window, uint width, uint height, int x, int y);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern IntPtr ulCreateOverlayWithView(IntPtr window, IntPtr view, int x, int y);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern void ulDestroyOverlay(IntPtr overlay);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern IntPtr ulOverlayGetView(IntPtr overlay);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern uint ulOverlayGetWidth(IntPtr overlay);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern uint ulOverlayGetHeight(IntPtr overlay);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern int ulOverlayGetX(IntPtr overlay);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern int ulOverlayGetY(IntPtr overlay);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern void ulOverlayMoveTo(IntPtr overlay, int x, int y);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern void ulOverlayResize(IntPtr overlay, uint width, uint height);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool ulOverlayIsHidden(IntPtr overlay);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern void ulOverlayHide(IntPtr overlay);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern void ulOverlayShow(IntPtr overlay);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool ulOverlayHasFocus(IntPtr overlay);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern void ulOverlayFocus(IntPtr overlay);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern void ulOverlayUnfocus(IntPtr overlay);

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

        [DllImport(LIB_APPCORE, ExactSpelling = true, CharSet = CharSet.Ansi, BestFitMapping = true)]
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

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern IntPtr ulCreateSettings();

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern IntPtr ulDestroySettings(IntPtr settings);

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern void ulSettingsSetDeveloperName(
            IntPtr settings,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string name
        );

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern void ulSettingsSetAppName(
            IntPtr settings,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string name
        );

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern void ulSettingsSetFileSystemPath(
            IntPtr settings,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string path
        );

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern void ulSettingsSetLoadShadersFromFileSystem(
            IntPtr settings,
            [MarshalAs(UnmanagedType.I1)] bool enabled
        );

        [DllImport(LIB_APPCORE, ExactSpelling = true)]
        internal static extern void ulSettingsSetForceCPURenderer(
            IntPtr settings,
            [MarshalAs(UnmanagedType.I1)] bool forceCpu
        );
    }

#pragma warning restore CA2101

}
