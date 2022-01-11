using System.Runtime.InteropServices;

namespace Mikomi.Graphics.Drivers.Vertices
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex_2f_4ub_2f_2f_28f : IVertex
    {
        [MarshalAs(UnmanagedType.LPArray, SizeConst = 2)]
        public float[] Pos;

        [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)]
        public byte[] Color;

        [MarshalAs(UnmanagedType.LPArray, SizeConst = 2)]
        public float[] Tex;

        [MarshalAs(UnmanagedType.LPArray, SizeConst = 2)]
        public float[] Obj;

        [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)]
        public float[] Data0;

        [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)]
        public float[] Data1;

        [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)]
        public float[] Data2;

        [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)]
        public float[] Data3;

        [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)]
        public float[] Data4;

        [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)]
        public float[] Data5;

        [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)]
        public float[] Data6;
    }
}
