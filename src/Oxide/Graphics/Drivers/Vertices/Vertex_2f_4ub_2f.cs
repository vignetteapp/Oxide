using System.Runtime.InteropServices;

namespace Oxide.Graphics.Drivers.Vertices
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex_2f_4ub_2f : IVertex
    {
        [MarshalAs(UnmanagedType.LPArray, SizeConst = 2)]
        public float[] Pos;

        [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)]
        public byte[] Color;

        [MarshalAs(UnmanagedType.LPArray, SizeConst = 2)]
        public float[] Obj;
    }
}
