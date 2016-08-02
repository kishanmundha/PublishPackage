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

        public string GetScript()
        {
            List<string> scriptList = new List<string>();
            foreach (var file in this.Files)
            {
                switch(file.Status)
                {
                    case DataCompareStatus.New:
                        scriptList.Add("$NEW | FILE | " + file.NewData.FilePath);
                        break;
                    case DataCompareStatus.Modified:
                        // compare sub directory
                        scriptList.Add("$NEW | REPLACE | " + file.NewData.FilePath);
                        break;
                    case DataCompareStatus.Renamed:
                        scriptList.Add("$RENAME | FILE | " + file.NewData.FilePath + " | " + file.OldData.FilePath);
                        break;
                    case DataCompareStatus.Deleted:
                        scriptList.Add("$DELETE | FILE | " + file.OldData.FilePath);
                        break;
                }
            }

            foreach (var folder in this.Folders)
            {
                switch (folder.Status)
                {
                    case DataCompareStatus.New:
                        scriptList.Add("$NEW | FOLDER | " + folder.NewData.FolderPath);
                        break;
                    case DataCompareStatus.Modified:
                        // compare sub directory
                        var folderCompareResult = PFolderCompareResult.Compare(folder.NewData, folder.OldData);
                        scriptList.Add(folderCompareResult.GetScript());
                        break;
                    case DataCompareStatus.Renamed:
                        scriptList.Add("$RENAME | FOLDER | " + folder.NewData.FolderPath);
                        break;
                    case DataCompareStatus.Deleted:
                        scriptList.Add("$DELETE | FOLDER | " + folder.OldData.FolderPath);
                        break;
                }
            }

            return string.Join("\r\n", scriptList);
        }

        public static PFolderCompareResult Compare(PFolder folder1, PFolder folder2)
        {
            var result = new PFolderCompareResult();

            result.Folders.AddRange(Helper.GetCompareResult<PFolder>(folder1.Folders.Cast<IDataCompare>().ToList(), folder2.Folders.Cast<IDataCompare>().ToList()));
            result.Files.AddRange(Helper.GetCompareResult<IPFile>(folder1.Files.Cast<IDataCompare>().ToList(), folder2.Files.Cast<IDataCompare>().ToList()));

            return result;
        }
    }
}
