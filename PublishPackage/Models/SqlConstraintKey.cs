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
            string script = "";

            if (IsPrimaryKey || IsUniqueKey)
            {
                script += string.Format("ALTER TABLE [{0}] ADD CONSTRAINT [{1}] {2} {3}\r\n(\r\n\t", this.TableName, this.KeyName, this.IsPrimaryKey ? "PRIMARY KEY" : "UNIQUE", this.KeyTypeDesc.ToUpper());
            }
            else
            {
                script += string.Format("CREATE {0} INDEX [{1}] ON [{2}]\r\n(\r\n\t", this.KeyTypeDesc.ToUpper(), this.KeyName, this.TableName);
            }

            script += string.Join(",\r\n\t", this.KeyColumns.Where(x => !x.IsIncludeColumn).Select(x => string.Format("[{0}] {1}", x.ColumnName, x.IsDecending ? "DESC" : "ASC")));

            if (this.KeyColumns.Where(x => x.IsIncludeColumn).Count() > 0)
            {
                script += "\r\n)\r\nINCLUDE\r\n(\r\n\t";
                script += string.Join(",\r\n\t", this.KeyColumns.Where(x => x.IsIncludeColumn).Select(x => string.Format("[{0}] {1}", x.ColumnName, x.IsDecending ? "DESC" : "ASC")));
            }

            script += "\r\n)\r\nGO\r\n\r\n";

            return script;

            #region OldCode

            //string script = "CONSTRAINT [" + KeyName + "] ";

            //if (IsPrimaryKey)
            //    script += "PRIMARY KEY";
            //if (IsUniqueKey)
            //    script += "UNIQUE";

            ////switch (KeyType)
            ////{
            ////    case SqlConstraintKeyType.PrimaryKey:
            ////        script += "PRIMARY KEY";
            ////        break;
            ////    case SqlConstraintKeyType.UniqueKey:
            ////        script += "UNIQUE";
            ////        break;
            ////    //case SqlConstraintKeyType.Index:
            ////    //    break;
            ////}

            ////script += " " + (IsClustred ? "CLUSTERED" : "NONCLUSTERED");

            //script += " " + KeyTypeDesc;

            //script += " (";

            //script += string.Join(", ", Columns.Select(x => "[" + x.Item1 + "] " + (x.Item2 ? "DESC" : "ASC")));

            //script += ")";

            //return script;

            #endregion
        }

        public string GetCreateScript()
        {
            return this.GetScript();
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

        public object GetJsonObject()
        {
            return new
            {
                KeyName = this.KeyName,
                TableName = this.TableName,
                IsPrimaryKey = this.IsPrimaryKey,
                IsUniqueKey = this.IsUniqueKey,
                KeyTypeId = this.KeyTypeId,
                KeyTypeDesc = this.KeyTypeDesc,
                KeyColumns = this.KeyColumns.Select(x => new
                {
                    x.ColumnName,
                    x.IsDecending,
                    x.KeyOrdinal,
                    x.IsIncludeColumn
                })
            };
        }

        public string GetDropScript()
        {
            return string.Format(@"ALTER TABLE [{0}]
DROP CONSTRAINT [{1}]
GO

", this.TableName, this.KeyName);
        }
    }
}
