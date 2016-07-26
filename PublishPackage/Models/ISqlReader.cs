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
        private static readonly string TABLE_LIST_COMMAND = "SELECT TABLE_NAME AS TableName FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'";
        private static readonly string VIEW_LIST_COMMAND = "select TABLE_NAME AS ViewName, VIEW_DEFINITION AS ViewDefinition from INFORMATION_SCHEMA.VIEWS";
        private static readonly string PROCEDURE_LIST_COMMAND = "SELECT ROUTINE_NAME AS ProcedureName, ROUTINE_DEFINITION AS Definition FROM INFORMATION_SCHEMA.ROUTINES";
        private static readonly string COLUMN_LIST_COMMAND = "select table_name as TableName, column_name as ColumnName, data_type as DataType, CHARACTER_MAXIMUM_LENGTH as Length, NUMERIC_PRECISION as Prec, NUMERIC_SCALE as Scale, datetime_precision as DateTimePrec, ORDINAL_POSITION as OrdinalPosition, IS_NULLABLE as IsNullable from INFORMATION_SCHEMA.COLUMNS";
        private static readonly string CHECK_CONSTRAINT_LIST_COMMAND = "select object_name(parent_object_id) as TableName, name as ConstraintName, definition as CheckClause from sys.check_constraints";

        private static readonly string IDENTITY_COLUMN_COMMAND = @"SELECT object_name(object_id) as TableName, name as ColumnName, seed_value as SeedValue, increment_value as IncrementValue
FROM sys.identity_columns ic
inner join INFORMATION_SCHEMA.TABLES t on t.table_name=object_name(object_id) and t.TABLE_TYPE='BASE TABLE'";

        private static readonly string DEFAULT_CONSTRAINT_LIST_COMMAND = @"select dc.name as KeyName, object_name(parent_object_id) as TableName, definition as Definition, c.name as ColumnName
from sys.default_constraints dc
inner join sys.columns c on dc.parent_object_id=c.object_id and dc.parent_column_id=c.column_id
";
        private static readonly string CONSTRAINT_KEY_LIST_COMMAND = @"SELECT 
     TableName = t.name,
     IndexName = ind.name,
     IndexId = ind.index_id,
     ColumnId = ic.index_column_id,
     ColumnName = col.name,
     ind.*,
     ic.*,
     col.* 
FROM 
     sys.indexes ind 
INNER JOIN 
     sys.index_columns ic ON  ind.object_id = ic.object_id and ind.index_id = ic.index_id 
INNER JOIN 
     sys.columns col ON ic.object_id = col.object_id and ic.column_id = col.column_id 
INNER JOIN 
     sys.tables t ON ind.object_id = t.object_id 
WHERE 
     ind.is_primary_key = 0 
     AND ind.is_unique = 0 
     AND ind.is_unique_constraint = 0 
     AND t.is_ms_shipped = 0 
ORDER BY 
     t.name, ind.name, ind.index_id, ic.index_column_id 
";
        private static readonly string FOREIGN_KEY_LIST_COMMAND = @"SELECT RC.CONSTRAINT_NAME KeyName, KF.TABLE_NAME TableName, KF.COLUMN_NAME KeyColumnName, KP.TABLE_NAME ForeignTableName, KP.COLUMN_NAME ForeignColumnName
FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS RC
JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KF ON RC.CONSTRAINT_NAME = KF.CONSTRAINT_NAME
JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KP ON RC.UNIQUE_CONSTRAINT_NAME = KP.CONSTRAINT_NAME
";

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

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(source))
            {
                conn.Open();

                System.Data.DataTable tables = this.GetDataTable(TABLE_LIST_COMMAND, conn);
                System.Data.DataTable columns = this.GetDataTable(COLUMN_LIST_COMMAND, conn);
                System.Data.DataTable identityColumns = this.GetDataTable(IDENTITY_COLUMN_COMMAND, conn);
                System.Data.DataTable checkConstraints = this.GetDataTable(CHECK_CONSTRAINT_LIST_COMMAND, conn);
                System.Data.DataTable defaultConstraints = this.GetDataTable(DEFAULT_CONSTRAINT_LIST_COMMAND, conn);
                System.Data.DataTable constraintsKeys = this.GetDataTable(CONSTRAINT_KEY_LIST_COMMAND, conn);
                System.Data.DataTable foreignKeys = this.GetDataTable(FOREIGN_KEY_LIST_COMMAND, conn);
                System.Data.DataTable views = this.GetDataTable(VIEW_LIST_COMMAND, conn);
                System.Data.DataTable procedures = this.GetDataTable(PROCEDURE_LIST_COMMAND, conn);

                conn.Close();

                database.DatabaseName = conn.Database;

                foreach(System.Data.DataRow row in tables.Rows)
                {
                    var table = new SqlTable();
                    table.TableName = row["TableName"] as string;

                    #region Columns
                    {
                        System.Data.DataView dvi = new System.Data.DataView(identityColumns);
                        dvi.RowFilter = "TableName='" + table.TableName + "'";
                        var dti = dvi.ToTable();

                        string IdentityColumnName = null;
                        SqlIdentityColumn identityColumn = null;

                        if (dti.Rows.Count > 0)
                        {
                            IdentityColumnName = dti.Rows[0]["ColumnName"] as string;
                            identityColumn = new SqlIdentityColumn();
                            identityColumn.IncrementValue = Convert.ToInt64(dti.Rows[0]["IncrementValue"]);
                            identityColumn.SeedValue = Convert.ToInt64(dti.Rows[0]["SeedValue"]);
                        }

                        System.Data.DataView dv = new System.Data.DataView(columns);
                        dv.RowFilter = "TableName='" + table.TableName + "'";
                        var dt = dv.ToTable();

                        foreach (System.Data.DataRow c in dt.Rows)
                        {
                            var column = new SqlColumn();
                            column.ColumnName = c["ColumnName"] as string;
                            column.DataType = c["DataType"] as string;
                            column.Length = (c["Length"] is DBNull) ? null : (int?)(c["Length"]);
                            column.Prec = (c["Prec"] is DBNull) ? null : (byte?)(c["Prec"]);
                            column.Scale = (c["Scale"] is DBNull) ? null : (int?)(c["Scale"]);
                            column.DateTimePrec = (c["DateTimePrec"] is DBNull) ? null : (short?)(c["DateTimePrec"]);
                            column.OridinalPosition = (int)(c["OrdinalPosition"]);
                            column.IsNullable = (c["IsNullable"] as string) == "YES";

                            if(identityColumn != null && IdentityColumnName == column.ColumnName)
                            {
                                column.Identity = identityColumn;
                            }

                            table.Columns.Add(column);
                        }
                    }
                    #endregion

                    #region ForeignKey
                    {
                        System.Data.DataView dv = new System.Data.DataView(foreignKeys);
                        dv.RowFilter = "TableName='" + table.TableName + "'";
                        var dt = dv.ToTable();

                        foreach (System.Data.DataRow c in dt.Rows)
                        {
                            var foreignKey = new SqlForeignKey();
                            foreignKey.KeyName = c["KeyName"] as string;
                            foreignKey.KeyColumnName = c["KeyColumnName"] as string;
                            foreignKey.ForeignTableName = c["ForeignTableName"] as string;
                            foreignKey.ForeignColumnName = c["ForeignColumnName"] as string;
                            table.ForeignKeys.Add(foreignKey);
                        }
                    }
                    #endregion

                    #region CheckConstraints
                    {
                        System.Data.DataView dv = new System.Data.DataView(checkConstraints);
                        dv.RowFilter = "TableName='" + table.TableName + "'";
                        var dt = dv.ToTable();

                        foreach (System.Data.DataRow c in dt.Rows)
                        {
                            var checkConstraint = new SqlCheckConstraint();
                            checkConstraint.ConstraintName = c["ConstraintName"] as string;
                            checkConstraint.CheckClause = c["CheckClause"] as string;
                            table.CheckConstraints.Add(checkConstraint);
                        }
                    }
                    #endregion

                    #region Default Constraints
                    {
                        System.Data.DataView dv = new System.Data.DataView(defaultConstraints);
                        dv.RowFilter = "TableName='" + table.TableName + "'";
                        var dt = dv.ToTable();

                        foreach (System.Data.DataRow c in dt.Rows)
                        {
                            var defaultConstraint = new SqlDefaultConstraint();
                            defaultConstraint.ColumnName = c["ColumnName"] as string;
                            defaultConstraint.Definition = c["Definition"] as string;
                            table.DefaultConstraints.Add(defaultConstraint);
                        }
                    }
                    #endregion

                    database.Tables.Add(table);
                }
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
