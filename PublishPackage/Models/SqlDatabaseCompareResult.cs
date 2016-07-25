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
        //public List<DataCompareResult> Views { get; set; }
        //public List<DataCompareResult> Procedures { get; set; }

        public SqlDatabaseCompareResult()
        {
            this.Tables = new List<DataCompareResult<SqlTable>>();
            //this.Views = new List<DataCompareResult>();
            //this.Procedures = new List<DataCompareResult>();
        }

        public string GetScript()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var table in this.Tables)
            {
                switch(table.Status)
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

            return sb.ToString();
        }

        public static SqlDatabaseCompareResult Compare(SqlDatabase db1, SqlDatabase db2)
        {
            var result = new SqlDatabaseCompareResult();

            result.Tables.AddRange(Helper.GetCompareResult<SqlTable>(db1.Tables.Cast<IDataCompare>().ToList(), db2.Tables.Cast<IDataCompare>().ToList()));

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
                        //sb.Append(column.OldData)
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
