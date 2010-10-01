using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace ModelViewer.Cache
{
    public class VarEquip
    {
        public int ID { get; set; }
        public int Texture { get; set; }
        public int TexPathID { get; set; }
        public int C110 { get; set; }
        public bool PgrpA { get; set; }
        public bool PgrpB { get; set; }
        public bool PgrpC { get; set; }
        public bool PgrpD { get; set; }
        public bool PgrpE { get; set; }
        public bool PgrpF { get; set; }
        public bool PgrpG { get; set; }
        public bool PgrpH { get; set; }
        public bool PgrpI { get; set; }
        public bool PgrpJ { get; set; }
        public bool PgrpK { get; set; }
        public bool PgrpL { get; set; }
        public bool PgrpM { get; set; }
        public bool PgrpN { get; set; }
        public bool PgrpO { get; set; }
        public bool PgrpP { get; set; }
        public float C0 { get; set; }
        public float C1 { get; set; }
        public float C2 { get; set; }
        public float C3 { get; set; }
        public float C4 { get; set; }
        public float C5 { get; set; }
        public float C6 { get; set; }
        public float C7 { get; set; }
        public float C8 { get; set; }
        public float C9 { get; set; }
        public float C10 { get; set; }
        public float C11 { get; set; }
        public float C12 { get; set; }
        public float C13 { get; set; }
        public float C14 { get; set; }
        public float C15 { get; set; }
        public float C16 { get; set; }
        public float C17 { get; set; }
        public float C18 { get; set; }
        public float C19 { get; set; }
        public float C20 { get; set; }
        public float C21 { get; set; }
        public float C22 { get; set; }
        public float C23 { get; set; }
        public float C24 { get; set; }
        public float C25 { get; set; }
        public float C26 { get; set; }
        public float C27 { get; set; }
        public float C28 { get; set; }
        public float C29 { get; set; }
        public float C30 { get; set; }
        public float C31 { get; set; }
        public float C32 { get; set; }
        public float C33 { get; set; }
        public float C34 { get; set; }
        public float C35 { get; set; }
        public float C36 { get; set; }
        public float C37 { get; set; }
        public float C38 { get; set; }
        public float C39 { get; set; }
        public float C40 { get; set; }
        public float C41 { get; set; }
        public float C42 { get; set; }
        public float C43 { get; set; }
        public float C44 { get; set; }
        public float C45 { get; set; }
        public float C46 { get; set; }
        public float C47 { get; set; }
        public float C48 { get; set; }
        public float C49 { get; set; }
        public float C50 { get; set; }
        public float C51 { get; set; }
        public float C52 { get; set; }
        public float C53 { get; set; }
        public float C54 { get; set; }
        public float C55 { get; set; }
        public float C56 { get; set; }
        public float C57 { get; set; }
        public float C58 { get; set; }
        public float C59 { get; set; }
        public float C60 { get; set; }
        public float C61 { get; set; }
        public float C62 { get; set; }
        public float C63 { get; set; }
        public float C64 { get; set; }
        public float C65 { get; set; }
        public float C66 { get; set; }
        public float C67 { get; set; }
        public float C68 { get; set; }
        public float C69 { get; set; }
        public float C70 { get; set; }
        public float C71 { get; set; }
        public float C72 { get; set; }
        public float C73 { get; set; }
        public float C74 { get; set; }
        public float C75 { get; set; }

        public float GetMaterialValue(int offset, int meshIndex, float originalValue)
        {
            int realOffset = meshIndex * 19 + offset;
            float matValue = 1000f;
            switch (realOffset)
            {
                case 0: matValue = C0; break;
                case 1: matValue = C1; break;
                case 2: matValue = C2; break;
                case 3: matValue = C3; break;
                case 4: matValue = C4; break;
                case 5: matValue = C5; break;
                case 6: matValue = C6; break;
                case 7: matValue = C7; break;
                case 8: matValue = C8; break;
                case 9: matValue = C9; break;
                case 10: matValue = C10; break;
                case 11: matValue = C11; break;
                case 12: matValue = C12; break;
                case 13: matValue = C13; break;
                case 14: matValue = C14; break;
                case 15: matValue = C15; break;
                case 16: matValue = C16; break;
                case 17: matValue = C17; break;
                case 18: matValue = C18; break;
                case 19: matValue = C19; break;
                case 20: matValue = C20; break;
                case 21: matValue = C21; break;
                case 22: matValue = C22; break;
                case 23: matValue = C23; break;
                case 24: matValue = C24; break;
                case 25: matValue = C25; break;
                case 26: matValue = C26; break;
                case 27: matValue = C27; break;
                case 28: matValue = C28; break;
                case 29: matValue = C29; break;
                case 30: matValue = C30; break;
                case 31: matValue = C31; break;
                case 32: matValue = C32; break;
                case 33: matValue = C33; break;
                case 34: matValue = C34; break;
                case 35: matValue = C35; break;
                case 36: matValue = C36; break;
                case 37: matValue = C37; break;
                case 38: matValue = C38; break;
                case 39: matValue = C39; break;
                case 40: matValue = C40; break;
                case 41: matValue = C41; break;
                case 42: matValue = C42; break;
                case 43: matValue = C43; break;
                case 44: matValue = C44; break;
                case 45: matValue = C45; break;
                case 46: matValue = C46; break;
                case 47: matValue = C47; break;
                case 48: matValue = C48; break;
                case 49: matValue = C49; break;
                case 50: matValue = C50; break;
                case 51: matValue = C51; break;
                case 52: matValue = C52; break;
                case 53: matValue = C53; break;
                case 54: matValue = C54; break;
                case 55: matValue = C55; break;
                case 56: matValue = C56; break;
                case 57: matValue = C57; break;
                case 58: matValue = C58; break;
                case 59: matValue = C59; break;
                case 60: matValue = C60; break;
                case 61: matValue = C61; break;
                case 62: matValue = C62; break;
                case 63: matValue = C63; break;
                case 64: matValue = C64; break;
                case 65: matValue = C65; break;
                case 66: matValue = C66; break;
                case 67: matValue = C67; break;
                case 68: matValue = C68; break;
                case 69: matValue = C69; break;
                case 70: matValue = C70; break;
                case 71: matValue = C71; break;
                case 72: matValue = C72; break;
                case 73: matValue = C73; break;
                case 74: matValue = C74; break;
                case 75: matValue = C75; break;
            }

            return (matValue == 1000f) ? originalValue : matValue;
        }

        public static VarEquip FromDataRow(System.Data.DataRow row)
        {
            if (row == null) { throw new ArgumentNullException("row"); }

            VarEquip result = new VarEquip()
            {
                ID = (int)row["ID"],
                C0 = (float)row["0"],
                C1 = (float)row["1"],
                C2 = (float)row["2"],
                C3 = (float)row["3"],
                C4 = (float)row["4"],
                C5 = (float)row["5"],
                C6 = (float)row["6"],
                C7 = (float)row["7"],
                C8 = (float)row["8"],
                C9 = (float)row["9"],
                C10 = (float)row["10"],
                C11 = (float)row["11"],
                C12 = (float)row["12"],
                C13 = (float)row["13"],
                C14 = (float)row["14"],
                C15 = (float)row["15"],
                C16 = (float)row["16"],
                C17 = (float)row["17"],
                C18 = (float)row["18"],
                C19 = (float)row["19"],
                C20 = (float)row["20"],
                C21 = (float)row["21"],
                C22 = (float)row["22"],
                C23 = (float)row["23"],
                C24 = (float)row["24"],
                C25 = (float)row["25"],
                C26 = (float)row["26"],
                C27 = (float)row["27"],
                C28 = (float)row["28"],
                C29 = (float)row["29"],
                C30 = (float)row["30"],
                C31 = (float)row["31"],
                C32 = (float)row["32"],
                C33 = (float)row["33"],
                C34 = (float)row["34"],
                C35 = (float)row["35"],
                C36 = (float)row["36"],
                C37 = (float)row["37"],
                C38 = (float)row["38"],
                C39 = (float)row["39"],
                C40 = (float)row["40"],
                C41 = (float)row["41"],
                C42 = (float)row["42"],
                C43 = (float)row["43"],
                C44 = (float)row["44"],
                C45 = (float)row["45"],
                C46 = (float)row["46"],
                C47 = (float)row["47"],
                C48 = (float)row["48"],
                C49 = (float)row["49"],
                C50 = (float)row["50"],
                C51 = (float)row["51"],
                C52 = (float)row["52"],
                C53 = (float)row["53"],
                C54 = (float)row["54"],
                C55 = (float)row["55"],
                C56 = (float)row["56"],
                C57 = (float)row["57"],
                C58 = (float)row["58"],
                C59 = (float)row["59"],
                C60 = (float)row["60"],
                C61 = (float)row["61"],
                C62 = (float)row["62"],
                C63 = (float)row["63"],
                C64 = (float)row["64"],
                C65 = (float)row["65"],
                C66 = (float)row["66"],
                C67 = (float)row["67"],
                C68 = (float)row["68"],
                C69 = (float)row["69"],
                C70 = (float)row["70"],
                C71 = (float)row["71"],
                C72 = (float)row["72"],
                C73 = (float)row["73"],
                C74 = (float)row["74"],
                C75 = (float)row["75"],
                PgrpA = (bool)row["76"],
                PgrpB = (bool)row["77"],
                PgrpC = (bool)row["78"],
                PgrpD = (bool)row["79"],
                PgrpE = (bool)row["80"],
                PgrpF = (bool)row["81"],
                PgrpG = (bool)row["82"],
                PgrpH = (bool)row["83"],
                PgrpI = (bool)row["84"],
                PgrpJ = (bool)row["85"],
                PgrpK = (bool)row["86"],
                PgrpL = (bool)row["87"],
                PgrpM = (bool)row["88"],
                PgrpN = (bool)row["89"],
                PgrpO = (bool)row["90"],
                PgrpP = (bool)row["91"],
                TexPathID = (int)row["108"],
                Texture = (int)row["109"],
                C110 = (int)row["110"]
            };

            return result;
        }

        public static VarEquip FromDataReader(SQLiteDataReader reader)
        {
            if (reader == null) { throw new ArgumentNullException("reader"); }

            VarEquip result = new VarEquip();
            result.ID = (int)reader["ID"];
            result.C0 = (float)reader["C0"];
            result.C1 = (float)reader["C1"];
            result.C2 = (float)reader["C2"];
            result.C3 = (float)reader["C3"];
            result.C4 = (float)reader["C4"];
            result.C5 = (float)reader["C5"];
            result.C6 = (float)reader["C6"];
            result.C7 = (float)reader["C7"];
            result.C8 = (float)reader["C8"];
            result.C9 = (float)reader["C9"];
            result.C10 = (float)reader["C10"];
            result.C11 = (float)reader["C11"];
            result.C12 = (float)reader["C12"];
            result.C13 = (float)reader["C13"];
            result.C14 = (float)reader["C14"];
            result.C15 = (float)reader["C15"];
            result.C16 = (float)reader["C16"];
            result.C17 = (float)reader["C17"];
            result.C18 = (float)reader["C18"];
            result.C19 = (float)reader["C19"];
            result.C20 = (float)reader["C20"];
            result.C21 = (float)reader["C21"];
            result.C22 = (float)reader["C22"];
            result.C23 = (float)reader["C23"];
            result.C24 = (float)reader["C24"];
            result.C25 = (float)reader["C25"];
            result.C26 = (float)reader["C26"];
            result.C27 = (float)reader["C27"];
            result.C28 = (float)reader["C28"];
            result.C29 = (float)reader["C29"];
            result.C30 = (float)reader["C30"];
            result.C31 = (float)reader["C31"];
            result.C32 = (float)reader["C32"];
            result.C33 = (float)reader["C33"];
            result.C34 = (float)reader["C34"];
            result.C35 = (float)reader["C35"];
            result.C36 = (float)reader["C36"];
            result.C37 = (float)reader["C37"];
            result.C38 = (float)reader["C38"];
            result.C39 = (float)reader["C39"];
            result.C40 = (float)reader["C40"];
            result.C41 = (float)reader["C41"];
            result.C42 = (float)reader["C42"];
            result.C43 = (float)reader["C43"];
            result.C44 = (float)reader["C44"];
            result.C45 = (float)reader["C45"];
            result.C46 = (float)reader["C46"];
            result.C47 = (float)reader["C47"];
            result.C48 = (float)reader["C48"];
            result.C49 = (float)reader["C49"];
            result.C50 = (float)reader["C50"];
            result.C51 = (float)reader["C51"];
            result.C52 = (float)reader["C52"];
            result.C53 = (float)reader["C53"];
            result.C54 = (float)reader["C54"];
            result.C55 = (float)reader["C55"];
            result.C56 = (float)reader["C56"];
            result.C57 = (float)reader["C57"];
            result.C58 = (float)reader["C58"];
            result.C59 = (float)reader["C59"];
            result.C60 = (float)reader["C60"];
            result.C61 = (float)reader["C61"];
            result.C62 = (float)reader["C62"];
            result.C63 = (float)reader["C63"];
            result.C64 = (float)reader["C64"];
            result.C65 = (float)reader["C65"];
            result.C66 = (float)reader["C66"];
            result.C67 = (float)reader["C67"];
            result.C68 = (float)reader["C68"];
            result.C69 = (float)reader["C69"];
            result.C70 = (float)reader["C70"];
            result.C71 = (float)reader["C71"];
            result.C72 = (float)reader["C72"];
            result.C73 = (float)reader["C73"];
            result.C74 = (float)reader["C74"];
            result.C75 = (float)reader["C75"];
            result.PgrpA = (bool)reader["PgrpA"];
            result.PgrpB = (bool)reader["PgrpB"];
            result.PgrpC = (bool)reader["PgrpC"];
            result.PgrpD = (bool)reader["PgrpD"];
            result.PgrpE = (bool)reader["PgrpE"];
            result.PgrpF = (bool)reader["PgrpF"];
            result.PgrpG = (bool)reader["PgrpG"];
            result.PgrpH = (bool)reader["PgrpH"];
            result.PgrpI = (bool)reader["PgrpI"];
            result.PgrpJ = (bool)reader["PgrpJ"];
            result.PgrpK = (bool)reader["PgrpK"];
            result.PgrpL = (bool)reader["PgrpL"];
            result.PgrpM = (bool)reader["PgrpM"];
            result.PgrpN = (bool)reader["PgrpN"];
            result.PgrpO = (bool)reader["PgrpO"];
            result.PgrpP = (bool)reader["PgrpP"];
            result.TexPathID = (int)reader["TexPathID"];
            result.Texture = (int)reader["Texture"];
            result.C110 = (int)reader["C110"];

            return result;
        }
    }

    public static class VarEquipCache
    {
        private static SQLiteCommand insert;
        private const int varEquipXmlFile = 61276161;

        public static void Init()
        {
            insert = new SQLiteCommand();
            insert.CommandText = insertCommandText;
            insert.Parameters.AddRange(insertParameters);
        }

        public static void Shutdown()
        {
            if (insert != null) { insert.Dispose(); }
        }

        public static VarEquip GetById(int varEquipId)
        {
            VarEquip result = null;
            using (var conn = CacheManager.CreateConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM VarEquip WHERE ID = @ID";
                    cmd.Parameters.AddWithValue("@ID", varEquipId);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (reader.Read())
                        {
                            result = VarEquip.FromDataReader(reader);
                        }
                    }
                }
            }
            return result;
        }

        public static void FillCache()
        {
            string currDir = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(Properties.Settings.Default.GameDirectory);
            
            var dt = DatDigger.FileLoaders.SheetDataLoader.LoadSingleSheet(varEquipXmlFile);
            Insert(dt);

            Directory.SetCurrentDirectory(currDir);
        }

        public static void Insert(VarEquip row)
        {
            using (var conn = CacheManager.CreateConnection())
            {
                insert.Connection = conn;
                conn.Open();
                InsertRow(row);
                insert.Connection = null;
            }
        }

        public static void InsertRange(List<VarEquip> rows)
        {
            using (var conn = CacheManager.CreateConnection())
            {
                insert.Connection = conn;
                conn.Open();
                using (var t = conn.BeginTransaction())
                {
                    rows.ForEach(x => InsertRow(x));
                    t.Commit();
                }
                insert.Connection = null;
            }
        }

        public static void Insert(System.Data.DataTable dt)
        {
            using (var conn = CacheManager.CreateConnection())
            {
                insert.Connection = conn;
                conn.Open();
                using (var t = conn.BeginTransaction())
                {
                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        var row = dt.Rows[i];
                        var data = VarEquip.FromDataRow(row);
                        InsertRow(data);
                    }
                    t.Commit();
                }
                insert.Connection = null;
            }
        }

        private static void InsertRow(VarEquip row)
        {
            insert.Parameters["@ID"].Value = row.ID;
            insert.Parameters["@Texture"].Value = row.Texture;
            insert.Parameters["@TexPathID"].Value = row.TexPathID;
            insert.Parameters["@C110"].Value = row.C110;
            insert.Parameters["@PgrpA"].Value = row.PgrpA;
            insert.Parameters["@PgrpB"].Value = row.PgrpB;
            insert.Parameters["@PgrpC"].Value = row.PgrpC;
            insert.Parameters["@PgrpD"].Value = row.PgrpD;
            insert.Parameters["@PgrpE"].Value = row.PgrpE;
            insert.Parameters["@PgrpF"].Value = row.PgrpF;
            insert.Parameters["@PgrpG"].Value = row.PgrpG;
            insert.Parameters["@PgrpH"].Value = row.PgrpH;
            insert.Parameters["@PgrpI"].Value = row.PgrpI;
            insert.Parameters["@PgrpJ"].Value = row.PgrpJ;
            insert.Parameters["@PgrpK"].Value = row.PgrpK;
            insert.Parameters["@PgrpL"].Value = row.PgrpL;
            insert.Parameters["@PgrpM"].Value = row.PgrpM;
            insert.Parameters["@PgrpN"].Value = row.PgrpN;
            insert.Parameters["@PgrpO"].Value = row.PgrpO;
            insert.Parameters["@PgrpP"].Value = row.PgrpP;
            insert.Parameters["@C0"].Value = row.C0;
            insert.Parameters["@C1"].Value = row.C1;
            insert.Parameters["@C2"].Value = row.C2;
            insert.Parameters["@C3"].Value = row.C3;
            insert.Parameters["@C4"].Value = row.C4;
            insert.Parameters["@C5"].Value = row.C5;
            insert.Parameters["@C6"].Value = row.C6;
            insert.Parameters["@C7"].Value = row.C7;
            insert.Parameters["@C8"].Value = row.C8;
            insert.Parameters["@C9"].Value = row.C9;
            insert.Parameters["@C10"].Value = row.C10;
            insert.Parameters["@C11"].Value = row.C11;
            insert.Parameters["@C12"].Value = row.C12;
            insert.Parameters["@C13"].Value = row.C13;
            insert.Parameters["@C14"].Value = row.C14;
            insert.Parameters["@C15"].Value = row.C15;
            insert.Parameters["@C16"].Value = row.C16;
            insert.Parameters["@C17"].Value = row.C17;
            insert.Parameters["@C18"].Value = row.C18;
            insert.Parameters["@C19"].Value = row.C19;
            insert.Parameters["@C20"].Value = row.C20;
            insert.Parameters["@C21"].Value = row.C21;
            insert.Parameters["@C22"].Value = row.C22;
            insert.Parameters["@C23"].Value = row.C23;
            insert.Parameters["@C24"].Value = row.C24;
            insert.Parameters["@C25"].Value = row.C25;
            insert.Parameters["@C26"].Value = row.C26;
            insert.Parameters["@C27"].Value = row.C27;
            insert.Parameters["@C28"].Value = row.C28;
            insert.Parameters["@C29"].Value = row.C29;
            insert.Parameters["@C30"].Value = row.C20;
            insert.Parameters["@C31"].Value = row.C31;
            insert.Parameters["@C32"].Value = row.C32;
            insert.Parameters["@C33"].Value = row.C33;
            insert.Parameters["@C34"].Value = row.C34;
            insert.Parameters["@C35"].Value = row.C35;
            insert.Parameters["@C36"].Value = row.C36;
            insert.Parameters["@C37"].Value = row.C37;
            insert.Parameters["@C38"].Value = row.C38;
            insert.Parameters["@C39"].Value = row.C39;
            insert.Parameters["@C40"].Value = row.C40;
            insert.Parameters["@C41"].Value = row.C41;
            insert.Parameters["@C42"].Value = row.C42;
            insert.Parameters["@C43"].Value = row.C43;
            insert.Parameters["@C44"].Value = row.C44;
            insert.Parameters["@C45"].Value = row.C45;
            insert.Parameters["@C46"].Value = row.C46;
            insert.Parameters["@C47"].Value = row.C47;
            insert.Parameters["@C48"].Value = row.C48;
            insert.Parameters["@C49"].Value = row.C49;
            insert.Parameters["@C50"].Value = row.C50;
            insert.Parameters["@C51"].Value = row.C51;
            insert.Parameters["@C52"].Value = row.C52;
            insert.Parameters["@C53"].Value = row.C53;
            insert.Parameters["@C54"].Value = row.C54;
            insert.Parameters["@C55"].Value = row.C55;
            insert.Parameters["@C56"].Value = row.C56;
            insert.Parameters["@C57"].Value = row.C57;
            insert.Parameters["@C58"].Value = row.C58;
            insert.Parameters["@C59"].Value = row.C59;
            insert.Parameters["@C60"].Value = row.C60;
            insert.Parameters["@C61"].Value = row.C61;
            insert.Parameters["@C62"].Value = row.C62;
            insert.Parameters["@C63"].Value = row.C63;
            insert.Parameters["@C64"].Value = row.C64;
            insert.Parameters["@C65"].Value = row.C65;
            insert.Parameters["@C66"].Value = row.C66;
            insert.Parameters["@C67"].Value = row.C67;
            insert.Parameters["@C68"].Value = row.C68;
            insert.Parameters["@C69"].Value = row.C69;
            insert.Parameters["@C70"].Value = row.C70;
            insert.Parameters["@C71"].Value = row.C71;
            insert.Parameters["@C72"].Value = row.C72;
            insert.Parameters["@C73"].Value = row.C73;
            insert.Parameters["@C74"].Value = row.C74;
            insert.Parameters["@C75"].Value = row.C75;

            insert.ExecuteNonQuery();
        }

        #region Sql Command Text
        private const string insertCommandText = @"INSERT INTO [VarEquip] ([ID], [PgrpA], [PgrpB], [PgrpC], [PgrpD], [PgrpE], [PgrpF], [PgrpG], [PgrpH],
                                                    [PgrpI], [PgrpJ], [PgrpK], [PgrpL], [PgrpM], [PgrpN], [PgrpO], [PgrpP], [Texture], [TexPathID],
                                                    [C0], [C1], [C2], [C3], [C4], [C5], [C6], [C7], [C8], [C9], [C10], [C11], [C12], [C13], [C14],
                                                    [C15], [C16], [C17], [C18], [C19], [C20], [C21], [C22], [C23], [C24], [C25], [C26], [C27], [C28],
                                                    [C29], [C30], [C31], [C32], [C33], [C34], [C35], [C36], [C37], [C38], [C39], [C40], [C41], [C42],
                                                    [C43], [C44], [C45], [C46], [C47], [C48], [C49], [C50], [C51], [C52], [C53], [C54], [C55], [C56],
                                                    [C57], [C58], [C59], [C60], [C61], [C62], [C63], [C64], [C65], [C66], [C67], [C68], [C69], [C70],
                                                    [C71], [C72], [C73], [C74], [C75], [C110])
                                                VALUES (@ID, @PgrpA, @PgrpB, @PgrpC, @PgrpD, @PgrpE, @PgrpF, @PgrpG, @PgrpH,
                                                    @PgrpI, @PgrpJ, @PgrpK, @PgrpL, @PgrpM, @PgrpN, @PgrpO, @PgrpP, @Texture, @TexPathID,
                                                    @C0, @C1, @C2, @C3, @C4, @C5, @C6, @C7, @C8, @C9, @C10, @C11, @C12, @C13, @C14,
                                                    @C15, @C16, @C17, @C18, @C19, @C20, @C21, @C22, @C23, @C24, @C25, @C26, @C27, @C28,
                                                    @C29, @C30, @C31, @C32, @C33, @C34, @C35, @C36, @C37, @C38, @C39, @C40, @C41, @C42,
                                                    @C43, @C44, @C45, @C46, @C47, @C48, @C49, @C50, @C51, @C52, @C53, @C54, @C55, @C56,
                                                    @C57, @C58, @C59, @C60, @C61, @C62, @C63, @C64, @C65, @C66, @C67, @C68, @C69, @C70,
                                                    @C71, @C72, @C73, @C74, @C75, @C110)";
        private static readonly SQLiteParameter[] insertParameters = {
                                                        new SQLiteParameter("@ID", System.Data.DbType.Int32),
                                                        new SQLiteParameter("@Texture", System.Data.DbType.Int32),
                                                        new SQLiteParameter("@TexPathID", System.Data.DbType.Int32),
                                                        new SQLiteParameter("@C110", System.Data.DbType.Int32),
                                                        new SQLiteParameter("@PgrpA", System.Data.DbType.Boolean),
                                                        new SQLiteParameter("@PgrpB", System.Data.DbType.Boolean),
                                                        new SQLiteParameter("@PgrpC", System.Data.DbType.Boolean),
                                                        new SQLiteParameter("@PgrpD", System.Data.DbType.Boolean),
                                                        new SQLiteParameter("@PgrpE", System.Data.DbType.Boolean),
                                                        new SQLiteParameter("@PgrpF", System.Data.DbType.Boolean),
                                                        new SQLiteParameter("@PgrpG", System.Data.DbType.Boolean),
                                                        new SQLiteParameter("@PgrpH", System.Data.DbType.Boolean),
                                                        new SQLiteParameter("@PgrpI", System.Data.DbType.Boolean),
                                                        new SQLiteParameter("@PgrpJ", System.Data.DbType.Boolean),
                                                        new SQLiteParameter("@PgrpK", System.Data.DbType.Boolean),
                                                        new SQLiteParameter("@PgrpL", System.Data.DbType.Boolean),
                                                        new SQLiteParameter("@PgrpM", System.Data.DbType.Boolean),
                                                        new SQLiteParameter("@PgrpN", System.Data.DbType.Boolean),
                                                        new SQLiteParameter("@PgrpO", System.Data.DbType.Boolean),
                                                        new SQLiteParameter("@PgrpP", System.Data.DbType.Boolean),
                                                        new SQLiteParameter("@C0", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C1", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C2", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C3", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C4", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C5", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C6", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C7", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C8", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C9", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C10", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C11", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C12", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C13", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C14", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C15", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C16", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C17", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C18", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C19", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C20", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C21", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C22", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C23", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C24", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C25", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C26", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C27", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C28", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C29", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C30", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C31", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C32", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C33", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C34", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C35", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C36", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C37", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C38", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C39", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C40", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C41", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C42", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C43", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C44", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C45", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C46", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C47", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C48", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C49", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C50", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C51", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C52", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C53", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C54", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C55", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C56", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C57", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C58", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C59", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C60", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C61", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C62", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C63", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C64", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C65", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C66", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C67", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C68", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C69", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C70", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C71", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C72", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C73", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C74", System.Data.DbType.Single),
                                                        new SQLiteParameter("@C75", System.Data.DbType.Single)
                                                    };
        #endregion
    }
}
