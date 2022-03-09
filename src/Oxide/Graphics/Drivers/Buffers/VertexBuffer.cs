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
        private readonly uint count;
        private readonly void* data;

        public ReadOnlySpan<Vertex_2f_4ub_2f_2f_28f> Vertex_2f_4ub_2f_2f_28f
        {
            get
            {
                if (Format != VertexBufferFormat.Format_2f_4ub_2f_2f_28f)
                    throw new InvalidOperationException();

                return new Span<Vertex_2f_4ub_2f_2f_28f>(data, (int)count);
            }
        }

        public ReadOnlySpan<Vertex_2f_4ub_2f> Vertex_2f_4ub_2f
        {
            get
            {
                if (Format != VertexBufferFormat.Format_2f_4ub_2f_2f_28f)
                    throw new InvalidOperationException();

                return new Span<Vertex_2f_4ub_2f>(data, (int)count);
            }
        }
    }
}
