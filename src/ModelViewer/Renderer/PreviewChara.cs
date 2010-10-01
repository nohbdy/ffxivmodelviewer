using System.Collections.Generic;
using DatDigger;
using DatDigger.Sections.Model;
using DatDigger.Sections.Texture;
using SlimDX;
using SlimDX.Direct3D9;

namespace ModelViewer.Renderer
{
    public class PreviewChara : Sample
    {
        private CharaModelData modelData;

        private Model[] models;
        private VisualSkeleton skeleton;
        private int maxNumModels;
        private List<List<DatDigger.Sections.Texture.TextureSection>>[] textures;
        private int[] textureIndices = new int[(int)ModelPart.NumModelParts];
        private Element[] textureUI = new Element[(int)ModelPart.NumModelParts];

        public PreviewChara(CharaModelData modelData)
            : base()
        {
            maxNumModels = (int)ModelPart.NumModelParts;
            this.models = new Model[maxNumModels];

            this.modelData = modelData;
            this.textures = new List<List<TextureSection>>[maxNumModels];

            for (var i = 0; i < (int)ModelPart.NumModelParts; i++)
            {
                if (this.modelData.Models[i] == null)
                {
                    continue;
                }

                var modelContainer = this.modelData.Models[i].FindChild<ModelContainerChunk>();
                var model = new Model(modelContainer);
                this.models[i] = model;
                this.renderList.Add(model);

                var textureList = this.modelData.Textures[i];

                this.textures[i] = new List<List<TextureSection>>();
                for (var j = 0; j < textureList.Count; j++)
                {
                    var texSections = this.modelData.Textures[i][j].FindAllChildren<TextureSection>();
                    this.textures[i].Add(texSections);
                }
            }
        }

        protected override void OnKeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            base.OnKeyUp(sender, e);

            int idx = -1;
            switch (e.KeyCode)
            {
                case System.Windows.Forms.Keys.D1: idx = 0; break;
                case System.Windows.Forms.Keys.D2: idx = 1; break;
                case System.Windows.Forms.Keys.D3: idx = 2; break;
                case System.Windows.Forms.Keys.D4: idx = 3; break;
                case System.Windows.Forms.Keys.D5: idx = 4; break;
                case System.Windows.Forms.Keys.D6: idx = 5; break;
                case System.Windows.Forms.Keys.X:
                    {
                        skeleton.Enabled = !skeleton.Enabled;
                        return;
                    }
                case System.Windows.Forms.Keys.P:
                    {
                        this.globalSettings.RenderPgrps = !this.globalSettings.RenderPgrps;
                        return;
                    }
            }

            if (idx < 0) { return; }
            if (this.textures[idx] == null) { return; }

            int newTexIdx = this.textureIndices[idx];

            if (e.Shift)
            {
                this.models[idx].Enabled = !this.models[idx].Enabled;
            }
            else
            {
                if (!this.models[idx].Enabled) { return; }
                if (this.textures[idx].Count <= 1) { return; }

                newTexIdx = (newTexIdx + 1) % this.textures[idx].Count;
                this.textureIndices[idx] = newTexIdx;

                this.globalSettings.TextureManager.AddRange(this.textures[idx][newTexIdx]);
            }

            if (this.models[idx].Enabled)
            {
                this.textureUI[idx].Label = string.Format("[{3}] {0} - {1}/{2}", (ModelPart)idx, newTexIdx + 1, this.textures[idx].Count, idx + 1);
            }
            else
            {
                this.textureUI[idx].Label = string.Format("[{1}] {0} - Hidden", (ModelPart)idx, idx + 1);
            }
        }

        protected override void OnInitialize()
        {
            this.globalSettings.IsSkinning = true;

            var skel = this.modelData.Skeleton.FindChild<DatDigger.Sections.Skeleton.SkeletonSection>();
            this.globalSettings.BaseFrame = new Renderer.SkeletalFrame(null);
            this.globalSettings.BaseFrame.LoadBones(skel);
            this.globalSettings.CurrentFrame = new Renderer.SkeletalFrame(this.globalSettings.BaseFrame);
            this.globalSettings.CurrentFrame.LoadBones(skel);
            

            this.globalSettings.ViewMatrix = Matrix.LookAtRH(
                new Vector3(0, 0, 10f),
                new Vector3(0, 0, 0f),
                new Vector3(0, 1, 0));
            this.globalSettings.ViewITMatrix = Matrix.Invert(this.globalSettings.ViewMatrix);
            this.globalSettings.ViewITMatrix = Matrix.Transpose(this.globalSettings.ViewITMatrix);

            this.globalSettings.PointLightPositions[0] = new Vector4(-50, 50, 50, 1);
            this.globalSettings.PointLightPositions[1] = new Vector4(-10, 10, 10, 1);
            this.globalSettings.PointLightColors[0] = new Vector4(0.5f, 0.5f, 0.5f, 0);
            this.globalSettings.PointLightColors[1] = new Vector4(0, 0, 0, 0);
            this.globalSettings.PointLightParameters[0] = new Vector4(1, 1, 1000f, 0.1f);
            this.globalSettings.PointLightParameters[1] = new Vector4(1, 1, 1000f, 0.1f);

            this.globalSettings.DirLightColors[0] = new Vector4(1f, 1f, 1f, 0);
            this.globalSettings.DirLightColors[1] = new Vector4(0.2f, 0.2f, 0.2f, 0);
            this.globalSettings.DirLightDirections[0] = new Vector4(0, -1, 0, 0);
            this.globalSettings.DirLightDirections[1] = new Vector4(1, -1, 0, 0);

            BoundingBox aabb = new BoundingBox();
            bool isFirst = true;
            for (var i = 0; i < maxNumModels; i++)
            {
                if (this.models[i] == null) { continue; }

                textureUI[i] = new Element();
                textureUI[i].Label = string.Format("[{2}] {0} - 1/{1}", (ModelPart)i, this.textures[i].Count, i + 1);
                this.UserInterface.Container.Add(textureUI[i]);

                var allTextures = this.textures[i];
                var currentTextures = allTextures[textureIndices[i]];
                this.globalSettings.TextureManager.AddRange(currentTextures);

                this.models[i].Init();
                if (isFirst)
                {
                    aabb = this.models[i].BoundingBox;
                }
                else
                {
                    aabb = BoundingBox.Merge(aabb, this.models[i].BoundingBox);
                }
            }

            sceneOffset.X = -(0.5f * aabb.Maximum.X + 0.5f * aabb.Minimum.X);
            sceneOffset.Y = -(0.5f * aabb.Maximum.Y + 0.5f * aabb.Minimum.Y);
            sceneOffset.Z = -(0.5f * aabb.Maximum.Z + 0.5f * aabb.Minimum.Z);

            if (this.modelData.ModelCommon != null)
            {
                this.globalSettings.TextureManager.AddRange(this.modelData.ModelCommon.FindAllChildren<DatDigger.Sections.Texture.TextureSection>());
            }

            this.skeleton = new VisualSkeleton(this.globalSettings.CurrentFrame);
            this.renderList.Add(this.skeleton);
        }

        protected override void OnResourceLoad()
        {
            base.OnResourceLoad();

            Context.Device.SetRenderState(RenderState.Lighting, false);
            Context.Device.SetRenderState(RenderState.CullMode, Cull.Clockwise);
            Context.Device.SetRenderState(RenderState.FillMode, FillMode.Solid);
            Context.Device.SetRenderState(RenderState.PointSize, 2.0f);
            Context.Device.SetRenderState(RenderState.AlphaBlendEnable, true);
            Context.Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
            Context.Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
            Context.Device.SetRenderState(RenderState.AlphaTestEnable, true);
            Context.Device.SetRenderState(RenderState.AlphaFunc, Compare.Greater);
            Context.Device.SetRenderState(RenderState.AlphaRef, 50);
        }

        protected override void Dispose(bool disposeManagedResources)
        {
            base.Dispose(disposeManagedResources);

            if (disposeManagedResources)
            {
                this.skeleton.Dispose();

                this.modelData.Skeleton = null;
                for (var i = 0; i < this.modelData.Models.Length; i++)
                {
                    this.modelData.Models[i] = null;
                    this.modelData.Textures[i] = null;
                }
                
                this.modelData.ModelCommon = null;
                this.models = null;
                this.skeleton = null;
                this.textures = null;
            }
        }
    }
}
