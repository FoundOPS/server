﻿using System;
using Microsoft.WindowsAzure.StorageClient;

namespace FoundOps.Core.Models.Azure
{
    public class InvoiceTableDataModel : TableServiceEntity
    {
        public InvoiceTableDataModel(string partitionKey, string rowKey)
            : base(partitionKey, rowKey)
        {
        }

        public InvoiceTableDataModel()
            : this(Guid.NewGuid().ToString(), String.Empty)
        {
        }

        public Guid InvoiceId { get; set; }
        public string ChangeType { get; set; }
    }
}
