// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Oxide.Graphics.Drivers.Vertices;

namespace Oxide.Graphics.Drivers.Buffers
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexBuffer
    {
        public VertexBufferFormat Format;
        private readonly uint count;
        private readonly IntPtr data;

        public IReadOnlyList<IVertex> Data
        {
            get
            {
                if (Format == VertexBufferFormat.Format_2f_4ub_2f_2f_28f)
                    return toVertices<Vertex_2f_4ub_2f_2f_28f>(data, count).Cast<IVertex>().ToList();

                if (Format == VertexBufferFormat.Format_2f_4ub_2f)
                    return toVertices<Vertex_2f_4ub_2f>(data, count).Cast<IVertex>().ToList();

                return null;
            }
        }

        private static T[] toVertices<T>(IntPtr ptr, uint count)
            where T : IVertex
        {
            int size = Marshal.SizeOf<T>();
            var array = new T[count];

            for (int i = 0; i < count; i++)
            {
                var item = new IntPtr(ptr.ToInt64() + i * size);
                array[i] = Marshal.PtrToStructure<T>(item);
            }

            return array;
        }
    }
}
