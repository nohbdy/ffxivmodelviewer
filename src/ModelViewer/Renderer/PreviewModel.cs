using System.Drawing;
using DatDigger.Sections.Model;
using SlimDX;
using SlimDX.Direct3D9;

namespace ModelViewer.Renderer
{
    public class PreviewModel : Sample
    {
        private Model model;

        public PreviewModel(ModelContainerChunk modelContainer)
            : base()
        {
            this.model = new Model(modelContainer);
        }

        protected override void OnInitialize()
        {
            this.globalSettings.IsSkinning = false;
            this.globalSettings.ViewMatrix = Matrix.LookAtRH(
                new Vector3(0, 0, 10f),
                new Vector3(0, 0, 0f),
                new Vector3(0, 1, 0));
            this.globalSettings.ViewITMatrix = Matrix.Invert(this.globalSettings.ViewMatrix);
            this.globalSettings.ViewITMatrix = Matrix.Transpose(this.globalSettings.ViewITMatrix);

            this.globalSettings.PointLightPositions[0] = new Vector4(-100, 100, -100, 1);
            this.globalSettings.PointLightPositions[1] = new Vector4(100, 100, -100, 1);
            this.globalSettings.PointLightColors[0] = new Vector4(1, 1, 1, 1);
            this.globalSettings.PointLightColors[1] = new Vector4(1, 1, 1, 1);
            this.globalSettings.PointLightParameters[0] = new Vector4(1, 1, 3000f, 1);
            this.globalSettings.PointLightParameters[1] = new Vector4(1, 1, 3000f, 1);

            this.globalSettings.DirLightColors[0] = new Vector4(1f, 1f, 1f, 0);
            this.globalSettings.DirLightColors[1] = new Vector4(0.2f, 0.2f, 0.2f, 0);
            this.globalSettings.DirLightDirections[0] = new Vector4(0, -1, 0, 0);
            this.globalSettings.DirLightDirections[1] = new Vector4(1, -1, 0, 0);

            this.globalSettings.ShaderManager.UsePixelShader = false;

            this.renderList.Add(this.model);
            this.model.Init();

            BoundingBox aabb = this.model.BoundingBox;

            sceneOffset.X = -(0.5f * aabb.Maximum.X + 0.5f * aabb.Minimum.X);
            sceneOffset.Y = -(0.5f * aabb.Maximum.Y + 0.5f * aabb.Minimum.Y);
            sceneOffset.Z = -(0.5f * aabb.Maximum.Z + 0.5f * aabb.Minimum.Z);
        }

        protected override void OnResourceLoad()
        {
            base.OnResourceLoad();

            Context.Device.SetRenderState(RenderState.Lighting, false);
            Context.Device.SetRenderState(RenderState.CullMode, Cull.Clockwise);
            Context.Device.SetRenderState(RenderState.Ambient, Color.Red.ToArgb());
            Context.Device.SetRenderState(RenderState.FillMode, FillMode.Solid);
            Context.Device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Diffuse);
            Context.Device.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.SelectArg1);
        }
    }
}
