using System;
using System.Runtime.InteropServices;

namespace Mikomi.Graphics.Drivers.Buffers
{
    [StructLayout(LayoutKind.Sequential)]
    public struct IndexBuffer
    {
        public uint Size;
        public IntPtr Data;
    }
}
