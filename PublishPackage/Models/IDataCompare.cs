using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishPackage.Models
{
    public interface IDataCompare
    {
        string KeyName { get; }
        string MD5Hash { get; }
        string GetScript();
    }

    public enum DataCompareStatus
    {
        NoChange,
        New,
        Modified,
        Renamed,
        Deleted
    }

    public class DataCompareResult<T> where T : IDataCompare
    {
        public string KeyName
        {
            get
            {
                if (this.OldData == null && this.NewData == null)
                    throw new InvalidOperationException("Data compare result empty");

                //return (this.OldData ?? this.NewData).KeyName;

                return (this.OldData == null ? this.NewData : this.OldData).KeyName;
            }
        }

        public DataCompareStatus Status
        {
            get
            {
                if (this.OldData == null && this.NewData == null)
                    throw new InvalidOperationException("Data compare result empty");

                DataCompareStatus status = DataCompareStatus.NoChange;

                if (this.OldData == null)
                    status = DataCompareStatus.New;
                else if (this.NewData == null)
                    status = DataCompareStatus.Deleted;
                else if (this.OldData.MD5Hash != this.NewData.MD5Hash)
                    status = DataCompareStatus.Modified;

                return status;
            }
        }

        public T OldData { get; set; }
        public T NewData { get; set; }

        //private List<DataCompareResult> _items { get; set; }
        //public List<DataCompareResult> Items
        //{
        //    get
        //    {
        //        return null;
        //    }
        //}

        public DataCompareResult()
        {
            
        }
    }
}
