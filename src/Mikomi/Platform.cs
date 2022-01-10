using System;
using System.Runtime.InteropServices;
using Mikomi.Graphics;

namespace Mikomi
{
    public class Platform
    {
        public static LogMessageCallback LogMessage
        {
            set => Ultralight.ulPlatformSetLogger(new ULLogger
            {
                LogMessage = new ULLogMessageCallback((level, message) =>
                {
                    using var messageString = new ULString(message);
                    value(level, messageString.Data);
                }),
            });
        }

        public static void SetSurfaceDefinition(SurfaceDefinition surfaceDefinition)
            => Ultralight.ulPlatformSetSurfaceDefinition(surfaceDefinition.Internal);
    }

    public enum ULLogLevel
    {
        Error,
        Warning,
        Info,
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ULLogger
    {
        public ULLogMessageCallback LogMessage;
    }

    public delegate void LogMessageCallback(ULLogLevel level, string message);

    internal delegate void ULLogMessageCallback(ULLogLevel level, IntPtr message);

    public partial class Ultralight
    {
        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulPlatformSetSurfaceDefinition(ULSurfaceDefinition surfaceDefinition);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulPlatformSetLogger(ULLogger logger);
    }
}
