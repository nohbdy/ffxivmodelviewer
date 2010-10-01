using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using DatDigger;
using DatDigger.Sections.Model;

namespace ModelViewer.Cache
{
    class EquipParamCache
    {
        private static SQLiteCommand insert;
        private const string cmnFilePath = "client/chara/pc/cmn/0001";

        public static void Init()
        {
            insert = new SQLiteCommand();
            insert.CommandText = insertCommandText;
            insertParameters[0] = new SQLiteParameter("@ID", System.Data.DbType.Int32);
            for (var i = 0; i < EquipParam.NumValues; i++)
            {
                insertParameters[i + 1] = new SQLiteParameter(String.Format("@C{0}", i), System.Data.DbType.Int32);
            }
            insert.Parameters.AddRange(insertParameters);
        }

        public static void Shutdown()
        {
            if (insert != null) { insert.Dispose(); }
        }

        public static EquipParam GetById(int modelId)
        {
            EquipParam result = null;
            using (var conn = CacheManager.CreateConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM EquipParams WHERE ID = @ID";
                    cmd.Parameters.AddWithValue("@ID", modelId);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (reader.Read())
                        {
                            result = EquipParam.FromDataReader(reader);
                        }
                    }
                }
            }
            return result;
        }

        public static void FillCache()
        {
            string path = Path.Combine(Properties.Settings.Default.GameDirectory, cmnFilePath);
            var cmnFile = DatDigger.Sections.SectionLoader.OpenFile(path);
            var eqp = cmnFile.FindChild<DatDigger.Sections.Model.EquipParamSection>();

            using (var conn = CacheManager.CreateConnection())
            {
                
                insert.Connection = conn;
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    eqp.EquipParams.ForEach(x => InsertRow(x));
                    trans.Commit();
                }
                insert.Connection = null;
            }
        }

        private static void InsertRow(EquipParam row)
        {
            insert.Parameters["@ID"].Value = row.ID;
            for (var i = 0; i < EquipParam.NumValues; i++)
            {
                insert.Parameters["@C" + i].Value = row.Values[i];
            }

            insert.ExecuteNonQuery();
        }

        private const string insertCommandText = @"INSERT INTO EquipParams
                         (ID, C0, C1, C2, C3, C4, C5, C6, C7, C8, C9, C10, C11, C12, C13, C14, C15, C16, C17, C18, C19, C20, C21, C22, C23, C24, C25, C26, C27, C28, C29, C30, C31, C32, C33, 
                         C34, C35, C36, C37, C38, C39, C40, C41, C42, C43, C44, C45, C46, C47, C48, C49, C50, C51, C52, C53, C54, C55, C56, C57, C58, C59, C60, C61, C62, C63, C64, C65, 
                         C66, C67, C68, C69, C70, C71, C72, C73, C74, C75, C76, C77, C78, C79, C80, C81, C82, C83, C84, C85, C86, C87, C88, C89, C90, C91, C92, C93, C94, C95, C96, C97, 
                         C98, C99, C100, C101, C102, C103, C104, C105, C106, C107, C108, C109, C110, C114, C113, C112, C111, C115, C295, C294, C293, C292, C291, C290, C289, 
                         C288, C287, C286, C285, C284, C283, C282, C281, C280, C279, C278, C277, C276, C275, C274, C273, C272, C271, C270, C269, C268, C267, C266, C265, C264, C263, 
                         C262, C261, C260, C259, C258, C257, C256, C255, C254, C253, C252, C251, C250, C249, C248, C247, C246, C245, C244, C243, C242, C241, C240, C239, C238, C237, 
                         C236, C235, C234, C233, C232, C231, C230, C229, C228, C227, C226, C225, C224, C223, C222, C221, C220, C219, C218, C217, C216, C215, C214, C213, C212, C211, 
                         C210, C209, C208, C207, C206, C205, C204, C203, C202, C201, C200, C199, C198, C197, C196, C195, C194, C193, C192, C191, C190, C189, C188, C187, C186, C185, 
                         C184, C183, C182, C181, C180, C179, C178, C177, C176, C175, C174, C173, C172, C171, C170, C169, C168, C167, C166, C165, C164, C163, C162, C161, C160, C159, 
                         C158, C157, C156, C155, C154, C153, C152, C151, C150, C149, C148, C147, C146, C145, C144, C143, C142, C141, C140, C139, C138, C137, C136, C135, C134, C133, 
                         C132, C131, C130, C129, C128, C127, C126, C125, C124, C123, C122, C121, C120, C119, C118, C117, C116)
VALUES        (@ID, @C0, @C1, @C2, @C3, @C4, @C5, @C6, @C7, @C8, @C9, @C10, @C11, @C12, @C13, @C14, @C15, @C16, @C17, @C18, @C19, @C20, @C21, @C22, @C23, @C24, @C25, @C26, @C27, @C28, @C29, @C30, @C31, @C32, @C33, 
                         @C34, @C35, @C36, @C37, @C38, @C39, @C40, @C41, @C42, @C43, @C44, @C45, @C46, @C47, @C48, @C49, @C50, @C51, @C52, @C53, @C54, @C55, @C56, @C57, @C58, @C59, @C60, @C61, @C62, @C63, @C64, @C65, 
                         @C66, @C67, @C68, @C69, @C70, @C71, @C72, @C73, @C74, @C75, @C76, @C77, @C78, @C79, @C80, @C81, @C82, @C83, @C84, @C85, @C86, @C87, @C88, @C89, @C90, @C91, @C92, @C93, @C94, @C95, @C96, @C97, 
                         @C98, @C99, @C100, @C101, @C102, @C103, @C104, @C105, @C106, @C107, @C108, @C109, @C110, @C114, @C113, @C112, @C111, @C115, @C295, @C294, @C293, @C292, @C291, @C290, @C289, 
                         @C288, @C287, @C286, @C285, @C284, @C283, @C282, @C281, @C280, @C279, @C278, @C277, @C276, @C275, @C274, @C273, @C272, @C271, @C270, @C269, @C268, @C267, @C266, @C265, @C264, @C263, 
                         @C262, @C261, @C260, @C259, @C258, @C257, @C256, @C255, @C254, @C253, @C252, @C251, @C250, @C249, @C248, @C247, @C246, @C245, @C244, @C243, @C242, @C241, @C240, @C239, @C238, @C237, 
                         @C236, @C235, @C234, @C233, @C232, @C231, @C230, @C229, @C228, @C227, @C226, @C225, @C224, @C223, @C222, @C221, @C220, @C219, @C218, @C217, @C216, @C215, @C214, @C213, @C212, @C211, 
                         @C210, @C209, @C208, @C207, @C206, @C205, @C204, @C203, @C202, @C201, @C200, @C199, @C198, @C197, @C196, @C195, @C194, @C193, @C192, @C191, @C190, @C189, @C188, @C187, @C186, @C185, 
                         @C184, @C183, @C182, @C181, @C180, @C179, @C178, @C177, @C176, @C175, @C174, @C173, @C172, @C171, @C170, @C169, @C168, @C167, @C166, @C165, @C164, @C163, @C162, @C161, @C160, @C159, 
                         @C158, @C157, @C156, @C155, @C154, @C153, @C152, @C151, @C150, @C149, @C148, @C147, @C146, @C145, @C144, @C143, @C142, @C141, @C140, @C139, @C138, @C137, @C136, @C135, @C134, @C133, 
                         @C132, @C131, @C130, @C129, @C128, @C127, @C126, @C125, @C124, @C123, @C122, @C121, @C120, @C119, @C118, @C117, @C116)";
        private static readonly SQLiteParameter[] insertParameters = new SQLiteParameter[EquipParam.NumValues + 1];
    }
}
