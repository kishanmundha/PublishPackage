using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishPackage.Models
{
    interface IFolderReader
    {
        PFolder Get(string source);
    }
}
