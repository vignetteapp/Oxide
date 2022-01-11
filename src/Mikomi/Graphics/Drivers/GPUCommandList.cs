using System.Runtime.InteropServices;

namespace Mikomi.Graphics.Drivers
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct GPUCommandList
    {
        public uint Size;
        public GPUCommand[] Commands;
    }
}
