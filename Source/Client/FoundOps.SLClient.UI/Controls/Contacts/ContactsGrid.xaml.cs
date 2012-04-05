using FoundOps.SLClient.Data.Tools;
using FoundOps.SLClient.UI.Tools;

namespace FoundOps.SLClient.UI.Controls.Contacts
{
    /// <summary>
    /// UI for displaying the proper Contacts depending on the current context.
    /// </summary>
    public partial class ContactsGrid
    {
        public bool IsMainGrid { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContactsGrid"/> class.
        /// </summary>
        public ContactsGrid()
        {
            InitializeComponent();

            this.DependentWhenVisible(VM.Contacts);
        }
    }
}
