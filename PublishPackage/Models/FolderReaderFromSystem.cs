using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PublishPackage.Models
{
    class FolderReaderFromSystem : IFolderReader
    {
        public PFolder Get(string source)
        {
            return Get(source, source);
        }

        private PFolder Get(string source, string fixPath)
        {
            var directories = Directory.GetDirectories(source);
            var files = Directory.GetFiles(source);

            PFolder folder = new PFolder();

            DirectoryInfo sdi = new DirectoryInfo(source);
            folder.FolderPath = sdi.FullName;
            folder.FolderName = sdi.Name;
            folder.RelativePath = sdi.FullName.Replace(fixPath, "");

            foreach (var file in files)
            {
                PFile pFile = new PFile();
                FileInfo fi = new FileInfo(file);
                pFile.FileName = fi.Name;
                pFile.FilePath = fi.FullName;
                pFile.RelativePath = fi.FullName.Replace(fixPath, "");

                folder.Files.Add(pFile);
            }

            foreach (var fld in directories)
            {
                //PFolder pFolder = new PFolder();
                //DirectoryInfo di = new DirectoryInfo(fld);
                //pFolder.FolderName = di.Name;
                //pFolder.FolderPath = di.FullName;

                PFolder pFolder = Get(fld, fixPath);

                folder.Folders.Add(pFolder);
            }

            return folder;
        }
    }

    class FolderReaderFromJson : IFolderReader
    {
        public PFolder Get(string source)
        {
            var str = System.IO.File.ReadAllText(source);

            dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject(str);

            return Get(obj, (string)obj.FolderPath);
        }

        public PFolder Get(dynamic obj, string fixPath)
        {
            PFolder folder = new PFolder();
            folder.FolderPath = obj.FolderPath;
            folder.FolderName = obj.FolderName;
            folder.RelativePath = obj.RelativePath;

            foreach (dynamic file in obj.Files)
            {
                PHashFile pFile = new PHashFile();
                pFile.FileName = file.FileName;
                pFile.FilePath = file.FilePath;
                pFile.RelativePath = file.RelativePath;
                pFile.Hash = file.Hash;

                folder.Files.Add(pFile);
            }

            foreach (var fld in obj.Folders)
            {
                PFolder pFolder = Get(fld, fixPath);

                folder.Folders.Add(pFolder);
            }

            return folder;
        }
    }
}
