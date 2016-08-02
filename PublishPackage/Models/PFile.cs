using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishPackage.Models
{
    class PFile : IDataCompare, IPFile
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
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
                if(this.Hash == null)
                {
                    this.Hash = Helper.GetMD5Hash(this.GetScript());
                }

                return this.Hash;
            }
        }

        public string GetScript()
        {
            return File.ReadAllText(this.FilePath);
        }
    }
}
