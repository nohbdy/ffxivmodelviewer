using System.Runtime.InteropServices;

using SlimDX;
using SlimDX.Direct3D9;

namespace ModelViewer.Renderer
{
    public class TexturedQuad : Renderable
    {
        VertexBuffer vertexBuffer;
        private string texName;
        private int width;
        private int height;

        public TexturedQuad(string texName, int width, int height)
            : base()
        {
            this.texName = texName;
            this.width = width;
            this.height = height;
        }

        public override void Init()
        {
            base.Init();

            vertexBuffer = new VertexBuffer(
                this.Device,
                4 * Marshal.SizeOf(typeof(TexturedVertex)),
                Usage.WriteOnly,
                VertexFormat.Position | VertexFormat.Texture1,
                Pool.Managed
            );

            try
            {
                var stream = vertexBuffer.Lock(0, 0, LockFlags.None);
                var x = width;
                var y = height;
                stream.WriteRange(new[] {
                    new TexturedVertex( new Vector3(-x, y, 0.5f), new Vector2(0, 0) ),
				    new TexturedVertex( new Vector3(x, y, 0.5f), new Vector2(1, 0) ),
                    new TexturedVertex( new Vector3(-x, -y, 0.5f), new Vector2(0, 1) ),
				    new TexturedVertex( new Vector3(x, -y, 0.5f), new Vector2(1, 1) ),
			    });
            }
            finally
            {
                vertexBuffer.Unlock();
            }
        }

        public override void Render()
        {
            base.Render();

            Device.SetRenderState(RenderState.ZEnable, false);
            Device.SetRenderState(RenderState.AlphaBlendEnable, true);
            Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
            Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);

            Device.SetRenderState(RenderState.AlphaTestEnable, true);
            Device.SetRenderState(RenderState.AlphaFunc, Compare.Greater);
            Device.SetRenderState(RenderState.AlphaRef, 0);

            Device.SetTransform(TransformState.View, RenderSettings.ViewMatrix);
            Device.SetTransform(TransformState.Projection, RenderSettings.ProjectionMatrix);
            Device.VertexFormat = VertexFormat.Position | VertexFormat.Texture1;
            Device.SetRenderState(RenderState.Lighting, false);
            Device.SetStreamSource(0, this.vertexBuffer, 0, Marshal.SizeOf(typeof(TexturedVertex)));
            Device.SetTexture(0, RenderSettings.TextureManager.GetByName(texName));
            Device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
        }

        protected override void Dispose(bool disposeManaged)
        {
            base.Dispose(disposeManaged);

            if (disposeManaged)
            {
                if (vertexBuffer != null) { vertexBuffer.Dispose(); }
            }
        }
    }
}
