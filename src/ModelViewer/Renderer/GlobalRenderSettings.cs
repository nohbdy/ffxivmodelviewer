using System;
using System.Collections.Generic;

using SlimDX;

namespace ModelViewer.Renderer
{
    public sealed class GlobalRenderSettings : IDisposable
    {
        [ThreadStatic]
        private static GlobalRenderSettings instance;

        public static GlobalRenderSettings Instance
        {
            get {
                if (instance == null) {
                    instance = new GlobalRenderSettings();
                }

                return instance;
            }
        }

        private GlobalRenderSettings()
        {
            PointLightPositions = new Vector4[2];
            PointLightColors = new Vector4[2];
            PointLightParameters = new Vector4[2];
            DirLightColors = new Vector4[2];
            DirLightDirections = new Vector4[2];

            RenderPgrps = true;

            JointMatrices = new Vector4[144];
            IsSkinning = false;

            EnableShadowFlag = new Vector4(0, 0, 0, 0);
            AmbientLightColor = new Vector3(0, 0, 0);
            FogColor = new Vector4(1, 1, 1, 0);
            AmbientColor = new Vector3(0, 0, 0);
            ModulateColor = new Vector4(1, 1, 1, 1);
            LatitudeParam = new Vector3(1, 0, 1);
            DiffuseColor = new Vector4(0.5f, 0.5f, 0.5f, 1.0f);
            MultiDiffuseColor = new Vector4(1, 1, 1, 0);
            SpecularColor = new Vector3(1f, 1f, 1f);
            MultiSpecularColor = new Vector3(1f, 1f, 1f);
            Shininess = 12;
            MultiShininess = 12;
            SpecularInfluence = 0;
            NormalPower = 1;
            ReflectMapLod = 3.6f;
            ReflectMapInfluence = 0.8f;
            Reflectivity = new Vector3(0.5f, 0.5f, 0.5f);
            MultiReflectivity = new Vector3(0.5f, 0.5f, 0.5f);
            RefAlphaRestrain = 1;
            LightDiffuseMapLod = 5.7f;
            LightDiffusePower = 0.67f;
            LightDiffuseInfluence = 0;
            FresnelExp = 1;
            FresnelLightDiffBias = 1;
            GlareLdrScale = 1;

            Pgrps = new List<string>();
            Shps = new List<string>();
            OGroups = new List<string>();
            Other = new List<string>();
        }

        public void Init()
        {
            ShaderManager = new ShaderManager();
            TextureManager = new TextureManager();
        }

        public Cache.Actor Actor;

        public SlimDX.Direct3D9.Device Device { get; set; }
        public ShaderManager ShaderManager { get; private set; }
        public TextureManager TextureManager { get; private set; }
        public bool RenderPgrps;

        public List<string> Pgrps;
        public List<string> Shps;
        public List<string> OGroups;
        public List<string> Other;

        public Matrix WorldMatrix;
        public Matrix WorldViewMatrix;
        public Matrix WorldViewProjMatrix;
        public Matrix WorldITMatrix;

        public Matrix ViewMatrix;
        public Matrix ViewITMatrix;
        public Matrix ProjectionMatrix;

        public Vector4[] JointMatrices;
        public bool IsSkinning;

        public Vector4[] PointLightPositions;
        public Vector4[] PointLightColors;
        public Vector4[] PointLightParameters;

        public Vector4[] DirLightColors;
        public Vector4[] DirLightDirections;

        public Vector3 AmbientLightColor;
        public Vector4 FogColor;
        public Vector4 ModulateColor;

        public Vector4 EnableShadowFlag;
        public Vector3 LatitudeParam;

        public Vector3 AmbientColor;
        public Vector4 DiffuseColor;
        public Vector4 MultiDiffuseColor;
        public Vector3 SpecularColor;
        public Vector3 MultiSpecularColor;
        public float Shininess;
        public float MultiShininess;
        public float SpecularInfluence;
        public float NormalPower;

        public Vector3 Reflectivity;
        public Vector3 MultiReflectivity;
        public float ReflectMapLod;
        public float ReflectMapInfluence;
        public float RefAlphaRestrain;

        public float LightDiffuseMapLod;
        public float LightDiffusePower;
        public float LightDiffuseInfluence;
        public float FresnelExp;
        public float FresnelLightDiffBias;
        public float GlareLdrScale;

        public ISkeleton BaseFrame;
        public ISkeleton CurrentFrame;
        public ISkeleton Skeleton;

        public void SetWorldMatrix(Matrix rotation, float scale, Vector3 offset)
        {
            WorldMatrix = Matrix.Translation(offset);
            WorldMatrix *= Matrix.Scaling(scale, scale, scale);
            WorldMatrix *= rotation;
            
            WorldITMatrix = Matrix.Invert(WorldMatrix);
            WorldITMatrix = Matrix.Transpose(WorldITMatrix);
            WorldViewMatrix = WorldMatrix * ViewMatrix;
            WorldViewProjMatrix = WorldMatrix * ViewMatrix * ProjectionMatrix;
        }

        public void SetViewMatrix(Vector3 position, Vector3 target, Vector3 up)
        {
            this.ViewMatrix = Matrix.LookAtRH(position, target, up);
            this.ViewITMatrix = Matrix.Invert(this.ViewMatrix);
            this.ViewITMatrix = Matrix.Transpose(this.ViewITMatrix);
        }

        ~GlobalRenderSettings()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposeManaged)
        {
            this.Device = null;

            if (disposeManaged)
            {
                if (this.TextureManager != null) { this.TextureManager.Dispose(); }
                if (this.ShaderManager != null) { this.ShaderManager.Dispose(); }
            }
        }
    }
}
