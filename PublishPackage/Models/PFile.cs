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
        public string RelativePath { get; set; }
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

        public override string ToString()
        {
            return this.FileName;
        }

        public object GetJsonObject()
        {
            return new
            {
                FileName = this.FileName,
                FilePath = this.FilePath,
                RelativePath = this.RelativePath,
                Hash = this.Hash
            };
        }

        public string GetJsonString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this.GetJsonObject());
        }

    }
}
