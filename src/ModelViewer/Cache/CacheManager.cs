using System;
using System.Data.SQLite;
using System.IO;

namespace ModelViewer.Cache
{
    public static class CacheManager
    {
        private const string cacheFileName = "cache.sqlite";
        private static string executionDirectory;
        private static string cachePath;

        public static bool CacheBuilt { get; private set; }
        public static string GameVersion { get; private set; }
        public static DateTime DateGenerated { get; private set; }

        public static bool CacheIsStale
        {
            get
            {
                return ((CacheBuilt == false) || (GameVersion != GetCurrentGameVersion()));
            }
        }

        public static void Init()
        {
            executionDirectory = System.Windows.Forms.Application.StartupPath;
            cachePath = Path.Combine(executionDirectory, cacheFileName);
            if (!File.Exists(cachePath))
            {
                CreateCacheDb();
                CacheBuilt = false;
            }
            else
            {
                LoadDbInfo();
            }

            VarEquipCache.Init();
            ActorCache.Init();
            TexturePathCache.Init();
            EquipParamCache.Init();
        }

        public static void Shutdown()
        {
            VarEquipCache.Shutdown();
            ActorCache.Shutdown();
            TexturePathCache.Shutdown();
            EquipParamCache.Shutdown();
        }

        private static string BuildConnectionString(bool failIfMissing)
        {
            SQLiteConnectionStringBuilder csb = new SQLiteConnectionStringBuilder();
            csb.DataSource = cachePath;
            csb.DateTimeFormat = SQLiteDateFormats.ISO8601;
            csb.FailIfMissing = failIfMissing;

            return csb.ConnectionString;
        }

        internal static SQLiteConnection CreateConnection()
        {
            return new SQLiteConnection(BuildConnectionString(true));
        }

        private static void CreateCacheDb()
        {
            using (SQLiteConnection conn = new SQLiteConnection(BuildConnectionString(false)))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = typeof(CacheManager).Assembly.GetResourceAsString("ModelViewer.Cache.create_cache.sql");
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static void LoadDbInfo()
        {
            using (SQLiteConnection conn = new SQLiteConnection(BuildConnectionString(true)))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT [Version], [Date] FROM BuildInfo";
                    using (var reader = cmd.ExecuteReader(System.Data.CommandBehavior.SingleRow))
                    {
                        if (!reader.Read())
                        {
                            CacheBuilt = false;
                        }
                        else
                        {
                            GameVersion = reader.GetString(0);
                            DateGenerated = reader.GetDateTime(1);
                            CacheBuilt = true;
                        }
                    }
                }
            }
        }

        private static void ClearCache()
        {
            CacheBuilt = false;
            if (File.Exists(cachePath))
            {
                File.Delete(cachePath);
            }

            CreateCacheDb();
        }

        private static string GetCurrentGameVersion()
        {
            string gameVersionFile = Path.Combine(Properties.Settings.Default.GameDirectory, "game.ver");
            using (FileStream fs = File.Open(gameVersionFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader reader = new StreamReader(fs))
            {
                return reader.ReadToEnd();
            }
        }

        public static void LoadCache()
        {
            DateTime dateGenerated = DateTime.Now;
            string version = GetCurrentGameVersion();

            using (SQLiteConnection conn = new SQLiteConnection(BuildConnectionString(true)))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO BuildInfo ([Version], [Date]) VALUES (@version, @date)";
                    cmd.Parameters.Add("version", System.Data.DbType.String).Value = version;
                    cmd.Parameters.Add("date", System.Data.DbType.DateTime).Value = dateGenerated;
                    cmd.ExecuteNonQuery();
                }
            }

            GameVersion = version;
            DateGenerated = dateGenerated;

            VarEquipCache.FillCache();
            ActorCache.FillCache();
            TexturePathCache.FillCache();
            EquipParamCache.FillCache();

            CacheBuilt = true;
        }

        public static void ReloadCache()
        {
            ClearCache();
            LoadCache();
        }
    }
}
