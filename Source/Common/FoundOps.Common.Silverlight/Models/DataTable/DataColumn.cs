using System;

namespace FoundOps.Common.Silverlight.Models.DataTable
{
    public class DataColumn<T>
    {
        public DataColumn()
        {
            this.DataType = typeof(T);
        }

        public Type DataType { get; set; }
        public string ColumnName { get; set; }
    }
}
