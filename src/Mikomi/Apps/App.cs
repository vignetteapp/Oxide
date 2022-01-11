using System;
using System.Runtime.InteropServices;

namespace Mikomi.Apps
{
    public class App : DisposableObject
    {
        /// <summary>
        /// The underlying Renderer instance.
        /// </summary>
        public readonly Renderer Renderer;

        /// <summary>
        /// Get the main monitor.
        /// </summary>
        public readonly Monitor Monitor;

#pragma warning disable IDE0052 // Holding reference to protect from GC

        private readonly AppUpdateCallback updateCallback;

#pragma warning restore IDE0052

        /// <summary>
        /// Create the app singleton
        /// <br/>
        /// You should only create one of these per application lifetime.
        /// <br/>
        /// Certain Config options may be overridden during App creation,
        /// most commonly <see cref="Config.FaceWinding"/> and <see cref="ViewConfig.InitialDeviceScale"/>
        /// </summary>
        /// <param name="appConfig">
        /// Settings to customize App runtime behavior. You can pass null for this parameter to use default settings.
        /// </param>
        /// <param name="rendererConfig">
        /// Config options for the Ultralight renderer. You can pass null for this parameter to use default config.
        /// </param>
        public App(AppConfig appConfig = null, Config rendererConfig = null)
            : base(AppCore.ulCreateApp(appConfig?.Handle ?? IntPtr.Zero, rendererConfig?.Handle ?? IntPtr.Zero))
        {
            Renderer = new Renderer(AppCore.ulAppGetRenderer(Handle));
            Monitor = new Monitor(AppCore.ulAppGetMainMonitor(Handle), this);
            AppCore.ulAppSetUpdateCallback(Handle, updateCallback = (_) => Update(), IntPtr.Zero);
        }

        protected virtual void Update()
        {
        }

        /// <summary>
        /// Runs this app.
        /// </summary>
        public void Run() => AppCore.ulAppRun(Handle);

        /// <summary>
        /// Quits this app.
        /// </summary>
        public void Quit() => AppCore.ulAppQuit(Handle);

        protected override void DisposeUnmanaged()
            => AppCore.ulDestroyApp(Handle);
    }

    internal delegate void AppUpdateCallback(IntPtr userData);

    public partial class AppCore
    {
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
    }
}
