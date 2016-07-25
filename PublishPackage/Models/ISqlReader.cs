using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishPackage.Models
{
    public interface ISqlReader
    {
        SqlDatabase Get(string source);
    }

    public class SqlReaderByDatabase : ISqlReader
    {
        private static readonly string TABLE_LIST_COMMAND = "SELECT TABLE_NAME AS TableName FROM INFORMATION_SCHEMA.TABLES";
        private static readonly string VIEW_LIST_COMMAND = "";
        private static readonly string PROCEDURE_LIST_COMMAND = "SELECT ROUTINE_NAME AS ProcedureName, ROUTINE_DEFINATION AS Defination FROM INFORMATION_SCHEMA.ROUTINES";
        private static readonly string COLUMN_LIST_COMMAND = "select top 2 * from INFORMATION_SCHEMA.COLUMNS";
        private static readonly string CHECK_CONSTRAINT_LIST_COMMAND = "";
        private static readonly string DEFAULT_CONSTRAINT_LIST_COMMAND = "";
        private static readonly string CONSTRAINT_KEY_LIST_COMMAND = "";
        private static readonly string FOREIGN_KEY_LIST_COMMAND = "";

        private System.Data.DataTable GetDataTable(string cmdText, System.Data.SqlClient.SqlConnection conn)
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(cmdText, conn))
            {
                using (System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader())
                {
                    dt.Load(reader);
                }
            }

            return dt;
        }

        public SqlDatabase Get(string source)
        {
            SqlDatabase database = new SqlDatabase();

            using(System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(source))
            {
                conn.Open();

                System.Data.DataTable tables = this.GetDataTable(TABLE_LIST_COMMAND, conn);
                System.Data.DataTable columns = new System.Data.DataTable();
                System.Data.DataTable checkConstraints = new System.Data.DataTable();

                conn.Close();
            }

            return database;
        }
    }

    public class SqlReaderByJson : ISqlReader
    {
        public SqlDatabase Get(string source)
        {
            throw new NotImplementedException();
        }
    }
}
