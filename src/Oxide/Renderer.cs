// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System;
using System.Runtime.CompilerServices;
using Oxide.Apps;

namespace Oxide
{
    /// <summary>
    /// Manages the lifetime of all Views and coordinates all
    /// painting, rendering, network requests, and event dispatch.
    /// </summary>
    public sealed class Renderer : DisposableObject
    {
        internal static Renderer Current { get; private set; }

        public readonly Session Session;

        internal Renderer(IntPtr handle)
            : base(handle, false)
        {
        }

        /// <summary>
        /// Create the Ultralight Renderer directly.
        /// </summary>
        /// <remarks>
        /// Unlike <see cref="App"/>, this does not use any native windows for drawing
        /// and allows you to manage your own runloop and painting. This method is
        /// recommended for those wishing to integrate the library into a game.
        /// <br/>
        /// You should only call this once per process lifetime.
        /// <br/>
        /// You shoud set up your platform handlers before calling this.
        /// You will also need to define a font loader before calling this--
        /// call <see cref="AppCore.EnablePlatformFontLoader"/>.
        /// <br/>
        /// You should not call this if you are creating your own app as it
        /// creates its own renderer and provides default implementations for
        /// various platform handlers automatically.
        /// </remarks>
        public Renderer(Config config)
            : base(Ultralight.ulCreateRenderer(config.Handle))
        {
            if (Current != null)
                throw new InvalidOperationException($"An instance of {nameof(Renderer)} already exists.");

            if (App.Current != null)
                throw new InvalidOperationException($"An instance of {nameof(App)} already exists.");

            Current = this;
            Session = new Session(Ultralight.ulDefaultSession(Handle));
        }

        /// <summary>
        /// Render all active Views.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Render()
            => Ultralight.ulRender(Handle);

        /// <summary>
        /// Update timers and dispatch internal callbacks (Javascript and network).
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
        {
            Ultralight.ulDestroyRenderer(Handle);
            Current = null;
        }
    }
}
