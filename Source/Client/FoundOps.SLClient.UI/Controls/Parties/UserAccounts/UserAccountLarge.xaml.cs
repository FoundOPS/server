using System.ComponentModel;
using FoundOps.SLClient.UI.ViewModels;

namespace FoundOps.SLClient.UI.Controls.Parties.UserAccounts
{
    /// <summary>
    /// UI for editing a UserAccount.
    /// </summary>
    public partial class UserAccountLarge
    {
        /// <summary>
        /// Gets the EmployeesVM.
        /// </summary>
        public EmployeesVM EmployeesVM { get { return (EmployeesVM)EmployeesVMHolder.DataContext; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAccountLarge"/> class.
        /// </summary>
        public UserAccountLarge()
        {
            InitializeComponent();

#if DEBUG
            if (DesignerProperties.IsInDesignTool) return;
#endif
            EmployeesVM.DependentWhenVisible(this);
        }
    }
}
