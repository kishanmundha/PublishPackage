using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishPackage.Models
{
    class PHashFile : IDataCompare, IPFile
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
                return this.Hash;
            }
        }

        public string RelativePath { get; set; }

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

        public string GetScript()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return this.FileName;
        }
    }
}
