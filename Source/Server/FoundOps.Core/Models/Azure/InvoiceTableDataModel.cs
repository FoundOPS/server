using System;
using Microsoft.WindowsAzure.StorageClient;

namespace FoundOps.Core.Models.Azure
{
    public class InvoiceTableDataModel : TableServiceEntity
    {
        public InvoiceTableDataModel(Guid id, string changeType)
            : base(id.ToString(), changeType) { }

        public InvoiceTableDataModel(Guid id)
            : this(id, String.Empty) { }

        public InvoiceTableDataModel()
            : this(Guid.NewGuid(), String.Empty) { }

        public Guid InvoiceId { get { return new Guid(PartitionKey); } }
        public string ChangeType { get { return RowKey; } set { RowKey = value; } }
    }
}
