using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using DatDigger;

namespace ModelViewer
{
    public enum CharaType
    {
        Monster = 0,
        BgObject = 1,
        Weapon = 2,
        Player = 3
    }

    public class CharaModelData
    {
        public CharaType CharaType { get; set; }
        public int ModelId { get; set; }
        public string ModelPath { get; set; }
        public int SubModelId { get; set; }
        public string SubModelPath { get; set; }
        public int TextureId { get; set; }

        public string[] ModelFiles = new string[(int)ModelPart.NumModelParts];
        public List<string> FaceFiles = new List<string>();
        public List<string> HairFiles = new List<string>();
        public List<string>[] TextureFiles = new List<string>[(int)ModelPart.NumModelParts];
        public string SkeletonFile { get; set; }
        public string ModelCommonFile { get; set; }

        public bool CanRender { get { return HasTextures; } }
        public bool CanAnimate { get { return CanRender && HasAnimation; } }
        public bool HasAnimation { get; set; }
        public bool HasTextures { get; set; }
        public bool HasSounds { get; set; }

        public INavigable[] Models = new INavigable[(int)ModelPart.NumModelParts];
        public List<INavigable>[] Textures = new List<INavigable>[(int)ModelPart.NumModelParts];
        public INavigable Skeleton { get; set; }
        public INavigable ModelCommon { get; set; }

        public CharaModelData() { }
        public CharaModelData(CharaModelData copy)
        {
            this.CharaType = copy.CharaType;
            this.ModelId = copy.ModelId;
            this.ModelPath = copy.ModelPath;
            this.SubModelId = copy.SubModelId;
            this.SubModelPath = copy.SubModelPath;
            this.TextureId = copy.TextureId;

            this.ModelCommonFile = copy.ModelCommonFile;
            this.TextureFiles = (List<string>[])copy.TextureFiles.Clone();
            this.ModelFiles = (string[])copy.ModelFiles.Clone();
            this.SkeletonFile = copy.SkeletonFile;

            this.HasAnimation = copy.HasAnimation;
            this.HasTextures = copy.HasTextures;
            this.HasSounds = copy.HasSounds;

            this.Models = (INavigable[])copy.Models.Clone();
            this.Textures = (List<INavigable>[])copy.Textures.Clone();
            this.Skeleton = copy.Skeleton;
            this.ModelCommon = copy.ModelCommon;
        }
    }

    public enum ModelPart
    {
        Top,
        Dwn,
        Sho,
        Met,
        Glv,
        Pch,
        NumModelParts
    }

    public static class CharaHelper
    {
        private static string[] modelTypes = new string[] { "top", "dwn", "sho", "met", "glv", "pch" };

        private static string GetFolder(this CharaType type)
        {
            switch (type)
            {
                case CharaType.Monster:
                    return "mon";
                case CharaType.BgObject:
                    return "bgobj";
                case CharaType.Player:
                    return "pc";
                case CharaType.Weapon:
                    return "wep";
                default:
                    throw new InvalidOperationException("Unknown CharaType");
            }
        }

        private static string GetPrefix(this CharaType type)
        {
            switch (type)
            {
                case CharaType.Monster:
                    return "m";
                case CharaType.BgObject:
                    return "b";
                case CharaType.Player:
                    return "c";
                case CharaType.Weapon:
                    return "w";
                default:
                    throw new InvalidOperationException("Unknown CharaType");
            }
        }

        public static void LoadCharaData(TreeNode rootNode, CharaType type)
        {
            string folder = type.GetFolder();
            string prefix = type.GetPrefix();

            string charaDir = Path.Combine(Properties.Settings.Default.GameDirectory, String.Format("client/chara/{0}/", folder));
            if (!Directory.Exists(charaDir))
            {
                rootNode.Nodes.Add(new TreeNode("!error - directory does not exist"));
                return;
            }

            string cmnDir = Path.Combine(charaDir, "cmn");
            string cmnFile = null;
            if (Directory.Exists(cmnDir))
            {
                cmnFile = Path.Combine(cmnDir, "0001");
            }

            var subdirs = Directory.GetDirectories(charaDir, prefix + "???", SearchOption.TopDirectoryOnly);
            foreach (string subdir in subdirs)
            {
                string dirName = Path.GetFileName(subdir);
                int modelId = 0;
                if (!Int32.TryParse(dirName.Substring(1), out modelId))
                {
                    continue;
                }

                CharaModelData charaModel = new CharaModelData();
                charaModel.CharaType = type;
                charaModel.ModelId = modelId;
                charaModel.ModelPath = subdir;
                charaModel.ModelCommonFile = cmnFile;

                TreeNode node = new TreeNode(dirName);
                if (LoadModel(charaModel, node))
                {
                    rootNode.Nodes.Add(node);
                }
            }
        }

        private static bool LoadModel(CharaModelData modelData, TreeNode parent)
        {
            string animDir = Path.Combine(modelData.ModelPath, "act/");
            bool hasAnim = Directory.Exists(animDir);
            string skeletonFile = Path.Combine(modelData.ModelPath, "skl/0001");
            string subModelDir = Path.Combine(modelData.ModelPath, "equ/");

            var subModels = Directory.GetDirectories(subModelDir, "e???", SearchOption.TopDirectoryOnly);
            foreach (string subModel in subModels)
            {
                string dirName = Path.GetFileName(subModel);
                int subModelId = 0;
                if (!Int32.TryParse(dirName.Substring(1), out subModelId))
                {
                    continue;
                }

                CharaModelData charaModelData = new CharaModelData(modelData);
                charaModelData.SkeletonFile = skeletonFile;
                charaModelData.SubModelId = subModelId;
                charaModelData.SubModelPath = subModel;
                charaModelData.HasAnimation = hasAnim;

                TreeNode node = new TreeNode(dirName);
                node.Tag = charaModelData;
                if (LoadSubModel(charaModelData, node))
                {
                    parent.Nodes.Add(node);
                }
            }

            return true;
        }

        private static bool LoadSubModel(CharaModelData modelData, TreeNode parent)
        {
            string soundDir = Path.Combine(modelData.SubModelPath, "top_snd/");

            // Load Sounds
            if (Directory.Exists(soundDir))
            {
                modelData.HasSounds = true;
            }

            // Load Model Parts
            foreach (ModelPart part in Enum.GetValues(typeof(ModelPart)))
            {
                modelData.HasTextures |= LoadModelPart(part, modelData);
            }

            return true;
        }

        private static bool LoadModelPart(ModelPart part, CharaModelData modelData)
        {
            if (part == ModelPart.NumModelParts) { return false; }

            modelData.ModelFiles[(int)part] = null;
            modelData.TextureFiles[(int)part] = null;

            string partFolder = modelTypes[(int)part];

            // Get Model File
            string modelDir = Path.Combine(modelData.SubModelPath, String.Format("{0}_mdl/", partFolder));
            string modelFile;
            if (Directory.Exists(modelDir))
            {
                modelFile = Path.Combine(modelDir, "0001");
            }
            else
            {
                return false;
            }

            // Get Textures Files
            string tex1Dir = Path.Combine(modelData.SubModelPath, String.Format("{0}_tex1/", partFolder));
            string tex2Dir = Path.Combine(modelData.SubModelPath, String.Format("{0}_tex2/", partFolder));

            string texDir = null;
            if (Directory.Exists(tex2Dir))
            {
                texDir = tex2Dir;
            }
            else if (Directory.Exists(tex1Dir))
            {
                texDir = tex1Dir;
            }

            if (texDir == null)
            {
                return false;
            }

            var textures = Directory.GetFiles(texDir);

            modelData.TextureFiles[(int)part] = new List<string>(textures);
            modelData.ModelFiles[(int)part] = modelFile;

            return true;
        }
    }
}
