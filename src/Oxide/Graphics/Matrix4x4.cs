using System.Runtime.InteropServices;

namespace Oxide.Graphics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix4x4
    {
        [MarshalAs(UnmanagedType.LPArray, SizeConst = 16)]
        public float[] Data;
    }

    public static class Matrix4x4Extensions
    {
        /// <summary>
        /// Sets up an orthographic projection matrix with a certain viewport width
        /// and height, multiplies it by 'transform', and returns the result.
        /// <br/>
        /// This should be used to calculate the model-view projection matrix for the
        /// vertex shaders using the current <see cref="GPUState"/>.
        /// </summary>
        public static Matrix4x4 ApplyProjection(this Matrix4x4 matrix, float viewportWidth, float viewportHeight, bool flipY)
            => Ultralight.ulApplyProjection(matrix, viewportWidth, viewportHeight, flipY);
    }
}
