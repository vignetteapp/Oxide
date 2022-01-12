using Oxide.Graphics.Bitmaps;
using Oxide.Graphics.Drivers.Buffers;

namespace Oxide.Graphics.Drivers
{
    public interface IGPUDriver
    {
        /// <summary>
        /// The callback invoked when the GPUDriver will begin dispatching commands
        /// (such as CreateTexture and UpdateCommandList) during the current call to
        /// <see cref="Renderer.Render"/>
        /// </summary>
        void BeginSynchronize();

        /// <summary>
        /// The callback invoked when the GPUDriver has finished dispatching commands
        /// during the current call to <see cref="Renderer.Render"/>
        /// </summary>
        void EndSynchronize();

        /// <summary>
        /// The callback invoked when the GPUDriver wants to get the next available
        /// texture ID.
        /// </summary>
        uint GetNextTextureId();

        /// <summary>
        /// The callback invoked when the GPUDriver wants to create a texture with a
        /// certain ID and optional bitmap.
        /// <br/>
        /// If <see cref="Bitmap.IsEmpty"/>, then a RTT Texture should be created instead.
        /// This will be used as a backing texture for a new RenderBuffer.
        /// </summary>
        void CreateTexture(uint textureId, Bitmap bitmap);

        /// <summary>
        /// The callback invoked when the GPUDriver wants to update an existing non-RTT
        /// texture with new bitmap data.
        /// </summary>
        void UpdateTexture(uint textureId, Bitmap bitmap);

        /// <summary>
        /// The callback invoked when the GPUDriver wants to destroy a texture.
        /// </summary>
        void DestroyTexture(uint textureId);

        /// <summary>
        /// The callback invoked when the GPUDriver wants to generate the next
        /// available render buffer ID.
        /// </summary>
        uint GetNextRenderBufferId();

        /// <summary>
        /// The callback invoked when the GPUDriver wants to create a render buffer
        /// with certain ID and buffer description.
        /// </summary>
        /// <param name="renderBufferId"></param>
        /// <param name="renderBuffer"></param>
        void CreateRenderBuffer(uint renderBufferId, RenderBuffer renderBuffer);

        /// <summary>
        /// The callback invoked when the GPUDriver wants to destroy a render buffer
        /// </summary>
        void DestroyRenderBuffer(uint renderBufferId);

        /// <summary>
        /// The callback invoked when the GPUDriver wants to generate the next
        /// available geometry ID.
        /// </summary>
        uint GetNextGeometryId();

        /// <summary>
        /// The callback invoked when the GPUDriver wants to create geometry with
        /// certain ID and vertex/index data.
        /// </summary>
        void CreateGeometry(uint geometryId, VertexBuffer vertexBuffer, IndexBuffer indexBuffer);

        /// <summary>
        /// The callback invoked when the GPUDriver wants to update existing geometry
        /// with new vertex/index data.
        /// </summary>
        void UpdateGeometry(uint geometryId, VertexBuffer vertexBuffer, IndexBuffer indexBuffer);

        /// <summary>
        /// The callback invoked when the GPUDriver wants to destroy geometry.
        /// </summary>
        void DestroyGeometry(uint geometryId);

        /// <summary>
        /// The callback invoked when the GPUDriver wants to update the command list.
        /// </summary>
        void UpdateCommandList();
    }
}
