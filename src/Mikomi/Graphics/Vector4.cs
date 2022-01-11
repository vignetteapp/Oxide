using System.Runtime.InteropServices;

namespace Mikomi.Graphics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector4
    {
        [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)]
        public float[] Value;
    }
}
