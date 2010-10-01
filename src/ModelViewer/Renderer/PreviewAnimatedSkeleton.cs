using System.Drawing;
using DatDigger;
using DatDigger.Sections.Animation;
using DatDigger.Sections.Skeleton;
using SlimDX;
using SlimDX.Direct3D9;

namespace ModelViewer.Renderer
{
    public class PreviewAnimatedSkeleton : Sample
    {
        private SkeletonSection skeleton;
        private MtbSection animationData;
        private AnimatedSkeleton animatedSkeleton;

        public PreviewAnimatedSkeleton(SkeletonSection skeleton) : base()
        {
            this.skeleton = skeleton;

            string skeletonFilePath = skeleton.GetParent<DatDigger.Sections.PwibSection>().FilePath;
            skeletonFilePath = System.IO.Path.GetDirectoryName(skeletonFilePath);
            skeletonFilePath = skeletonFilePath.Substring(0, skeletonFilePath.LastIndexOfAny(new char[] { '\\', '/' }));

            string animationFilePath = System.IO.Path.Combine(skeletonFilePath, "act/emp_emp/bid/base/0000");
            if (!System.IO.File.Exists(animationFilePath))
            {
                throw new System.InvalidOperationException("File " + animationFilePath + " does not exist");
            }

            var animFile = DatDigger.Sections.SectionLoader.OpenFile(animationFilePath);
            animationData = animFile.FindChild<MtbSection>(x => x.ResourceId == "cbnm_id0");
            if (animationData == null)
            {
                throw new System.InvalidOperationException("Cannot find cbnm_id0");
            }
        }

        protected override void OnInitialize()
        {
            this.globalSettings.BaseFrame = new Renderer.SkeletalFrame(null);
            this.globalSettings.BaseFrame.LoadBones(this.skeleton);
            animatedSkeleton = new Renderer.AnimatedSkeleton(this.globalSettings.BaseFrame, animationData);
            this.globalSettings.CurrentFrame = animatedSkeleton;
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

        protected override void OnUpdate()
        {
            base.OnUpdate();

            animatedSkeleton.Update(this.FrameDelta);
        }
    }
}
