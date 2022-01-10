using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Mikomi.Graphics
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct SurfaceDefinition : IDisposable
    {
        private static List<GCHandle> handles { get; } = new List<GCHandle>();

        private SurfaceDefinitionCreateCallback create;
        private SurfaceDefinitionDestroyCallback destroy;
        private SurfaceDefinitionGetWidthCallback getWidth;
        private SurfaceDefinitionGetHeightCallback getHeight;
        private SurfaceDefinitionGetRowBytesCallback getRowBytes;
        private SurfaceDefinitionGetSizeCallback getSize;
        private SurfaceDefinitionLockPixelsCallback lockPixels;
        private SurfaceDefinitionUnlockPixelsCallback unlockPixels;
        private SurfaceDefinitionResizeCallback resize;

        public SurfaceDefinitionCreateCallback Create
        {
            get => create;
            set => handles.Add(GCHandle.Alloc(create = value, GCHandleType.Normal));
        }

        public SurfaceDefinitionDestroyCallback Destroy
        {
            get => destroy;
            set => handles.Add(GCHandle.Alloc(destroy = value, GCHandleType.Normal));
        }

        public SurfaceDefinitionGetWidthCallback GetWidth
        {
            get => getWidth;
            set => handles.Add(GCHandle.Alloc(getWidth = value, GCHandleType.Normal));
        }

        public SurfaceDefinitionGetHeightCallback GetHeight
        {
            get => getHeight;
            set => handles.Add(GCHandle.Alloc(getHeight = value, GCHandleType.Normal));
        }

        public SurfaceDefinitionGetRowBytesCallback GetRowBytes
        {
            get => getRowBytes;
            set => handles.Add(GCHandle.Alloc(getRowBytes = value, GCHandleType.Normal));
        }

        public SurfaceDefinitionGetSizeCallback GetSize
        {
            get => getSize;
            set => handles.Add(GCHandle.Alloc(getSize = value, GCHandleType.Normal));
        }

        public SurfaceDefinitionLockPixelsCallback LockPixels
        {
            get => lockPixels;
            set => handles.Add(GCHandle.Alloc(lockPixels = value, GCHandleType.Normal));
        }

        public SurfaceDefinitionUnlockPixelsCallback UnlockPixels
        {
            get => unlockPixels;
            set => handles.Add(GCHandle.Alloc(unlockPixels = value, GCHandleType.Normal));
        }

        public SurfaceDefinitionResizeCallback Resize
        {
            get => resize;
            set => handles.Add(GCHandle.Alloc(resize = value, GCHandleType.Normal));
        }

        public void Dispose()
        {
            foreach (var handle in handles)
            {
                handle.Free();
                handles.Remove(handle);
            }
        }
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
