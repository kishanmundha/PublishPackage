using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishPackage.Models
{
    public class SqlCheckConstraint : IDataCompare
    {
        public string ConstraintName { get; set; }
        public string CheckClause { get; set; }

        public virtual string GetScript(string TableName)
        {
            return string.Format(@"ALTER TABLE [{0}] WITH CHECK ADD CONSTRAINT [{1}] CHECK ({2})
GO

ALTER TABLE [{0}] CHECK CONSTRAINT [{1}]
GO

", TableName, ConstraintName, CheckClause);
        }

        public string KeyName
        {
            get { return this.ConstraintName; }
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
