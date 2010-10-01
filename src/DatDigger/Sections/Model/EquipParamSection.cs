using System.Collections.Generic;

namespace DatDigger.Sections.Model
{
    public class EquipParam
    {
        public const int Header0 = 0;
        public const int Header1 = 1;
        public const int Header2 = 2;
        public const int Header3 = 3;
        public const int HasTop = 4;
        public const int TopGloveComparisonBase = 8;
        public const int TopHelmComparisonBase = 12;
        public const int TopEnableShpPcb = 19;
        public const int TopEnableShpPcr = 20;
        public const int TopEnableShpPcl = 21;
        public const int TopEnableShpBlt = 22;
        public const int TopHasPants = 37;
        public const int TopHasGloves = 38;
        public const int TopHasBelt = 39;
        public const int TopHasHelm = 40;
        public const int TopEnableOInr = 41;
        public const int TopEnableONek = 42;
        public const int TopEnableOWrl = 43;
        public const int TopEnableOWrr = 44;
        public const int TopEnableAtrLpd = 53;
        public const int TopEnableAtrPcb = 54;
        public const int TopEnableAtrPcr = 55;
        public const int TopEnableAtrPcl = 56;
        public const int TopEnableAtrBlt = 57;
        public const int HasPants = 78;
        public const int PantLength = 79;
        public const int PantShoeComparisonBase = 82;
        public const int PantTopComparisonBase = 86;
        public const int PantEnableShpBlt = 92;
        public const int PantHasShoes = 97;
        public const int HasGloves = 113;
        public const int GloveLength = 114;
        public const int GloveTopComparison = 118;
        public const int GloveEnableOWrl = 127;
        public const int GloveEnableOWrr = 128;
        public const int GloveEnableOIdl = 129;
        public const int GloveEnableOIdr = 130;
        public const int GloveEnableORil = 131;
        public const int GloveEnableORir = 132;
        public const int GloveEnableBTopSodeL = 141;
        public const int GloveEnableBTopSodeR = 142;
        public const int HasShoes = 146;
        public const int ShoeLength = 147;
        public const int ShoePantComparison = 151;
        public const int HasBelt = 175;
        public const int HasHelm = 198;
        public const int HelmLength = 199;
        public const int Helm200 = 200;
        public const int HelmTopComparison = 203;
        public const int HelmEnableShpEri = 209;
        public const int HelmEnableONek = 236;
        public const int HelmEnableOEal1 = 237;
        public const int HelmEnableOEar1 = 238;
        public const int HelmEnableOEal2 = 239;
        public const int HelmEnableOEar2 = 240;
        public const int HelmEnableOEal3 = 241;
        public const int HelmEnableOEar3 = 242;
        public const int HelmEnableAtrMim = 260;
        public const int HelmEnableAtrTop = 261;
        public const int HelmEnableBHirKeUf = 273;
        public const int HelmEnableBHirKeUl = 274;
        public const int HelmEnableBHirKeUr = 275;
        public const int HelmEnableBHirKeUb = 276;
        public const int HelmEnableBHirKeSi = 277;
        public const int HelmEnableBHirKeCf = 278;
        public const int HelmEnableBHirKeCl = 279;
        public const int HelmEnableBHirKeCr = 280;
        public const int HelmEnableBHirKeCb = 281;
        public const int HelmEnableBHirKeOf = 282;
        public const int HelmEnableBHirKeOl = 283;
        public const int HelmEnableBHirKeOr = 284;
        public const int HelmEnableBHirKeOb = 285;
        public const int HelmEnableBHirKeUp = 286;

        public static readonly EquipParam Null = new EquipParam();

        public const int NumValues = 296;
        public EquipParam() { this.Values = new int[NumValues]; }

        public int ID { get; set; }
        public int[] Values { get; set; }

        public static EquipParam FromDataReader(System.Data.Common.DbDataReader reader)
        {
            EquipParam result = new EquipParam();
            result.ID = (int)reader["ID"];
            for (var i = 0; i < NumValues; i++)
            {
                result.Values[i] = (int)reader["C" + i];
            }

            return result;
        }
    }

    public class EquipParamSection : CibSection
    {
        public short NumEntries { get; private set; }
        public short EntryLength { get; private set; }
        public short ValuesPerEntry { get; private set; }

        public List<EquipParam> EquipParams { get; private set; }
        public byte[] ValueLengths { get; private set; }

        public override void LoadSection(BinaryReaderEx reader)
        {
            base.LoadSection(reader);

            this.NumEntries = reader.ReadInt16(Endianness.BigEndian);
            this.EntryLength = reader.ReadInt16(Endianness.BigEndian);
            this.ValuesPerEntry = reader.ReadInt16(Endianness.BigEndian);

            this.EquipParams = new List<EquipParam>(this.NumEntries);
            this.ValueLengths = new byte[this.ValuesPerEntry];

            for (var i = 0; i < this.ValuesPerEntry; i++)
            {
                this.ValueLengths[i] = reader.ReadByte();
            }

            for (var p = 0; p < this.NumEntries; p++)
            {
                var param = new EquipParam();
                param.ID = p + 1;
                var paramData = reader.ReadBytes(this.EntryLength);
                int offset = 0;
                byte thisByte = paramData[offset];
                int bitCnt = 8;
                for (var v = 0; v < this.ValuesPerEntry; v++)
                {
                    int thisLen = this.ValueLengths[v];
                    int val = 0;
                    do
                    {
                        val <<= 1;
                        if ((thisByte & 0x80) != 0)
                        {
                            val |= 1;
                        }
                        thisByte <<= 1;
                        bitCnt--;
                        if (bitCnt == 0)
                        {
                            offset++;
                            thisByte = paramData[offset];
                            bitCnt = 8;
                        }
                        thisLen--;
                    } while (thisLen != 0);
                    param.Values[v] = val;
                }
                this.EquipParams.Add(param);
            }
        }
    }
}
