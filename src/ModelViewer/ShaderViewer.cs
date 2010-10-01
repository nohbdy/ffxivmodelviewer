using System.Windows.Forms;

namespace ModelViewer
{
    public partial class ShaderViewer : Form
    {
        private string filePath;

        public ShaderViewer(string shaderName, string filePath)
        {
            InitializeComponent();

            this.filePath = filePath;
            this.Text = shaderName;
            this.webBrowser1.Navigate(filePath);
        }

        private void ShaderViewer_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.IO.File.Delete(filePath);
        }
    }
}
