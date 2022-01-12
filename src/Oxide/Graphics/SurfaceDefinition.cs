using System;
using System.Runtime.InteropServices;

namespace Oxide.Graphics
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct SurfaceDefinition
    {
        public SurfaceDefinitionCreateCallback Create;
        public SurfaceDefinitionDestroyCallback Destroy;
        public SurfaceDefinitionGetWidthCallback GetWidth;
        public SurfaceDefinitionGetHeightCallback GetHeight;
        public SurfaceDefinitionGetRowBytesCallback GetRowBytes;
        public SurfaceDefinitionGetSizeCallback GetSize;
        public SurfaceDefinitionLockPixelsCallback LockPixels;
        public SurfaceDefinitionUnlockPixelsCallback UnlockPixels;
        public SurfaceDefinitionResizeCallback Resize;
    }

    internal delegate IntPtr SurfaceDefinitionCreateCallback(uint width, uint height);
    internal delegate void SurfaceDefinitionDestroyCallback(IntPtr userData);
    internal delegate uint SurfaceDefinitionGetWidthCallback(IntPtr userData);
    internal delegate uint SurfaceDefinitionGetHeightCallback(IntPtr userData);
    internal delegate uint SurfaceDefinitionGetRowBytesCallback(IntPtr userData);
    internal delegate uint SurfaceDefinitionGetSizeCallback(IntPtr userData);
    internal delegate IntPtr SurfaceDefinitionLockPixelsCallback(IntPtr userData);
    internal delegate void SurfaceDefinitionUnlockPixelsCallback(IntPtr userData);
    internal delegate void SurfaceDefinitionResizeCallback(IntPtr userData, uint width, uint height);
}
