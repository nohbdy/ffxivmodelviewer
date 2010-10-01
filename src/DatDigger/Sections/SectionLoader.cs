using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace DatDigger.Sections
{
    public delegate SectionBase SectionFactory();

    public static class SectionLoader
    {
        private const int PWIB = 0x42495750;
        private const int GTEX = 0x58455447;
        private const int SSCF = 0x46435353;
        private const int SEDB = 0x42444553;
        private const int SQEX = 0x58455153;
        private const int RLE = 0x0C656C72;
        private const int SANE = 0x656E6173;
        private const int SPKL = 0x4C4B5053;

        private static Dictionary<int, SectionFactory> sections = new Dictionary<int, SectionFactory>();
        private static Dictionary<long, SectionType> sectionMagicNumbers = new Dictionary<long, SectionType>();

        private static SectionBase GenericFactory<T>() where T : SectionBase, new()
        {
            return new T();
        }

        private static void RegisterSection<T>(SectionType type, SectionMagicNumber magicNumber = SectionMagicNumber.Unknown) where T : SectionBase, new()
        {
            sections.Add((int)type, GenericFactory<T>);

            if (magicNumber != SectionMagicNumber.Unknown)
            {
                sectionMagicNumbers[(long)magicNumber] = type;
            }
        }

        static SectionLoader()
        {
            RegisterSection<Resource.ResourceSection>(SectionType.bin, SectionMagicNumber.Resource);
            RegisterSection<Resource.ResourceSection>(SectionType.trb, SectionMagicNumber.Resource);
            RegisterSection<Texture.TextureSection>(SectionType.txb, SectionMagicNumber.Texture);
            RegisterSection<Shader.ShaderSection>(SectionType.sdrb);
            RegisterSection<Skeleton.SkeletonSection>(SectionType.skl);
            RegisterSection<Model.ModelSection>(SectionType.wrb);
            RegisterSection<Sound.SscfSection>(SectionType.sscf, SectionMagicNumber.Sound);
            RegisterSection<PhbSection>(SectionType.phb, SectionMagicNumber.Phb);
            RegisterSection<Animation.MtbSection>(SectionType.mtb, SectionMagicNumber.Mtb);
            RegisterSection<Animation.McbSection>(SectionType.mcb);
            RegisterSection<Animation.ScbSection>(SectionType.scb);
            RegisterSection<Skeleton.ElbSection>(SectionType.elb);
            RegisterSection<Model.EquipParamSection>(SectionType.eqp);
            RegisterSection<DummySection>(SectionType.veff, SectionMagicNumber.Veff);
            RegisterSection<DummySection>(SectionType.vins, SectionMagicNumber.Vins);
            RegisterSection<DummySection>(SectionType.vmdl, SectionMagicNumber.Vmdl);
            RegisterSection<Texture.VtexSection>(SectionType.vtex, SectionMagicNumber.Vtex);
            RegisterSection<DummySection>(SectionType.leaf, SectionMagicNumber.Leaf);
            RegisterSection<DummySection>(SectionType.acb);
            RegisterSection<CibSection>(SectionType.ciba);
            RegisterSection<CibSection>(SectionType.cibb);
            RegisterSection<Animation.CibcSection>(SectionType.cibc);
            RegisterSection<CibSection>(SectionType.cibe);
            RegisterSection<CibSection>(SectionType.cibf);
            RegisterSection<CibSection>(SectionType.cibg);
            RegisterSection<CibSection>(SectionType.cibh);
            RegisterSection<Animation.CibmSection>(SectionType.cibm);
            RegisterSection<CibSection>(SectionType.cibp);
            RegisterSection<CibSection>(SectionType.cibs);
            RegisterSection<Animation.CibtSection>(SectionType.cibt);
            RegisterSection<CibSection>(SectionType.cibw);
        }

        /// <summary>Creates a section of a given type</summary>
        /// <param name="type">The type of section desired</param>
        /// <returns>A newly created section of the desired type</returns>
        private static SectionBase CreateSection(SectionType type)
        {
            SectionFactory factory;
            if (sections.TryGetValue((int)type, out factory))
            {
                SectionBase section = factory();
                section.SectionType = type;
                return section;
            }

            throw new InvalidOperationException(String.Format("Unknown Section Type: {0}", Encoding.ASCII.GetString(BitConverter.GetBytes((int)type))));
        }

        /// <summary>Attempts to create a section by guessing based on the next 8 bytes</summary>
        /// <param name="reader">A BinaryReaderEx positioned at the start of a section</param>
        /// <returns>A newly created section</returns>
        private static SectionBase CreateSection(BinaryReaderEx reader)
        {
            // Attempt to guess which type of section this is based on the first few bytes
            long offset = reader.BaseStream.Position;
            long id = reader.ReadInt64();

            SectionType typeGuess;
            if (!sectionMagicNumbers.TryGetValue(id, out typeGuess))
            {
                throw new InvalidOperationException(String.Format("Unable to guess what section uses header {0}", Encoding.ASCII.GetString(BitConverter.GetBytes(id))));
            }

            reader.BaseStream.Position = offset;
            return CreateSection(typeGuess);
        }

        /// <summary>Attempts to load a section of a given type</summary>
        /// <param name="sectionType">The type of section to load</param>
        /// <param name="reader">A BinaryReaderEx positioned at the start of a section</param>
        /// <returns>A newly created and loaded section</returns>
        public static SectionBase LoadSection(SectionType sectionType, BinaryReaderEx reader, INavigable parent, int sectionLength)
        {
            SectionBase result = CreateSection(sectionType);
            if (result != null)
            {
                result.Parent = parent;
                result.SectionLength = sectionLength;
                result.LoadSection(reader);
            }

            return result;
        }

        /// <summary>Attempts to load a section by guessing based on the next 8 bytes</summary>
        /// <param name="reader">A BinaryReaderEx positioned at the start of a section</param>
        /// <returns>A newly created and loaded section</returns>
        public static SectionBase LoadSection(BinaryReaderEx reader, INavigable parent)
        {
            SectionBase result = CreateSection(reader);
            if (result != null)
            {
                result.Parent = parent;
                result.LoadSection(reader);
            }
            
            return result;
        }

        public static INavigable OpenFile(string path)
        {
            INavigable result = null;

            byte[] fileData;
            using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var fileLen = fs.Length;
                fileData = new byte[fileLen];
                fs.Read(fileData, 0, (int)fileLen);
            }

            try
            {
                using (MemoryStream memStream = new MemoryStream(fileData, false))
                using (BinaryReaderEx reader = new BinaryReaderEx(memStream))
                {
                    int magicNumber = reader.ReadInt32();
                    reader.BaseStream.Position = 0;
                    switch (magicNumber)
                    {
                        case SANE:
                            {
                                if (false)
                                {
                                    reader.BaseStream.Position = 4;
                                    int len = (int)reader.BaseStream.Length - 4;
                                    byte[] data = reader.ReadBytes(len);
                                    for (var i = 0; i < len; i++) { data[i] ^= 0x73; }
                                    string savePath = Path.Combine(Path.GetDirectoryName(path), "sanfile.san");
                                    using (var fs = new FileStream(savePath, FileMode.CreateNew, FileAccess.Write))
                                    {
                                        fs.Write(data, 0, data.Length);
                                        fs.Close();
                                    }
                                    Trace.WriteLine("SAN File saved to " + savePath);
                                }
                                result = new Sections.DummySection();
                                Trace.WriteLine("View code for SAN file stuff");
                            }
                            break;
                        case RLE:
                            {
                                var lua = new Sections.Script.LuaFile();
                                lua.LoadFile(reader);
                                result = lua;
                            }
                            break;
                        case PWIB:
                            {
                                var pwib = new DatDigger.Sections.PwibSection();
                                pwib.LoadData(reader);
                                pwib.FilePath = path;
                                result = pwib;
                            }
                            break;
                        case GTEX:
                            {
                                var gtex = new Sections.Texture.GtexData();
                                gtex.LoadSection(reader);
                                result = gtex;
                            }
                            break;
                        case SPKL:
                            {
                                var spkl = new Sections.UI.SparkleFile(path);
                                spkl.LoadFile(reader);
                                result = spkl;
                            }
                            break;
                        case SEDB:
                            {
                                var rootSection = Sections.SectionLoader.LoadSection(reader, null);
                                result = rootSection;
                            }
                            break;
                        case SQEX:
                            {
                                var sqexFile = new Sections.UI.SqexFile(path, Path.GetFileName(path));
                                sqexFile.LoadFile(reader);
                                result = sqexFile;
                                Trace.WriteLine("Decoded String");
                                Trace.WriteLine("-------------------------------------");
                                Trace.WriteLine(sqexFile.Contents);
                            }
                            break;
                        default:
                            return null;
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                Trace.WriteLine("Exception while loading " + path);
                Trace.WriteLine(ex.Message);
                Trace.WriteLine(ex.StackTrace);
                result = null;
            }

            return result;
        }
    }
}
