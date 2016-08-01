using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishPackage.Models
{
    public class SqlDatabase
    {
        public string DatabaseName { get; set; }
        public List<SqlTable> Tables { get; set; }
        public List<SqlView> Views { get; set; }
        public List<SqlProcedure> Procedures { get; set; }

        public List<SqlCheckConstraint> CheckConstraints { get; set; }
        public List<SqlDefaultConstraint> DefaultConstraints { get; set; }
        public List<SqlConstraintKey> ConstraintKeys { get; set; }
        public List<SqlForeignKey> ForeignKeys { get; set; }

        public SqlDatabase()
        {
            this.Tables = new List<SqlTable>();
            this.Views = new List<SqlView>();
            this.Procedures = new List<SqlProcedure>();

            this.CheckConstraints = new List<SqlCheckConstraint>();
            this.DefaultConstraints = new List<SqlDefaultConstraint>();
            this.ConstraintKeys = new List<SqlConstraintKey>();
            this.ForeignKeys = new List<SqlForeignKey>();
        }

        //public static void Compare(SqlDatabase db1, SqlDatabase db2)
        //{

        //}

        public string GetCreateScript()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("CREATE DATABASE [" + this.DatabaseName + "]\r\nGO\r\n\r\n");

            sb.Append("USE [" + this.DatabaseName + "]\r\nGO\r\n\r\n");

            sb.Append(string.Join("", this.Tables.Select(x => x.GetCreateScript())));

            sb.Append(string.Join("", this.DefaultConstraints.Select(x => x.GetCreateScript())));

            sb.Append(string.Join("", this.ForeignKeys.Select(x => x.GetCreateScript())));

            sb.Append(string.Join("", this.CheckConstraints.Select(x => x.GetCreateScript())));

            sb.Append(string.Join("", this.ConstraintKeys.Select(x => x.GetCreateScript())));

            return sb.ToString();
        }

        public string GetDiffScript(SqlDatabase db)
        {
            if (db == null)
                throw new ArgumentNullException("db");

            StringBuilder sb = new StringBuilder();

            var commonTable = this.Tables;// Helper.GetCommonData(this.Tables, db.Tables) as List<SqlTable>;
            var newTable = this.Tables.Where(x => !db.Tables.Select(y => y.TableName).Contains(x.TableName));
            var deletedTable = db.Tables.Where(x => !this.Tables.Select(y => y.TableName).Contains(x.TableName));

            foreach(var table in commonTable)
            {
                var t2 = db.Tables.Find(x => x.TableName == table.TableName);
                sb.Append(table.GetAlterScript(t2));
            }

            foreach(var table in newTable)
            {
                sb.Append(table.GetCreateScript());
            }

            foreach(var table in deletedTable)
            {
                sb.Append(table.GetDropScript());
            }

            throw new NotImplementedException();
        }

        public object GetJsonObject()
        {
            return new
            {
                Tables = this.Tables.Select(x=>x.GetJsonObject()),
                DefaultConstraints = this.DefaultConstraints.Select(x => x.GetJsonObject()),
                ConstraintKeys = this.ConstraintKeys.Select(x => x.GetJsonObject()),
                CheckConstraints = this.CheckConstraints.Select(x => x.GetJsonObject()),
                ForeignKeys = this.ForeignKeys.Select(x => x.GetJsonObject())
            };
        }

        public string GetJsonString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this.GetJsonObject());
        }
    }
}
