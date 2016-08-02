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
}
