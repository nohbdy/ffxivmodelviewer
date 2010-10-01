using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace ModelViewer.Cache
{
    public class Actor
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Name_fr { get; set; }
        public string Name_de { get; set; }
        public string Name_ja { get; set; }
        public int BaseModel { get; set; }
        public int HelmModel { get; set; }
        public int HelmVarEquipId { get; set; }
        public int TopModel { get; set; }
        public int TopVarEquipId { get; set; }
        public int BottomModel { get; set; }
        public int BottomVarEquipId { get; set; }
        public int GloveModel { get; set; }
        public int GloveVarEquipId { get; set; }
        public int ShoeModel { get; set; }
        public int ShoeVarEquipId { get; set; }
        public int BeltModel { get; set; }
        public int Weapon1 { get; set; }
        public int Weapon2 { get; set; }
        public int Weapon3 { get; set; }
        public int Weapon4 { get; set; }
        public int Weapon5 { get; set; }
        public int Hair { get; set; }
        public int HairVariation { get; set; }
        public int HairIdx { get; set; }
        public int HairVarEquipId { get; set; }
        public int Face { get; set; }
        public int FaceVariation { get; set; }
        public int FaceIdx { get; set; }
        public int FaceVarEquipId { get; set; }
        public int C7 { get; set; }
        public int C14 { get; set; }
        public int C15 { get; set; }
        public int C16 { get; set; }
        public int C17 { get; set; }
        public int C18 { get; set; }
        public int C19 { get; set; }
        public int C20 { get; set; }
        public int C21 { get; set; }
        public int C22 { get; set; }
        public int C23 { get; set; }
        public int C24 { get; set; }
        public int C28 { get; set; }
        public int C31 { get; set; }
        public int C38 { get; set; }

        public DatDigger.Xml.Format.ModelType ModelType { get { return (DatDigger.Xml.Format.ModelType)(this.BaseModel / 10000); } }
        public int BaseModelFolder { get { return this.BaseModel % 10000; } }

        private int CalculateArmorVarEquipId(ModelPart modelPart, int actorVal)
        {
            if (actorVal == 0) { return 0; }

            int subModelId = actorVal >> 10;
            int texture = (actorVal >> 5) & 0x1F;
            int variation = actorVal & 0x1F;

            int varEquipId = 0;
            var modelType = (DatDigger.Xml.Format.ModelType)(this.BaseModel / 10000);
            switch (modelType)
            {
                case DatDigger.Xml.Format.ModelType.Monster:
                    varEquipId += 1000000000;
                    varEquipId += this.BaseModelFolder * 1000000;
                    varEquipId += subModelId * 1000;
                    varEquipId += texture * 10;
                    varEquipId += variation;
                    switch (modelPart)
                    {
                        case ModelPart.Top:
                            varEquipId += 0;
                            break;
                        case ModelPart.Dwn:
                            varEquipId += 200;
                            break;
                        case ModelPart.Glv:
                            varEquipId += 300;
                            break;
                        case ModelPart.Sho:
                            varEquipId += 400;
                            break;
                        case ModelPart.Met:
                            varEquipId += 600;
                            break;
                        default:
                            throw new System.InvalidOperationException("Unexpected Model Part " + modelPart);
                    }
                    break;
                case DatDigger.Xml.Format.ModelType.PC:
                    varEquipId = 100000000;
                    varEquipId += subModelId * 100000;
                    varEquipId += texture * 100;
                    varEquipId += variation;
                    switch (modelPart)
                    {
                        case ModelPart.Top:
                            varEquipId += 10000;
                            break;
                        case ModelPart.Dwn:
                            varEquipId += 20000;
                            break;
                        case ModelPart.Glv:
                            varEquipId += 30000;
                            break;
                        case ModelPart.Sho:
                            varEquipId += 40000;
                            break;
                        case ModelPart.Met:
                            varEquipId += 60000;
                            break;
                        default:
                            throw new System.InvalidOperationException("Unexpected Model Part " + modelPart);
                    }
                    break;
                case DatDigger.Xml.Format.ModelType.BgObject:
                case DatDigger.Xml.Format.ModelType.Weapon:
                    varEquipId = 0;
                    break;
                default:
                    throw new InvalidOperationException("Unknown Model Type: " + modelType);
            }

            return varEquipId;
        }

        public void CalculateVarEquipIds()
        {
            // Face and Hair - Only for PCs
            if (BaseModel / 10000 == 0)
            {
                this.FaceVarEquipId = 0;
                this.FaceVarEquipId += 200000000;
                this.FaceVarEquipId += this.BaseModelFolder * 1000000;
                this.FaceVarEquipId += this.Face * 1000;
                this.FaceVarEquipId += this.FaceVariation * 100;
                this.FaceVarEquipId += this.FaceIdx;

                this.HairVarEquipId = 0;
                this.HairVarEquipId += 300000000;
                this.HairVarEquipId += this.BaseModelFolder * 1000000;
                this.HairVarEquipId += this.Hair * 1000;
                this.HairVarEquipId += this.HairVariation * 100;
                this.HairVarEquipId += this.HairIdx;
            }

            // Armor parts
            this.HelmVarEquipId = CalculateArmorVarEquipId(ModelPart.Met, this.HelmModel);
            this.TopVarEquipId = CalculateArmorVarEquipId(ModelPart.Top, this.TopModel);
            this.BottomVarEquipId = CalculateArmorVarEquipId(ModelPart.Dwn, this.BottomModel);
            this.GloveVarEquipId = CalculateArmorVarEquipId(ModelPart.Glv, this.GloveModel);
            this.ShoeVarEquipId = CalculateArmorVarEquipId(ModelPart.Sho, this.ShoeModel);
        }

        public string Race
        {
            get
            {
                int type = BaseModel / 10000;
                switch (type)
                {
                    case 0:
                        int race = BaseModel % 10000;
                        switch (race)
                        {
                            case 1: return "Hume Male";
                            case 2: return "Hume Female";
                            case 3: return "Elezen Male";
                            case 4: return "Elezen Female";
                            case 5: return "Lalafel Male";
                            case 6: return "Lalafel Female";
                            case 7: return "Roegadyn";
                            case 8: return "Mi'qote";
                            case 9: return "Hume Male";
                            default: return String.Format("Unknown [c{0:D3}]", race);
                        }
                    case 1: return "Monster";
                    case 2: return "Background Object";
                    case 3: return "Weapon";
                    default: return "Unknown";
                }
            }
        }

        public static Actor FromDataRow(DataRow row)
        {
            if (row == null) { throw new ArgumentNullException("row"); }
            Actor actor = new Actor()
            {
                ID = (int)row["ID"],
                BaseModel = (int)row["6"],
                Hair = (int)row["8"],
                HairIdx = (int)row["9"],
                HairVariation = (int)row["10"],
                Face = (int)row["11"],
                FaceIdx = (int)row["12"],
                FaceVariation = (int)row["13"],
                C7 = (int)row["7"],
                C14 = (int)row["14"],
                C15 = (int)row["15"],
                C16 = (int)row["16"],
                C17 = (int)row["17"],
                C18 = (int)row["18"],
                C19 = (int)row["19"],
                C20 = (int)row["20"],
                C21 = (int)row["21"],
                C22 = (int)row["22"],
                C23 = (int)row["23"],
                C24 = (int)row["24"],
                Weapon1 = (int)row["25"],
                Weapon2 = (int)row["26"],
                Weapon3 = (int)row["27"],
                C28 = (int)row["28"],
                Weapon4 = (int)row["29"],
                Weapon5 = (int)row["30"],
                C31 = (int)row["31"],
                HelmModel = (int)row["32"],
                TopModel = (int)row["33"],
                BottomModel = (int)row["34"],
                GloveModel = (int)row["35"],
                ShoeModel = (int)row["36"],
                BeltModel = (int)row["37"],
                C38 = (int)row["38"]
            };

            actor.CalculateVarEquipIds();
            return actor;
        }

        public static Actor FromDataReader(SQLiteDataReader reader)
        {
            if (reader == null) { throw new ArgumentNullException("reader"); }
            Actor actor = new Actor()
            {
                ID = (int)reader["ActorclassID"],
                Name = (string)reader["Name"],
                Name_fr = (string)reader["Name_fr"],
                Name_de = (string)reader["Name_de"],
                Name_ja = (string)reader["Name_ja"],
                BaseModel = (int)reader["BaseModel"],
                Hair = (int)reader["Hair"],
                HairIdx = (int)reader["HairIdx"],
                HairVariation = (int)reader["HairVariation"],
                HairVarEquipId = (int)reader["HairVarEquipId"],
                Face = (int)reader["Face"],
                FaceIdx = (int)reader["FaceIdx"],
                FaceVariation = (int)reader["FaceVariation"],
                FaceVarEquipId = (int)reader["FaceVarEquipId"],
                C7 = (int)reader["C7"],
                C14 = (int)reader["C14"],
                C15 = (int)reader["C15"],
                C16 = (int)reader["C16"],
                C17 = (int)reader["C17"],
                C18 = (int)reader["C18"],
                C19 = (int)reader["C19"],
                C20 = (int)reader["C20"],
                C21 = (int)reader["C21"],
                C22 = (int)reader["C22"],
                C23 = (int)reader["C23"],
                C24 = (int)reader["C24"],
                Weapon1 = (int)reader["Weapon1"],
                Weapon2 = (int)reader["Weapon2"],
                Weapon3 = (int)reader["Weapon3"],
                C28 = (int)reader["C28"],
                Weapon4 = (int)reader["Weapon4"],
                Weapon5 = (int)reader["Weapon5"],
                C31 = (int)reader["C31"],
                HelmModel = (int)reader["HelmModel"],
                HelmVarEquipId = (int)reader["HelmVarEquipId"],
                TopModel = (int)reader["TopModel"],
                TopVarEquipId = (int)reader["TopVarEquipId"],
                BottomModel = (int)reader["BottomModel"],
                BottomVarEquipId = (int)reader["BottomVarEquipId"],
                GloveModel = (int)reader["GloveModel"],
                GloveVarEquipId = (int)reader["GloveVarEquipId"],
                ShoeModel = (int)reader["ShoeModel"],
                ShoeVarEquipId = (int)reader["ShoeVarEquipId"],
                BeltModel = (int)reader["BeltModel"],
                C38 = (int)reader["C38"]
            };

            return actor;
        }
    }

    public static class ActorCache
    {
        private static SQLiteCommand insert;
        private const int actorClassFile = 16973832;
        private const int actorClassGraphicFile = 16973890;
        private const int xtxDisplayNameFile = 189071360;

        public static void Init()
        {
            insert = new SQLiteCommand();
            insert.CommandText = insertCommand;
            insert.Parameters.AddRange(insertParameters);
        }

        public static void Shutdown()
        {
            if (insert != null) { insert.Dispose(); }
        }

        public static void FillCache()
        {
            string currDir = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(Properties.Settings.Default.GameDirectory);
            
            var actorclass_graphic = DatDigger.FileLoaders.SheetDataLoader.LoadSingleSheet(actorClassGraphicFile);
            var actorclass = DatDigger.FileLoaders.SheetDataLoader.LoadSingleSheet(actorClassFile);
            var displayNameEn = DatDigger.FileLoaders.SheetDataLoader.LoadLanguageSheet(xtxDisplayNameFile, DatDigger.Language.English);

            Insert(actorclass_graphic, actorclass, displayNameEn);

            Directory.SetCurrentDirectory(currDir);
        }

        public static ReadOnlyCollection<Actor> SearchByName(string search)
        {
            search = "%" + search + "%";

            List<Actor> result = new List<Actor>();
            using (var conn = CacheManager.CreateConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM Actors WHERE Name LIKE @search ORDER BY Name ASC LIMIT 200";
                    cmd.Parameters.AddWithValue("@search", search);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        while (reader.Read())
                        {
                            result.Add(Actor.FromDataReader(reader));
                        }
                    }
                }
            }
            return result.AsReadOnly();
        }

        private static void InsertRange(List<Actor> rows)
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

        private static void Insert(DataTable actorclass_graphic, DataTable actorclass, DataTable displayEn)
        {
            using (var conn = CacheManager.CreateConnection())
            {
                insert.Connection = conn;
                conn.Open();
                using (var t = conn.BeginTransaction())
                {
                    for (var i = 0; i < actorclass_graphic.Rows.Count; i++)
                    {
                        var row = actorclass_graphic.Rows[i];
                        var data = Actor.FromDataRow(row);
                        var actorClassRow = actorclass.Select("ID = " + data.ID.ToString());
                        if (actorClassRow.Length == 0) { continue; }
                        var displayEnRow = displayEn.Select("ID = " + actorClassRow[0]["5"]);
                        if (displayEnRow.Length == 0) { continue; }
                        data.Name = (string)displayEnRow[0]["1"];
                        InsertRow(data);
                    }
                    t.Commit();
                }
                insert.Connection = null;
            }
        }

        private static void InsertRow(Actor row)
        {
            insert.Parameters["@ActorclassId"].Value = row.ID;
            insert.Parameters["@Name"].Value = row.Name;
            insert.Parameters["@BaseModel"].Value = row.BaseModel;
            insert.Parameters["@HelmModel"].Value = row.HelmModel;
            insert.Parameters["@HelmVarEquipId"].Value = row.HelmVarEquipId;
            insert.Parameters["@TopModel"].Value = row.TopModel;
            insert.Parameters["@TopVarEquipId"].Value = row.TopVarEquipId;
            insert.Parameters["@BottomModel"].Value = row.BottomModel;
            insert.Parameters["@BottomVarEquipId"].Value = row.BottomVarEquipId;
            insert.Parameters["@GloveModel"].Value = row.GloveModel;
            insert.Parameters["@GloveVarEquipId"].Value = row.GloveVarEquipId;
            insert.Parameters["@ShoeModel"].Value = row.ShoeModel;
            insert.Parameters["@ShoeVarEquipId"].Value = row.ShoeVarEquipId;
            insert.Parameters["@BeltModel"].Value = row.BeltModel;
            insert.Parameters["@Weapon1"].Value = row.Weapon1;
            insert.Parameters["@Weapon2"].Value = row.Weapon2;
            insert.Parameters["@Weapon3"].Value = row.Weapon3;
            insert.Parameters["@Weapon4"].Value = row.Weapon4;
            insert.Parameters["@Weapon5"].Value = row.Weapon5;
            insert.Parameters["@Hair"].Value = row.Hair;
            insert.Parameters["@HairIdx"].Value = row.HairIdx;
            insert.Parameters["@HairVariation"].Value = row.HairVariation;
            insert.Parameters["@HairVarEquipId"].Value = row.HairVarEquipId;
            insert.Parameters["@Face"].Value = row.Face;
            insert.Parameters["@FaceIdx"].Value = row.FaceIdx;
            insert.Parameters["@FaceVariation"].Value = row.FaceVariation;
            insert.Parameters["@FaceVarEquipId"].Value = row.FaceVarEquipId;
            insert.Parameters["@C7"].Value = row.C7;
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
            insert.Parameters["@C28"].Value = row.C28;
            insert.Parameters["@C31"].Value = row.C31;
            insert.Parameters["@C38"].Value = row.C38;

            insert.ExecuteNonQuery();
        }

        #region SQL Setup Stuff
        private const string insertCommand = @"INSERT INTO Actors ([ActorclassId], [Name], [Name_fr], [Name_de], [Name_ja],
                                                    [BaseModel], [HelmModel], [TopModel], [BottomModel], [GloveModel],
                                                    [HelmVarEquipId], [TopVarEquipId], [BottomVarEquipId], [GloveVarEquipId], [ShoeVarEquipId],
                                                    [ShoeModel], [BeltModel], [Weapon1], [Weapon2], [Weapon3], [Weapon4],
                                                    [Weapon5], [Face], [FaceIdx], [FaceVariation], [Hair], [HairIdx], [HairVariation],
                                                    [FaceVarEquipId], [HairVarEquipId],
                                                    [C7],[C14],[C15],[C16],[C17],[C18],[C19],[C20],[C21],[C22],[C23],[C24],[C28],[C31],[C38])
                                                VALUES (@ActorclassId, @Name, '', '', '', @BaseModel,
                                                    @HelmModel, @TopModel, @BottomModel, @GloveModel,
                                                    @HelmVarEquipId, @TopVarEquipId, @BottomVarEquipId, @GloveVarEquipId, @ShoeVarEquipId,
                                                    @ShoeModel, @BeltModel, @Weapon1, @Weapon2, @Weapon3, @Weapon4,
                                                    @Weapon5, @Face, @FaceIdx, @FaceVariation, @Hair, @HairIdx, @HairVariation,
                                                    @FaceVarEquipId, @HairVarEquipId,
                                                    @C7,@C14,@C15,@C16,@C17,@C18,@C19,@C20,@C21,@C22,@C23,@C24,@C28,@C31,@C38)";
        private static readonly SQLiteParameter[] insertParameters = {
                                                    new SQLiteParameter("@ActorclassId", System.Data.DbType.Int32),
                                                    new SQLiteParameter("@Name", System.Data.DbType.String),
                                                    new SQLiteParameter("@BaseModel", System.Data.DbType.Int32),
                                                    new SQLiteParameter("@HelmModel", System.Data.DbType.Int32),
                                                    new SQLiteParameter("@HelmVarEquipId", System.Data.DbType.Int32),
                                                    new SQLiteParameter("@TopModel", System.Data.DbType.Int32),
                                                    new SQLiteParameter("@TopVarEquipId", System.Data.DbType.Int32),
                                                    new SQLiteParameter("@BottomModel", System.Data.DbType.Int32),
                                                    new SQLiteParameter("@BottomVarEquipId", System.Data.DbType.Int32),
                                                    new SQLiteParameter("@GloveModel", System.Data.DbType.Int32),
                                                    new SQLiteParameter("@GloveVarEquipId", System.Data.DbType.Int32),
                                                    new SQLiteParameter("@ShoeModel", System.Data.DbType.Int32),
                                                    new SQLiteParameter("@ShoeVarEquipId", System.Data.DbType.Int32),
                                                    new SQLiteParameter("@BeltModel", System.Data.DbType.Int32),
                                                    new SQLiteParameter("@Weapon1", System.Data.DbType.Int32),
                                                    new SQLiteParameter("@Weapon2", System.Data.DbType.Int32),
                                                    new SQLiteParameter("@Weapon3", System.Data.DbType.Int32),
                                                    new SQLiteParameter("@Weapon4", System.Data.DbType.Int32),
                                                    new SQLiteParameter("@Weapon5", System.Data.DbType.Int32),
                                                    new SQLiteParameter("@Face", System.Data.DbType.Int32),
                                                    new SQLiteParameter("@FaceIdx", System.Data.DbType.Int32),
                                                    new SQLiteParameter("@FaceVariation", System.Data.DbType.Int32),
                                                    new SQLiteParameter("@FaceVarEquipId", System.Data.DbType.Int32),
                                                    new SQLiteParameter("@Hair", System.Data.DbType.Int32),
                                                    new SQLiteParameter("@HairIdx", System.Data.DbType.Int32),
                                                    new SQLiteParameter("@HairVariation", System.Data.DbType.Int32),
                                                    new SQLiteParameter("@HairVarEquipId", System.Data.DbType.Int32),
                                                    new SQLiteParameter("@C7", System.Data.DbType.Int32),
                                                    new SQLiteParameter("@C14", System.Data.DbType.Int32),
                                                    new SQLiteParameter("@C15", System.Data.DbType.Int32),
                                                    new SQLiteParameter("@C16", System.Data.DbType.Int32),
                                                    new SQLiteParameter("@C17", System.Data.DbType.Int32),
                                                    new SQLiteParameter("@C18", System.Data.DbType.Int32),
                                                    new SQLiteParameter("@C19", System.Data.DbType.Int32),
                                                    new SQLiteParameter("@C20", System.Data.DbType.Int32),
                                                    new SQLiteParameter("@C21", System.Data.DbType.Int32),
                                                    new SQLiteParameter("@C22", System.Data.DbType.Int32),
                                                    new SQLiteParameter("@C23", System.Data.DbType.Int32),
                                                    new SQLiteParameter("@C24", System.Data.DbType.Int32),
                                                    new SQLiteParameter("@C28", System.Data.DbType.Int32),
                                                    new SQLiteParameter("@C31", System.Data.DbType.Int32),
                                                    new SQLiteParameter("@C38", System.Data.DbType.Int32)};
        #endregion
    }
}
