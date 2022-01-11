using System.Runtime.InteropServices;

namespace Mikomi.Graphics
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct Vertex_2f_4ub_2f
    {
        public float[] Pos;
        public byte[] Color;
        public float[] Obj;
    }
}
