using System;
using System.Runtime.InteropServices;
using Mikomi.Graphics;
using Mikomi.Platform;

namespace Mikomi.Platform
{
    public class Platform
    {

#pragma warning disable IDE0052 // References are made to prevent garbage collection

        private static Logger logger;
        private static FileSystem fileSystem;
        private static SurfaceDefinition surfaceDefinition;

#pragma warning restore IDE0052

        public static LoggerMessageCallback LogMessage
        {
            set => Ultralight.ulPlatformSetLogger(logger = new Logger { LogMessage = value });
        }

        public static ISurfaceDefinition SurfaceDefinition
        {
            set => Ultralight.ulPlatformSetSurfaceDefinition(surfaceDefinition = new SurfaceDefinition
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

        public static IFileSystem FileSystem
        {
            set => Ultralight.ulPlatformSetFileSystem(fileSystem = new FileSystem
            {
                OpenFile = (p, _) => value.OpenFile(p),
                CloseFile = value.CloseFile,
                FileExists = value.FileExists,
                GetFileSize = value.GetFileSize,
                GetMimeType = value.GetMimeType,
                ReadFromFile = (h, d, l) =>
                {
                    byte[] data = new byte[l];
                    Marshal.Copy(d, data, 0, data.Length);
                    return value.ReadFile(h, new Span<byte>(data));
                },
            });
        }
    }
}

namespace Mikomi
{
    public partial class Ultralight
    {
        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulPlatformSetSurfaceDefinition(SurfaceDefinition surfaceDefinition);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulPlatformSetLogger(Logger logger);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulPlatformSetFileSystem(FileSystem fileSystem);
    }
}
