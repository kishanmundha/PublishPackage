using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishPackage.Models
{
    public class SqlView : IDataCompare
    {
        public string ViewName { get; set; }
        public string ViewDefinition { get; set; }

        public string KeyName
        {
            get { return this.ViewName; }
        }

        public string MD5Hash
        {
            get { return Helper.GetMD5Hash(this.ViewDefinition); }
        }

        public string GetScript()
        {
            return this.ViewDefinition;
        }

        public string GetCreateScript()
        {
            return this.ViewDefinition;
        }

        public string GetAlterScript()
        {
            throw new NotImplementedException();
        }

        public string GetDropScript()
        {
            return "DROP VIEW " + this.ViewName + "\r\nGO\r\n\r\n";
        }

        public object GetJsonObject()
        {
            return new
            {
                this.ViewName,
                this.ViewDefinition
            };
        }
    }
}
