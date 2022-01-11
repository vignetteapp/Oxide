using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mikomi
{
    /// <summary>
    /// Manages the lifetime of all Views and coordinates all
    /// painting, rendering, network requests, and event dispatch.
    /// </summary>
    public class Renderer : DisposableObject
    {
        public readonly Session Session;

        internal Renderer(IntPtr handle)
            : base(handle, false)
        {
        }

        /// <summary>
        /// Create the Ultralight Renderer directly.
        /// </summary>
        /// <remarks>
        /// Unlike ulCreateApp(), this does not use any native windows for drawing
        /// and allows you to manage your own runloop and painting. This method is
        /// recommended for those wishing to integrate the library into a game.
        /// <br/>
        /// You should only call this once per process lifetime.
        /// <br/>
        /// You shoud set up your platform handlers (eg, ulPlatformSetLogger,
        /// ulPlatformSetFileSystem, etc.) before calling this.
        /// You will also need to define a font loader before calling this--
        /// as of this writing (v1.2) the only way to do this in C API is by calling
        /// ulEnablePlatformFontLoader() (available in <AppCore/CAPI.h>).
        /// <br/>
        /// You should not call this if you are using ulCreateApp(), it
        /// creates its own renderer and provides default implementations for
        /// various platform handlers automatically.
        /// </remarks>
        public Renderer(Config config)
            : base(Ultralight.ulCreateRenderer(config.Handle))
        {
            Session = new Session(Ultralight.ulDefaultSession(Handle));
        }

        /// <summary>
        /// Render all active Views.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Render()
            => Ultralight.ulRender(Handle);

        /// <summary>
        /// Update timers and dispatch internal callbacks (JavaScript and network).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update()
            => Ultralight.ulUpdate(Handle);

        /// <summary>
        /// Attempt to release as much memory as possible. Don't call this from any
        /// callbacks or driver code.
        /// </summary>
        public void PurgeMemory()
            => Ultralight.ulPurgeMemory(Handle);

        /// <summary>
        /// Print detailed memory usage statistics to the log.
        /// </summary>
        public void LogMemoryUsage()
            => Ultralight.ulLogMemoryUsage(Handle);

        protected override void DisposeUnmanaged()
            => Ultralight.ulDestroyRenderer(Handle);
    }

    public partial class Ultralight
    {
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
    }
}
