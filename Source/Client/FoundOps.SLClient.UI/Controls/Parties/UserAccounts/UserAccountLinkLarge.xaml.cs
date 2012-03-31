using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.Tools;
using FoundOps.SLClient.UI.Tools;
using System.Windows;

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
            UserAccountsAutoCompleteBox.HookupSearchSuggestions(VM.UserAccounts.ManuallyUpdateSuggestions);
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
