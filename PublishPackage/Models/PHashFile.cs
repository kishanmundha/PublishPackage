using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishPackage.Models
{
    class PHashFile : IDataCompare
    {
        public string FileName { get; set; }
        public string Hash { get; set; }
        public string KeyName
        {
            get
            {
                return this.FileName;
            }
        }

        public string MD5Hash
        {
            get
            {
                return this.Hash;
            }
        }

        public string GetScript()
        {
            throw new NotImplementedException();
        }
    }
}
