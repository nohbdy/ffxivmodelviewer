using System;
using System.Collections.Generic;
using DatDigger;
using DatDigger.Sections.Shader;
using SlimDX;
using SlimDX.Direct3D9;

namespace ModelViewer.Renderer
{
    public delegate void SetConstantValueDelegate(ConstantTable cTable, Device device, EffectHandle handle);

    public class ShaderInfo : IDisposable
    {
        protected ShaderBytecode bytecode;
        public string Name { get; protected set; }
        public bool IsVertexShader { get; protected set; }
        public List<EffectHandle> ConstantHandles { get; set; }
        public List<ConstantDescription> ConstantDescriptions { get; set; }

        public ShaderInfo(FileChunk fileChunk)
        {
            bytecode = new ShaderBytecode(fileChunk.CompiledShader);
            this.Name = fileChunk.Name;
            this.ConstantHandles = new List<EffectHandle>();
            this.ConstantDescriptions = new List<ConstantDescription>();

            int i = 0;
            ConstantTable cTable = bytecode.ConstantTable;
            if (cTable != null)
            {
                EffectHandle handle = bytecode.ConstantTable.GetConstant(null, 0);
                while (handle != null)
                {
                    ConstantHandles.Add(handle);
                    var desc = bytecode.ConstantTable.GetConstantDescription(handle);
                    ConstantDescriptions.Add(desc);
                    i++;
                    handle = bytecode.ConstantTable.GetConstant(null, i);
                }
            }
        }

        ~ShaderInfo() { Dispose(false); }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposeManaged)
        {
            if (bytecode != null)
            {
                bytecode.Dispose();
                bytecode = null;
            }
        }
    }

    public class VertexShaderInfo : ShaderInfo
    {
        public VertexShader Shader { get; protected set; }

        public VertexShaderInfo(FileChunk fileChunk) : base(fileChunk)
        {
            this.IsVertexShader = true;
            this.Shader = new VertexShader(GlobalRenderSettings.Instance.Device, bytecode);
        }

        protected override void Dispose(bool disposeManaged)
        {
            base.Dispose(disposeManaged);

            if (Shader != null) { Shader.Dispose(); Shader = null; }
        }
    }

    public class PixelShaderInfo : ShaderInfo
    {
        public PixelShader Shader { get; protected set; }

        public PixelShaderInfo(FileChunk fileChunk) : base(fileChunk)
        {
            this.Shader = new PixelShader(GlobalRenderSettings.Instance.Device, bytecode);
        }

        protected override void Dispose(bool disposeManaged)
        {
            base.Dispose(disposeManaged);

            if (Shader != null) { Shader.Dispose(); Shader = null; }
        }
    }

    public class Effect : IDisposable
    {
        private Device device;
        private ShaderSection effect;
        private PramChunk paramData;
        public List<ShaderInfo> Shaders { get; private set; }
        public string Path { get; protected set; }
        public string Name { get; protected set; }

        public VertexShader VertexShader { get; protected set; }
        public VertexShaderInfo VertexShaderInfo { get; protected set; }
        public PixelShader PixelShader { get; protected set; }
        public PixelShaderInfo PixelShaderInfo { get; protected set; }

        public Dictionary<string, SetConstantValueDelegate> vtxSetters = new Dictionary<string, SetConstantValueDelegate>();
        public Dictionary<string, SetConstantValueDelegate> pxlSetters = new Dictionary<string, SetConstantValueDelegate>();

        public Effect(ShaderSection effect)
        {
            this.device = GlobalRenderSettings.Instance.Device;

            this.effect = effect;
            this.paramData = effect.FindChild<PramChunk>();

            this.Shaders = new List<ShaderInfo>();
            List<ShaderChunk> shaderChunks = effect.GetChildrenOfType<ShaderChunk>();
            foreach (ShaderChunk chunk in shaderChunks)
            {
                List<FileChunk> files = chunk.GetChildrenOfType<FileChunk>();
                foreach (FileChunk file in files)
                {
                    if (file.Name.EndsWith("vpo"))
                    {
                        var shaderInfo = new VertexShaderInfo(file);
                        this.Shaders.Add(shaderInfo);
                        this.VertexShaderInfo = shaderInfo;
                        this.VertexShader = shaderInfo.Shader;
                    }
                    else
                    {
                        var shaderInfo = new PixelShaderInfo(file);
                        this.Shaders.Add(shaderInfo);
                        this.PixelShaderInfo = shaderInfo;
                        this.PixelShader = shaderInfo.Shader;
                    }
                }
            }

            this.Path = effect.ResourcePath;
            this.Name = System.IO.Path.GetFileNameWithoutExtension(this.Path);

            InitVertexConstantSetters();
            InitPixelConstantSetters();
        }

        ~Effect() { Dispose(false); }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposeManaged)
        {
            if (disposeManaged)
            {
                this.Shaders.ForEach(x => x.Dispose());
                this.Shaders = null;
            }
        }

        private void InitVertexConstantSetters()
        {
            vtxSetters.Add("worldViewProjMatrix", (c, d, h) => c.SetValue(d, h, Matrix.Transpose(GlobalRenderSettings.Instance.WorldViewProjMatrix)));
            vtxSetters.Add("worldMatrix", (c, d, h) => c.SetValue(d, h, Matrix.Transpose(GlobalRenderSettings.Instance.WorldMatrix)));
            vtxSetters.Add("worldViewMatrix", (c, d, h) => c.SetValue(d, h, Matrix.Transpose(GlobalRenderSettings.Instance.WorldViewMatrix)));
            vtxSetters.Add("worldITMatrix", (c, d, h) => c.SetValue(d, h, Matrix.Transpose(GlobalRenderSettings.Instance.WorldITMatrix)));
            vtxSetters.Add("viewITMatrix", (c, d, h) => c.SetValue(d, h, Matrix.Transpose(GlobalRenderSettings.Instance.ViewITMatrix)));
            vtxSetters.Add("ModelBBoxOffSet", (c, d, h) => c.SetValue(d, h, new SlimDX.Vector4(0, 0, 0, 0)));
            vtxSetters.Add("ModelBBoxScale", (c, d, h) => c.SetValue(d, h, new SlimDX.Vector4(1, 1, 1, 1)));
            vtxSetters.Add("isSkining", (c, d, h) => c.SetValue(d, h, GlobalRenderSettings.Instance.IsSkinning));
            vtxSetters.Add("DirLightColors", (c, d, h) => c.SetValue(d, h, GlobalRenderSettings.Instance.DirLightColors));
            vtxSetters.Add("DirLightDirections", (c, d, h) => c.SetValue(d, h, GlobalRenderSettings.Instance.DirLightDirections));
            vtxSetters.Add("PointLightColors", (c, d, h) => c.SetValue(d, h, GlobalRenderSettings.Instance.PointLightColors));
            vtxSetters.Add("PointLightParams", (c, d, h) => c.SetValue(d, h, GlobalRenderSettings.Instance.PointLightParameters));
            vtxSetters.Add("PointLightPositions", (c, d, h) => c.SetValue(d, h, GlobalRenderSettings.Instance.PointLightPositions));
            vtxSetters.Add("uvofs0", (c, d, h) => c.SetValue(d, h, new SlimDX.Vector4(0, 0, 0, 0)));
            vtxSetters.Add("fogParam", (c, d, h) => c.SetValue(d, h, new SlimDX.Vector4(0, 10, 1, 0)));
            vtxSetters.Add("jointMatrices", (c, d, h) => c.SetValue(d, h, GlobalRenderSettings.Instance.JointMatrices));
            vtxSetters.Add("EnableShadowFlag", (c, d, h) => c.SetValue(d, h, GlobalRenderSettings.Instance.EnableShadowFlag));
        }

        public void LoadVertexShaderParameters()
        {
            this.VertexShader.Function.ConstantTable.SetDefaults(device);

            int numConstants = this.VertexShaderInfo.ConstantDescriptions.Count;
            for (var i = 0; i < numConstants; i++)
            {
                var desc = this.VertexShaderInfo.ConstantDescriptions[i];
                var handle = this.VertexShaderInfo.ConstantHandles[i];

                SetConstantValueDelegate del;
                if (vtxSetters.TryGetValue(desc.Name, out del))
                {
                    del(this.VertexShader.Function.ConstantTable, device, handle);
                }
            }

            foreach (ParameterData pdata in paramData.Parameters)
            {
                if (pdata.IsPixelShaderParameter) { continue; }

                this.VertexShader.Function.ConstantTable.SetValue(device, pdata.EffectHandle, pdata.Defaults);
            }
        }

        private void InitPixelConstantSetters()
        {
            pxlSetters.Add("DirLightColors", (c, d, h) => c.SetValue(d, h, GlobalRenderSettings.Instance.DirLightColors));
            pxlSetters.Add("DirLightDirections", (c, d, h) => c.SetValue(d, h, GlobalRenderSettings.Instance.DirLightDirections));
            pxlSetters.Add("PointLightColors", (c, d, h) => c.SetValue(d, h, GlobalRenderSettings.Instance.PointLightColors));
            pxlSetters.Add("PointLightParams", (c, d, h) => c.SetValue(d, h, GlobalRenderSettings.Instance.PointLightParameters));
            pxlSetters.Add("PointLightPositions", (c, d, h) => c.SetValue(d, h, GlobalRenderSettings.Instance.PointLightPositions));
            pxlSetters.Add("EnableShadowFlag", (c, d, h) => c.SetValue(d, h, GlobalRenderSettings.Instance.EnableShadowFlag));
            pxlSetters.Add("ambientLightColor", (c, d, h) => c.SetValue(d, h, GlobalRenderSettings.Instance.AmbientLightColor));
            pxlSetters.Add("latitudeParam", (c, d, h) => c.SetValue(d, h, GlobalRenderSettings.Instance.LatitudeParam));
            pxlSetters.Add("ambientColor", (c, d, h) => c.SetValue(d, h, GlobalRenderSettings.Instance.AmbientColor));
            pxlSetters.Add("diffuseColor", (c, d, h) => c.SetValue(d, h, GlobalRenderSettings.Instance.DiffuseColor));
            pxlSetters.Add("specularColor", (c, d, h) => c.SetValue(d, h, GlobalRenderSettings.Instance.SpecularColor));
            pxlSetters.Add("shininess", (c, d, h) => c.SetValue(d, h, GlobalRenderSettings.Instance.Shininess));
            pxlSetters.Add("specularInfluence", (c, d, h) => c.SetValue(d, h, GlobalRenderSettings.Instance.SpecularInfluence));
            pxlSetters.Add("normalPower", (c, d, h) => c.SetValue(d, h, GlobalRenderSettings.Instance.NormalPower));
            pxlSetters.Add("fogColor", (c, d, h) => c.SetValue(d, h, GlobalRenderSettings.Instance.FogColor));
            pxlSetters.Add("reflectMapLod", (c, d, h) => c.SetValue(d, h, GlobalRenderSettings.Instance.ReflectMapLod));
            pxlSetters.Add("reflectivity", (c, d, h) => c.SetValue(d, h, GlobalRenderSettings.Instance.Reflectivity));
            pxlSetters.Add("reflectMapInfluence", (c, d, h) => c.SetValue(d, h, GlobalRenderSettings.Instance.ReflectMapInfluence));
            pxlSetters.Add("refAlphaRestrain", (c, d, h) => c.SetValue(d, h, GlobalRenderSettings.Instance.RefAlphaRestrain));
            pxlSetters.Add("lightDiffuseMapLod", (c, d, h) => c.SetValue(d, h, GlobalRenderSettings.Instance.LightDiffuseMapLod));
            pxlSetters.Add("lightDiffusePower", (c, d, h) => c.SetValue(d, h, GlobalRenderSettings.Instance.LightDiffusePower));
            pxlSetters.Add("lightDiffuseInfluence", (c, d, h) => c.SetValue(d, h, GlobalRenderSettings.Instance.LightDiffuseInfluence));
            pxlSetters.Add("fresnelExp", (c, d, h) => c.SetValue(d, h, GlobalRenderSettings.Instance.FresnelExp));
            pxlSetters.Add("fresnelLightDiffBias", (c, d, h) => c.SetValue(d, h, GlobalRenderSettings.Instance.FresnelLightDiffBias));
            pxlSetters.Add("modulateColor", (c, d, h) => c.SetValue(d, h, GlobalRenderSettings.Instance.ModulateColor));
            pxlSetters.Add("glareLdrScale", (c, d, h) => c.SetValue(d, h, GlobalRenderSettings.Instance.GlareLdrScale));
            pxlSetters.Add("multiDiffuseColor", (c, d, h) => c.SetValue(d, h, GlobalRenderSettings.Instance.MultiDiffuseColor));
        }

        public void LoadPixelShaderParameters(Cache.VarEquip material, int materialMeshOffset)
        {
            TextureManager texMan = GlobalRenderSettings.Instance.TextureManager;

            int numConstants = this.PixelShaderInfo.ConstantDescriptions.Count;
            for (var i = 0; i < numConstants; i++)
            {
                var desc = this.PixelShaderInfo.ConstantDescriptions[i];
                var handle = this.PixelShaderInfo.ConstantHandles[i];

                if (desc.RegisterSet == RegisterSet.Sampler)
                {
                    for (var s = 0; s < paramData.NumSamplers; s++)
                    {
                        var sampler = paramData.Samplers[s];
                        if (sampler.Name == desc.Name)
                        {
                            BaseTexture tex = null;
                            for (var t = 0; t < sampler.StringTable.Count; t++)
                            {
                                if (sampler.Name == "lightDiffuseMap")
                                {
                                    tex = texMan.LightDiffuseMap;
                                }
                                else
                                {
                                    tex = texMan.GetByName(sampler.StringTable[t]);
                                }

                                if (tex != null)
                                {
                                    device.SetTexture(desc.RegisterIndex, tex);
                                    device.SetSamplerState(desc.RegisterIndex, SamplerState.MinFilter, TextureFilter.Anisotropic);
                                    device.SetSamplerState(desc.RegisterIndex, SamplerState.MagFilter, TextureFilter.Anisotropic);
                                    device.SetSamplerState(desc.RegisterIndex, SamplerState.MipFilter, TextureFilter.Anisotropic);
                                    break;
                                }
                            }
                            if (tex == null)
                            {
                                throw new InvalidOperationException("Unable to load texture " + sampler.StringTable[0]);
                            }
                        }
                    }
                    continue;
                }

                SetConstantValueDelegate del;
                if (pxlSetters.TryGetValue(desc.Name, out del))
                {
                    del(this.PixelShader.Function.ConstantTable, device, handle);
                }
            }

            // Set the defaults specified in the pixel shader bytecode
            this.PixelShader.Function.ConstantTable.SetDefaults(device);

            // Set the defaults specified in the PRAM data
            foreach (ParameterData pdata in paramData.Parameters)
            {
                if (!pdata.IsPixelShaderParameter) { continue; }

                Vector4 val = pdata.Defaults;
                if (material != null)
                {
                    switch (pdata.Name)
                    {
                        case "modulateColor":
                            var actor = GlobalRenderSettings.Instance.Actor;
                            if (actor != null)
                            {
                                if (paramData.Samplers.Find(x => x.Name.Equals("lightToneMap")) != null)
                                {
                                    Color3 color = ColorHelper.GetSkinTone(actor.BaseModelFolder, actor.C22);
                                    val.X = color.Red;
                                    val.Y = color.Green;
                                    val.Z = color.Blue;
                                }
                                else if (paramData.Parameters.Find(x => x.Name.Equals("velvetParam")) != null)
                                {
                                    Color3 color = ColorHelper.GetHairColor(actor.BaseModelFolder, actor.C21);
                                    val.X = color.Red;
                                    val.Y = color.Green;
                                    val.Z = color.Blue;
                                }
                            }
                            break;
                        case "diffuseColor":
                            val.X = material.GetMaterialValue(0, materialMeshOffset, val.X);
                            val.Y = material.GetMaterialValue(1, materialMeshOffset, val.Y);
                            val.Z = material.GetMaterialValue(2, materialMeshOffset, val.Z);
                            break;
                        case "multiDiffuseColor":
                            val.X = material.GetMaterialValue(3, materialMeshOffset, val.X);
                            val.Y = material.GetMaterialValue(4, materialMeshOffset, val.Y);
                            val.Z = material.GetMaterialValue(5, materialMeshOffset, val.Z);
                            val.W = material.GetMaterialValue(6, materialMeshOffset, val.W);
                            break;
                        /*case "specularColor":
                            val.X = material.GetMaterialValue(7, materialMeshOffset, val.X);
                            val.Y = material.GetMaterialValue(8, materialMeshOffset, val.Y);
                            val.Z = material.GetMaterialValue(9, materialMeshOffset, val.Z);
                            break;
                        case "multiSpecularColor":
                            val.X = material.GetMaterialValue(10, materialMeshOffset, val.X);
                            val.Y = material.GetMaterialValue(11, materialMeshOffset, val.Y);
                            val.Z = material.GetMaterialValue(12, materialMeshOffset, val.Z);
                            break;
                        case "reflectivity":
                            val.X = material.GetMaterialValue(7, materialMeshOffset, val.X);
                            val.Y = material.GetMaterialValue(8, materialMeshOffset, val.Y);
                            val.Z = material.GetMaterialValue(9, materialMeshOffset, val.Z);
                            break;
                        case "multiReflectivity":
                            val.X = material.GetMaterialValue(10, materialMeshOffset, val.X);
                            val.Y = material.GetMaterialValue(11, materialMeshOffset, val.Y);
                            val.Z = material.GetMaterialValue(12, materialMeshOffset, val.Z);
                            break;*/
                        case "shininess":
                            val.X = material.GetMaterialValue(13, materialMeshOffset, val.X);
                            break;
                        case "multiShininess":
                            val.X = material.GetMaterialValue(14, materialMeshOffset, val.X);
                            break;
                    }
                }
                this.PixelShader.Function.ConstantTable.SetValue(device, pdata.EffectHandle, val);
            }
        }
    }

    public class ShaderManager : IDisposable
    {
        public List<Effect> Effects { get; private set; }
        private Effect CurrentEffect { get; set; }
        private SlimDX.Direct3D9.Device device;
        public bool UsePixelShader { get; set; }
        private int CurrentMaterialId { get; set; }

        public ShaderManager()
        {
            device = GlobalRenderSettings.Instance.Device;

            this.UsePixelShader = true;
            this.Effects = new List<Effect>();
        }

        ~ShaderManager() { this.Dispose(false); }

        public void Add(ShaderSection shader)
        {
            if (shader == null) { return; }

            var effect = new Effect(shader);
            this.Effects.Add(effect);
        }

        public void AddRange(List<ShaderSection> shaders)
        {
            if (shaders == null) { return; }

            foreach (ShaderSection shader in shaders)
            {
                Add(shader);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposeManaged)
        {
            if (disposeManaged)
            {
                this.Effects.ForEach(x => x.Dispose());
                this.Effects = null;
            }
        }

        public void Begin()
        {
            CurrentEffect = null;
            CurrentMaterialId = -1;
        }

        private int GetMaterialOffset(string effectName)
        {
            if (effectName.EndsWith("_a")) { return 0; }
            else if (effectName.EndsWith("_b")) { return 1; }
            else if (effectName.EndsWith("_c")) { return 2; }
            else if (effectName.EndsWith("_d")) { return 3; }
            return 0;
        }

        public void SetEffect(string effectName, Cache.VarEquip material, int materialMeshOffset)
        {
            materialMeshOffset = GetMaterialOffset(effectName);

            // If Effect is changing - load everything
            if ((CurrentEffect == null) || (effectName != CurrentEffect.Name)) {
                var newEffect = this.Effects.Find(x => x.Name == effectName);
                if (newEffect == null)
                {
                    throw new InvalidOperationException("Cannot find shader named " + effectName);
                }

                CurrentEffect = newEffect;
                if (material != null) { CurrentMaterialId = material.ID; }
                device.VertexShader = CurrentEffect.VertexShader;
                CurrentEffect.LoadVertexShaderParameters();

                if (UsePixelShader)
                {
                    device.PixelShader = CurrentEffect.PixelShader;
                    CurrentEffect.LoadPixelShaderParameters(material, materialMeshOffset);
                }

                return;
            }

            // If only the material is changing, load shader parameters
            if (material != null && material.ID != CurrentMaterialId)
            {
                CurrentMaterialId = material.ID;
                CurrentEffect.LoadVertexShaderParameters();

                if (UsePixelShader)
                {
                    CurrentEffect.LoadPixelShaderParameters(material, materialMeshOffset);
                }

                return;
            }
        }
    }
}
