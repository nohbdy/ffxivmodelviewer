using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SlimDX;
using DatDigger;

namespace ModelViewer
{
    public partial class Cartographer : Form
    {
        private const int mapSheetFileId = 0x01030360; // /01/03/03/60.DAT
        private static DataTable mapDataTable;
        private static List<MapData> mapData;

        static Cartographer() {
            var relativeFilePath = DatDigger.Utilities.DataFileIdToRelativePath(mapSheetFileId);
            var filePath = System.IO.Path.Combine(Properties.Settings.Default.GameDirectory, relativeFilePath);
            var cd = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(Properties.Settings.Default.GameDirectory);
            using (var fs = File.OpenRead(filePath))
            {
                var fileLoader = new DatDigger.FileLoaders.SheetLoader();
                fileLoader.ReadFile(fs);

                var dataLoader = new DatDigger.FileLoaders.SheetDataLoader();
                dataLoader.ReadSheet(fileLoader.Sheets[0]);

                mapDataTable = dataLoader.Data;
                mapData = new List<MapData>();
                for (var i = 0; i < mapDataTable.Rows.Count; i++)
                {
                    DataRow row = mapDataTable.Rows[i];
                    var md = new MapData();
                    md.ID = (int)row[0];
                    md.Folder = (int)row[1];
                    md.ParentId = (int)row[2];
                    md.Unknown1 = (int)row[3];
                    md.Unknown2 = (int)row[4];
                    md.Width = (int)row[5];
                    md.Height = (int)row[6];
                    mapData.Add(md);
                }
            }
            Directory.SetCurrentDirectory(cd);
        }

        private Renderer.DeviceContext9 context;
        private Bitmap currentImage;

        public Cartographer()
        {
            InitializeComponent();

            BuildTree(null, -1);

            this.Disposed += new EventHandler(Cleanup);

            var settings = new Renderer.DeviceSettings9()
            {
                AdapterOrdinal = 0,
                CreationFlags = SlimDX.Direct3D9.CreateFlags.HardwareVertexProcessing,
                Width = 128,
                Height = 128
            };
            context = new Renderer.DeviceContext9(this.Handle, settings);
        }

        private void Cleanup(object sender, EventArgs e)
        {
            if (context != null) { context.Dispose(); }
            if (currentImage != null) { currentImage.Dispose(); }
        }

        private void BuildTree(TreeNode parentNode, int parentId)
        {
            var rows = mapData.FindAll(x => x.ParentId == parentId);
            foreach (var row in rows)
            {
                var node = new TreeNode(row.Folder.ToString("X"));
                node.Tag = row;
                if (parentNode != null)
                {
                    parentNode.Nodes.Add(node);
                }
                else
                {
                    tvMaps.Nodes.Add(node);
                }
                BuildTree(node, row.ID);
            }
        }

        private void btnUnfurl_Click(object sender, EventArgs e)
        {
            try
            {
                if (tvMaps.SelectedNode == null) { return; }

                var mapData = tvMaps.SelectedNode.Tag as MapData;

                if ((mapData.Width == 0) || (mapData.Height == 0))
                {
                    return;
                }

                var tileId = 0;
                var folderId = mapData.Folder << 16;
                Bitmap bmp;
                Bitmap dstBmp = new Bitmap(mapData.Width, mapData.Height);
                System.Drawing.Imaging.BitmapData srcData;
                System.Drawing.Imaging.BitmapData dstData;
                Rectangle dstRect = new Rectangle(0, 0, 256, 256);
                Rectangle srcRect = new Rectangle(0, 0, 256, 256);
                byte[] srcBytes = new byte[256 * 256 * 4];

                for (var x = 0; x < mapData.Rows; x++)
                {
                    dstRect.X = x * 256;

                    for (var y = 0; y < mapData.Columns; y++)
                    {
                        dstRect.Y = y * 256;

                        var thisFileId = folderId + (y * mapData.Rows + x);
                        var filePath = DatDigger.Utilities.DataFileIdToRelativePath(thisFileId);
                        filePath = Path.Combine(Properties.Settings.Default.GameDirectory, filePath);
                        var file = DatDigger.Sections.SectionLoader.OpenFile(filePath);
                        if (file == null)
                        {
                            dstBmp.Dispose();
                            return;
                        }
                        var gtex = file.FindChild<DatDigger.Sections.Texture.GtexData>();
                        using (var tex = new SlimDX.Direct3D9.Texture(context.Device,
                            gtex.Header.Width,
                            gtex.Header.Height,
                            1,
                            SlimDX.Direct3D9.Usage.None,
                            gtex.Format,
                            SlimDX.Direct3D9.Pool.Scratch))
                        {
                            var texData = tex.LockRectangle(0, SlimDX.Direct3D9.LockFlags.None);
                            texData.Data.Write(gtex.TextureData[0], 0, gtex.TextureData[0].Length);
                            tex.UnlockRectangle(0);

                            using (var bmpStream = SlimDX.Direct3D9.Texture.ToStream(tex, SlimDX.Direct3D9.ImageFileFormat.Bmp))
                            {
                                bmp = new Bitmap(bmpStream);
                            }
                        }

                        srcData = bmp.LockBits(srcRect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                        dstData = dstBmp.LockBits(dstRect, System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                        System.Runtime.InteropServices.Marshal.Copy(srcData.Scan0, srcBytes, 0, srcBytes.Length);
                        for (var i = 0; i < 256; i++)
                        {
                            int dstOffset = dstData.Stride * i;
                            System.Runtime.InteropServices.Marshal.Copy(srcBytes, 256 * 4 * i, dstData.Scan0.Increment(dstOffset), 256 * 4);
                        }

                        bmp.UnlockBits(srcData);
                        dstBmp.UnlockBits(dstData);

                        bmp.Dispose();

                        tileId++;
                    }
                }

                if (currentImage != null) { currentImage.Dispose(); }
                currentImage = dstBmp;
                pictureMap.Image = dstBmp;
                pictureMap.Width = mapData.Width;
                pictureMap.Height = mapData.Height;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to Unfurl map: " + ex.Message);
            }
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            if (currentImage == null) { return; }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.DefaultExt = "png";
                sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                sfd.OverwritePrompt = true;
                sfd.RestoreDirectory = true;
                sfd.Filter = "PNG Files|*.png";
                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    using (var str = sfd.OpenFile())
                    {
                        currentImage.Save(str, System.Drawing.Imaging.ImageFormat.Png);
                    }
                }
            }
        }
    }

    public class MapData
    {
        public int ID { get; set; }
        public int Folder { get; set; }
        public int ParentId { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Unknown1 { get; set; }
        public int Unknown2 { get; set; }
        public int Rows { get { return Width / 256; } }
        public int Columns { get { return Height / 256; } }
        public int TotalTiles { get { return Rows * Columns; } }
    }
}
