using System.Collections.Generic;

namespace DatDigger.Sections.Animation
{
    public class MotionInfoBlock
    {
        public string MotionName { get; set; }
        public sbyte FrameCount { get; set; }
        public sbyte CurveCoefficient { get; set; } // Divide by 10
        public sbyte InitialVelocity { get; set; } // Divide by 10
        public sbyte RollingFrameEnd { get; set; }
        public sbyte CaseOfRightFootEnd { get; set; } // ??
        public sbyte CaseOfLeftFootEnd { get; set; } // ??
        public sbyte Unk1 { get; set; }
        public sbyte Unk2 { get; set; }
        public string SubstituteMotionName { get; set; }
        public sbyte SubstituteMotionStartFrame { get; set; }
        public sbyte IdleFramesToBlend { get; set; }
        public sbyte FrameStartingRotationStop { get; set; }
        public sbyte EndsOnWhichFoot { get; set; } // 1 = left, 0 = right
        public short Unknown { get; set; } // == zero

        public override string ToString()
        {
            return MotionName;
        }
    }

    public class CibmSection : CibSection
    {
        private const int bytesPerBlock = 0x2E;

        public List<MotionInfoBlock> MotionInfoBlocks { get; private set; }

        public override void LoadSection(BinaryReaderEx reader)
        {
            base.LoadSection(reader);

            this.MotionInfoBlocks = new List<MotionInfoBlock>();
            int numMIBs = (this.SectionLength - 4) / bytesPerBlock;
            for (var i = 0; i < numMIBs; i++)
            {
                var mib = new MotionInfoBlock();
                mib.MotionName = reader.ReadFixedLengthString(16).Trim();
                mib.FrameCount = reader.ReadSByte();
                mib.CurveCoefficient = reader.ReadSByte();
                mib.InitialVelocity = reader.ReadSByte();
                mib.RollingFrameEnd = reader.ReadSByte();
                mib.CaseOfRightFootEnd = reader.ReadSByte();
                mib.CaseOfLeftFootEnd = reader.ReadSByte();
                mib.Unk1 = reader.ReadSByte();
                mib.Unk2 = reader.ReadSByte();
                mib.SubstituteMotionName = reader.ReadFixedLengthString(16).Trim();
                mib.SubstituteMotionStartFrame = reader.ReadSByte();
                mib.IdleFramesToBlend = reader.ReadSByte();
                mib.FrameStartingRotationStop = reader.ReadSByte();
                mib.EndsOnWhichFoot = reader.ReadSByte();
                mib.Unknown = reader.ReadInt16();

                this.MotionInfoBlocks.Add(mib);
            }
        }
    }
}
