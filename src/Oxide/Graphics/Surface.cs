// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Oxide.Graphics.Bitmaps;

namespace Oxide.Graphics
{
    public sealed class Surface : DisposableObject
    {
        /// <summary>
        /// The surface's width in pixels.
        /// </summary>
        public int Width
        {
            get => (int)Ultralight.ulSurfaceGetWidth(Handle);
            set => Ultralight.ulSurfaceResize(Handle, (uint)value, (uint)Height);
        }

        /// <summary>
        /// The surface's height in pixels.
        /// </summary>
        public int Height
        {
            get => (int)Ultralight.ulSurfaceGetHeight(Handle);
            set => Ultralight.ulSurfaceResize(Handle, (uint)Width, (uint)value);
        }

        /// <summary>
        /// The surface's size in pixels.
        /// </summary>
        public Vector2 Size
        {
            get => new Vector2(Width, Height);
            set => Ultralight.ulSurfaceResize(Handle, (uint)value.X, (uint)value.Y);
        }

        /// <summary>
        /// Number of bytes between rows (usually width * 4)
        /// </summary>
        public uint RowBytes => Ultralight.ulSurfaceGetRowBytes(Handle);

        /// <summary>
        /// Size in bytes.
        /// </summary>
        public uint ByteSize => Ultralight.ulSurfaceGetSize(Handle);

        /// <summary>
        /// Get the underlying Bitmap from the default Surface.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a custom surface is used.
        /// </exception>
        public Bitmap Bitmap
        {
            get
            {
                if (Ultralight.ulSurfaceGetUserData(Handle) != IntPtr.Zero)
                    throw new InvalidOperationException("Cannot get bitmap as a custom surface is defined.");

                return new Bitmap(Ultralight.ulBitmapSurfaceGetBitmap(Handle), false);
            }
        }

        /// <summary>
        /// Gets or sets the dirty bounds.
        /// <br/>
        /// The returned <see cref="RectI"/> can be used to determine which portion of the pixel buffer has
        /// been updated since the last dirty bound clear.
        /// </summary>
        /// <code>
        /// // The general algorithm to determine if a Surface needs display is:
        ///  if (!ulIntRectIsEmpty(ulSurfaceGetDirtyBounds(surface))) {
        ///     // Surface pixels are dirty and needs display.
        ///     // Cast Surface to native Surface and use it here (pseudo code)
        ///     DisplaySurface(surface);
        ///
        ///     // Once you're done, clear the dirty bounds:
        ///     ulSurfaceClearDirtyBounds(surface);
        ///  }
        /// </code>
        public RectI DirtyBounds
        {
            get => Ultralight.ulSurfaceGetDirtyBounds(Handle);
            set => Ultralight.ulSurfaceSetDirtyBounds(Handle, value);
        }

        internal Surface(IntPtr handle)
            : base(handle, true)
        {
        }

        /// <summary>
        /// Lock the pixel buffer and get a pointer to the beginning of the data
        /// for reading/writing.
        /// <br/>
        /// Native pixel format is premultiplied BGRA 32-bit (8 bits per channel).
        /// </summary>
        /// <returns>Pointer to the pixel data.</returns>
        public IntPtr LockPixels()
            => Ultralight.ulSurfaceLockPixels(Handle);

        /// <summary>
        /// Unlock the pixel buffer.
        /// </summary>
        public void UnlockPixels()
            => Ultralight.ulSurfaceUnlockPixels(Handle);

        /// <summary>
        /// Clear the dirty bounds.
        /// <br/>
        /// You should call this after you're done displaying the Surface.
        /// </summary>
        public void ClearDirtyBounds()
            => Ultralight.ulSurfaceClearDirtyBounds(Handle);

        /// <summary>
        /// Gets the underlying <see cref="ISurfaceDefinition"/> defined with <see cref="Platform.SurfaceDefinition"/>
        /// </summary>
        /// <typeparam name="T">The <see cref="ISurfaceDefinition"/> to cast to upon returning.</typeparam>
        /// <returns>The <see cref="ISurfaceDefinition"/> or <see cref="null"/> if a custom definition was not set.</returns>
        public T GetDefiniton<T>()
            where T : class, ISurfaceDefinition
        {
            var definition = Ultralight.ulSurfaceGetUserData(Handle);

            if (definition == IntPtr.Zero)
                return null;

            return ((GCHandle)definition).Target as T;
        }
    }
}
