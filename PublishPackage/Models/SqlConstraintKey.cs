using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishPackage.Models
{
    public class SqlConstraintKey : IDataCompare
    {
        public SqlConstraintKey()
        {
            this.Columns = new List<Tuple<string, bool>>();
        }

        public string KeyName { get; set; }
        public string TableName { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsUniqueKey { get; set; }


        public SqlConstraintKeyType KeyType { get; set; }
        public bool IsClustred { get; set; }

        /// <summary>
        /// Column list (ColumnName, Desending)
        /// </summary>
        public List<Tuple<string, bool>> Columns { get; set; }

        public virtual string GetScript()
        {
            string script = "CONSTRAINT [" + KeyName + "] ";

            switch(KeyType)
            {
                case SqlConstraintKeyType.PrimaryKey:
                    script += "PRIMARY KEY";
                    break;
                case SqlConstraintKeyType.UniqueKey:
                    script += "UNIQUE";
                    break;
                //case SqlConstraintKeyType.Index:
                //    break;
            }

            script += " " + (IsClustred ? "CLUSTERED" : "NONCLUSTERED");

            script += " (";

            script += string.Join(", ", Columns.Select(x => "[" + x.Item1 + "] " + (x.Item2 ? "DESC" : "ASC")));

            script += ")";

            return script;
        }


        public string MD5Hash
        {
            get { return Helper.GetMD5Hash(this.GetScript()); }
        }
    }
}
