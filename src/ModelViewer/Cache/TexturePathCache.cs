using System;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace ModelViewer.Cache
{
    public static class TexturePathCache
    {
        private static SQLiteCommand insert;
        private const int varTexPathFileId = 61276933;

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

        public static int GetTextureFolderId(int texPath, int race)
        {
            if (texPath == 0) { return race; }

            int result = 1;
            using (var conn = CacheManager.CreateConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT TexFolder FROM TexturePath WHERE RaceID = @RaceID AND TexPath = @TexPath";
                    cmd.Parameters.AddWithValue("@RaceID", race);
                    cmd.Parameters.AddWithValue("@TexPath", texPath);
                    result = System.Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            return result;
        }

        public static void FillCache()
        {
            string currDir = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(Properties.Settings.Default.GameDirectory);

            var varTexPath = DatDigger.FileLoaders.SheetDataLoader.LoadSingleSheet(varTexPathFileId);

            Insert(varTexPath);

            Directory.SetCurrentDirectory(currDir);
        }

        private static void Insert(DataTable varTexPath)
        {
            using (var conn = CacheManager.CreateConnection())
            {
                insert.Connection = conn;
                conn.Open();
                using (var t = conn.BeginTransaction())
                {
                    for (var i = 0; i < varTexPath.Rows.Count; i++)
                    {
                        int texPath = (int)varTexPath.Rows[i]["ID"];
                        for (var j = 1; j < varTexPath.Columns.Count; j++)
                        {
                            int raceID = 1 + Int32.Parse(varTexPath.Columns[j].ColumnName); // Add 1 to the column number to get RaceID
                            int texFolder = (int)(short)varTexPath.Rows[i][j];

                            InsertRow(raceID, texPath, texFolder);
                        }
                    }

                    t.Commit();
                }

                insert.Connection = null;
            }
        }

        private static void InsertRow(int raceID, int texPath, int texFolder)
        {
            insert.Parameters["@RaceID"].Value = raceID;
            insert.Parameters["@TexPath"].Value = texPath;
            insert.Parameters["@TexFolder"].Value = texFolder;

            insert.ExecuteNonQuery();
        }

        #region Sql Stuff
        private const string insertCommand = @"INSERT INTO TexturePath (RaceID, TexPath, TexFolder) VALUES (@RaceID, @TexPath, @TexFolder)";
        private static readonly SQLiteParameter[] insertParameters = {
                                                                         new SQLiteParameter("@RaceID", DbType.Int32),
                                                                         new SQLiteParameter("@TexPath", DbType.Int32),
                                                                         new SQLiteParameter("@TexFolder", DbType.Int32)
                                                                     };
        #endregion
    }
}
