using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace FoundOps.Core.Models.Azure
{
    public class InvoiceTableDataServiceContext : TableServiceContext
    {
        public InvoiceTableDataServiceContext(string baseAddress, StorageCredentials credentials)
            : base(baseAddress, credentials)
        {
            //If you are familiar with Linq, then you expect the FirstOrDefault to return null if no record can be found. 
            //However, with WCF Data Services and Azure Table Storage if your query uses both RowKey and PartitionKey 
            //it will return a DataServiceQueryException with a 404 Resource not found as the InnerException.
            //To solve this, you must set the IgnoreResourceNotFoundException = true
            IgnoreResourceNotFoundException = true;
        }

        public const string InvoiceTableName = "InvoiceTable";

        public IQueryable<InvoiceTableDataModel> InvoiceTable
        {
            get
            {
                return this.CreateQuery<InvoiceTableDataModel>(InvoiceTableName);
            }
        }
    }
}
