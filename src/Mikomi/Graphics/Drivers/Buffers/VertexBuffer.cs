using System;
using System.Runtime.InteropServices;

namespace Mikomi.Graphics.Drivers.Buffers
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexBuffer
    {
        public VertexBufferFormat Format;
        public uint Size;
        public IntPtr Data;
    }
}
