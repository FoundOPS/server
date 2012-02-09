using System;
using System.Windows;
using System.Windows.Browser;
using FoundOps.Common.Silverlight.UI.ViewModels;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.SLClient.UI.Controls.Parties.UserAccounts
{
    /// <summary>
    /// View/Edit User account settings.
    /// </summary>
    public partial class UserAccountEdit
    {
        #region Entity Dependency Property

        /// <summary>
        /// Entity
        /// </summary>
        public UserAccount Entity
        {
            get { return (UserAccount) GetValue(EntityProperty); }
            set { SetValue(EntityProperty, value); }
        }

        /// <summary>
        /// Entity Dependency Property.
        /// </summary>
        public static readonly DependencyProperty EntityProperty =
            DependencyProperty.Register(
                "Entity",
                typeof (UserAccount),
                typeof (UserAccountEdit),
                new PropertyMetadata(null));

        #endregion

        #region ISaveDiscardChangesCommands Dependency Property

        /// <summary>
        /// ISaveDiscardChangesCommands
        /// </summary>
        public ISaveDiscardChangesCommands ISaveDiscardChangesCommands
        {
            get { return (ISaveDiscardChangesCommands) GetValue(ISaveDiscardChangesCommandsProperty); }
            set { SetValue(ISaveDiscardChangesCommandsProperty, value); }
        }

        /// <summary>
        /// ISaveDiscardChangesCommands Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ISaveDiscardChangesCommandsProperty =
            DependencyProperty.Register(
                "ISaveDiscardChangesCommands",
                typeof (ISaveDiscardChangesCommands),
                typeof (UserAccountEdit),
                new PropertyMetadata(null));

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAccountEdit"/> class.
        /// </summary>
        public UserAccountEdit()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Hyperlinks the button click. Navigate to /Account/ChangePassword
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void HyperlinkButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
#if DEBUG
            HtmlPage.Window.Navigate(new Uri("https://localhost:44300/Account/ChangePassword"));
#else
            HtmlPage.Window.Navigate(new Uri("http://www.foundops.com/Account/ChangePassword"));
#endif
        }
    }
}
