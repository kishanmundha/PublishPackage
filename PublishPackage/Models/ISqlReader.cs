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
     KeyName = ind.name,
     ColumnName = col.name,
	 KeyTypeId = ind.type,
	 KeyTypeDesc = ind.type_desc,
	 IsUniqueKey = ind.is_unique,
	 IsPrimaryKey = ind.is_primary_key,
	 KeyOrdinal = ic.key_ordinal,
	 IsDecending = ic.is_descending_key,
	 IsIncludeColumn = ic.is_included_column
FROM 
     sys.indexes ind 
INNER JOIN 
     sys.index_columns ic ON  ind.object_id = ic.object_id and ind.index_id = ic.index_id 
INNER JOIN 
     sys.columns col ON ic.object_id = col.object_id and ic.column_id = col.column_id 
INNER JOIN 
     sys.tables t ON ind.object_id = t.object_id 
--WHERE 
--     ind.is_primary_key = 0 
--     AND ind.is_unique = 0 
--     AND ind.is_unique_constraint = 0 
--     AND t.is_ms_shipped = 0 
ORDER BY 
     t.name, ind.name, ind.index_id, ic.index_column_id 
";
        //        private static readonly string FOREIGN_KEY_LIST_COMMAND = @"SELECT RC.CONSTRAINT_NAME KeyName, KF.TABLE_NAME TableName, KF.COLUMN_NAME KeyColumnName, KP.TABLE_NAME ForeignTableName, KP.COLUMN_NAME ForeignColumnName
        //FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS RC
        //JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KF ON RC.CONSTRAINT_NAME = KF.CONSTRAINT_NAME
        //JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KP ON RC.UNIQUE_CONSTRAINT_NAME = KP.CONSTRAINT_NAME
        //";

        private static readonly string FOREIGN_KEY_LIST_COMMAND = @"SELECT f.name AS KeyName, 
   OBJECT_NAME(f.parent_object_id) AS TableName, 
   COL_NAME(fc.parent_object_id, fc.parent_column_id) AS KeyColumnName, 
   OBJECT_NAME (f.referenced_object_id) AS ForeignTableName, 
   COL_NAME(fc.referenced_object_id, fc.referenced_column_id) AS ForeignColumnName 
FROM sys.foreign_keys AS f 
INNER JOIN sys.foreign_key_columns AS fc 
   ON f.OBJECT_ID = fc.constraint_object_id
ORDER BY KeyName
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

                #region Tables
                foreach (System.Data.DataRow row in tables.Rows)
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

                            if (identityColumn != null && IdentityColumnName == column.ColumnName)
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
                            foreignKey.TableName = c["TableName"] as string;
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
                            defaultConstraint.TableName = table.TableName;
                            defaultConstraint.KeyName = c["KeyName"] as string;
                            defaultConstraint.ColumnName = c["ColumnName"] as string;
                            defaultConstraint.Definition = c["Definition"] as string;
                            table.DefaultConstraints.Add(defaultConstraint);
                        }
                    }
                    #endregion

                    #region Constraint Key
                    {
                        System.Data.DataView dv = new System.Data.DataView(constraintsKeys);
                        dv.RowFilter = "TableName='" + table.TableName + "'";
                        var dt = dv.ToTable();

                        var distinctKeys = dt.Rows.OfType<System.Data.DataRow>()
                            .Select(x => new
                            {
                                KeyName = x["KeyName"].ToString(),
                                KeyTypeId = (byte)x["KeyTypeId"],
                                KeyTypeDesc = (string)x["KeyTypeDesc"],
                                IsPrimaryKey = (bool)x["IsPrimaryKey"],
                                IsUniqueKey = (bool)x["IsUniqueKey"]
                            }).Distinct();

                        foreach (var dk in distinctKeys)
                        {
                            var constraintsKey = new SqlConstraintKey();
                            constraintsKey.TableName = table.TableName;
                            constraintsKey.KeyName = dk.KeyName;
                            constraintsKey.KeyTypeId = dk.KeyTypeId;
                            constraintsKey.KeyTypeDesc = dk.KeyTypeDesc;

                            constraintsKey.IsPrimaryKey = dk.IsPrimaryKey;
                            constraintsKey.IsUniqueKey = dk.IsUniqueKey;


                            //constraintsKey.KeyType = (dk.KeyType == "" ? SqlConstraintKeyType.PrimaryKey : dk.KeyType == "" ? SqlConstraintKeyType.UniqueKey : SqlConstraintKeyType.Index);
                            //constraintsKey.IsClustred = dk.IsClustred;

                            System.Data.DataView dv2 = new System.Data.DataView(dt);
                            dv2.RowFilter = "KeyName='" + constraintsKey.KeyName + "'";
                            var dt2 = dv2.ToTable();

                            foreach (System.Data.DataRow c2 in dt2.Rows)
                            {
                                //constraintsKey.Columns.Add(new Tuple<string, bool>(c2["ColumnName"] as string, (bool)c2["IsDesending"]));
                                constraintsKey.KeyColumns.Add(new SqlConstraintKey.SqlConstraintKeyColumn
                                {
                                    ColumnName = c2["ColumnName"] as string,
                                    KeyOrdinal = (byte)c2["KeyOrdinal"],
                                    IsDecending = (bool)c2["IsDecending"],
                                    IsIncludeColumn = (bool)c2["IsIncludeColumn"]
                                });
                            }

                            table.ConstraintKeys.Add(constraintsKey);
                        }
                    }
                    #endregion

                    database.Tables.Add(table);
                }
                #endregion

                #region Constraint Keys
                {
                    var dt = constraintsKeys;

                    var distinctKeys = dt.Rows.OfType<System.Data.DataRow>()
                        .Select(x => new
                        {
                            TableName = x["TableName"].ToString(),
                            KeyName = x["KeyName"].ToString(),
                            KeyTypeId = (byte)x["KeyTypeId"],
                            KeyTypeDesc = (string)x["KeyTypeDesc"],
                            IsPrimaryKey = (bool)x["IsPrimaryKey"],
                            IsUniqueKey = (bool)x["IsUniqueKey"]
                        }).Distinct();

                    foreach (var dk in distinctKeys)
                    {
                        var constraintsKey = new SqlConstraintKey();
                        constraintsKey.TableName = dk.TableName;
                        constraintsKey.KeyName = dk.KeyName;
                        constraintsKey.KeyTypeId = dk.KeyTypeId;
                        constraintsKey.KeyTypeDesc = dk.KeyTypeDesc;

                        constraintsKey.IsPrimaryKey = dk.IsPrimaryKey;
                        constraintsKey.IsUniqueKey = dk.IsUniqueKey;


                        //constraintsKey.KeyType = (dk.KeyType == "" ? SqlConstraintKeyType.PrimaryKey : dk.KeyType == "" ? SqlConstraintKeyType.UniqueKey : SqlConstraintKeyType.Index);
                        //constraintsKey.IsClustred = dk.IsClustred;

                        System.Data.DataView dv2 = new System.Data.DataView(dt);
                        dv2.RowFilter = "KeyName='" + constraintsKey.KeyName + "'";
                        var dt2 = dv2.ToTable();

                        foreach (System.Data.DataRow c2 in dt2.Rows)
                        {
                            //constraintsKey.Columns.Add(new Tuple<string, bool>(c2["ColumnName"] as string, (bool)c2["IsDesending"]));
                            constraintsKey.KeyColumns.Add(new SqlConstraintKey.SqlConstraintKeyColumn
                            {
                                ColumnName = c2["ColumnName"] as string,
                                KeyOrdinal = (byte)c2["KeyOrdinal"],
                                IsDecending = (bool)c2["IsDecending"],
                                IsIncludeColumn = (bool)c2["IsIncludeColumn"]
                            });
                        }

                        database.ConstraintKeys.Add(constraintsKey);
                    }
                }
                #endregion

                #region Default Constraints
                {
                    var dt = defaultConstraints;

                    foreach (System.Data.DataRow c in dt.Rows)
                    {
                        var defaultConstraint = new SqlDefaultConstraint();
                        defaultConstraint.KeyName = c["KeyName"] as string;
                        defaultConstraint.TableName = c["TableName"] as string;
                        defaultConstraint.ColumnName = c["ColumnName"] as string;
                        defaultConstraint.Definition = c["Definition"] as string;
                        database.DefaultConstraints.Add(defaultConstraint);
                    }
                }
                #endregion

                #region Foreign Keys
                {
                    var dt = foreignKeys;

                    foreach (System.Data.DataRow c in dt.Rows)
                    {
                        var foreignKey = new SqlForeignKey();
                        foreignKey.TableName = c["TableName"] as string;
                        foreignKey.KeyName = c["KeyName"] as string;
                        foreignKey.KeyColumnName = c["KeyColumnName"] as string;
                        foreignKey.ForeignTableName = c["ForeignTableName"] as string;
                        foreignKey.ForeignColumnName = c["ForeignColumnName"] as string;
                        database.ForeignKeys.Add(foreignKey);
                    }

                }
                #endregion

                #region Views
                foreach (System.Data.DataRow row in views.Rows)
                {
                    SqlView v = new SqlView();
                    v.ViewName = row["ViewName"] as string;
                    v.ViewDefinition = row["ViewDefinition"] as string;

                    database.Views.Add(v);
                }
                #endregion

                #region Procedures
                foreach (System.Data.DataRow row in procedures.Rows)
                {
                    SqlProcedure procedure = new SqlProcedure();
                    procedure.ProcedureName = row["ProcedureName"] as string;
                    procedure.ProcedureDefination = row["Definition"] as string;

                    database.Procedures.Add(procedure);
                }
                #endregion
            }

            return database;
        }
    }

    public class SqlReaderByJson : ISqlReader
    {
        public SqlDatabase Get(string source)
        {
            var str = System.IO.File.ReadAllText(source);

            dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject(str);

            SqlDatabase database = new SqlDatabase();

            {

                database.DatabaseName = obj.DatabaseName;

                #region Tables
                foreach (dynamic table in obj.Tables)
                {
                    SqlTable sTable = new SqlTable();
                    sTable.TableName = table.TableName;

                    foreach (dynamic column in table.Columns)
                    {
                        SqlColumn sColumn = new SqlColumn();

                        sColumn.ColumnName = column.ColumnName;
                        sColumn.DataType = column.DataType;
                        sColumn.DateTimePrec = column.DateTimePrec;
                        sColumn.IsNullable = column.IsNullable;
                        sColumn.Length = column.Length;
                        sColumn.OridinalPosition = column.OridinalPosition;
                        sColumn.Prec = column.Prec;
                        sColumn.Scale = column.Scale;

                        if (column.Identity != null)
                        {
                            SqlIdentityColumn iColumn = new SqlIdentityColumn();
                            sColumn.Identity = iColumn;
                            iColumn.IncrementValue = column.Identity.IncrementValue;
                            iColumn.SeedValue = column.Identity.SeedValue;
                        }

                        sTable.Columns.Add(sColumn);
                    }

                    database.Tables.Add(sTable);
                }
                #endregion

                #region Constraint Keys
                foreach (dynamic constraintKey in obj.ConstraintKeys)
                {
                    SqlConstraintKey sKey = new SqlConstraintKey();
                    sKey.KeyName = constraintKey.KeyName;
                    sKey.TableName = constraintKey.TableName;
                    sKey.IsPrimaryKey = constraintKey.IsPrimaryKey;
                    sKey.IsUniqueKey = constraintKey.IsUniqueKey;
                    sKey.KeyTypeId = constraintKey.KeyTypeId;
                    sKey.KeyTypeDesc = constraintKey.KeyTypeDesc;

                    foreach(dynamic keyColumn in constraintKey.KeyColumns)
                    {
                        sKey.KeyColumns.Add(new SqlConstraintKey.SqlConstraintKeyColumn
                        {
                            ColumnName = keyColumn.ColumnName,
                            IsDecending = keyColumn.IsDecending,
                            IsIncludeColumn = keyColumn.IsIncludeColumn,
                            KeyOrdinal = keyColumn.KeyOrdinal
                        });
                    }

                    database.ConstraintKeys.Add(sKey);
                }
                #endregion

                #region Default Constraints
                foreach(dynamic defaultConstraint in obj.DefaultConstraints)
                {
                    database.DefaultConstraints.Add(new SqlDefaultConstraint
                    {
                        ColumnName = defaultConstraint.ColumnName,
                        Definition = defaultConstraint.Defination,
                        KeyName = defaultConstraint.KeyName,
                        TableName = defaultConstraint.TableName
                    });
                }
                #endregion

                #region Foreign Keys
                foreach (dynamic foreignKey in obj.ForeignKeys)
                {
                    database.ForeignKeys.Add(new SqlForeignKey
                    {
                        TableName = foreignKey.TableName,
                        KeyName = foreignKey.KeyName,
                        ForeignColumnName = foreignKey.ForeignColumnName,
                        ForeignTableName = foreignKey.ForeignTableName,
                        KeyColumnName = foreignKey.KeyColumnName
                    });
                }
                #endregion

                #region Views
                foreach (dynamic view in obj.Views)
                {
                    database.Views.Add(new SqlView
                    {
                        ViewName = view.ViewName,
                        ViewDefinition = view.ViewDefinition
                    });
                }
                #endregion

                #region Procedures
                foreach (dynamic procedure in obj.Procedures)
                {
                    database.Procedures.Add(new SqlProcedure
                    {
                        ProcedureName = procedure.ProcedureName,
                        ProcedureDefination = procedure.ProcedureDefination
                    });
                }
                #endregion
            }

            return database;
        }
    }
}
