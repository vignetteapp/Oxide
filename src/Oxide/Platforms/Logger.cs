// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System.Runtime.InteropServices;
using Oxide.Interop;

namespace Oxide.Platforms
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
    internal struct Logger
    {
        public LoggerMessageCallback LogMessage;
    }
}
