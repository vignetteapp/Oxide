using System;

namespace Mikomi.Graphics
{
    public interface ISurfaceDefinition
    {
        uint Width { get; }
        uint Height { get; }
        uint RowBytes { get; }
        uint ByteSize { get; }
        void Create(uint width, uint height);
        void Destroy();
        IntPtr LockPixels();
        void UnlockPixels();
        void Resize(uint width, uint height);
    }
}
