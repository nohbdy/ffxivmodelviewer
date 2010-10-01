using SlimDX;
using SlimDX.Direct3D9;

namespace ModelViewer.Renderer
{
    public class VisualSkeleton : Renderable
    {
        private DynamicPrimitiveBuffer9<ColoredVertex> lineBuffer;
        private ISkeleton skeletonFrame;
        private StateBlock stateBlock;
        public bool Enabled { get; set; }

        public VisualSkeleton(ISkeleton skeleton)
        {
            this.skeletonFrame = skeleton;
        }

        public override void BeginRender()
        {
            if (!Enabled) { return; }

            base.BeginRender();

            RenderSettings.CurrentFrame.CalculateJointMatrices();
            Renderer.Bone[] bones = RenderSettings.CurrentFrame.Bones;

            int color = System.Drawing.Color.FromArgb(50, System.Drawing.Color.Honeydew).ToArgb();

            Vector3[] positions = new Vector3[bones.Length];
            for (var i = 0; i < bones.Length; i++)
            {
                Vector3 startPos;
                if (bones[i].ParentId < 0)
                {
                    startPos = new Vector3(0, 0, 0);
                }
                else
                {
                    startPos = positions[bones[i].ParentId];
                }
                positions[i] = bones[i].Position;
                lineBuffer.Add(new ColoredVertex(startPos, color));
                lineBuffer.Add(new ColoredVertex(positions[i], color));
            }

            lineBuffer.Commit();
        }

        public override void EndRender()
        {
            if (!Enabled) { return; }

            try
            {
                stateBlock.Capture();

                Device.SetTransform(TransformState.World, RenderSettings.WorldMatrix);
                Device.SetTransform(TransformState.View, RenderSettings.ViewMatrix);
                Device.SetTransform(TransformState.Projection, RenderSettings.ProjectionMatrix);
                Device.SetRenderState(RenderState.AlphaBlendEnable, true);
                Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
                Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
                Device.SetRenderState(RenderState.ZFunc, Compare.Always);
                Device.SetRenderState(RenderState.ZWriteEnable, false);
                Device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Diffuse);
                Device.SetTextureStageState(0, TextureStage.ColorArg2, TextureArgument.Current);
                Device.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.SelectArg1);
                Device.VertexShader = null;
                Device.PixelShader = null;
                Device.VertexFormat = VertexFormat.Position | VertexFormat.Diffuse;
                Device.SetStreamSource(0, lineBuffer.UnderlyingBuffer, 0, lineBuffer.ElementSize);
                Device.DrawPrimitives(PrimitiveType.LineList, 0, lineBuffer.Count / 2);
                lineBuffer.Clear();
            }
            finally
            {
                stateBlock.Apply();
            }
        }

        public override void ResourceLoad()
        {
            base.ResourceLoad();

            stateBlock = new StateBlock(Device, StateBlockType.All);
            lineBuffer = new DynamicPrimitiveBuffer9<ColoredVertex>(Device);
        }

        public override void ResourceUnload()
        {
            base.ResourceUnload();

            if (stateBlock != null)
            {
                stateBlock.Dispose();
                stateBlock = null;
            }

            if (lineBuffer != null)
            {
                lineBuffer.Dispose();
                lineBuffer = null;
            }
        }

        protected override void Dispose(bool disposeManaged)
        {
            base.Dispose(disposeManaged);

            if (stateBlock != null)
            {
                stateBlock.Dispose();
                lineBuffer = null;
            }

            if (lineBuffer != null)
            {
                lineBuffer.Dispose();
                lineBuffer = null;
            }
        }
    }
}
