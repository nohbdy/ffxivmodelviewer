using System.Collections.Generic;
using DatDigger;
using DatDigger.Sections.Model;
using SlimDX;

namespace ModelViewer.Renderer
{
    public class Model : RenderableContainer<SubModel>
    {
        private ModelContainerChunk modelContainer;
        private Cache.VarEquip varEquip;
        public BoundingBox BoundingBox { get; private set; }
        private bool isInitialized;
        public bool Enabled { get; set; }
        SkeletonExtension extBaseSkeleton;
        SkeletonExtension extCurrentSkeleton;

        public Model(ModelContainerChunk modelContainer)
        {
            this.modelContainer = modelContainer;
            this.Enabled = true;
        }

        public Model(ModelContainerChunk modelContainer, Cache.VarEquip varEquip)
        {
            this.modelContainer = modelContainer;
            this.Enabled = true;
            this.varEquip = varEquip;
        }

        public override void Init()
        {
            if (isInitialized) { return; }

            base.Init();

            var parentResource = this.modelContainer.GetParent<DatDigger.Sections.Resource.ResourceSection>();
            List<DatDigger.Sections.Shader.ShaderSection> shaders = parentResource.GetChildrenOfType<DatDigger.Sections.Shader.ShaderSection>();
            RenderSettings.ShaderManager.AddRange(shaders);

            // Load any additional bones used by this model
            if (RenderSettings.IsSkinning)
            {
                var skeletonSection = parentResource.FindChild<DatDigger.Sections.Skeleton.SkeletonSection>();
                if (skeletonSection.BoneCount > 0)
                {
                    this.extBaseSkeleton = new SkeletonExtension(RenderSettings.BaseFrame, null);
                    this.extBaseSkeleton.LoadBones(skeletonSection);
                    RenderSettings.Skeleton = this.extBaseSkeleton;

                    this.extCurrentSkeleton = new SkeletonExtension(RenderSettings.BaseFrame, this.extBaseSkeleton);
                    this.extCurrentSkeleton.LoadBones(skeletonSection);
                }
                else
                {
                    RenderSettings.Skeleton = RenderSettings.BaseFrame;
                }
            }

            // Load any textures embedded in the file with the model
            var textures = parentResource.GetChildrenOfType<DatDigger.Sections.Texture.TextureSection>();
            RenderSettings.TextureManager.AddRange(textures);

            ModelChunk model;
            int idx = 0;
            BoundingBox aabb = new BoundingBox();
            bool isFirst = true;
            int meshStartIdx = 0;
            do
            {
                model = this.modelContainer.GetChildOfType<ModelChunk>(idx);
                if (model != null)
                {
                    idx++;
                    var m = new Renderer.SubModel(model, this.varEquip);
                    this.Children.Add(m);
                    m.MeshStartIndex = meshStartIdx;
                    m.Init();
                    meshStartIdx += m.NumMeshes;
                    if (isFirst)
                    {
                        aabb = m.BoundingBox;
                        isFirst = false;
                    }
                    else
                    {
                        aabb = BoundingBox.Merge(aabb, m.BoundingBox);
                    }
                }
            } while (model != null);

            this.BoundingBox = aabb;

            isInitialized = true;
        }

        public override void Render()
        {
            if (!Enabled) { return; }

            if (RenderSettings.IsSkinning)
            {
                if (extCurrentSkeleton != null)
                {
                    extCurrentSkeleton.CurrentFrame = RenderSettings.CurrentFrame;
                    RenderSettings.Skeleton = extCurrentSkeleton;
                }
                else
                {
                    RenderSettings.Skeleton = RenderSettings.CurrentFrame;
                }
            }

            base.Render();
        }
    }
}
