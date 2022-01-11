using System.Runtime.InteropServices;

namespace Mikomi.Graphics.Drivers
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
