using System;
using System.Runtime.InteropServices;
using Mikomi.Graphics;
using Mikomi.Graphics.Fonts;

namespace Mikomi
{
    public sealed class Config : ManagedObject
    {
        /// <summary>
        /// Set the file path to a writable directory that will be used to store
        /// cookies, cached resources, and other persistent data.
        /// </summary>
        public string CachePath { get; set; }

        /// <summary>
        /// The winding order for front-facing triangles.
        /// </summary>
        /// <remarks>
        /// This is only used with custom GPUDrivers
        /// </remarks>
        public FaceWinding FaceWinding { get; set; }

        /// <summary>
        /// The hinting algorithm to use when rendering fonts.
        /// </summary>
        public FontHinting FontHinting { get; set; }

        /// <summary>
        /// The gamma to use when compositing font glyphs, change this value to
        /// adjust contrast (Adobe and Apple prefer 1.8, others may prefer 2.2).
        /// <br/>
        /// (Default = 1.8)
        /// </summary>
        public double FontGamma { get; set; }

        /// <summary>
        /// Set user stylesheet (CSS).
        /// <br/>
        /// (Default = Empty)
        /// </summary>
        public string UserStylesheet { get; set; }

        /// <summary>
        /// Set whether or not we should continuously repaint any Views or compositor
        /// layers, regardless if they are dirty or not. This is mainly used to
        /// diagnose painting/shader issues.
        /// <br/>
        /// (Default = False)
        /// </summary>
        public bool ForceRepaint { get; set; }

        /// <summary>
        /// Set the amount of time to wait before triggering another repaint when a
        /// CSS animation is active.
        /// <br/>
        /// (Default = 1.0 / 60.0)
        /// </summary>
        public double AnimationTimerDelay { get; set; }

        /// <summary>
        /// When a smooth scroll animation is active, the amount of time (in seconds)
        /// to wait before triggering another repaint.
        /// <br/>
        /// (Default = 60 Hz)
        /// </summary>
        public double ScrollTimerDelay { get; set; }

        /// <summary>
        /// The amount of time (in seconds) to wait before running the recycler (will
        /// attempt to return excess memory back to the system).
        /// <br/>
        /// (Default = 4.0)
        /// </summary>
        public double RecycleDelay { get; set; }

        /// <summary>
        /// Set the size of WebCore's memory cache for decoded images, scripts, and
        /// other assets in bytes.
        /// <br/>
        /// (Default = 64 * 1024 * 1024)
        /// </summary>
        public uint MemoryCacheSize { get; set; }

        /// <summary>
        /// Set the number of pages to keep in the cache.
        /// <br/>
        /// (Default = 0)
        /// </summary>
        public uint PageCacheSize { get; set; }

        /// <summary>
        /// JavaScriptCore tries to detect the system's physical RAM size to set
        /// reasonable allocation limits. Set this to anything other than 0 to
        /// override the detected value. Size is in bytes.
        /// <br/>
        /// This can be used to force JavaScriptCore to be more conservative with
        /// its allocation strategy (at the cost of some performance).
        /// </summary>
        public uint OverrideRAMSize { get; set; }

        /// <summary>
        /// The minimum size of large VM heaps in JavaScriptCore. Set this to a
        /// lower value to make these heaps start with a smaller initial value.
        /// </summary>
        public uint MinLargeHeapSize { get; set; }

        /// <summary>
        /// The minimum size of small VM heaps in JavaScriptCore. Set this to a
        /// lower value to make these heaps start with a smaller initial value.
        /// </summary>
        public uint MinSmallHeapSize { get; set; }

        /// <summary>
        /// Create config with default values.
        /// </summary>
        public Config()
            : base(Ultralight.ulCreateConfig())
        {
        }

        protected override void DisposeUnmanaged()
            => Ultralight.ulDestroyConfig(Handle);
    }

    public partial class Ultralight
    {
        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern IntPtr ulCreateConfig();

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulDestroyConfig(IntPtr handle);
    }
}
