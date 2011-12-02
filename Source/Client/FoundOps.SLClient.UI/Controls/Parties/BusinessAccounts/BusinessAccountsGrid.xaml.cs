using System.ComponentModel;
using FoundOps.SLClient.UI.ViewModels;

namespace FoundOps.SLClient.UI.Controls.Parties.BusinessAccounts
{
    /// <summary>
    /// Displays a list of the BusinessAccounts
    /// </summary>
    public partial class BusinessAccountsGrid
    {
        public bool IsMainGrid { get; set; }

        /// <summary>
        /// Gets the business accounts VM.
        /// </summary>
        public BusinessAccountsVM BusinessAccountsVM { get { return (BusinessAccountsVM) DataContext; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessAccountsGrid"/> class.
        /// </summary>
        public BusinessAccountsGrid()
        {
            InitializeComponent();

#if DEBUG
            if(DesignerProperties.IsInDesignTool)
                return;
#endif

            BusinessAccountsVM.DependentWhenVisible(this);
        }
    }
}
