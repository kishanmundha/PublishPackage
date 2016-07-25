﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishPackage.Models
{
    public class SqlForeignKey : IDataCompare
    {
        public string KeyName { get; set; }
        public string KeyColumnName { get; set; }
        public string ForeignColumnName { get; set; }
        public string ForeignTableName { get; set; }

        public virtual string GetScript(string TableName)
        {
            string script = "ALTER TABLE [" + TableName + "] WITH CHECK ADD CONSTRAINT [" + KeyName + "] FOREIGN KEY ([" + KeyColumnName + "]) REFERENCES [" + ForeignTableName + "] ([" + ForeignColumnName + "])\r\nGO\r\n\r\n";

            script += "ALTER TABLE [" + TableName + "] CHECK CONSTRAINT [" + this.KeyName + "]\r\nGO\r\n\r\n";

            return script;
        }

        public virtual string GetDropScript(string TableName)
        {
            return "ALTER TABLE [" + TableName + "] DROP CONSTRAINT [" + KeyName + "]\r\nGO\r\n\r\n";
        }


        public string MD5Hash
        {
            get { throw new NotImplementedException(); }
        }

        public string GetScript()
        {
            throw new NotImplementedException();
        }
    }
}
