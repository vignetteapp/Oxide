using System;
using System.Runtime.InteropServices;
using Oxide.Graphics.Drivers.Buffers;

namespace Oxide.Graphics.Drivers
{
    internal delegate void GPUDriverBeginSynchronizeCallback();
    internal delegate void GPUDriverEndSynchronizeCallback();
    internal delegate uint GPUDriverNextTextureIdCallback();
    internal delegate void GPUDriverCreateTextureCallback(uint textureId, IntPtr bitmap);
    internal delegate void GPUDriverUpdateTextureCallback(uint textureId, IntPtr bitmap);
    internal delegate void GPUDriverDestroyTextureCallback(uint textureId);
    internal delegate uint GPUDriverNextRenderBufferIdCallback();
    internal delegate void GPUDriverCreateRenderBufferCallback(uint renderBufferId, RenderBuffer buffer);
    internal delegate void GPUDriverDestroyRenderBufferCallback(uint renderBufferId);
    internal delegate uint GPUDriverNextGeometryIdCallback();
    internal delegate void GPUDriverCreateGeometryCallback(uint geometryId, VertexBuffer vertices, IndexBuffer indices);
    internal delegate void GPUDriverUpdateGeometryCallback(uint geometryId, VertexBuffer vertices, IndexBuffer indices);
    internal delegate void GPUDriverDestroyGeometryCallback(uint geometryId);
    internal delegate void GPUDriverUpdateCommandListCallback(GPUCommandList list);

    [StructLayout(LayoutKind.Sequential)]
    internal struct GPUDriver
    {
        public GPUDriverBeginSynchronizeCallback BeginSynchronize;
        public GPUDriverEndSynchronizeCallback EndSynchronize;
        public GPUDriverNextTextureIdCallback NextTextureId;
        public GPUDriverCreateTextureCallback CreateTexture;
        public GPUDriverUpdateTextureCallback UpdateTexture;
        public GPUDriverDestroyTextureCallback DestroyTexture;
        public GPUDriverNextRenderBufferIdCallback NextRenderBufferId;
        public GPUDriverCreateRenderBufferCallback CreateRenderBuffer;
        public GPUDriverDestroyRenderBufferCallback DestroyRenderBuffer;
        public GPUDriverNextGeometryIdCallback NextGeometryId;
        public GPUDriverCreateGeometryCallback CreateGeometry;
        public GPUDriverUpdateGeometryCallback UpdateGeometry;
        public GPUDriverDestroyGeometryCallback DestroyGeometry;
        public GPUDriverUpdateCommandListCallback UpdateCommandList;
    }
}
