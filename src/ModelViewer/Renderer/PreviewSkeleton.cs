using System.Drawing;
using DatDigger.Sections.Skeleton;
using SlimDX;
using SlimDX.Direct3D9;

namespace ModelViewer.Renderer
{
    public class PreviewSkeleton : Sample
    {
        private SkeletonSection skeleton;

        public PreviewSkeleton(SkeletonSection skeleton) : base()
        {
            this.skeleton = skeleton;
        }

        protected override void OnInitialize()
        {
            this.globalSettings.BaseFrame = new Renderer.SkeletalFrame(null);
            this.globalSettings.BaseFrame.LoadBones(this.skeleton);
            this.globalSettings.CurrentFrame = new Renderer.SkeletalFrame(this.globalSettings.BaseFrame);
            this.globalSettings.CurrentFrame.LoadBones(this.skeleton);

            this.globalSettings.ViewMatrix = Matrix.LookAtRH(
                new Vector3(0, 0, 10f),
                new Vector3(0, 0, 0f),
                new Vector3(0, 1, 0));
            this.globalSettings.ViewITMatrix = Matrix.Invert(this.globalSettings.ViewMatrix);
            this.globalSettings.ViewITMatrix = Matrix.Transpose(this.globalSettings.ViewITMatrix);

            var visSkel = new Renderer.VisualSkeleton(this.globalSettings.CurrentFrame) {
                Enabled = true
            };
            renderList.Add(visSkel);
            this.globalSettings.ShaderManager.UsePixelShader = false;
        }

        protected override void OnResourceLoad()
        {
            base.OnResourceLoad();

            Context.Device.SetRenderState(RenderState.Lighting, false);
            Context.Device.SetRenderState(RenderState.CullMode, Cull.Clockwise);
            Context.Device.SetRenderState(RenderState.Ambient, Color.Red.ToArgb());
            Context.Device.SetRenderState(RenderState.FillMode, FillMode.Solid);
        }
    }
}
