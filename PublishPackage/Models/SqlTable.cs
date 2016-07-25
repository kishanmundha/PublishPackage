using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishPackage.Models
{
    public class SqlTable : IDataCompare
    {
        private string _md5Hash { get; set; }

        public string TableName { get; set; }

        public List<SqlColumn> Columns { get; set; }
        public List<SqlCheckConstraint> CheckConstraints { get; set; }
        public List<SqlDefaultConstraint> DefaultConstraints { get; set; }
        public List<SqlConstraintKey> ConstraintKeys { get; set; }
        public List<SqlForeignKey> ForeignKeys { get; set; }

        public string KeyName
        {
            get
            {
                return this.TableName;
            }
        }

        public string MD5Hash
        {
            get
            {
                if (this._md5Hash == null)
                    this._md5Hash = Helper.GetMD5Hash(this.GetCreateScript());

                return this._md5Hash;
            }
        }

        public SqlTable()
        {
            this.Columns = new List<SqlColumn>();
            this.CheckConstraints = new List<SqlCheckConstraint>();
            this.DefaultConstraints = new List<SqlDefaultConstraint>();
            this.ForeignKeys = new List<SqlForeignKey>();
            this.ConstraintKeys = new List<SqlConstraintKey>();
        }

        public string JsonString()
        {
            throw new NotImplementedException();
        }

        public string GetCreateScript()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("CREATE TABLE [" + this.TableName + "](\r\n");

            // columns
            sb.Append("\t" + string.Join(",\r\n\t", this.Columns.Select(x => x.GetScript())));

            // Default constraints

            // Check Constraints

            // Constraints (Index)

            // Foregin Key

            sb.Append("\r\n)\r\n");
            sb.Append("GO\r\n\r\n");

            sb.Append(string.Join("", this.DefaultConstraints.Select(x => x.GetAddScript(this.TableName))));

            sb.Append(string.Join("", this.ForeignKeys.Select(x => x.GetScript(this.TableName))));

            sb.Append(string.Join("", this.CheckConstraints.Select(x => x.GetScript(this.TableName))));

            return sb.ToString();
        }

        public string GetDropScript()
        {
            return "DROP TABLE " + this.TableName + "\r\nGO\r\n";
        }

        public string GetAlterScript(SqlTable t2)
        {
            throw new NotImplementedException();
        }

        public string GetScript()
        {
            return this.GetCreateScript();
        }
    }
}
