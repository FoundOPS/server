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
