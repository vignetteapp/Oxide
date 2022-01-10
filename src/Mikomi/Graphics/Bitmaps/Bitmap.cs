using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Mikomi.Graphics.Bitmaps;

namespace Mikomi.Graphics.Bitmaps
{
    public class Bitmap : DisposableObject, ICloneable
    {
        /// <summary>
        /// Get the width in pixels.
        /// </summary>
        public uint Width => Ultralight.ulBitmapGetWidth(Handle);

        /// <summary>
        /// Get the height in pixels.
        /// </summary>
        public uint Height => Ultralight.ulBitmapGetHeight(Handle);

        /// <summary>
        /// Get the size in pixels.
        /// </summary>
        public Vector2 Size => new Vector2(Width, Height);

        /// <summary>
        /// Get the number of bytes per row.
        /// </summary>
        public uint RowBytes => Ultralight.ulBitmapGetRowBytes(Handle);

        /// <summary>
        /// Get the size in bytes of the underlying pixel buffer.
        /// </summary>
        public uint ByteSize => Ultralight.ulBitmapGetSize(Handle);

        /// <summary>
        /// Get the pixel format.
        /// </summary>
        public BitmapFormat Format => Ultralight.ulBitmapGetFormat(Handle);

        /// <summary>
        /// Get the bytes per pixel.
        /// </summary>
        public uint Bpp => Ultralight.ulBitmapGetBpp(Handle);

        /// <summary>
        /// Whether or not this bitmap owns its own pixel buffer.
        /// </summary>
        public bool OwnsPixels => Ultralight.ulBitmapOwnsPixels(Handle);

        /// <summary>
        /// Get raw pixel buffer-- you should only call this if Bitmap is already locked.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when access is attempted before <see cref="LockPixels"/> is called.
        /// </exception>
        public IntPtr RawPixels
        {
            get
            {
                if (!isLocked)
                    throw new InvalidOperationException("Bitmap must be locked first before getting the pixel data.");

                return Ultralight.ulBitmapRawPixels(Handle);
            }
        }

        /// <summary>
        /// Whether or not this bitmap is empty.
        /// </summary>
        public bool IsEmpty => Ultralight.ulBitmapIsEmpty(Handle);

        internal Bitmap(IntPtr handle, bool owned = true)
            : base(handle, owned)
        {
        }

        /// <summary>
        /// Create empty bitmap.
        /// </summary>
        public Bitmap()
            : base(Ultralight.ulCreateEmptyBitmap())
        {
        }

        /// <summary>
        /// Create bitmap with certain dimensions and pixel format.
        /// </summary>
        /// <param name="width">The width in pixels.</param>
        /// <param name="height">The height in pixels.</param>
        /// <param name="format">The format to use.</param>
        public Bitmap(int width, int height, BitmapFormat format)
            : base(Ultralight.ulCreateBitmap((uint)width, (uint)height, format))
        {
        }

        private bool isLocked;

        /// <summary>
        /// Lock pixels for reading/writing, returns pointer to pixel buffer.
        /// </summary>
        /// <returns>The pixel data of this bitmap.</returns>
        public IntPtr LockPixels()
        {
            isLocked = true;
            return Ultralight.ulBitmapLockPixels(Handle);
        }

        /// <summary>
        /// Unlock pixels after locking.
        /// </summary>
        public void UnlockPixels()
        {
            isLocked = false;
            Ultralight.ulBitmapUnlockPixels(Handle);
        }

        /// <summary>
        /// Reset bitmap pixels to 0.
        /// </summary>
        public void Clear()
            => Ultralight.ulBitmapErase(Handle);

        /// <summary>
        /// Create bitmap from copy.
        /// </summary>
        /// <returns>The previous bitmap as a copy.</returns>
        public Bitmap Clone()
            => new Bitmap(Ultralight.ulCreateBitmapFromCopy(Handle));

        /// <summary>
        /// Write bitmap to a PNG on disk.
        /// </summary>
        /// <param name="path">The path where to save the image.</param>
        /// <returns>Whether it has successfully written to disk or not.</returns>
        public bool SaveAsPNG(string path)
            => Ultralight.ulBitmapWritePNG(Handle, path);

        /// <summary>
        /// This converts a BGRA bitmap to RGBA bitmap and vice-versa by swapping
        /// the red and blue channels.
        /// </summary>
        public void SwapRedBlueChannels()
            => Ultralight.ulBitmapSwapRedBlueChannels(Handle);

        object ICloneable.Clone()
            => Clone();

        protected override void DisposeUnmanaged()
            => Ultralight.ulDestroyBitmap(Handle);
    }
}

namespace Mikomi
{

#pragma warning disable CA2101 // Custom marshaler is used

    public partial class Ultralight
    {
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
    }

#pragma warning restore CA2101

}
