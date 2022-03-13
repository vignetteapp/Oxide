// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System;
using System.Runtime.InteropServices;
using Oxide.Graphics.Drivers.Vertices;

namespace Oxide.Graphics.Drivers.Buffers
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct VertexBuffer
    {
        public VertexBufferFormat Format;
        public readonly uint Count;
        public readonly IntPtr Pointer;

        public ReadOnlySpan<Vertex_2f_4ub_2f_2f_28f> Vertex_2f_4ub_2f_2f_28f
        {
            get
            {
                if (Format != VertexBufferFormat.Format_2f_4ub_2f_2f_28f)
                    throw new InvalidOperationException();

                return new Span<Vertex_2f_4ub_2f_2f_28f>((void*)Pointer, (int)Count);
            }
        }

        public ReadOnlySpan<Vertex_2f_4ub_2f> Vertex_2f_4ub_2f
        {
            get
            {
                if (Format != VertexBufferFormat.Format_2f_4ub_2f)
                    throw new InvalidOperationException();

                return new Span<Vertex_2f_4ub_2f>((void*)Pointer, (int)Count);
            }
        }
    }
}
