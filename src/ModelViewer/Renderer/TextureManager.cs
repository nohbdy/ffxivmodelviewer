using System;
using System.Collections.Generic;
using DatDigger.Sections.Texture;
using SlimDX.Direct3D9;

namespace ModelViewer.Renderer
{
    public struct TextureHolder
    {
        public BaseTexture Texture;
        public string Name;
        public GtexData Gtex;
    }

    public class TextureManager : IDisposable
    {
        public const int MaxSamplers = 8;

        private List<TextureHolder> textures = new List<TextureHolder>();
        private Texture defaultTexture;
        private byte[] defaultData = { 255, 255, 255, 255 };
        private CubeTexture enviroMap;
        private CubeTexture lightDiffuseMap;
        private Dictionary<string, TextureHolder> textureLookup = new Dictionary<string, TextureHolder>();
        private Device device;
        private string[] currentTextures = new string[MaxSamplers];

        public CubeTexture LightDiffuseMap { get { return lightDiffuseMap; } }
        public CubeTexture EnvironmentTexture { get { return enviroMap; } }
        public List<TextureHolder> Textures { get { return textures; } }

        public TextureManager()
        {
            device = GlobalRenderSettings.Instance.Device;

            defaultTexture = new Texture(device, 1, 1, 1, Usage.None, Format.X8R8G8B8, Pool.Managed);
            var dataRect = defaultTexture.LockRectangle(0, LockFlags.None);
            dataRect.Data.Write(defaultData, 0, 4);

            enviroMap = new CubeTexture(device, 256, 1, Usage.None, Format.X8R8G8B8, Pool.Managed);
            LoadCubeMapFace(enviroMap, CubeMapFace.NegativeX, "ModelViewer.Resources.landscape_negative_x.png", 256, Format.X8R8G8B8);
            LoadCubeMapFace(enviroMap, CubeMapFace.NegativeY, "ModelViewer.Resources.landscape_negative_y.png", 256, Format.X8R8G8B8);
            LoadCubeMapFace(enviroMap, CubeMapFace.NegativeZ, "ModelViewer.Resources.landscape_negative_z.png", 256, Format.X8R8G8B8);
            LoadCubeMapFace(enviroMap, CubeMapFace.PositiveX, "ModelViewer.Resources.landscape_positive_x.png", 256, Format.X8R8G8B8);
            LoadCubeMapFace(enviroMap, CubeMapFace.PositiveY, "ModelViewer.Resources.landscape_positive_y.png", 256, Format.X8R8G8B8);
            LoadCubeMapFace(enviroMap, CubeMapFace.PositiveZ, "ModelViewer.Resources.landscape_positive_z.png", 256, Format.X8R8G8B8);

            lightDiffuseMap = new CubeTexture(device, 1, 1, Usage.None, Format.X8R8G8B8, Pool.Managed);
            FillLightDiffuseMap(CubeMapFace.NegativeX, 0xFFDDDDDD);
            FillLightDiffuseMap(CubeMapFace.NegativeY, 0xFFCCCCCC);
            FillLightDiffuseMap(CubeMapFace.NegativeZ, 0xFFDDDDDD);
            FillLightDiffuseMap(CubeMapFace.PositiveX, 0xFFDDDDDD);
            FillLightDiffuseMap(CubeMapFace.PositiveY, 0xFFFFFFFF);
            FillLightDiffuseMap(CubeMapFace.PositiveZ, 0xFFDDDDDD);
        }

        private void FillLightDiffuseMap(CubeMapFace face, uint color)
        {
            var dst = lightDiffuseMap.LockRectangle(face, 0, LockFlags.None);
            dst.Data.Write(color);
            lightDiffuseMap.UnlockRectangle(face, 0);
        }

        private void LoadCubeMapFace(CubeTexture tex, CubeMapFace face, string fileName, int size, Format format)
        {
            var data = Surface.CreateOffscreenPlain(device, size, size, format, Pool.Scratch);
            Surface.FromFileInStream(data, this.GetType().Assembly.GetManifestResourceStream(fileName), Filter.None, 0);
            var dstRect = tex.LockRectangle(face, 0, LockFlags.None);
            var srcRect = data.LockRectangle(LockFlags.ReadOnly);

            srcRect.Data.CopyTo(dstRect.Data);

            data.UnlockRectangle();
            tex.UnlockRectangle(face, 0);

            data.Dispose();
        }

        ~TextureManager()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Add(TextureSection textureSection)
        {
            Texture texture = null;
            CubeTexture cube = null;
            try
            {
                TextureHolder texHolder = new TextureHolder();
                int texWidth = textureSection.Gtex.Header.Width;
                int texHeight = textureSection.Gtex.Header.Height;
                if (textureSection.Gtex.Header.IsCubeMap)
                {
                    cube = new CubeTexture(device, texWidth, 0, Usage.None, textureSection.Gtex.Format, Pool.Managed);
                    texHolder.Texture = cube;
                    texHolder.Name = textureSection.ResourceId;
                    texHolder.Gtex = textureSection.Gtex;

                    CubeMapFace[] faces = new CubeMapFace[] { CubeMapFace.NegativeX,
                                                              CubeMapFace.NegativeY,
                                                              CubeMapFace.NegativeZ,
                                                              CubeMapFace.PositiveX,
                                                              CubeMapFace.PositiveY,
                                                              CubeMapFace.PositiveZ };
                    for (var i = 0; i < 6; i++)
                    {
                        var texData = cube.LockRectangle(faces[i], 0, LockFlags.None);
                        texData.Data.Write(texHolder.Gtex.TextureData[i], 0, texHolder.Gtex.TextureData[i].Length);
                        cube.UnlockRectangle(faces[i], 0);
                    }
                }
                else
                {
                    int numMipMaps = textureSection.Gtex.Header.MipMapCount;
                    Format format = textureSection.Gtex.Format;

                    texture = new Texture(device, texWidth, texHeight, numMipMaps, Usage.None, format, Pool.Managed);

                    texHolder.Texture = texture;
                    texHolder.Name = textureSection.ResourceId;
                    texHolder.Gtex = textureSection.Gtex;

                    for (var i = 0; i < numMipMaps; i++)
                    {
                        var texData = texture.LockRectangle(i, LockFlags.None);
                        texData.Data.Write(texHolder.Gtex.TextureData[i], 0, texHolder.Gtex.TextureData[i].Length);
                        texture.UnlockRectangle(i);
                    }
                }

                TextureHolder removeMe;
                if (this.textureLookup.TryGetValue(texHolder.Name, out removeMe))
                {
                    removeMe.Texture.Dispose();
                    this.textures.Remove(removeMe);
                    this.textureLookup.Remove(texHolder.Name);
                }

                this.textures.Add(texHolder);
                this.textureLookup.Add(texHolder.Name, texHolder);
            }
            catch
            {
                if (texture != null) { texture.Dispose(); }
                if (cube != null) { cube.Dispose(); }

                throw;
            }
        }

        public void AddRange(List<TextureSection> textureSections)
        {
            textureSections.ForEach(x => this.Add(x));
        }

        protected virtual void Dispose(bool disposeManaged)
        {
            textures.ForEach(x => x.Texture.Dispose());
            if (defaultTexture != null) { defaultTexture.Dispose(); }
            if (enviroMap != null) { enviroMap.Dispose(); }
            if (lightDiffuseMap != null) { lightDiffuseMap.Dispose(); }
            textures = null;
        }

        public void Begin()
        {
            for (var i = 0; i < currentTextures.Length; i++) { currentTextures[i] = null; }
        }

        public void SetTexture(int sampler, string name)
        {
            if (sampler < 0 || sampler >= MaxSamplers) { throw new ArgumentOutOfRangeException("sampler"); }
            if (String.Equals(name, currentTextures[sampler])) return;

            var tex = GetByName(name);
            if (tex != null)
            {
                device.SetTexture(sampler, tex);
                currentTextures[sampler] = name;
            }
        }

        public BaseTexture GetByName(string name)
        {
            if (name.StartsWith("DrawEnv00Texture"))
            {
                return this.enviroMap;
            }
            else if (name.StartsWith("Global00Texture_"))
            {
                name = "c001f000f_t";
            }
            else if (name.StartsWith("Global01Texture_"))
            {
                name = "c001f000f_y";
            }
            else if (name.StartsWith("Global02Texture_"))
            {
                name = "c001e000t_t";
            }

            TextureHolder texHolder;
            if (!this.textureLookup.TryGetValue(name, out texHolder))
            {
                name = System.IO.Path.GetFileNameWithoutExtension(name);
                if (!this.textureLookup.TryGetValue(name, out texHolder))
                {
                    return null;
                }
            }

            return texHolder.Texture;
        }
    }
}
