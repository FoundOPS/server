using System.Windows;
using System.Windows.Data;
using Telerik.Windows.Controls;
using FoundOps.SLClient.Data.Tools;
using FoundOps.SLClient.UI.ViewModels;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.SLClient.UI.Controls.Parties.UserAccounts
{
    /// <summary>
    /// Displays the linked UserAccount
    /// </summary>
    public partial class UserAccountLinkLarge
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserAccountLinkLarge"/> class.
        /// </summary>
        public UserAccountLinkLarge()
        {
            InitializeComponent();

            UserAccountsVM.PropertyChanged += (sender, e) =>
            {
                //After the User Accounts are loaded, setup two way binding
                if (e.PropertyName == "IsLoading" && !UserAccountsVM.IsLoading)
                {
                    UserAccountsRadComboBox.SetBinding(Selector.SelectedValueProperty, new Binding("SelectedUserAccount") { Source = this, Mode = BindingMode.TwoWay });
                }
            };

            this.DependentWhenVisible(UserAccountsVM);
        }

        /// <summary>
        /// Gets the user accounts VM.
        /// </summary>
        public UserAccountsVM UserAccountsVM
        {
            get { return (UserAccountsVM)DataContext; }
        }

        #region SelectedUserAccount Dependency Property

        /// <summary>
        /// SelectedUserAccount
        /// </summary>
        public UserAccount SelectedUserAccount
        {
            get { return (UserAccount) GetValue(SelectedUserAccountProperty); }
            set { SetValue(SelectedUserAccountProperty, value); }
        }

        /// <summary>
        /// SelectedUserAccount Dependency Property.
        /// </summary>
        public static readonly DependencyProperty SelectedUserAccountProperty =
            DependencyProperty.Register(
                "SelectedUserAccount",
                typeof (UserAccount),
                typeof (UserAccountLinkLarge),
                new PropertyMetadata(null));

        #endregion
    }
}
