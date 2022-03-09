// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System;
using System.Runtime.InteropServices;

namespace Oxide.Graphics.Drivers.Buffers
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct IndexBuffer
    {
        private readonly uint size;
        private readonly byte* data;
        public ReadOnlySpan<byte> Data => new Span<byte>(data, (int)size);
    }
}
