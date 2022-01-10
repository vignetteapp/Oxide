using System.Runtime.InteropServices;
using Mikomi.Graphics;
using Mikomi.Platform;

namespace Mikomi.Platform
{
    public class Platform
    {
        public static LoggerMessageCallback LogMessage
        {
            set => Ultralight.ulPlatformSetLogger(new Logger
            {
                LogMessage = (level, message) => value(level, message),
            });
        }

        public static ISurfaceDefinition SurfaceDefinition
        {
            set => Ultralight.ulPlatformSetSurfaceDefinition(new SurfaceDefinition
            {
                Create = (w, h) =>
                {
                    value.Create(w, h);
                    return GCHandle.ToIntPtr(GCHandle.Alloc(value, GCHandleType.Normal));
                },
                Destroy = u =>
                {
                    value.Destroy();
                    ((GCHandle)u).Free();
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
}

namespace Mikomi
{
    public partial class Ultralight
    {
        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulPlatformSetSurfaceDefinition(SurfaceDefinition surfaceDefinition);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulPlatformSetLogger(Logger logger);
    }
}
