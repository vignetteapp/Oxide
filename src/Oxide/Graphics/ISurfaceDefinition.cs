using System;

namespace Oxide.Graphics
{
    public interface ISurfaceDefinition
    {
        /// <summary>
        /// The surface's width in pixels.
        /// </summary>
        uint Width { get; }

        /// <summary>
        /// The surface's height in pixels.
        /// </summary>
        uint Height { get; }

        /// <summary>
        /// The surface's row bytes.
        /// </summary>
        uint RowBytes { get; }

        /// <summary>
        /// The surface's size in bytes.
        /// </summary>
        uint ByteSize { get; }

        /// <summary>
        /// Called when a new surface with this definition is created.
        /// </summary>
        /// <param name="width">The width in pixels.</param>
        /// <param name="height">The height in pixels.</param>
        void Create(uint width, uint height);

        /// <summary>
        /// Called when the surface with this definition is to be destroyed.
        /// </summary>
        void Destroy();

        /// <summary>
        /// Called when the surface's pixels have to be locked.
        /// </summary>
        /// <returns>The surface's pixels.</returns>
        IntPtr LockPixels();

        /// <summary>
        /// Called when the surface's pixels have to be unlocked.
        /// </summary>
        void UnlockPixels();

        /// <summary>
        /// Called when a surface has to be resized.
        /// </summary>
        /// <param name="width">The width in pixels.</param>
        /// <param name="height">The height in pixels.</param>
        void Resize(uint width, uint height);
    }
}
