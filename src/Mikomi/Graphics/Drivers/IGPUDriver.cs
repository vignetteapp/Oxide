using Mikomi.Graphics.Bitmaps;
using Mikomi.Graphics.Drivers.Buffers;

namespace Mikomi.Graphics.Drivers
{
    public interface IGPUDriver
    {
        void BeginSynchronize();
        void EndSynchronize();
        uint GetNextTextureId();
        void CreateTexture(uint textureId, Bitmap bitmap);
        void UpdateTexture(uint textureId, Bitmap bitmap);
        void DestroyTexture(uint textureId);
        uint GetNextRenderBufferId();
        void CreateRenderBuffer(uint renderBufferId);
        void DestroyRenderBuffer(uint renderBufferId);
        uint GetNextGeometryId();
        void CreateGeometry(uint geometryId, VertexBuffer vertexBuffer, IndexBuffer indexBuffer);
        void UpdateGeometry(uint geometryId, VertexBuffer vertexBuffer, IndexBuffer indexBuffer);
        void DestroyGeometry(uint geometryId, VertexBuffer vertexBuffer, IndexBuffer indexBuffer);
        void UpdateCommandList();
    }
}
