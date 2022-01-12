using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Oxide.Graphics.Drivers.Buffers
{
    [StructLayout(LayoutKind.Sequential)]
    public struct IndexBuffer
    {
        private readonly uint size;
        private readonly IntPtr data;

        public IReadOnlyList<byte> Data
        {
            get
            {
                byte[] array = new byte[size];
                Marshal.Copy(data, array, 0, array.Length);
                return array;
            }
        }
    }
}
