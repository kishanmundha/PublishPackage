using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishPackage.Models
{
    public interface IPFile : IDataCompare
    {
        string FileName { get; set; }
        string FilePath { get; set; }
        string RelativePath { get; set; }
    }
}
