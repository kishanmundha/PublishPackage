using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishPackage.Models
{
    public class SqlDatabaseCompareResult
    {
        public List<DataCompareResult<SqlTable>> Tables { get; set; }
        public List<DataCompareResult<SqlView>> Views { get; set; }
        public List<DataCompareResult<SqlProcedure>> Procedures { get; set; }

        public List<DataCompareResult<SqlCheckConstraint>> CheckConstraints { get; set; }
        public List<DataCompareResult<SqlDefaultConstraint>> DefaultConstraints { get; set; }
        public List<DataCompareResult<SqlConstraintKey>> ConstraintKeys { get; set; }
        public List<DataCompareResult<SqlForeignKey>> ForeignKeys { get; set; }

        public SqlDatabaseCompareResult()
        {
            this.Tables = new List<DataCompareResult<SqlTable>>();
            this.Views = new List<DataCompareResult<SqlView>>();
            this.Procedures = new List<DataCompareResult<SqlProcedure>>();

            this.CheckConstraints = new List<DataCompareResult<SqlCheckConstraint>>();
            this.DefaultConstraints = new List<DataCompareResult<SqlDefaultConstraint>>();
            this.ConstraintKeys = new List<DataCompareResult<SqlConstraintKey>>();
            this.ForeignKeys = new List<DataCompareResult<SqlForeignKey>>();
        }

        public string GetScript()
        {
            StringBuilder sb = new StringBuilder();

            #region Table
            foreach (var table in this.Tables)
            {
                switch (table.Status)
                {
                    case DataCompareStatus.New:
                        sb.Append(table.NewData.GetCreateScript());
                        break;
                    case DataCompareStatus.Modified:
                        var tableCompareResult = SqlTableCompareResult.Compare(table.OldData, table.NewData);
                        sb.Append(tableCompareResult.GetScript(table.KeyName));
                        break;
                    case DataCompareStatus.Deleted:
                        sb.Append(table.OldData.GetDropScript());
                        break;
                }
            }
            #endregion

            #region CheckConstraints
            foreach (var checkConstraint in this.CheckConstraints)
            {
                switch (checkConstraint.Status)
                {
                    case DataCompareStatus.New:
                        sb.Append(checkConstraint.NewData.GetCreateScript());
                        break;
                    case DataCompareStatus.Modified:
                        sb.Append(checkConstraint.OldData.GetDropScript());
                        sb.Append(checkConstraint.NewData.GetCreateScript());
                        break;
                    case DataCompareStatus.Deleted:
                        sb.Append(checkConstraint.OldData.GetDropScript());
                        break;
                }
            }
            #endregion

            #region DefaultConstraints
            foreach (var defaultConstraint in this.DefaultConstraints)
            {
                switch (defaultConstraint.Status)
                {
                    case DataCompareStatus.New:
                        sb.Append(defaultConstraint.NewData.GetCreateScript());
                        break;
                    case DataCompareStatus.Modified:
                        sb.Append(defaultConstraint.OldData.GetDropScript());
                        sb.Append(defaultConstraint.NewData.GetCreateScript());
                        break;
                    case DataCompareStatus.Deleted:
                        sb.Append(defaultConstraint.OldData.GetDropScript());
                        break;
                }
            }
            #endregion
            #region ConstraintKeys
            foreach (var constraintKey in this.ConstraintKeys)
            {
                switch (constraintKey.Status)
                {
                    case DataCompareStatus.New:
                        sb.Append(constraintKey.NewData.GetCreateScript());
                        break;
                    case DataCompareStatus.Modified:
                        sb.Append(constraintKey.OldData.GetDropScript());
                        sb.Append(constraintKey.NewData.GetCreateScript());
                        break;
                    case DataCompareStatus.Deleted:
                        sb.Append(constraintKey.OldData.GetDropScript());
                        break;
                }
            }
            #endregion
            #region ForeignKeys
            foreach (var foreignKey in this.ForeignKeys)
            {
                switch (foreignKey.Status)
                {
                    case DataCompareStatus.New:
                        sb.Append(foreignKey.NewData.GetCreateScript());
                        break;
                    case DataCompareStatus.Modified:
                        sb.Append(foreignKey.OldData.GetDropScript());
                        sb.Append(foreignKey.NewData.GetCreateScript());
                        break;
                    case DataCompareStatus.Deleted:
                        sb.Append(foreignKey.OldData.GetDropScript());
                        break;
                }
            }
            #endregion

            #region Views
            foreach (var view in this.Views)
            {
                switch (view.Status)
                {
                    case DataCompareStatus.New:
                        sb.Append(view.NewData.GetCreateScript());
                        break;
                    case DataCompareStatus.Modified:
                        sb.Append(view.NewData.GetAlterScript());
                        break;
                    case DataCompareStatus.Deleted:
                        sb.Append(view.OldData.GetDropScript());
                        break;
                }
            }
            #endregion

            #region Procedures
            foreach (var procedure in this.Procedures)
            {
                switch (procedure.Status)
                {
                    case DataCompareStatus.New:
                        sb.Append(procedure.NewData.GetCreateScript());
                        break;
                    case DataCompareStatus.Modified:
                        sb.Append(procedure.NewData.GetAlterScript());
                        break;
                    case DataCompareStatus.Deleted:
                        sb.Append(procedure.OldData.GetDropScript());
                        break;
                }
            }
            #endregion

            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db1">Old</param>
        /// <param name="db2">New</param>
        /// <returns></returns>
        public static SqlDatabaseCompareResult Compare(SqlDatabase db1, SqlDatabase db2)
        {
            var result = new SqlDatabaseCompareResult();

            if (db1 == null)
                db1 = new SqlDatabase();

            if (db2 == null)
                db2 = new SqlDatabase();

            result.Tables.AddRange(Helper.GetCompareResult<SqlTable>(db1.Tables.Cast<IDataCompare>().ToList(), db2.Tables.Cast<IDataCompare>().ToList()));
            result.CheckConstraints.AddRange(Helper.GetCompareResult<SqlCheckConstraint>(db1.CheckConstraints.Cast<IDataCompare>().ToList(), db2.CheckConstraints.Cast<IDataCompare>().ToList()));
            result.DefaultConstraints.AddRange(Helper.GetCompareResult<SqlDefaultConstraint>(db1.DefaultConstraints.Cast<IDataCompare>().ToList(), db2.DefaultConstraints.Cast<IDataCompare>().ToList()));
            result.ConstraintKeys.AddRange(Helper.GetCompareResult<SqlConstraintKey>(db1.ConstraintKeys.Cast<IDataCompare>().ToList(), db2.ConstraintKeys.Cast<IDataCompare>().ToList()));
            result.ForeignKeys.AddRange(Helper.GetCompareResult<SqlForeignKey>(db1.ForeignKeys.Cast<IDataCompare>().ToList(), db2.ForeignKeys.Cast<IDataCompare>().ToList()));

            result.Views.AddRange(Helper.GetCompareResult<SqlView>(db1.Views.Cast<IDataCompare>().ToList(), db2.Views.Cast<IDataCompare>().ToList()));
            result.Procedures.AddRange(Helper.GetCompareResult<SqlProcedure>(db1.Procedures.Cast<IDataCompare>().ToList(), db2.Procedures.Cast<IDataCompare>().ToList()));

            return result;
        }
    }

    public class SqlTableCompareResult
    {
        public List<DataCompareResult<SqlColumn>> Columns { get; private set; }
        public List<DataCompareResult<SqlCheckConstraint>> CheckConstraints { get; private set; }
        public List<DataCompareResult<SqlDefaultConstraint>> DefaultConstraints { get; private set; }
        public List<DataCompareResult<SqlConstraintKey>> ConstraintKeys { get; private set; }
        public List<DataCompareResult<SqlForeignKey>> ForeignKeys { get; private set; }

        public SqlTableCompareResult()
        {
            this.Columns = new List<DataCompareResult<SqlColumn>>();
            this.CheckConstraints = new List<DataCompareResult<SqlCheckConstraint>>();
            this.DefaultConstraints = new List<DataCompareResult<SqlDefaultConstraint>>();
            this.ConstraintKeys = new List<DataCompareResult<SqlConstraintKey>>();
            this.ForeignKeys = new List<DataCompareResult<SqlForeignKey>>();
        }

        public string GetScript(string tableName)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var column in this.Columns)
            {
                switch (column.Status)
                {
                    case DataCompareStatus.New:
                        sb.Append(column.NewData.GetAddScript(tableName));
                        break;
                    case DataCompareStatus.Modified:
                        sb.Append(column.NewData.GetAlterScript(tableName));
                        break;
                    case DataCompareStatus.Renamed:
                        sb.Append(column.OldData.GetRenameScript(tableName, column.NewData.KeyName));
                        break;
                    case DataCompareStatus.Deleted:
                        sb.Append(column.OldData.GetDropScript(tableName));
                        break;
                }
            }

            return sb.ToString();
        }

        public static SqlTableCompareResult Compare(SqlTable table1, SqlTable table2)
        {
            var result = new SqlTableCompareResult();

            result.Columns.AddRange(Helper.GetCompareResult<SqlColumn>(table1.Columns.Cast<IDataCompare>().ToList(), table2.Columns.Cast<IDataCompare>().ToList()));
            result.CheckConstraints.AddRange(Helper.GetCompareResult<SqlCheckConstraint>(table1.CheckConstraints.Cast<IDataCompare>().ToList(), table2.CheckConstraints.Cast<IDataCompare>().ToList()));
            result.DefaultConstraints.AddRange(Helper.GetCompareResult<SqlDefaultConstraint>(table1.DefaultConstraints.Cast<IDataCompare>().ToList(), table2.DefaultConstraints.Cast<IDataCompare>().ToList()));
            result.ConstraintKeys.AddRange(Helper.GetCompareResult<SqlConstraintKey>(table1.ConstraintKeys.Cast<IDataCompare>().ToList(), table2.ConstraintKeys.Cast<IDataCompare>().ToList()));
            result.ForeignKeys.AddRange(Helper.GetCompareResult<SqlForeignKey>(table1.ForeignKeys.Cast<IDataCompare>().ToList(), table2.ForeignKeys.Cast<IDataCompare>().ToList()));

            return result;
        }
    }
}
