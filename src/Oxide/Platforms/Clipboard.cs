// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System.Runtime.InteropServices;
using Oxide.Interop;

namespace Oxide.Platforms
{
    internal delegate void ClipboardClearCallback();
    internal delegate void ClipboardReadPlainTextCallback([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] out string text);
    internal delegate void ClipboardWritePlainTextCallback([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string text);

    [StructLayout(LayoutKind.Sequential)]
    internal struct Clipboard
    {
        public ClipboardClearCallback Clear;
        public ClipboardReadPlainTextCallback ReadPlainText;
        public ClipboardWritePlainTextCallback WritePlainText;
    }
}
