// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using Oxide.Graphics;
using Oxide.Graphics.Fonts;

namespace Oxide
{
    public sealed class Config : DisposableObject
    {
        private string cachePath;

        /// <summary>
        /// Set the file path to a writable directory that will be used to store
        /// cookies, cached resources, and other persistent data.
        /// </summary>
        public string CachePath
        {
            get => cachePath;
            set => Ultralight.ulConfigSetCachePath(Handle, cachePath = value);
        }

        private FaceWinding faceWinding;

        /// <summary>
        /// The winding order for front-facing triangles.
        /// </summary>
        /// <remarks>
        /// This is only used with custom GPUDrivers
        /// </remarks>
        public FaceWinding FaceWinding
        {
            get => faceWinding;
            set => Ultralight.ulConfigSetFaceWinding(Handle, faceWinding = value);
        }

        private FontHinting fontHinting;

        /// <summary>
        /// The hinting algorithm to use when rendering fonts.
        /// </summary>
        public FontHinting FontHinting
        {
            get => fontHinting;
            set => Ultralight.ulConfigSetFontHinting(Handle, fontHinting = value);
        }

        private double fontGamma;

        /// <summary>
        /// The gamma to use when compositing font glyphs, change this value to
        /// adjust contrast (Adobe and Apple prefer 1.8, others may prefer 2.2).
        /// <br/>
        /// (Default = 1.8)
        /// </summary>
        public double FontGamma
        {
            get => fontGamma;
            set => Ultralight.ulConfigSetFontGamma(Handle, fontGamma = value);
        }

        private string userStylesheet;

        /// <summary>
        /// Set user stylesheet (CSS).
        /// <br/>
        /// (Default = Empty)
        /// </summary>
        public string UserStylesheet
        {
            get => userStylesheet;
            set => Ultralight.ulConfigSetUserStyleSheet(Handle, userStylesheet = value);
        }

        private bool forceRepaint;

        /// <summary>
        /// Set whether or not we should continuously repaint any Views or compositor
        /// layers, regardless if they are dirty or not. This is mainly used to
        /// diagnose painting/shader issues.
        /// <br/>
        /// (Default = False)
        /// </summary>
        public bool ForceRepaint
        {
            get => forceRepaint;
            set => Ultralight.ulConfigSetForceRepaint(Handle, forceRepaint = value);
        }

        private double animationTimerDelay;

        /// <summary>
        /// Set the amount of time to wait before triggering another repaint when a
        /// CSS animation is active.
        /// <br/>
        /// (Default = 1.0 / 60.0)
        /// </summary>
        public double AnimationTimerDelay
        {
            get => animationTimerDelay;
            set => Ultralight.ulConfigSetAnimationTimerDelay(Handle, animationTimerDelay = value);
        }

        private double scrollTimerDelay;

        /// <summary>
        /// When a smooth scroll animation is active, the amount of time (in seconds)
        /// to wait before triggering another repaint.
        /// <br/>
        /// (Default = 60 Hz)
        /// </summary>
        public double ScrollTimerDelay
        {
            get => scrollTimerDelay;
            set => Ultralight.ulConfigSetScrollTimerDelay(Handle, scrollTimerDelay = value);
        }

        private double recycleDelay;

        /// <summary>
        /// The amount of time (in seconds) to wait before running the recycler (will
        /// attempt to return excess memory back to the system).
        /// <br/>
        /// (Default = 4.0)
        /// </summary>
        public double RecycleDelay
        {
            get => recycleDelay;
            set => Ultralight.ulConfigSetRecycleDelay(Handle, recycleDelay = value);
        }

        private uint memoryCacheSize;

        /// <summary>
        /// Set the size of WebCore's memory cache for decoded images, scripts, and
        /// other assets in bytes.
        /// <br/>
        /// (Default = 64 * 1024 * 1024)
        /// </summary>
        public uint MemoryCacheSize
        {
            get => memoryCacheSize;
            set => Ultralight.ulConfigSetMemoryCacheSize(Handle, memoryCacheSize = value);
        }

        private uint pageCacheSize;

        /// <summary>
        /// Set the number of pages to keep in the cache.
        /// <br/>
        /// (Default = 0)
        /// </summary>
        public uint PageCacheSize
        {
            get => pageCacheSize;
            set => Ultralight.ulConfigSetPageCacheSize(Handle, pageCacheSize = value);
        }

        private uint overrideRAMSize;

        /// <summary>
        /// JavaScriptCore tries to detect the system's physical RAM size to set
        /// reasonable allocation limits. Set this to anything other than 0 to
        /// override the detected value. Size is in bytes.
        /// <br/>
        /// This can be used to force JavaScriptCore to be more conservative with
        /// its allocation strategy (at the cost of some performance).
        /// </summary>
        public uint OverrideRAMSize
        {
            get => overrideRAMSize;
            set => Ultralight.ulConfigSetOverrideRAMSize(Handle, overrideRAMSize = value);
        }

        private uint minLargeHeapSize;

        /// <summary>
        /// The minimum size of large VM heaps in JavaScriptCore. Set this to a
        /// lower value to make these heaps start with a smaller initial value.
        /// </summary>
        public uint MinLargeHeapSize
        {
            get => minLargeHeapSize;
            set => Ultralight.ulConfigSetMinLargeHeapSize(Handle, minLargeHeapSize = value);
        }

        private uint minSmallHeapSize;

        /// <summary>
        /// The minimum size of small VM heaps in JavaScriptCore. Set this to a
        /// lower value to make these heaps start with a smaller initial value.
        /// </summary>
        public uint MinSmallHeapSize
        {
            get => minSmallHeapSize;
            set => Ultralight.ulConfigSetMinSmallHeapSize(Handle, minSmallHeapSize = value);
        }

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
}
