﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishPackage.Models
{
    public class SqlDefaultConstraint : IDataCompare
    {
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string KeyName { get; set; }
        public string Definition { get; set; }

        public virtual string GetCreateScript()
        {
            return this.GetAddScript();
        }

        public virtual string GetAddScript()
        {
            return "ALTER TABLE [" + TableName + "] ADD CONSTRAINT [" + KeyName + "] DEFAULT " + Definition + " FOR [" + ColumnName + "]\r\nGO\r\n\r\n";
        }

        public virtual string GetRenameScript(string OldKeyName)
        {
            return string.Format("EXEC sp_rename N'{0}', N'{1}', N'OBJECT'", OldKeyName, KeyName);
        }

        public virtual string GetDropScript()
        {
            return "ALTER TABLE [" + TableName + "] DROP CONSTRAINT [" + KeyName + "]\r\nGO\r\n\r\n";
        }


        public string MD5Hash
        {
            get { return Helper.GetMD5Hash(this.GetScript()); }
        }

        public string GetScript()
        {
            return this.GetAddScript();
        }

        public object GetJsonObject()
        {
            return new
            {
                TableName = this.TableName,
                ColumnName = this.ColumnName,
                KeyName = this.KeyName,
                Definition = this.Definition
            };
        }
    }
}
