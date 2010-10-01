using System.Collections.Generic;
using DatDigger;
using DatDigger.Sections.Model;
using SlimDX;
using SlimDX.Direct3D9;

namespace ModelViewer.Renderer
{
    public class SubModel : Renderable
    {
        private DatDigger.Sections.Model.ModelChunk model;
        private Cache.VarEquip varEquip;

        private List<Mesh> meshes;
        public int MeshStartIndex { get; set; }
        public int NumMeshes { get { return meshes.Count; } }
        public BoundingBox BoundingBox { get; private set; }
        private bool isInitialized;

        private Vector4 bboxOffset;
        private Vector4 bboxScale;

        public SubModel(DatDigger.Sections.Model.ModelChunk model, Cache.VarEquip varEquip) : base()
        {
            this.model = model;
            this.varEquip = varEquip;
            this.meshes = new List<Mesh>();
        }

        protected override void Dispose(bool disposeManaged)
        {
            base.Dispose(disposeManaged);

            if (disposeManaged)
            {
                meshes.ForEach(x => x.Dispose());
            }
        }

        public override void Init()
        {
            if (isInitialized) { return; }

            MeshChunk mesh;
            int idx = 0;
            do
            {
                mesh = this.model.GetChildOfType<MeshChunk>(idx);
                if (mesh != null)
                {
                    idx++;
                    Mesh m = new Mesh(mesh, this.varEquip);
                    this.meshes.Add(m);
                    m.Init();
                }
            } while (mesh != null);

            BoundingBoxContainerChunk aabbContainer = this.model.GetChildOfType<BoundingBoxContainerChunk>();
            CompChunk aabbChunk = aabbContainer.GetChildOfType<CompChunk>();
            this.BoundingBox = aabbChunk.BoundingBox;
            var aabb = this.BoundingBox;

            bboxOffset = new Vector4(0, 0, 0, 0);
            bboxScale = new Vector4(1f, 1f, 1f, 1f);

            bboxScale.X = (aabb.Maximum.X - aabb.Minimum.X) / 2f;
            bboxScale.Y = (aabb.Maximum.Y - aabb.Minimum.Y) / 2f;
            bboxScale.Z = (aabb.Maximum.Z - aabb.Minimum.Z) / 2f;

            bboxOffset.X = aabb.Maximum.X - bboxScale.X;
            bboxOffset.Y = aabb.Maximum.Y - bboxScale.Y;
            bboxOffset.Z = aabb.Maximum.Z - bboxScale.Z;

            isInitialized = true;
        }

        public override void Render()
        {
            using (var modelBBoxOffSet = new EffectHandle("ModelBBoxOffSet"))
            using (var modelBBoxScale = new EffectHandle("ModelBBoxScale"))
            {
                foreach (Mesh mesh in this.meshes)
                {
                    if (this.RenderSettings.IsSkinning)
                    {
                        this.RenderSettings.Skeleton.LoadMatrices(mesh.BoneIds);
                    }

                    this.RenderSettings.ShaderManager.SetEffect(mesh.ShaderName, this.varEquip, mesh.MeshHeader.Unknown4);

                    this.Device.VertexShader.Function.ConstantTable.SetValue(this.Device, modelBBoxOffSet, bboxOffset);
                    this.Device.VertexShader.Function.ConstantTable.SetValue(this.Device, modelBBoxScale, bboxScale);

                    mesh.Render();
                }
            }
        }
    }
}
