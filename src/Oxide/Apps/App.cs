// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System;

namespace Oxide.Apps
{
    public class App : DisposableObject
    {
        internal static App Current { get; private set; }

        /// <summary>
        /// The underlying Renderer instance.
        /// </summary>
        public readonly Renderer Renderer;

        /// <summary>
        /// Get the main monitor.
        /// </summary>
        public readonly Monitor Monitor;

        /// <summary>
        /// Called every frame while this app is running.
        /// </summary>
        public event EventHandler OnUpdate;

        private AppUpdateCallback updateCallback;

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
            if (Current != null)
                throw new InvalidOperationException($"An instance of {nameof(App)} already exists.");

            if (Renderer.Current != null)
                throw new InvalidOperationException($"An instance of {nameof(Renderer)} already exists.");

            Current = this;

            Renderer = new Renderer(AppCore.ulAppGetRenderer(Handle));
            Monitor = new Monitor(AppCore.ulAppGetMainMonitor(Handle), this);
            AppCore.ulAppSetUpdateCallback(Handle, updateCallback += handleAppUpdate, IntPtr.Zero);
        }

        /// <summary>
        /// Called every frame.
        /// </summary>
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

        protected override void DisposeManaged()
        {
            updateCallback -= handleAppUpdate;
        }

        protected override void DisposeUnmanaged()
        {
            AppCore.ulDestroyApp(Handle);
            Current = null;
        }

        private void handleAppUpdate(IntPtr userData)
        {
            Update();
            OnUpdate?.Invoke(this, EventArgs.Empty);
        }
    }

    internal delegate void AppUpdateCallback(IntPtr userData);
}
