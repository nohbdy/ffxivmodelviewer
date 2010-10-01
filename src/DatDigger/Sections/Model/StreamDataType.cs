using System;
using System.Collections.Generic;
using System.IO;

using SlimDX.Direct3D9;

namespace DatDigger.Sections.Model
{
    public enum StreamDataTypeId : short
    {
        Position = 0,
        Normal = 2,
        Color = 3,
        Binormal = 4,
        UV_1 = 8,
        UV_2 = 9,
        UV_3 = 10,
        UV_4 = 11,
        Tangent = 13,
        BoneWeight = 14,
        BoneIndex = 15,
        Index = 0xFF
    }

    class PositionDataType : StreamDataType
    {
        public PositionDataType(short id, int numValues, int dataSize, DeclarationType declarationType, DeclarationUsage declarationUsage, byte usageIndex)
            : base(id, numValues, dataSize, declarationType, declarationUsage, usageIndex) { }

        public override void Decompress(BinaryReaderEx reader, BinaryWriter writer, StreamElement element)
        {
            FloatStreamDecompressor decompressor = StreamDecompressors.GetFloatDecompressor(element.CompressionType);
            float x = decompressor(reader);
            float y = decompressor(reader);
            float z = decompressor(reader);

            writer.Write(x);
            writer.Write(y);
            writer.Write(z);            
        }
    }

    class VectorDataType : StreamDataType
    {
        public VectorDataType(short id, int numValues, int dataSize, DeclarationType declarationType, DeclarationUsage declarationUsage, byte usageIndex)
            : base(id, numValues, dataSize, declarationType, declarationUsage, usageIndex) { }

        public override void Decompress(BinaryReaderEx reader, BinaryWriter writer, StreamElement element)
        {
            FloatStreamDecompressor decompressor = StreamDecompressors.GetFloatDecompressor(element.CompressionType);
            float x = decompressor(reader);
            float y = decompressor(reader);
            float z = decompressor(reader);

            writer.Write(x);
            writer.Write(y);
            writer.Write(z);
        }
    }

    class IndexDataType : StreamDataType
    {
        public IndexDataType(short id, int numValues, int dataSize, DeclarationType declarationType, DeclarationUsage declarationUsage, byte usageIndex)
            : base(id, numValues, dataSize, declarationType, declarationUsage, usageIndex) { }

        public override void Decompress(BinaryReaderEx reader, BinaryWriter writer, StreamElement element)
        {
            var decompressor = StreamDecompressors.GetUShortDecompressor(element.CompressionType);
            var val = decompressor(reader);
            writer.Write(val);
        }
    }

    class BoneIndexDataType : StreamDataType
    {
        public BoneIndexDataType(short id, int numValues, int dataSize, DeclarationType declarationType, DeclarationUsage declarationUsage, byte usageIndex)
            : base(id, numValues, dataSize, declarationType, declarationUsage, usageIndex) { }

        public override void Decompress(BinaryReaderEx reader, BinaryWriter writer, StreamElement element)
        {
            var decompressor = StreamDecompressors.GetIntDecompressor(element.CompressionType);
            int count = Math.Min(StreamDataType.MaxBonesPerVert, element.NumElements);
            for (var i = 0; i < count; i++)
            {
                var val = decompressor(reader);
                writer.Write(val);
            }
        }
    }

    public class StreamDataType
    {
        private static Dictionary<short, StreamDataType> map = new Dictionary<short, StreamDataType>();

        public const int MaxBonesPerVert = 8;

        public static readonly StreamDataType Position = new PositionDataType(0, 3, 12, DeclarationType.Float3, DeclarationUsage.Position, 0);
        public static readonly StreamDataType Normal = new VectorDataType(2, 3, 12, DeclarationType.Float3, DeclarationUsage.Normal, 0);
        public static readonly StreamDataType Color = new StreamDataType(3, 3, 12, DeclarationType.Float3, DeclarationUsage.Color, 0);
        public static readonly StreamDataType Binormal = new VectorDataType(4, 3, 12, DeclarationType.Float3, DeclarationUsage.TextureCoordinate, 5);
        public static readonly StreamDataType UV_1 = new StreamDataType(8, 2, 8, DeclarationType.Float2, DeclarationUsage.TextureCoordinate, 0);
        public static readonly StreamDataType UV_2 = new StreamDataType(9, 2, 8, DeclarationType.Float2, DeclarationUsage.TextureCoordinate, 1);
        public static readonly StreamDataType UV_3 = new StreamDataType(10, 2, 8, DeclarationType.Float2, DeclarationUsage.TextureCoordinate, 2);
        public static readonly StreamDataType UV_4 = new StreamDataType(11, 2, 8, DeclarationType.Float2, DeclarationUsage.TextureCoordinate, 3);
        public static readonly StreamDataType Tangent = new VectorDataType(13, 3, 12, DeclarationType.Float3, DeclarationUsage.TextureCoordinate, 4);
        public static readonly StreamDataType BoneWeight = new StreamDataType(14, MaxBonesPerVert, 32, DeclarationType.Float4, DeclarationUsage.TextureCoordinate, 6);
        public static readonly StreamDataType BoneIndex = new BoneIndexDataType(15, MaxBonesPerVert, 32, DeclarationType.Ubyte4, DeclarationUsage.TextureCoordinate, 7);
        public static readonly StreamDataType Index = new IndexDataType(0xFF, 1, 2, DeclarationType.Unused, DeclarationUsage.TextureCoordinate, 0);

        static StreamDataType()
        {
            map.Add(Position.Id, Position);
            map.Add(Normal.Id, Normal);
            map.Add(Color.Id, Color);
            map.Add(Binormal.Id, Binormal);
            map.Add(UV_1.Id, UV_1);
            map.Add(UV_2.Id, UV_2);
            map.Add(UV_3.Id, UV_3);
            map.Add(UV_4.Id, UV_4);
            map.Add(Tangent.Id, Tangent);
            map.Add(BoneWeight.Id, BoneWeight);
            map.Add(BoneIndex.Id, BoneIndex);
            map.Add(Index.Id, Index);
        }

        public static StreamDataType GetStreamDataType(short id)
        {
            return map[id];
        }

        public StreamDataType(short id, int numValues, int dataSize, DeclarationType declarationType, DeclarationUsage declarationUsage, byte usageIndex)
        {
            this.Id = id;
            this.NumValues = numValues;
            this.DataSize = dataSize;
            this.DeclarationType = declarationType;
            this.DeclarationUsage = declarationUsage;
            this.UsageIndex = usageIndex;
        }

        /// <summary>Unique Id of this data type</summary>
        public short Id { get; private set; }

        /// <summary>Number of values to read for this data type</summary>
        public int NumValues { get; private set; }

        /// <summary>DataSize per vertex for this items of this type in bytes</summary>
        public int DataSize { get; private set; }

        public SlimDX.Direct3D9.DeclarationType DeclarationType { get; private set; }
        public SlimDX.Direct3D9.DeclarationUsage DeclarationUsage { get; private set; }
        public byte UsageIndex { get; private set; }

        public virtual void Decompress(BinaryReaderEx reader, BinaryWriter writer, StreamElement element)
        {
            int count = Math.Min(this.NumValues, element.NumElements);
            FloatStreamDecompressor decompressor = StreamDecompressors.GetFloatDecompressor(element.CompressionType);
            for (var i = 0; i < count; i++)
            {
                float val = decompressor(reader);
                writer.Write(val);
            }
        }
    }
}
