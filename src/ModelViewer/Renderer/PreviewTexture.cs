using DatDigger.Sections.Texture;
using SlimDX;

namespace ModelViewer.Renderer
{
    public class PreviewTexture : Renderer.Sample
    {
        private TextureSection tex;

        public PreviewTexture(GtexData gtex) : base()
        {
            this.tex = new TextureSection("__previewTexture", gtex);
            this.EnableCamera = false;
        }

        public PreviewTexture(TextureSection tex)
            : base()
        {
            this.tex = tex;
        }

        protected override void OnInitialize()
        {
            this.globalSettings.ViewMatrix = Matrix.LookAtLH(
                    new Vector3(0, 0, -1),
                    new Vector3(0, 0, 0),
                    new Vector3(0, 1, 0)
                );

            this.globalSettings.TextureManager.Add(this.tex);
            this.renderList.Add(new TexturedQuad(this.tex.ResourceId, this.tex.Gtex.Header.Width, this.tex.Gtex.Header.Height));
        }

        protected override void OnResourceLoad()
        {
            base.OnResourceLoad();

            this.globalSettings.ProjectionMatrix = Matrix.OrthoLH(this.WindowWidth, this.WindowHeight, 1f, 100f);
        }
    }
}
