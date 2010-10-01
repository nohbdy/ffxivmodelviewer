using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ModelViewer.Properties;

namespace ModelViewer
{
    public partial class PreferencesForm : Form
    {
        public string GameDirectory { get; set; }

        public PreferencesForm()
        {
            InitializeComponent();
        }

        private void PreferencesForm_Load(object sender, EventArgs e)
        {
            GameDirectory = Settings.Default.GameDirectory;
            gameDirectoryPath.Text = GameDirectory;
        }

        private void chooseGameDirectoryButton_Click(object sender, EventArgs e)
        {
            if (!gameDirectoryPath.Text.IsNullOrWhiteSpace())
            {
                folderBrowser.SelectedPath = gameDirectoryPath.Text;
            }

            bool gotDir = false;

            do
            {
                var dialogResult = folderBrowser.ShowDialog();

                if (dialogResult != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }

                if (folderBrowser.SelectedPath.DirectoryContains("ffxivgame.exe"))
                {
                    gotDir = true;
                }
                else
                {
                    var abortRetryIgnore = MessageBox.Show("Selected directory does not contain ffxivgame.exe", "Are you sure?", MessageBoxButtons.AbortRetryIgnore);
                    switch (abortRetryIgnore)
                    {
                        case System.Windows.Forms.DialogResult.Abort:
                            return;
                        case System.Windows.Forms.DialogResult.Ignore:
                            gotDir = true;
                            break;
                    }
                }
            } while (!gotDir);

            GameDirectory = folderBrowser.SelectedPath;
            gameDirectoryPath.Text = GameDirectory;
        }

        private void PreferencesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            // Save Preferences
            Settings.Default.GameDirectory = GameDirectory;

            Settings.Default.Save();
        }

        private void okayButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}
