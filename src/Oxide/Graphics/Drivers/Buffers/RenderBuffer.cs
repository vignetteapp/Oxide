// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System.Runtime.InteropServices;

namespace Oxide.Graphics.Drivers.Buffers
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RenderBuffer
    {
        public uint TextureId;
        public uint Width;
        public uint Height;

        [MarshalAs(UnmanagedType.I1)]
        public bool HasStencilBuffer;

        [MarshalAs(UnmanagedType.I1)]
        public bool HasDepthBuffer;
    }
}
