using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.IO;


namespace ModelViewer.Cache
{
    [RunInstaller(true)]
    public partial class DeleteCacheOnUninstallAction : System.Configuration.Install.Installer
    {
        public DeleteCacheOnUninstallAction()
        {
            InitializeComponent();
        }

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
            try
            {
                // delete any addictional files (or comepletely remove the folder)
                string pathtodelete = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                // MessageBox.Show("Deleting: " + pathtodelete);
                if (pathtodelete != null && Directory.Exists(pathtodelete))
                {
                    // Attempt to delete the cache file
                    string cacheFile = Path.Combine(pathtodelete, "cache.sqlite");
                    SafeDeleteFile(cacheFile);
                }
            }
            catch
            {
            }
        }

        private static void SafeDeleteFile(string file)
        {
            try
            {
                File.Delete(file);
            }
            catch
            {
            }
        }

        private static void SafeDeleteDirectory(string directory)
        {
            try
            {
                Directory.Delete(directory, true);
            }
            catch (Exception)
            {
            }
        }
    }
}
