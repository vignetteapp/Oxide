// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

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
        private static Clipboard clipboard;
        private static IClipboard clipboardImpl;
        private static FileSystem fileSystem;
        private static IFileSystem fileSystemImpl;
        private static SurfaceDefinition surfaceDefinition;
        private static ISurfaceDefinition surfaceImpl;

#pragma warning restore IDE0052

        /// <summary>
        /// Sets the logger callback.
        /// </summary>
        /// <param name="callback">The callback.</param>
        public static LoggerMessageCallback Logger
        {
            set => Ultralight.ulPlatformSetLogger(logger = new Logger { LogMessage = value });
        }

        /// <summary>
        /// Sets the surface definition.
        /// <br/>
        /// Used when <see cref="ViewConfig.IsAccelerated"/> = false.
        /// </summary>
        /// <param name="definition">The surface definition to use.</param>
        public static ISurfaceDefinition SetSurfaceDefinition
        {
            set
            {
                surfaceImpl = value;
                Ultralight.ulPlatformSetSurfaceDefinition(surfaceDefinition = new SurfaceDefinition
                {
                    Create = (w, h) =>
                    {
                        value.Create(w, h);
                        return GCHandle.ToIntPtr(GCHandle.Alloc(value, GCHandleType.Normal));
                    },
                    Destroy = u =>
                    {
                        value.Destroy();
                        GCHandle.FromIntPtr(u).Free();
                    },
                    GetSize = _ => value.ByteSize,
                    GetWidth = _ => value.Width,
                    GetHeight = _ => value.Height,
                    GetRowBytes = _ => value.RowBytes,
                    LockPixels = _ => value.LockPixels(),
                    UnlockPixels = _ => value.UnlockPixels(),
                    Resize = (_, w, h) => value.Resize(w, h),
                });
            }
        }

        /// <summary>
        /// Sets the GPU Driver.
        /// <br/>
        /// Used when <see cref="ViewConfig.IsAccelerated"/> = true.
        /// </summary>
        /// <param name="driver">The driver to use.</param>
        public static IGPUDriver SetGPUDriver
        {
            set
            {
                gpuDriverImpl = value;
                Ultralight.ulPlatformSetGPUDriver(gpuDriver = new GPUDriver
                {
                    BeginSynchronize = value.BeginSynchronize,
                    EndSynchronize = value.EndSynchronize,
                    NextTextureId = value.GetNextTextureId,
                    CreateTexture = (i, b) => value.CreateTexture(i, new Bitmap(b, false)),
                    UpdateTexture = (i, b) => value.UpdateTexture(i, new Bitmap(b, false)),
                    DestroyTexture = value.DestroyTexture,
                    NextGeometryId = value.GetNextGeometryId,
                    CreateGeometry = value.CreateGeometry,
                    UpdateGeometry = value.UpdateGeometry,
                    DestroyGeometry = value.DestroyGeometry,
                    NextRenderBufferId = value.GetNextRenderBufferId,
                    CreateRenderBuffer = value.CreateRenderBuffer,
                    DestroyRenderBuffer = value.DestroyRenderBuffer,
                });
            }
        }

        /// <summary>
        /// Sets the platform file system.
        /// </summary>
        /// <param name="filesystem">The filesystem to use.</param>
        public unsafe static IFileSystem FileSystem
        {
            set
            {
                fileSystemImpl = value;
                Ultralight.ulPlatformSetFileSystem(fileSystem = new FileSystem
                {
                    OpenFile = (p, _) => value.OpenFile(p),
                    CloseFile = value.CloseFile,
                    FileExists = value.FileExists,
                    GetFileSize = value.GetFileSize,
                    GetMimeType = value.GetMimeType,
                    ReadFromFile = (h, d, l) => value.ReadFile(h, new Span<byte>(d, (int)l)),
                });
            }
        }

        /// <summary>
        /// Sets the platform clipboard.
        /// </summary>
        /// <param name="clipboard">The clipboard to use.</param>
        public static IClipboard Clipboard
        {
            set
            {
                clipboardImpl = value;
                Ultralight.ulPlatformSetClipboard(clipboard = new Clipboard
                {
                    Clear = value.Clear,
                    WritePlainText = value.SetText,
                    ReadPlainText = (out string str) => str = value.GetText(),
                });
            }
        }
    }
}
