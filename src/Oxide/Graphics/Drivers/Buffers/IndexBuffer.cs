// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System;
using System.Runtime.InteropServices;

namespace Oxide.Graphics.Drivers.Buffers
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct IndexBuffer
    {
        public readonly uint Count;
        public readonly IntPtr Pointer;
        public ReadOnlySpan<byte> Data => new Span<byte>((void*)Pointer, (int)Count);
    }
}
