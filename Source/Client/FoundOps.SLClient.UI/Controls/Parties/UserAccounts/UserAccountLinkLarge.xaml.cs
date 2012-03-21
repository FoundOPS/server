using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.Tools;
using FoundOps.SLClient.UI.Tools;
using System.Windows;
using System.Windows.Controls;

namespace FoundOps.SLClient.UI.Controls.Parties.UserAccounts
{
    /// <summary>
    /// Displays the linked UserAccount.
    /// </summary>
    public partial class UserAccountLinkLarge
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserAccountLinkLarge"/> class.
        /// </summary>
        public UserAccountLinkLarge()
        {
            InitializeComponent();

            this.DependentWhenVisible(VM.UserAccounts);
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

        private void UserAccountsAutoCompleteBox_OnPopulating(object sender, PopulatingEventArgs e)
        {
            // Allow us to wait for the response
            e.Cancel = true;
            VM.UserAccounts.ManuallyUpdateSuggestions(UserAccountsAutoCompleteBox.Text, UserAccountsAutoCompleteBox);
        }
    }
}
