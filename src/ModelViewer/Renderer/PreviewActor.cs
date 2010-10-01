using DatDigger;
using DatDigger.Sections.Model;
using SlimDX;
using SlimDX.Direct3D9;

namespace ModelViewer.Renderer
{
    public class PreviewActor : Sample
    {
        private Cache.Actor actor;
        private DatDigger.Xml.Format.ModelType modelType;
        private int baseModelFolder;
        private Model[] models;
        private VisualSkeleton skeleton;
        private int maxNumModels;

        private void LoadSkeleton()
        {
            string skeletonFile;
            switch (modelType)
            {
                case DatDigger.Xml.Format.ModelType.PC:
                    skeletonFile = string.Format("client/chara/pc/c{0:D3}/skl/0001", baseModelFolder);
                    break;
                case DatDigger.Xml.Format.ModelType.Monster:
                    skeletonFile = string.Format("client/chara/mon/m{0:D3}/skl/0001", baseModelFolder);
                    break;
                case DatDigger.Xml.Format.ModelType.BgObject:
                    skeletonFile = string.Format("client/chara/bgobj/b{0:D3}/skl/0001", baseModelFolder);
                    break;
                case DatDigger.Xml.Format.ModelType.Weapon:
                    skeletonFile = string.Format("client/chara/wep/w{0:D3}/skl/0001", baseModelFolder);
                    break;
                default:
                    throw new System.InvalidOperationException("Unknown Model Type " + (int)modelType);
            }
            skeletonFile = System.IO.Path.Combine(Properties.Settings.Default.GameDirectory, skeletonFile);

            this.globalSettings.IsSkinning = true;
            var skeletonContainer = DatDigger.Sections.SectionLoader.OpenFile(skeletonFile);
            var skel = skeletonContainer.FindChild<DatDigger.Sections.Skeleton.SkeletonSection>();
            this.globalSettings.BaseFrame = new Renderer.SkeletalFrame(null);
            this.globalSettings.BaseFrame.LoadBones(skel);
            this.globalSettings.CurrentFrame = new Renderer.SkeletalFrame(this.globalSettings.BaseFrame);
            this.globalSettings.CurrentFrame.LoadBones(skel);
        }

        private void LoadCommonTextures()
        {
            if (modelType == DatDigger.Xml.Format.ModelType.PC)
            {
                string texturePath = "client/chara/pc/cmn/0001";
                texturePath = System.IO.Path.Combine(Properties.Settings.Default.GameDirectory, texturePath);
                if (System.IO.File.Exists(texturePath))
                {
                    var texFile = DatDigger.Sections.SectionLoader.OpenFile(texturePath);
                    this.globalSettings.TextureManager.AddRange(texFile.FindAllChildren<DatDigger.Sections.Texture.TextureSection>());
                }
            }
        }

        private void LoadFace(int faceId)
        {
            if (faceId == 0) { return; }

            // Build varEquipId
            int varEquipId = this.actor.FaceVarEquipId;

            Cache.VarEquip equip = Cache.VarEquipCache.GetById(varEquipId);

            string texturePath = string.Format("client/chara/pc/c{0:D3}/kao/f{1:D3}/fac_tex2/0000", baseModelFolder, faceId);
            texturePath = System.IO.Path.Combine(Properties.Settings.Default.GameDirectory, texturePath);
            if (System.IO.File.Exists(texturePath))
            {
                var texFile = DatDigger.Sections.SectionLoader.OpenFile(texturePath);
                this.globalSettings.TextureManager.AddRange(texFile.FindAllChildren<DatDigger.Sections.Texture.TextureSection>());
            }

            string modelPath = string.Format("client/chara/pc/c{0:D3}/kao/f{1:D3}/fac_mdl/0001", baseModelFolder, faceId);
            modelPath = System.IO.Path.Combine(Properties.Settings.Default.GameDirectory, modelPath);
            var mdlFile = DatDigger.Sections.SectionLoader.OpenFile(modelPath);
            var mdlc = mdlFile.FindChild<DatDigger.Sections.Model.ModelContainerChunk>();
            var model = new Model(mdlc, equip);
            this.models[(int)ModelPart.NumModelParts] = model;
            this.renderList.Add(model);
        }

        private void LoadHair(int hairId)
        {
            if (hairId == 0) { return; }

            // Build varEquipId
            int varEquipId = this.actor.HairVarEquipId;

            Cache.VarEquip equip = Cache.VarEquipCache.GetById(varEquipId);

            string texturePath = string.Format("client/chara/pc/c{0:D3}/kao/h{1:D3}/hir_tex2/0000", baseModelFolder, hairId);
            texturePath = System.IO.Path.Combine(Properties.Settings.Default.GameDirectory, texturePath);
            if (System.IO.File.Exists(texturePath))
            {
                var texFile = DatDigger.Sections.SectionLoader.OpenFile(texturePath);
                this.globalSettings.TextureManager.AddRange(texFile.FindAllChildren<DatDigger.Sections.Texture.TextureSection>());
            }

            string modelPath = string.Format("client/chara/pc/c{0:D3}/kao/h{1:D3}/hir_mdl/0001", baseModelFolder, hairId);
            modelPath = System.IO.Path.Combine(Properties.Settings.Default.GameDirectory, modelPath);
            var mdlFile = DatDigger.Sections.SectionLoader.OpenFile(modelPath);
            var mdlc = mdlFile.FindChild<DatDigger.Sections.Model.ModelContainerChunk>();
            var model = new Model(mdlc, equip);
            this.models[(int)ModelPart.NumModelParts + 1] = model;
            this.renderList.Add(model);
        }

        private void LoadModel(int modelId, ModelPart part)
        {
            if (modelId == 0) { return; }

            int subModelId = modelId >> 10;
            int texture = (modelId >> 5) & 0x1F;
            //int variation = modelId & 0x1F;
            int varEquipId = 0;

            string charaFolder;
            string charaPrefix;
            string partFolder = "top";

            switch (part)
            {
                case ModelPart.Top:
                    partFolder = "top";
                    varEquipId = this.actor.TopVarEquipId;
                    break;
                case ModelPart.Dwn:
                    partFolder = "dwn";
                    varEquipId = this.actor.BottomVarEquipId;
                    break;
                case ModelPart.Glv:
                    partFolder = "glv";
                    varEquipId = this.actor.GloveVarEquipId;
                    break;
                case ModelPart.Sho:
                    partFolder = "sho";
                    varEquipId = this.actor.ShoeVarEquipId;
                    break;
                case ModelPart.Met:
                    partFolder = "met";
                    varEquipId = this.actor.HelmVarEquipId;
                    break;
                case ModelPart.Pch:
                    partFolder = "pch";
                    varEquipId = 0;
                    break;
                default:
                    throw new System.InvalidOperationException("Unexpected Model Part " + part);
            }

            switch (modelType)
            {
                case DatDigger.Xml.Format.ModelType.Monster:
                    charaFolder = "mon";
                    charaPrefix = "m";
                    break;
                case DatDigger.Xml.Format.ModelType.BgObject:
                    charaFolder = "bgobj";
                    charaPrefix = "b";
                    break;
                case DatDigger.Xml.Format.ModelType.Weapon:
                    charaFolder = "wep";
                    charaPrefix = "w";
                    break;
                case DatDigger.Xml.Format.ModelType.PC:
                    charaFolder = "pc";
                    charaPrefix = "c";
                    break;
                default:
                    throw new System.InvalidOperationException("Unknown ModelType: " + modelType);
            }

            Cache.VarEquip equip = null;
            int texFolder;
            if (varEquipId > 0)
            {
                equip = Cache.VarEquipCache.GetById(varEquipId);
                if (equip != null)
                {
                    texFolder = Cache.TexturePathCache.GetTextureFolderId(equip.TexPathID, this.baseModelFolder);
                }
                else
                {
                    texFolder = this.baseModelFolder;
                }
            }
            else
            {
                texFolder = this.baseModelFolder;
            }

            string texturePath = string.Format("client/chara/{0}/{1}{2:D3}/equ/e{3:D3}/{4}_tex2/{5:D4}", charaFolder, charaPrefix, texFolder, subModelId, partFolder, texture);
            texturePath = System.IO.Path.Combine(Properties.Settings.Default.GameDirectory, texturePath);
            if (System.IO.File.Exists(texturePath))
            {
                var texFile = DatDigger.Sections.SectionLoader.OpenFile(texturePath);
                this.globalSettings.TextureManager.AddRange(texFile.FindAllChildren<DatDigger.Sections.Texture.TextureSection>());
            }

            string modelPath = string.Format("client/chara/{0}/{1}{2:D3}/equ/e{3:D3}/{4}_mdl/0001", charaFolder, charaPrefix, baseModelFolder, subModelId, partFolder);
            modelPath = System.IO.Path.Combine(Properties.Settings.Default.GameDirectory, modelPath);
            var mdlFile = DatDigger.Sections.SectionLoader.OpenFile(modelPath);
            var mdlc = mdlFile.FindChild<DatDigger.Sections.Model.ModelContainerChunk>();

            // There is a possibility that some model files only contain effects and not actual models in which case don't create a model
            if (mdlc != null)
            {
                var model = new Model(mdlc, equip);
                this.models[(int)part] = model;
                this.renderList.Add(model);
            }
        }

        public PreviewActor(Cache.Actor actor)
            : base()
        {
            this.actor = actor;
            this.modelType = (DatDigger.Xml.Format.ModelType)(this.actor.BaseModel / 10000);
            this.baseModelFolder = this.actor.BaseModel % 10000;

            maxNumModels = (int)ModelPart.NumModelParts + 2;
            this.models = new Model[maxNumModels];
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

            if ((idx != -1) && (e.Shift) && (this.models[idx] != null))
            {
                this.models[idx].Enabled = !this.models[idx].Enabled;
            }
        }

        protected override void OnInitialize()
        {
            this.globalSettings.Actor = this.actor;
            LoadSkeleton();

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

            LoadCommonTextures();
            LoadModel(actor.HelmModel, ModelPart.Met);
            LoadModel(actor.TopModel, ModelPart.Top);
            LoadModel(actor.BottomModel, ModelPart.Dwn);
            LoadModel(actor.GloveModel, ModelPart.Glv);
            LoadModel(actor.ShoeModel, ModelPart.Sho);
            LoadModel(actor.BeltModel, ModelPart.Pch);
            LoadFace(actor.Face);
            LoadHair(actor.Hair);

            DetermineConfiguration();
            HideHiddenModels();

            BoundingBox aabb = new BoundingBox();
            bool isFirst = true;
            for (var i = 0; i < (int)ModelPart.NumModelParts; i++)
            {
                if (this.models[i] == null) { continue; }

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
                this.models = null;
                this.skeleton = null;
            }
        }

        private void HideHiddenModels()
        {
            if (this.models[(int)ModelPart.Top] != null && this.globalSettings.OGroups.Contains("o_top")) {
                this.models[(int)ModelPart.Top].Enabled = false;
            }
            if (this.models[(int)ModelPart.Dwn] != null && this.globalSettings.OGroups.Contains("o_dwn")) {
                this.models[(int)ModelPart.Dwn].Enabled = false;
            }
            if (this.models[(int)ModelPart.Glv] != null && this.globalSettings.OGroups.Contains("o_glv")) {
                this.models[(int)ModelPart.Glv].Enabled = false;
            }
            if (this.models[(int)ModelPart.Sho] != null && this.globalSettings.OGroups.Contains("o_sho")) {
                this.models[(int)ModelPart.Sho].Enabled = false;
            }
            if (this.models[(int)ModelPart.Met] != null && this.globalSettings.OGroups.Contains("o_met")) {
                this.models[(int)ModelPart.Met].Enabled = false;
            }
            if (this.models[(int)ModelPart.Pch] != null && this.globalSettings.OGroups.Contains("o_pch")) {
                this.models[(int)ModelPart.Pch].Enabled = false;
            }
            if (this.models[this.models.Length - 2] != null && this.globalSettings.OGroups.Contains("o_fac")) {
                this.models[this.models.Length - 2].Enabled = false; }
            if (this.models[this.models.Length - 1] != null && this.globalSettings.OGroups.Contains("o_hir")) {
                this.models[this.models.Length - 1].Enabled = false; }
        }

        private void DetermineConfiguration()
        {
            int[] modelIds = new int[6];
            string[] tmpList;
            modelIds[0] = (actor.HelmModel >> 10) & 0x3FF;
            modelIds[1] = (actor.TopModel >> 10) & 0x3FF;
            modelIds[2] = (actor.BottomModel >> 10) & 0x3FF;
            modelIds[3] = (actor.GloveModel >> 10) & 0x3FF;
            modelIds[4] = (actor.ShoeModel >> 10) & 0x3FF;
            modelIds[5] = (actor.BeltModel >> 10) & 0x3FF;

            EquipParam top;
            EquipParam pants;
            EquipParam helm;
            EquipParam gloves;
            EquipParam shoes;
            EquipParam belt;

            top = (modelIds[1] != 0) ? Cache.EquipParamCache.GetById(modelIds[1]) : EquipParam.Null;

            int v166 = 1;

            int v143 = 1;
            int v144 = 1;
            int v145 = 1;
            int v146 = 1;
            int v147 = 0;
            var tmp = (modelIds[2] != 0) ? Cache.EquipParamCache.GetById(modelIds[2]) : EquipParam.Null;
            if (tmp.Values[97] == 0) { v147 = 1; }

            // Determine if top should take place of the pants
            if (top.Values[EquipParam.TopHasPants] == 0)
            {
                modelIds[2] = modelIds[1];
            }

            // Determine if top should take the place of the gloves
            if (top.Values[EquipParam.TopHasGloves] == 0)
            {
                modelIds[3] = modelIds[1];
            }

            // Determine if top should take the place of the belt
            if (top.Values[EquipParam.TopHasBelt] == 0)
            {
                modelIds[5] = modelIds[1];
            }

            // Determine if top should take the place of the helmet
            if (top.Values[EquipParam.TopHasHelm] == 0)
            {
                modelIds[0] = modelIds[1];
            }

            pants = (modelIds[2] != 0) ? Cache.EquipParamCache.GetById(modelIds[2]) : EquipParam.Null;
            if (top.Values[97] == 0)
            {
                modelIds[4] = modelIds[1];
            }

            gloves = (modelIds[3] != 0) ? Cache.EquipParamCache.GetById(modelIds[3]) : EquipParam.Null;
            shoes = (modelIds[4] != 0) ? Cache.EquipParamCache.GetById(modelIds[4]) : EquipParam.Null;
            belt = (modelIds[5] != 0) ? Cache.EquipParamCache.GetById(modelIds[5]) : EquipParam.Null;
            helm = (modelIds[0] != 0) ? Cache.EquipParamCache.GetById(modelIds[0]) : EquipParam.Null;

            if (gloves.Values[EquipParam.HasGloves] != 0)
            {
                var gloves114 = gloves.Values[114];
                if (gloves114 != 0)
                {
                    if (gloves.Values[118] <= top.Values[8 + gloves114])
                    {
                        this.globalSettings.Pgrps.Add("atr_arm");
                        this.globalSettings.Shps.Add("shp_arm");
                        v143 = 0;
                        v146 = 0;
                    }
                }
            }
            if (helm.Values[198] != 0)
            {
                if (helm.Values[199] != 0)
                {
                    if (helm.Values[203] <= top.Values[12])
                    {
                        this.globalSettings.Pgrps.Add("atr_inr");
                        this.globalSettings.Shps.Add("shp_inr");
                        v145 = 0;
                    }
                }
            }
            if (pants.Values[78] != 0)
            {
                if (pants.Values[79] != 0)
                {
                    if (pants.Values[86] <= top.Values[13])
                    {
                        this.globalSettings.Pgrps.Add("atr_skd");
                        v144 = 0;
                    }
                }
            }

            tmpList = new string[] { "shp_pcb", "shp_pcr", "shp_pcl" };
            for (var i = 0; i < tmpList.Length; i++)
            {
                if (top.Values[19 + i] != 0)
                {
                    this.globalSettings.Shps.Add(tmpList[i]);
                }
            }

            if ((top.Values[22] != 0) && (pants.Values[92] != 0))
            {
                this.globalSettings.Shps.Add("shp_blt");
            }

            tmpList = new string[] { "o_dwn", "o_glv", "o_pch", "o_met", "o_inr", "o_nek", "o_wrl", "o_wrr" };
            for (var i = 0; i < tmpList.Length; i++)
            {
                if (top.Values[37 + i] == 0)
                {
                    this.globalSettings.OGroups.Add(tmpList[i]);
                }
            }

            tmpList = new string[] { "atr_lpd", "atr_pcb", "atr_pcr", "atr_pcl", "atr_blt" };
            for (var i = 0; i < tmpList.Length; i++)
            {
                if (top.Values[53 + i] == 0)
                {
                    this.globalSettings.Pgrps.Add(tmpList[i]);
                }
            }

            switch (top.Values[5])
            {
                case 1:
                    this.globalSettings.Shps.Add("shp_kos");
                    this.globalSettings.Pgrps.Add("atr_kod");
                    break;
                case 2:
                    this.globalSettings.Shps.Add("shp_kos");
                    this.globalSettings.Pgrps.Add("atr_kod");
                    break;
                case 3:
                    this.globalSettings.Shps.Add("shp_kos");
                    this.globalSettings.Shps.Add("shp_mom");
                    this.globalSettings.Shps.Add("shp_sho");
                    this.globalSettings.Shps.Add("atr_kod");
                    break;
            }

            if (v143 == 1)
            {
                this.globalSettings.Other.Add("b_top_sode");
                this.globalSettings.Other.Add("b_top_sode_l");
                this.globalSettings.Other.Add("b_top_sode_r");
            }

            v143 = 1;
            if (pants.Values[78] != 0)
            {
                if (shoes.Values[146] != 0)
                {
                    var shoePantsVal = shoes.Values[147];
                    if (shoePantsVal == 4) { shoePantsVal = 3; }
                    if (shoes.Values[151] <= pants.Values[82 + shoePantsVal])
                    {
                        this.globalSettings.Pgrps.Add("atr_leg");
                        this.globalSettings.Pgrps.Add("atr_lpd");
                        this.globalSettings.Shps.Add("shp_leg");
                    }
                }

                if ((v147 != 0) || (pants.Values[97] == 0))
                {
                    this.globalSettings.OGroups.Add("o_sho");
                }

                if (v144 != 0)
                {
                    switch (pants.Values[79])
                    {
                        case 1:
                        case 2:
                            this.globalSettings.Pgrps.Add("atr_skt");
                            this.globalSettings.Shps.Add("shp_sso");
                            break;
                        case 3:
                            this.globalSettings.Pgrps.Add("atr_skt");
                            this.globalSettings.Pgrps.Add("atr_lpd");
                            this.globalSettings.Shps.Add("shp_sso");
                            this.globalSettings.Shps.Add("shp_mom");
                            this.globalSettings.Shps.Add("shp_sho");
                            break;
                    }
                }
            }

            if (gloves.Values[113] != 0)
            {
                if (v146 != 0)
                {
                    tmpList = new string[] { "o_wrl", "o_wrr" };
                    for (var i = 0; i < tmpList.Length; i++)
                    {
                        if (gloves.Values[127 + i] == 0)
                        {
                            this.globalSettings.OGroups.Add(tmpList[i]);
                        }
                    }

                    tmpList = new string[] { "b_top_sode_l", "b_top_sode_r" };
                    for (var i = 0; i < tmpList.Length; i++)
                    {
                        if (gloves.Values[141 + i] == 0)
                        {
                            this.globalSettings.Other.Add(tmpList[i]);
                        }
                    }

                    switch (gloves.Values[114])
                    {
                        case 1:
                            this.globalSettings.Shps.Add("shp_hij");
                            break;
                        case 2:
                            this.globalSettings.Pgrps.Add("atr_hij");
                            this.globalSettings.Shps.Add("shp_ude");
                            break;
                        case 3:
                            this.globalSettings.Pgrps.Add("atr_hij");
                            this.globalSettings.Pgrps.Add("atr_ude");
                            this.globalSettings.Shps.Add("shp_kat");
                            break;
                    }
                }

                tmpList = new string[] { "o_idl", "o_idr", "o_ril", "o_rir" };
                for (var i = 0; i < tmpList.Length; i++)
                {
                    if (gloves.Values[129 + i] == 0)
                    {
                        this.globalSettings.OGroups.Add(tmpList[i]);
                    }
                }
            }

            if (shoes.Values[146] != 0)
            {
                if (v143 != 0)
                {
                    switch (shoes.Values[147])
                    {
                        case 1:
                            this.globalSettings.Shps.Add("shp_sne");
                            break;
                        case 2:
                            this.globalSettings.Pgrps.Add("atr_sne");
                            this.globalSettings.Shps.Add("shp_hiz");
                            break;
                        case 3:
                            this.globalSettings.Pgrps.Add("atr_sne");
                            this.globalSettings.Pgrps.Add("atr_hiz");
                            this.globalSettings.Shps.Add("shp_mom");
                            break;
                        case 4:
                            this.globalSettings.Pgrps.Add("atr_sne");
                            this.globalSettings.Pgrps.Add("atr_hiz");
                            this.globalSettings.Pgrps.Add("atr_mom");
                            this.globalSettings.Shps.Add("shp_mom");
                            break;
                    }
                }
            }

            if (belt.Values[175] == 1)
            {
                this.globalSettings.Shps.Add("shp_skd");
                this.globalSettings.Shps.Add("shp_sso");
                this.globalSettings.Shps.Add("shp_kos");
                this.globalSettings.Pgrps.Add("atr_kos");
                this.globalSettings.Pgrps.Add("atr_kod");
            }

            if (helm.Values[198] != 0)
            {
                if (helm.Values[209] != 0)
                {
                    this.globalSettings.Shps.Add("shp_eri");
                }

                if ((helm.Values[200] != 0) && (v145 != 0))
                {
                    this.globalSettings.Shps.Add("shp_nek");
                }

                if (helm.Values[236] == 0)
                {
                    this.globalSettings.OGroups.Add("o_nek");
                }

                // Something here.. uses the 12 element array w/ 1-3
                // Helm Offset 235

                if (helm.Values[200] != 0)
                {
                    if (v145 != 0)
                    {
                        this.globalSettings.Pgrps.Add("atr_nek");
                    }

                    this.globalSettings.Pgrps.Add("atr_cho");
                    this.globalSettings.Pgrps.Add("atr_kub");
                }

                if ((helm.Values[260] == 0) && (v166 == 2))
                {
                    this.globalSettings.Pgrps.Add("atr_mim");
                }

                if ((helm.Values[261] == 0) && (v166 == 3))
                {
                    this.globalSettings.Pgrps.Add("atr_top");
                }

                tmpList = new string[] { "b_hir_ke_uf", "b_hir_ke_ul", "b_hir_ke_ur", "b_hir_ke_ub", "b_hir_ke_si", "b_hir_ke_cf", "b_hir_ke_cl", "b_hir_ke_cr", "b_hir_ke_cb", "b_hir_ke_of", "b_hir_ke_ol", "b_hir_ke_or", "b_hir_ke_ob", "b_hir_ke_up" };
                for (var i = 0; i < tmpList.Length; i++)
                {
                    if (helm.Values[273 + i] == 0)
                    {
                        this.globalSettings.Other.Add(tmpList[i]);
                    }
                }

                switch (helm.Values[199])
                {
                    case 1:
                        this.globalSettings.Shps.Add("shp_hia");
                        break;
                    case 2:
                        this.globalSettings.Pgrps.Add("atr_kam");
                        this.globalSettings.Shps.Add("shp_hib");
                        break;
                    case 3:
                        this.globalSettings.Pgrps.Add("atr_kam");
                        this.globalSettings.Pgrps.Add("atr_lng");
                        this.globalSettings.Shps.Add("shp_hic");
                        break;
                    case 4:
                        this.globalSettings.Pgrps.Add("atr_sta");
                        this.globalSettings.Pgrps.Add("atr_kam");
                        this.globalSettings.OGroups.Add("o_hir");
                        break;
                    case 5:
                        this.globalSettings.OGroups.Add("o_hir");
                        this.globalSettings.OGroups.Add("o_fac");
                        break;
                }
            }
        }
    }
}
