// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System.Runtime.InteropServices;
using Oxide.Graphics.Bitmaps;

namespace Oxide.Graphics.Drivers
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RenderTarget
    {
        public bool IsEmpty;
        public uint Width;
        public uint Height;
        public uint TextureId;
        public uint TextureWidth;
        public uint TextureHeight;
        public BitmapFormat TextureFormat;
        public Rect UVCoords;
        public uint RenderBufferId;
    }
}
