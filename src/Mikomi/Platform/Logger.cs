using System;
using System.Runtime.InteropServices;

namespace Mikomi.Platform
{
    public delegate void LoggerMessageCallback(
        LogLevel level,
        [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string message
    );

    public enum LogLevel
    {
        Error,
        Warning,
        Info,
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Logger : IDisposable
    {
        private GCHandle handle { get; set; }
        private LoggerMessageCallback logMessage;

        public LoggerMessageCallback LogMessage
        {
            get => logMessage;
            set => handle = GCHandle.Alloc(logMessage = value, GCHandleType.Normal);
        }

        public void Dispose()
        {
            handle.Free();
        }
    }
}
