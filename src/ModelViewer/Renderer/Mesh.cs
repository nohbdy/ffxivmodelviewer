using System.Collections.Generic;
using DatDigger;
using DatDigger.Sections.Model;
using SlimDX.Direct3D9;

namespace ModelViewer.Renderer
{
    class IndexList
    {
        private bool[] used;
        private short[] indices;

        public bool AllUsed { get { return indices.Length == this.used.Length; } }
        public int UsedCount { get; private set; }
        public int UnusedCount { get { return indices.Length - UsedCount; } }

        public IndexList(StreamChunk indexStream)
        {
            this.indices = new short[indexStream.VertexCount];
            this.used = new bool[indexStream.VertexCount / 3];

            // Copy indices
            using (var ms = new System.IO.MemoryStream(indexStream.VertexData, false))
            using (var br = new System.IO.BinaryReader(ms))
            {
                for (var i = 0; i < indexStream.VertexCount; i++)
                {
                    indices[i] = br.ReadInt16();
                }
            }
        }

        public IndexBuffer ExtractSubset(Device device, short[] triList)
        {
            int numTris = triList.Length;
            IndexBuffer result = null;
            try
            {
                result = new IndexBuffer(device, numTris * 6, Usage.WriteOnly, Pool.Managed, true);
                try
                {
                    var str = result.Lock(0, 0, LockFlags.None);
                    for (var i = 0; i < numTris; i++)
                    {
                        int triNum = triList[i];
                        int offset = triNum * 3;

                        str.Write(this.indices[offset]);
                        str.Write(this.indices[offset+1]);
                        str.Write(this.indices[offset+2]);

                        if (!used[triNum])
                        {
                            used[triNum] = true;
                            UsedCount += 3; // 3 indices used
                        }
                    }
                }
                finally
                {
                    result.Unlock();
                }
                return result;
            }
            catch
            {
                if (result != null) { result.Dispose(); }
                throw;
            }
        }

        public IndexBuffer BuildRemainder(Device device)
        {
            if (UsedCount == indices.Length) { return null; }

            int numIndices = indices.Length - UsedCount;
            IndexBuffer result = null;
            try
            {
                result = new IndexBuffer(device, numIndices * 2, Usage.WriteOnly, Pool.Managed, true);

                try
                {
                    var str = result.Lock(0, 0, LockFlags.None);
                    int numTris = used.Length;
                    for (var i = 0; i < numTris; i++)
                    {
                        if (used[i]) { continue; }

                        int offset = i * 3;

                        str.Write(this.indices[offset]);
                        str.Write(this.indices[offset + 1]);
                        str.Write(this.indices[offset + 2]);
                    }
                }
                finally
                {
                    result.Unlock();
                }
                return result;
            }
            catch
            {
                if (result != null) { result.Dispose(); }
                throw;
            }
        }
    }

    class Pgrp : Renderable
    {
        public string Name { get; private set; }
        private PgrpChunk pgrp;
        private IndexList idxList;
        private IndexBuffer indexBuffer;
        private int numVertices;
        private int numTriangles;
        private bool enabled = true;

        public Pgrp(DatDigger.Sections.Model.PgrpChunk pgrp, IndexList idxList, int numVertices)
            : base()
        {
            this.pgrp = pgrp;
            this.idxList = idxList;
            this.numVertices = numVertices;
            this.numTriangles = pgrp.Triangles.Length;
            this.Name = pgrp.PgrpName;
        }

        public void DetermineEnableStatus(Cache.VarEquip varEquip)
        {
            if (varEquip == null) { return; }

            string nameEnd = this.Name.Substring(this.Name.Length - 2);
            switch (nameEnd)
            {
                case "_a": this.enabled = !varEquip.PgrpA; break;
                case "_b": this.enabled = !varEquip.PgrpB; break;
                case "_c": this.enabled = !varEquip.PgrpC; break;
                case "_d": this.enabled = !varEquip.PgrpD; break;
                case "_e": this.enabled = !varEquip.PgrpE; break;
                case "_f": this.enabled = !varEquip.PgrpF; break;
                case "_g": this.enabled = !varEquip.PgrpG; break;
                case "_h": this.enabled = !varEquip.PgrpH; break;
                case "_i": this.enabled = !varEquip.PgrpI; break;
                case "_j": this.enabled = !varEquip.PgrpJ; break;
                case "_k": this.enabled = !varEquip.PgrpK; break;
                case "_l": this.enabled = !varEquip.PgrpL; break;
                case "_m": this.enabled = !varEquip.PgrpM; break;
                case "_n": this.enabled = !varEquip.PgrpN; break;
                case "_o": this.enabled = !varEquip.PgrpO; break;
                case "_p": this.enabled = !varEquip.PgrpP; break;
                default: this.enabled = true; break;
            }

            if (this.RenderSettings.Pgrps.Contains(this.Name))
            {
                this.enabled = false;
            }
        }

        public override void Init()
        {
            base.Init();

            indexBuffer = idxList.ExtractSubset(this.Device, this.pgrp.Triangles);
        }

        public override void Render()
        {
            base.Render();

            if (!enabled) { return; }

            this.Device.Indices = indexBuffer;
            this.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, numVertices, 0, numTriangles);
        }

        protected override void Dispose(bool disposeManaged)
        {
            base.Dispose(disposeManaged);

            if (disposeManaged) { if (indexBuffer != null) { indexBuffer.Dispose(); } }
        }
    }

    public class Mesh : Renderable
    {
        private Cache.VarEquip varEquip;
        private MeshChunk mesh;
        private IndexBuffer indexBuffer;
        private VertexBuffer vertexBuffer;
        private VertexDeclaration vertexDecl;
        private VertexElement[] vertexElements;
        private List<Pgrp> pgrps = new List<Pgrp>();
        private int numTriangles;
        private int numVertices;
        private int vertexSize;
        private byte[] vertexBufferData;
        private byte[] indexBufferData;

        public string ShaderName { get; private set; }
        public int[] BoneIds { get; private set; }
        public MeshHeaderChunk MeshHeader { get { return this.mesh.Header; } }

        public Mesh(DatDigger.Sections.Model.MeshChunk mesh, Cache.VarEquip varEquip) : base()
        {
            this.mesh = mesh;
            this.varEquip = varEquip;
        }

        public override void Init()
        {
            base.Init();

            StringChunk strChunk = mesh.GetChildOfType<StringChunk>();
            this.ShaderName = strChunk.String;

            StreamChunk indexChunk = mesh.GetChildOfType<StreamChunk>();
            this.indexBufferData = indexChunk.VertexData;
            this.numTriangles = indexChunk.VertexCount / 3;

            StreamChunk dataChunk = mesh.GetChildOfType<StreamChunk>(1);
            this.vertexBufferData = dataChunk.VertexData;
            this.numVertices = dataChunk.VertexCount;
            this.vertexSize = dataChunk.VertexSize;

            this.vertexElements = new VertexElement[dataChunk.NumElements + 1];
            for (var i = 0; i < dataChunk.NumElements; i++)
            {
                this.vertexElements[i] = dataChunk.Elements[i].GenerateVertexElement();
            }
            this.vertexElements[dataChunk.NumElements] = VertexElement.VertexDeclarationEnd;
            this.vertexDecl = new VertexDeclaration(Device, vertexElements);

            vertexBuffer = new VertexBuffer(
                Device,
                vertexBufferData.Length,
                Usage.WriteOnly,
                VertexFormat.Position,
                Pool.Managed
            );

            var stream = vertexBuffer.Lock(0, 0, LockFlags.None);
            stream.WriteRange(vertexBufferData);
            vertexBuffer.Unlock();

            IndexList idxList = new IndexList(indexChunk);

            List<PgrpChunk> pgrpChunks = mesh.GetChildrenOfType<PgrpChunk>();
            foreach (var p in pgrpChunks)
            {
                var pgrp = new Pgrp(p, idxList, numVertices);
                this.pgrps.Add(pgrp);
                pgrp.Init();
                pgrp.DetermineEnableStatus(varEquip);
            }

            if (idxList.AllUsed)
            {
                indexBuffer = null;
            }
            else
            {
                indexBuffer = idxList.BuildRemainder(this.Device);
                numTriangles = idxList.UnusedCount / 3;
            }

            if (RenderSettings.IsSkinning)
            {
                var envdChunks = mesh.GetChildrenOfType<EnvdChunk>();
                List<string> boneNames = new List<string>();
                envdChunks.ForEach(x => boneNames.Add(x.BoneName));
                this.BoneIds = this.RenderSettings.Skeleton.BoneNamesToIds(boneNames);
            }
        }

        public override void Render()
        {
            Device.VertexDeclaration = vertexDecl;
            Device.SetStreamSource(0, vertexBuffer, 0, vertexSize);
            if (indexBuffer != null)
            {
                Device.Indices = indexBuffer;
                Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, numVertices, 0, numTriangles);
            }

            if (RenderSettings.RenderPgrps)
            {
                foreach (var pgrp in pgrps)
                {
                    pgrp.Render();
                }
            }
        }

        protected override void Dispose(bool disposeManaged)
        {
            if (disposeManaged)
            {
                if (indexBuffer != null)
                {
                    indexBuffer.Dispose();
                    indexBuffer = null;
                }

                if (vertexBuffer != null)
                {
                    vertexBuffer.Dispose();
                    vertexBuffer = null;
                }

                if (vertexDecl != null)
                {
                    vertexDecl.Dispose();
                    vertexDecl = null;
                }

                if (pgrps != null)
                {
                    pgrps.ForEach(x => x.Dispose());
                    pgrps = null;
                }
            }
        }
    }
}
