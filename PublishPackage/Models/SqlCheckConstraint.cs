using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishPackage.Models
{
    public class SqlCheckConstraint : IDataCompare
    {
        public string TableName { get; set; }
        public string ConstraintName { get; set; }
        public string CheckClause { get; set; }

        public virtual string GetCreateScript()
        {
            return GetScript();
        }

        public virtual string GetScript()
        {
            return string.Format(@"ALTER TABLE [{0}] WITH CHECK ADD CONSTRAINT [{1}] CHECK ({2})
GO

ALTER TABLE [{0}] CHECK CONSTRAINT [{1}]
GO

", this.TableName, ConstraintName, CheckClause);
        }

        public string KeyName
        {
            get { return this.ConstraintName; }
        }

        public string MD5Hash
        {
            get { throw new NotImplementedException(); }
        }

        public object GetJsonObject()
        {
            return new
            {
                this.TableName,
                this.KeyName,
                this.CheckClause
            };
        }

        public string GetDropScript()
        {
            throw new NotImplementedException();
        }
    }
}
