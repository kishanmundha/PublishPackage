using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishPackage.Models
{
    class PFolderCompareResult
    {
        public List<DataCompareResult<PFolder>> Folders { get; set; }
        public List<DataCompareResult<IPFile>> Files { get; set; }

        public PFolderCompareResult()
        {
            this.Folders = new List<DataCompareResult<PFolder>>();
            this.Files = new List<DataCompareResult<IPFile>>();
        }

        private List<string> GetFolderNewScript(PFolder folder)
        {
            List<string> scriptList = new List<string>();
            scriptList.Add("$NEW     | FOLDER | " + folder.RelativePath);

            foreach(var file in folder.Files)
            {
                scriptList.Add("$NEW     | FILE   | " + (file as IPFile).RelativePath);
            }

            foreach(var fld in folder.Folders)
            {
                scriptList.AddRange(GetFolderNewScript(fld));
            }

            return scriptList;
        }

        public string GetScript()
        {
            List<string> scriptList = new List<string>();
            foreach (var file in this.Files)
            {
                switch(file.Status)
                {
                    case DataCompareStatus.New:
                        scriptList.Add("$NEW     | FILE   | " + file.NewData.RelativePath);
                        break;
                    case DataCompareStatus.Modified:
                        // compare sub directory
                        scriptList.Add("$REPLACE | FILE   | " + file.NewData.RelativePath);
                        break;
                    case DataCompareStatus.Renamed:
                        scriptList.Add("$RENAME  | FILE   | " + file.NewData.RelativePath + " | " + file.OldData.RelativePath);
                        break;
                    case DataCompareStatus.Deleted:
                        scriptList.Add("$DELETE  | FILE   | " + file.OldData.RelativePath);
                        break;
                }
            }

            foreach (var folder in this.Folders)
            {
                switch (folder.Status)
                {
                    case DataCompareStatus.New:
                        scriptList.AddRange(GetFolderNewScript(folder.NewData));
                        //scriptList.Add("$NEW     | FOLDER | " + folder.NewData.RelativePath);
                        break;
                    case DataCompareStatus.Modified:
                        // compare sub directory
                        var folderCompareResult = PFolderCompareResult.Compare(folder.NewData, folder.OldData);
                        scriptList.Add(folderCompareResult.GetScript());
                        break;
                    case DataCompareStatus.Renamed:
                        scriptList.Add("$RENAME  | FOLDER | " + folder.NewData.RelativePath);
                        break;
                    case DataCompareStatus.Deleted:
                        scriptList.Add("$DELETE  | FOLDER | " + folder.OldData.RelativePath);
                        break;
                }
            }

            return string.Join("\r\n", scriptList);
        }

        private List<string> GetFolderNewFileList(PFolder folder)
        {
            List<string> list = new List<string>();
            foreach (var file in folder.Files)
            {
                list.Add((file as IPFile).RelativePath);
            }

            foreach (var fld in folder.Folders)
            {
                list.AddRange(GetFolderNewFileList(fld));
            }

            return list;
        }

        public List<string> GetFileList()
        {
            return GetFileList(this);
        }

        public List<string> GetFileList(PFolderCompareResult data)
        {
            List<string> list = new List<string>();

            foreach (var file in data.Files)
            {
                switch (file.Status)
                {
                    case DataCompareStatus.New:
                        list.Add(file.NewData.RelativePath);
                        break;
                    case DataCompareStatus.Modified:
                        list.Add(file.NewData.RelativePath);
                        break;
                }
            }

            foreach (var folder in this.Folders)
            {
                switch (folder.Status)
                {
                    case DataCompareStatus.New:
                        list.AddRange(GetFolderNewFileList(folder.NewData));
                        break;
                    case DataCompareStatus.Modified:
                        // compare sub directory
                        var folderCompareResult = PFolderCompareResult.Compare(folder.OldData, folder.NewData);
                        list.AddRange(folderCompareResult.GetFileList());
                        break;
                }
            }

            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="folder1">Old Folder</param>
        /// <param name="folder2">New Folder</param>
        /// <returns></returns>
        public static PFolderCompareResult Compare(PFolder folder1, PFolder folder2)
        {
            var result = new PFolderCompareResult();

            if (folder1 == null)
                folder1 = new PFolder();

            if (folder2 == null)
                folder2 = new PFolder();

            result.Folders.AddRange(Helper.GetCompareResult<PFolder>(folder1.Folders.Cast<IDataCompare>().ToList(), folder2.Folders.Cast<IDataCompare>().ToList()));
            result.Files.AddRange(Helper.GetCompareResult<IPFile>(folder1.Files.Cast<IDataCompare>().ToList(), folder2.Files.Cast<IDataCompare>().ToList()));

            return result;
        }
    }
}
