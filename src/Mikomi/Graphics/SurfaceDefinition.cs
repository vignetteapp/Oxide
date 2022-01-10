using System;
using System.Runtime.InteropServices;

namespace Mikomi.Graphics
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct ULSurfaceDefinition
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
    internal delegate void SurfaceDefinitionGetSizeCallback(IntPtr userData);
    internal delegate IntPtr SurfaceDefinitionLockPixelsCallback(IntPtr userData);
    internal delegate void SurfaceDefinitionUnlockPixelsCallback(IntPtr userData);
    internal delegate void SurfaceDefinitionResizeCallback(IntPtr userData, uint width, uint height);

    public abstract class SurfaceDefinition
    {
        public abstract uint Width { get; }
        public abstract uint Height { get; }
        public abstract uint RowBytes { get; }

        public abstract void Create(uint width, uint height);
        public abstract void Destroy();
        public abstract void GetSize();
        public abstract IntPtr LockPixels();
        public abstract void UnlockPixels();
        public abstract void Resize(uint width, uint height);

        private GCHandle handle;

        internal ULSurfaceDefinition Internal => new ULSurfaceDefinition
        {
            Create = new SurfaceDefinitionCreateCallback(CreateInternal),
            Destroy = new SurfaceDefinitionDestroyCallback(DestroyInternal),
            GetWidth = new SurfaceDefinitionGetWidthCallback(_ => getFunc(nameof(Width)).Invoke()),
            GetHeight = new SurfaceDefinitionGetHeightCallback(_ => getFunc(nameof(Height)).Invoke()),
            GetRowBytes = new SurfaceDefinitionGetRowBytesCallback(_ => getFunc(nameof(RowBytes)).Invoke()),
            GetSize = new SurfaceDefinitionGetSizeCallback(_ => GetSize()),
            LockPixels = new SurfaceDefinitionLockPixelsCallback(_ => LockPixels()),
            UnlockPixels = new SurfaceDefinitionUnlockPixelsCallback(_ => UnlockPixels()),
            Resize = new SurfaceDefinitionResizeCallback((_, w, h) => Resize(w, h)),
        };

        protected internal IntPtr CreateInternal(uint width, uint height)
        {
            Create(width, height);
            return GCHandle.ToIntPtr(handle = GCHandle.Alloc(this, GCHandleType.Pinned));
        }

        protected internal void DestroyInternal(IntPtr _)
            => handle.Free();

        private Func<uint> getFunc(string name)
            => GetType().GetProperty(name).GetGetMethod().CreateDelegate<Func<uint>>();
    }
}
