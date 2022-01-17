using System;
using System.Numerics;

namespace Oxide.Apps
{
    public class Monitor : DisposableObject
    {
        /// <summary>
        /// The app it owns.
        /// </summary>
        public readonly App App;

        /// <summary>
        /// Get the monitor's DPI scale (1.0 = 100%).
        /// </summary>
        public double Scale => AppCore.ulMonitorGetScale(Handle);

        /// <summary>
        /// Get the resolution of the monitor (in pixels).
        /// </summary>
        public Vector2 Resolution => new Vector2(Width, Height);

        /// <summary>
        /// Get the width of the monitor (in pixels).
        /// </summary>
        public int Width => (int)AppCore.ulMonitorGetWidth(Handle);

        /// <summary>
        /// Get the height of the monitor (in pixels).
        /// </summary>
        public int Height => (int)AppCore.ulMonitorGetHeight(Handle);

        internal Monitor(IntPtr handle, App app)
            : base(handle, false)
        {
            App = app;
        }

        /// <summary>
        /// Creates a new window from this monitor.
        /// </summary>
        public Window CreateWindow(int width, int height, bool isFullScreen, WindowFlags flags)
            => new Window(this, width, height, isFullScreen, flags);
    }
}
