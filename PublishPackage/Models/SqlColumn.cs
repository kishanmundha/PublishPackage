using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishPackage.Models
{
    public class SqlColumn : IDataCompare
    {
        private string _md5Hash { get; set; }

        private string _ColumnName;
        private string _DataType;

        public string ColumnName
        {
            get { return this._ColumnName; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("ColumnName");

                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Value cannot be empty.", "ColumnName");

                this._ColumnName = value;
            }
        }
        public string DataType
        {
            get
            {
                return this._DataType;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("DataType");

                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Value cannot be empty.", "DataType");

                this._DataType = value.ToLower();
            }
        }
        public string KeyName
        {
            get
            {
                return this.ColumnName;
            }
        }

        public string MD5Hash
        {
            get
            {
                if (this._md5Hash == null)
                    this._md5Hash = Helper.GetMD5Hash(this.GetScript());

                return this._md5Hash;
            }
        }

        public int? Length { get; set; }
        public int? Prec { get; set; }
        public int? Scale { get; set; }
        public int? DateTimePrec { get; set; }
        public bool IsNullable { get; set; }
        public int OridinalPosition { get; set; }

        public SqlIdentityColumn Identity { get; set; }

        public override string ToString()
        {
            return this.ColumnName;
        }
        public virtual string GetScript()
        {
            string script = "[" + ColumnName + "] [" + DataType + "]";

            string[] LengthDataTypes = new string[] { "char", "varchar", "nchar", "nvarchar", "binary", "varbinary" };

            if (LengthDataTypes.Contains(DataType))
                script += "(" + (Length != -1 ? Length.ToString() : "max") + ")";
            else if (DataType == "decimal" || DataType == "numeric")
                script += "(" + Prec + "," + Scale + ")";
            else if (DataType == "datetime2" || DataType == "datetimeoffset" || DataType == "time")
                script += "(" + DateTimePrec + ")";

            if (this.Identity != null)
                script += " IDENTITY(" + this.Identity.SeedValue + "," + this.Identity.IncrementValue + ")";

            if (!IsNullable)
                script += " NOT";

            script += " NULL";

            return script;
        }

        public virtual string GetAddScript(string tableName)
        {
            return string.Format(@"ALTER TABLE [{0}]
ADD {1}
GO

", tableName, this.GetScript());
        }

        public virtual string GetDropScript(string tableName)
        {
            return string.Format(@"ALTER TABLE [{0}]
DROP COLUMN [{1}]
GO

", tableName, ColumnName);
        }

        public virtual string GetRenameScript(string tableName, string newColumnName)
        {
            throw new NotImplementedException();
        }

        public virtual string GetAlterScript(string tableName)
        {
            return string.Format(@"ALTER TABLE [{0}]
ALTER COLUMN {1}
GO

", tableName, this.GetScript());
        }

        public object GetJsonObject()
        {
            return new
            {
                ColumnName = this.ColumnName,
                DataType = this.DataType,
                Length = this.Length,
                Prec = this.Prec,
                Scale = this.Scale,
                DateTimePrec = this.DateTimePrec,
                IsNullable = this.IsNullable,
                OridinalPosition = this.OridinalPosition,
                Identity = this.Identity == null ? null : this.Identity.GetJsonObject()
            };
        }
    }
}
