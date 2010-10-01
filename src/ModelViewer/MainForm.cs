using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using DatDigger;
using ModelViewer.Properties;

namespace ModelViewer
{
    public partial class MainForm : Form
    {
        private const string ApplicationName = "FFXIV File Explorer";
        private const string RegKeyx64 = @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\{F2C4E6E0-EB78-4824-A212-6DF6AF0E8E82}";
        private const string RegKeyx32 = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{F2C4E6E0-EB78-4824-A212-6DF6AF0E8E82}";

        private INavigable openFile;
        private TextBoxTraceListener traceListener;

        public MainForm()
        {
            InitializeComponent();

            traceListener = new TextBoxTraceListener(debugText);
            System.Diagnostics.Trace.Listeners.Add(traceListener);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Sound.SoundManager.Init();
            Cache.CacheManager.Init();

            System.Threading.Thread thread = new System.Threading.Thread(() =>
            {
                using (CacheProgress progressFrm = new CacheProgress())
                {
                    progressFrm.ShowDialog();
                }
            });
            thread.Start();

            try
            {
                // If dir not set, try to load from registry
                if (Settings.Default.GameDirectory.IsNullOrWhiteSpace())
                {
                    string installLocation = null;
                    string displayName = null;
                    string path;
                    string gameVerPath;
                    installLocation = Microsoft.Win32.Registry.GetValue(RegKeyx64, "InstallLocation", null) as string;
                    if (installLocation != null)
                    {
                        displayName = Microsoft.Win32.Registry.GetValue(RegKeyx64, "DisplayName", null) as string;
                        path = Path.Combine(installLocation, displayName);
                        gameVerPath = Path.Combine(path, "game.ver");
                        if (File.Exists(gameVerPath))
                        {
                            Settings.Default.GameDirectory = path;
                            Settings.Default.Save();
                        }
                    }
                    else
                    {
                        installLocation = Microsoft.Win32.Registry.GetValue(RegKeyx32, "InstallLocation", null) as string;
                        if (installLocation != null)
                        {
                            displayName = Microsoft.Win32.Registry.GetValue(RegKeyx64, "DisplayName", null) as string;
                            path = Path.Combine(installLocation, displayName);
                            gameVerPath = Path.Combine(path, "game.ver");
                            if (File.Exists(gameVerPath))
                            {
                                Settings.Default.GameDirectory = path;
                                Settings.Default.Save();
                            }
                        }
                    }
                }

                if (Settings.Default.GameDirectory.IsNullOrWhiteSpace())
                {
                    if (!OpenPreferenceWindow())
                    {
                        return;
                    }
                }
                else
                {
                    LoadCharaData();
                    LoadScriptData();
                }

                if (Cache.CacheManager.CacheIsStale)
                {
                    Cache.CacheManager.ReloadCache();
                }
            }
            finally
            {
                thread.Abort();
            }
        }

        private bool OpenPreferenceWindow(bool isRequired = false)
        {
            using (PreferencesForm preferences = new PreferencesForm())
            {
                string oldDir = Settings.Default.GameDirectory;
                if (preferences.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    // Only load the base xml if the directory is changed
                    if (!String.Equals(oldDir, Settings.Default.GameDirectory))
                    {
                        Directory.SetCurrentDirectory(Settings.Default.GameDirectory);
                        LoadCharaData();
                    }
                }
                else
                {
                    if (isRequired)
                    {
                        Application.Exit();
                        return false;
                    }
                }
            }

            return true;
        }

        private void LoadScriptData()
        {
            TreeNode scriptRoot = new TreeNode("Scripts");
            this.charaSelector.Nodes.Add(scriptRoot);
            LoadScriptRecursive(Path.Combine(Settings.Default.GameDirectory, "client/script"), scriptRoot);
        }

        private void LoadScriptRecursive(string path, TreeNode parent)
        {
            var dirs = Directory.GetDirectories(path);
            foreach (string dir in dirs)
            {
                TreeNode dirNode = new TreeNode(DatDigger.Sections.Script.NameDecoder.Decode(Path.GetFileName(dir)));
                parent.Nodes.Add(dirNode);
                LoadScriptRecursive(dir, dirNode);
            }

            var files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                TreeNode fileNode = new TreeNode(DatDigger.Sections.Script.NameDecoder.Decode(Path.GetFileName(file)));
                fileNode.Tag = file;
                parent.Nodes.Add(fileNode);
            }
        }

        private void LoadCharaData()
        {
            this.charaSelector.Nodes.Clear();
            TreeNode monstersRoot = new TreeNode("Monsters");
            this.charaSelector.Nodes.Add(monstersRoot);
            CharaHelper.LoadCharaData(monstersRoot, CharaType.Monster);

            TreeNode weaponsRoot = new TreeNode("Weapons");
            this.charaSelector.Nodes.Add(weaponsRoot);
            CharaHelper.LoadCharaData(weaponsRoot, CharaType.Weapon);

            TreeNode playerRoot = new TreeNode("Players");
            this.charaSelector.Nodes.Add(playerRoot);
            CharaHelper.LoadCharaData(playerRoot, CharaType.Player);

            TreeNode bgRoot = new TreeNode("Background Objects");
            this.charaSelector.Nodes.Add(bgRoot);
            CharaHelper.LoadCharaData(bgRoot, CharaType.BgObject);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            var result = DatDigger.Sections.SectionLoader.OpenFile(openFileDialog.FileName);

            if (result == null)
            {
                MessageBox.Show("Unable to open file " + openFileDialog.FileName);
                return;
            }

            openFile = result;

            // Build Tree View
            TreeNode rootNode = BuildNavigator(openFile);
            rootNode.ExpandAll();
            fileNavigator.Nodes.Add(rootNode);
            this.Text = String.Format("{0} - {1}", ApplicationName, openFileDialog.FileName);
        }

        private TreeNode BuildNavigator(INavigable item)
        {
            TreeNode node = new TreeNode(item.DisplayName);
            node.Tag = item;
            if (item.Children != null)
            {
                foreach (INavigable child in item.Children)
                {
                    if (child != null)
                    {
                        TreeNode childNode = BuildNavigator(child);
                        node.Nodes.Add(childNode);
                    }
                }
            }

            return node;
        }

        private void fileNavigator_AfterSelect(object sender, TreeViewEventArgs e)
        {
            object selected = null;
            if (e != null)
            {
                selected = e.Node.Tag;
                propertyGrid1.SelectedObject = selected;
            }

            btnDecompileShader.Visible = (selected is DatDigger.Sections.Shader.FileChunk);
            btnViewTexture.Visible = (selected is DatDigger.Sections.Texture.GtexData);
            btnRenderModel.Visible = (selected is DatDigger.Sections.Model.ModelContainerChunk);
            btnPlaySound.Enabled = (selected is DatDigger.Sound.IPlayable);
            btnRenderSkeleton.Visible = (selected is DatDigger.Sections.Skeleton.SkeletonSection);
            btnAnimatedSkeleton.Visible = (selected is DatDigger.Sections.Skeleton.SkeletonSection);
            btnSaveScript.Visible = (selected is DatDigger.Sections.Script.LuaFile);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFile = null;
            fileNavigator.Nodes.Clear();
            propertyGrid1.SelectedObject = null;
            this.Text = ApplicationName;

            fileNavigator_AfterSelect(null, null);
        }

        private void btnDecompileShader_Click(object sender, EventArgs e)
        {
            var shaderFile = fileNavigator.SelectedNode.Tag as DatDigger.Sections.Shader.FileChunk;
            if (shaderFile == null)
            {
                return;
            }

            using (var shader = new SlimDX.Direct3D9.ShaderBytecode(shaderFile.CompiledShader))
            {

                var data = shader.Disassemble(true);

                // Save data to temporary file
                var tempFileName = Path.GetTempFileName();
                using (FileStream fs = File.OpenWrite(tempFileName))
                {
                    data.CopyTo(fs);
                }

                var viewer = new ShaderViewer(shaderFile.Name, tempFileName);
                viewer.Show();
            }
        }

        private void btnViewTexture_Click(object sender, EventArgs e)
        {
            var gtex = fileNavigator.SelectedNode.Tag as DatDigger.Sections.Texture.GtexData;
            if (gtex == null)
            {
                return;
            }

            // Launch the texture renderer in a separate thread
            System.Threading.Thread t = new System.Threading.Thread((data) =>
            {
                using (Renderer.PreviewTexture sample = new Renderer.PreviewTexture((DatDigger.Sections.Texture.GtexData)data))
                {
                    sample.Run();
                }
            });
            t.Start(gtex);
        }

        private void clearLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.debugText.Clear();
        }

        private void btnPlaySound_Click(object sender, EventArgs e)
        {
            DatDigger.Sound.IPlayable playable = fileNavigator.SelectedNode.Tag as DatDigger.Sound.IPlayable;
            if (playable == null)
            {
                return;
            }

            Sound.SoundManager.Instance.Play(playable);
        }

        private void btnSoundStop_Click(object sender, EventArgs e)
        {
            Sound.SoundManager.Instance.Stop();
        }

        private void btnSoundPause_Click(object sender, EventArgs e)
        {
            Sound.SoundManager.Instance.Pause();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Sound.SoundManager.Shutdown();
            Cache.CacheManager.Shutdown();
        }

        private void btnRenderModel_Click(object sender, EventArgs e)
        {
            var model = fileNavigator.SelectedNode.Tag as DatDigger.Sections.Model.ModelContainerChunk;
            if (model == null)
            {
                return;
            }

            // Launch the texture renderer in a separate thread
            System.Threading.Thread t = new System.Threading.Thread((data) =>
            {
                using (Renderer.PreviewModel sample = new Renderer.PreviewModel((DatDigger.Sections.Model.ModelContainerChunk)data))
                {
                    sample.Run();
                }
            });
            t.Start(model);
        }

        private void showVertexShaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SlimDX.Direct3D9.ShaderBytecode shader = null;
            try
            {
                shader = SlimDX.Direct3D9.ShaderBytecode.Compile(this.GetType().Assembly.GetResourceAsString("ModelViewer.vshader.vsh"), "vs_main", "vs_3_0", SlimDX.Direct3D9.ShaderFlags.OptimizationLevel3);

                var data = shader.Disassemble(true);

                // Save data to temporary file
                var tempFileName = Path.GetTempFileName();
                using (FileStream fs = File.OpenWrite(tempFileName))
                {
                    data.CopyTo(fs);
                }

                var viewer = new ShaderViewer("vshader.vsh", tempFileName);
                viewer.Show();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return;
            }
            finally
            {
                if (shader != null) { shader.Dispose(); }
            }
        }

        private void showPixelShaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SlimDX.Direct3D9.ShaderBytecode shader = null;
            try
            {
                shader = SlimDX.Direct3D9.ShaderBytecode.Compile(this.GetType().Assembly.GetResourceAsString("ModelViewer.pshader.psh"), "ps_main", "ps_3_0", SlimDX.Direct3D9.ShaderFlags.Debug);

                var data = shader.Disassemble(true);

                // Save data to temporary file
                var tempFileName = Path.GetTempFileName();
                using (FileStream fs = File.OpenWrite(tempFileName))
                {
                    data.CopyTo(fs);
                }

                var viewer = new ShaderViewer("pshader.psh", tempFileName);
                viewer.Show();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return;
            }
            finally
            {
                if (shader != null) { shader.Dispose(); }
            }
        }

        private void modelListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rootContainer.Panel2Collapsed = !rootContainer.Panel2Collapsed;
        }

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenPreferenceWindow();
        }

        private void charaSelector_AfterSelect(object sender, TreeViewEventArgs e)
        {
            CharaModelData modelData = (e.Node.Tag as CharaModelData);
            if (modelData == null) { return; }

            btnRenderChara.Enabled = modelData.CanRender;
            btnCharaAnimation.Enabled = modelData.HasAnimation && modelData.CanRender;
        }

        private void btnHideChara_Click(object sender, EventArgs e)
        {
            rootContainer.Panel2Collapsed = true;
        }

        private void btnRenderChara_Click(object sender, EventArgs e)
        {
            var modelData = charaSelector.SelectedNode.Tag as CharaModelData;

            for (var i = 0; i < modelData.ModelFiles.Length; i++)
            {
                if (modelData.ModelFiles[i] != null)
                {
                    modelData.Models[i] = DatDigger.Sections.SectionLoader.OpenFile(modelData.ModelFiles[i]);

                    int numTextures = modelData.TextureFiles[i].Count;
                    modelData.Textures[i] = new List<INavigable>(numTextures);
                    for (var t = 0; t < numTextures; t++)
                    {
                        modelData.Textures[i].Add(DatDigger.Sections.SectionLoader.OpenFile(modelData.TextureFiles[i][t]));
                    }
                }
            }

            modelData.Skeleton = DatDigger.Sections.SectionLoader.OpenFile(modelData.SkeletonFile);
            if (modelData.ModelCommonFile != null) { modelData.ModelCommon = DatDigger.Sections.SectionLoader.OpenFile(modelData.ModelCommonFile); }

            // Launch the texture renderer in a separate thread
            System.Threading.Thread thread = new System.Threading.Thread((data) =>
            {
                using (Renderer.PreviewChara sample = new Renderer.PreviewChara((CharaModelData)data))
                {
                    sample.Run();
                }
            });
            thread.Start(modelData);
        }

        private void btnRenderSkeleton_Click(object sender, EventArgs e)
        {
            var skele = fileNavigator.SelectedNode.Tag as DatDigger.Sections.Skeleton.SkeletonSection;
            if (skele == null)
            {
                return;
            }

            // Launch the texture renderer in a separate thread
            System.Threading.Thread t = new System.Threading.Thread((data) =>
            {
                using (Renderer.PreviewSkeleton sample = new Renderer.PreviewSkeleton((DatDigger.Sections.Skeleton.SkeletonSection)data))
                {
                    sample.Run();
                }
            });
            t.Start(skele);
        }

        private void btnSaveScript_Click(object sender, EventArgs e)
        {
            var luaFile = fileNavigator.SelectedNode.Tag as DatDigger.Sections.Script.LuaFile;
            if (luaFile == null)
            {
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.OverwritePrompt = true;
                sfd.RestoreDirectory = true;
                sfd.SupportMultiDottedExtensions = true;
                sfd.DefaultExt = "luac";
                if (sfd.ShowDialog() != System.Windows.Forms.DialogResult.OK) { return; }

                using (var fs = new FileStream(sfd.FileName, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    fs.Write(luaFile.Data, 0, luaFile.Data.Length);
                    fs.Close();
                }
            }
        }

        private void cacheStatusMenuItem_Click(object sender, EventArgs e)
        {
            if (Cache.CacheManager.CacheBuilt)
            {
                Trace.WriteLine(String.Format("Cache Built {0:F} (Game Version: {1})", Cache.CacheManager.DateGenerated, Cache.CacheManager.GameVersion));
            }
            else
            {
                Trace.WriteLine("Cache: Cache Not Build");
            }
        }

        private void cacheReloadMenuItem_Click(object sender, EventArgs e)
        {
            Trace.WriteLine("Loading Cache, Please Wait...");
            DateTime start = DateTime.Now;
            Cache.CacheManager.ReloadCache();
            DateTime end = DateTime.Now;
            TimeSpan duration = end - start;
            Trace.WriteLine("Cache Load Complete in " + duration.ToString());
        }

        private void txtNpcSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13) // If <Enter> was pressed
            {
                e.Handled = true;
                var results = Cache.ActorCache.SearchByName(txtNpcSearch.Text);
                npcSearchDataGrid.AutoGenerateColumns = false;
                npcSearchDataGrid.Columns.Clear();

                var renderCol = new DataGridViewButtonColumn();
                renderCol.HeaderText = "";
                renderCol.Name = "RenderBtn";
                renderCol.Frozen = true;
                renderCol.SortMode = DataGridViewColumnSortMode.NotSortable;
                renderCol.Text = ">";
                renderCol.UseColumnTextForButtonValue = true;
                renderCol.DisplayIndex = 0;
                npcSearchDataGrid.Columns.Add(renderCol);

                var idCol = new DataGridViewTextBoxColumn();
                idCol.HeaderText = "ID";
                idCol.Name = "ID";
                idCol.DataPropertyName = "ID";
                idCol.SortMode = DataGridViewColumnSortMode.Automatic;
                idCol.DisplayIndex = 1;
                npcSearchDataGrid.Columns.Add(idCol);

                var nameCol = new DataGridViewTextBoxColumn();
                nameCol.HeaderText = "Name";
                nameCol.Name = "Name";
                nameCol.DataPropertyName = "Name";
                nameCol.SortMode = DataGridViewColumnSortMode.Automatic;
                nameCol.DisplayIndex = 2;
                nameCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                npcSearchDataGrid.Columns.Add(nameCol);

                var raceCol = new DataGridViewTextBoxColumn();
                raceCol.HeaderText = "Race";
                raceCol.Name = "Race";
                raceCol.DataPropertyName = "Race";
                raceCol.SortMode = DataGridViewColumnSortMode.Automatic;
                raceCol.DisplayIndex = 3;
                npcSearchDataGrid.Columns.Add(raceCol);

                npcSearchDataGrid.DataSource = results;
            }
        }

        private void npcSearchDataGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                var actor = ((System.Collections.ObjectModel.ReadOnlyCollection<Cache.Actor>)npcSearchDataGrid.DataSource)[e.RowIndex];

                // Launch the texture renderer in a separate thread
                System.Threading.Thread t = new System.Threading.Thread((data) =>
                {
                    try
                    {
                        using (var sample = new Renderer.PreviewActor((Cache.Actor)data))
                        {
                            sample.Run();
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.Message);
                        Trace.WriteLine(ex.StackTrace);
                    }
                });
                t.Start(actor);
            }
        }

        private void cartographerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cartographer frm = new Cartographer();
            frm.Show();
        }

        private void btnAnimatedSkeleton_Click(object sender, EventArgs e)
        {
            var skele = fileNavigator.SelectedNode.Tag as DatDigger.Sections.Skeleton.SkeletonSection;
            if (skele == null)
            {
                return;
            }

            // Launch the texture renderer in a separate thread
            System.Threading.Thread t = new System.Threading.Thread((data) =>
            {
                using (Renderer.PreviewAnimatedSkeleton sample = new Renderer.PreviewAnimatedSkeleton((DatDigger.Sections.Skeleton.SkeletonSection)data))
                {
                    sample.Run();
                }
            });
            t.Start(skele);
        }

        private void btnCharaAnimation_Click(object sender, EventArgs e)
        {
            var modelData = charaSelector.SelectedNode.Tag as CharaModelData;

            for (var i = 0; i < modelData.ModelFiles.Length; i++)
            {
                if (modelData.ModelFiles[i] != null)
                {
                    modelData.Models[i] = DatDigger.Sections.SectionLoader.OpenFile(modelData.ModelFiles[i]);

                    int numTextures = modelData.TextureFiles[i].Count;
                    modelData.Textures[i] = new List<INavigable>(numTextures);
                    for (var t = 0; t < numTextures; t++)
                    {
                        modelData.Textures[i].Add(DatDigger.Sections.SectionLoader.OpenFile(modelData.TextureFiles[i][t]));
                    }
                }
            }

            modelData.Skeleton = DatDigger.Sections.SectionLoader.OpenFile(modelData.SkeletonFile);
            if (modelData.ModelCommonFile != null) { modelData.ModelCommon = DatDigger.Sections.SectionLoader.OpenFile(modelData.ModelCommonFile); }

            // Launch the texture renderer in a separate thread
            System.Threading.Thread thread = new System.Threading.Thread((data) =>
            {
                try
                {
                    using (Renderer.PreviewAnimatedChara sample = new Renderer.PreviewAnimatedChara((CharaModelData)data))
                    {
                        sample.Run();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                    System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                }
            });
            thread.Start(modelData);
        }
    }
}
