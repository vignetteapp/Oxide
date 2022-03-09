// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System.Runtime.InteropServices;

namespace Oxide.Graphics.Drivers
{
    [StructLayout(LayoutKind.Sequential)]
    public struct GPUCommand
    {
        [MarshalAs(UnmanagedType.U1)]
        public GPUCommandType CommandType;
        public GPUState State;
        public uint GeometryId;
        public uint IndexCount;
        public uint IndexOffset;
    }
}
