using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishPackage.Models
{
    class PFolder : IDataCompare
    {
        public string FolderName { get; set; }
        public string FolderPath { get; set; }
        public string RelativePath { get; set; }
        public PFolder()
        {
            this.Files = new List<IPFile>();
            this.Folders = new List<PFolder>();
        }

        public string KeyName
        {
            get
            {
                return this.FolderName;
            }
        }

        public string MD5Hash
        {
            get
            {
                return Helper.GetMD5Hash(string.Join("", this.Files.Select(x => x.MD5Hash)) + string.Join("", this.Folders.Select(x=>x.MD5Hash)));
            }
        }

        public string GetScript()
        {
            throw new NotImplementedException();
        }

        public List<IPFile> Files { get; set; }
        public List<PFolder> Folders { get; set; }

        public override string ToString()
        {
            return this.FolderName;
        }

        public object GetJsonObject()
        {
            return new
            {
                FolderName = this.FolderName,
                FolderPath = this.FolderPath,
                RelativePath = this.RelativePath,
                Files = this.Files.Select(x=>x.GetJsonObject()),
                Folders = this.Folders.Select(x=>x.GetJsonObject())
            };
        }

        public string GetJsonString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this.GetJsonObject());
        }
    }
}
