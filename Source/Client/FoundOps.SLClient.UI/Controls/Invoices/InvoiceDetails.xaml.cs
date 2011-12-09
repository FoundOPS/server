using FoundOps.SLClient.UI.Tools;
using FoundOps.SLClient.Data.Tools;

namespace FoundOps.SLClient.UI.Controls.Invoices
{
    /// <summary>
    /// UI for viewing and editing an Invoice's details.
    /// </summary>
    public partial class InvoiceDetails
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceDetails"/> class.
        /// </summary>
        public InvoiceDetails()
        {
            InitializeComponent();

            this.DependentWhenVisible(VM.Locations);
        }
    }
}
