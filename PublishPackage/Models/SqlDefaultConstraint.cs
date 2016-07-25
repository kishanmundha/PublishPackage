using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishPackage.Models
{
    public class SqlDefaultConstraint : IDataCompare
    {
        public string ColumnName { get; set; }
        public string KeyName { get; set; }
        public string Defination { get; set; }

        public virtual string GetAddScript(string TableName)
        {
            return "ALTER TABLE [" + TableName + "] ADD CONSTRAINT [" + KeyName + "] DEFAULT " + Defination + " FOR [" + ColumnName + "]\r\nGO\r\n\r\n";
        }

        public virtual string GetRenameScript(string OldKeyName)
        {
            return string.Format("EXEC sp_rename N'{0}', N'{1}', N'OBJECT'", OldKeyName, KeyName);
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
