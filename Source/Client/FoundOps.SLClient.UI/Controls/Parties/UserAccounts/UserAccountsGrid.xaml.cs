using System;
using System.ComponentModel;
using FoundOps.SLClient.UI.ViewModels;
using Telerik.Windows;
using Telerik.Windows.Controls.GridView;

namespace FoundOps.SLClient.UI.Controls.Parties.UserAccounts
{
    /// <summary>
    /// Displays a list of the UserAccounts
    /// </summary>
    public partial class UserAccountsGrid
    {
        /// <summary>
        /// Gets the user accounts VM.
        /// </summary>
        public UserAccountsVM UserAccountsVM { get { return (UserAccountsVM)DataContext; } }

        /// <summary>
        /// Gets or sets if this is the main grid.
        /// </summary>
        public bool IsMainGrid { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAccountsGrid"/> class.
        /// </summary>
        public UserAccountsGrid()
        {
            InitializeComponent();

#if DEBUG
            if (DesignerProperties.IsInDesignTool)
                return;
#endif

            UserAccountsVM.DependentWhenVisible(this);

            UserAccountsRadGridView.AddHandler(GridViewCellBase.CellDoubleClickEvent, new EventHandler<RadRoutedEventArgs>(OnCellDoubleClick), true);
        }

        private void OnCellDoubleClick(object sender, RadRoutedEventArgs e)
        {
            //TODO: Should there be conditions?
            if (!IsMainGrid)
                UserAccountsVM.MoveToDetailsView.Execute(null);
        }
    }
}
