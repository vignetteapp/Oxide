using System;
using System.Runtime.InteropServices;
using Oxide.Graphics;
using Oxide.Graphics.Bitmaps;
using Oxide.Graphics.Drivers;

namespace Oxide.Platforms
{
    public class Platform
    {

#pragma warning disable IDE0052 // References are made to prevent garbage collection

        private static Logger logger;
        private static GPUDriver gpuDriver;
        private static IGPUDriver gpuDriverImpl;
        private static Clipboard plClipboard;
        private static IClipboard plClipboardImpl;
        private static FileSystem plFileSystem;
        private static IFileSystem plFileSystemImpl;
        private static SurfaceDefinition surfaceDefinition;
        private static ISurfaceDefinition surfaceImpl;

#pragma warning restore IDE0052

        /// <summary>
        /// Sets the logger callback.
        /// </summary>
        /// <param name="callback">The callback.</param>
        public static void SetLogger(LoggerMessageCallback callback)
            => Ultralight.ulPlatformSetLogger(logger = new Logger { LogMessage = callback });

        /// <summary>
        /// Sets the surface definition.
        /// <br/>
        /// Used when <see cref="ViewConfig.IsAccelerated"/> = false.
        /// </summary>
        /// <param name="definition">The surface definition to use.</param>
        public static void SetSurfaceDefinition(ISurfaceDefinition definition)
        {
            surfaceImpl = definition;
            Ultralight.ulPlatformSetSurfaceDefinition(surfaceDefinition = new SurfaceDefinition
            {
                Create = (w, h) =>
                {
                    definition.Create(w, h);
                    return GCHandle.ToIntPtr(GCHandle.Alloc(definition, GCHandleType.Normal));
                },
                Destroy = u =>
                {
                    definition.Destroy();
                    GCHandle.FromIntPtr(u).Free();
                },
                GetSize = _ => definition.ByteSize,
                GetWidth = _ => definition.Width,
                GetHeight = _ => definition.Height,
                GetRowBytes = _ => definition.RowBytes,
                LockPixels = _ => definition.LockPixels(),
                UnlockPixels = _ => definition.UnlockPixels(),
                Resize = (_, w, h) => definition.Resize(w, h),
            });
        }

        /// <summary>
        /// Sets the GPU Driver.
        /// <br/>
        /// Used when <see cref="ViewConfig.IsAccelerated"/> = true.
        /// </summary>
        /// <param name="driver">The driver to use.</param>
        public static void SetGPUDriver(IGPUDriver driver)
        {
            gpuDriverImpl = driver;
            Ultralight.ulPlatformSetGPUDriver(gpuDriver = new GPUDriver
            {
                BeginSynchronize = driver.BeginSynchronize,
                EndSynchronize = driver.EndSynchronize,
                NextTextureId = driver.GetNextTextureId,
                CreateTexture = (i, b) => driver.CreateTexture(i, new Bitmap(b, false)),
                UpdateTexture = (i, b) => driver.UpdateTexture(i, new Bitmap(b, false)),
                DestroyTexture = driver.DestroyTexture,
                NextGeometryId = driver.GetNextGeometryId,
                CreateGeometry = driver.CreateGeometry,
                UpdateGeometry = driver.UpdateGeometry,
                DestroyGeometry = driver.DestroyGeometry,
                NextRenderBufferId = driver.GetNextRenderBufferId,
                CreateRenderBuffer = driver.CreateRenderBuffer,
                DestroyRenderBuffer = driver.DestroyRenderBuffer,
            });
        }

        /// <summary>
        /// Sets the platform file system.
        /// </summary>
        /// <param name="filesystem">The filesystem to use.</param>
        public unsafe static void SetFileSystem(IFileSystem filesystem)
        {
            plFileSystemImpl = filesystem;
            Ultralight.ulPlatformSetFileSystem(plFileSystem = new FileSystem
            {
                OpenFile = (p, _) => filesystem.OpenFile(p),
                CloseFile = filesystem.CloseFile,
                FileExists = filesystem.FileExists,
                GetFileSize = filesystem.GetFileSize,
                GetMimeType = filesystem.GetMimeType,
                ReadFromFile = (h, d, l) => filesystem.ReadFile(h, new Span<byte>(d, (int)l)),
            });
        }

        /// <summary>
        /// Sets the platform clipboard.
        /// </summary>
        /// <param name="clipboard">The clipboard to use.</param>
        public static void SetClipboard(IClipboard clipboard)
        {
            plClipboardImpl = clipboard;
            Ultralight.ulPlatformSetClipboard(plClipboard = new Clipboard
            {
                Clear = clipboard.Clear,
                WritePlainText = clipboard.SetText,
                ReadPlainText = (out string str) => str = clipboard.GetText(),
            });
        }
    }
}
