using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishPackage.Models
{
    public class SqlIdentityColumn
    {
        public long SeedValue { get; set; }
        public long IncrementValue { get; set; }

        public object GetJsonObject()
        {
            return new
            {
                SeedValue = this.SeedValue,
                IncrementValue = this.IncrementValue
            };
        }
    }
}
