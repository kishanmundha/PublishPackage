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
            this.KeyColumns = new List<SqlConstraintKeyColumn>();
        }

        public string KeyName { get; set; }
        public string TableName { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsUniqueKey { get; set; }
        public int KeyTypeId { get; set; }
        public string KeyTypeDesc { get; set; }

        public List<SqlConstraintKeyColumn> KeyColumns { get; set; }


        public SqlConstraintKeyType KeyType { get; set; }
        public bool IsClustred { get; set; }

        /// <summary>
        /// Column list (ColumnName, Desending)
        /// </summary>
        public List<Tuple<string, bool>> Columns { get; set; }

        public virtual string GetScript()
        {
            string script = "CONSTRAINT [" + KeyName + "] ";

            if (IsPrimaryKey)
                script += "PRIMARY KEY";
            if (IsUniqueKey)
                script += "UNIQUE";

            //switch (KeyType)
            //{
            //    case SqlConstraintKeyType.PrimaryKey:
            //        script += "PRIMARY KEY";
            //        break;
            //    case SqlConstraintKeyType.UniqueKey:
            //        script += "UNIQUE";
            //        break;
            //    //case SqlConstraintKeyType.Index:
            //    //    break;
            //}

            //script += " " + (IsClustred ? "CLUSTERED" : "NONCLUSTERED");

            script += " " + KeyTypeDesc;

            script += " (";

            script += string.Join(", ", Columns.Select(x => "[" + x.Item1 + "] " + (x.Item2 ? "DESC" : "ASC")));

            script += ")";

            return script;
        }


        public string MD5Hash
        {
            get { return Helper.GetMD5Hash(this.GetScript()); }
        }

        public class SqlConstraintKeyColumn
        {
            public string ColumnName { get; set; }
            public int KeyOrdinal { get; set; }
            public bool IsDecending { get; set; }
            public bool IsIncludeColumn { get; set; }
        }
    }
}
