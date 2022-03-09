// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System;
using System.Runtime.InteropServices;

namespace Oxide.Graphics.Drivers
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct GPUCommandList
    {
        private readonly uint count;
        private readonly void* commands;
        public ReadOnlySpan<GPUCommand> Commands => new Span<GPUCommand>(commands, (int)count);
    }
}
