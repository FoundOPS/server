using System;
using System.Windows;
using FoundOps.SLClient.UI.ViewModels;

namespace FoundOps.SLClient.UI.Controls.Settings
{
    /// <summary>
    /// View of QuickBooks Settings
    /// </summary>
    public partial class QuickbooksSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuickbooksSettings"/> class.
        /// </summary>
        public QuickbooksSettings()
        {
            InitializeComponent();
            Loaded += PageLoaded;
        }

        /// <summary>
        /// Gets the business account settings VM.
        /// </summary>
        public BusinessAccountSettingsVM BusinessAccountSettingsVM { get { return this.LayoutRoot.DataContext as BusinessAccountSettingsVM; } }

        void PageLoaded(object sender, RoutedEventArgs e)
        {
            if (BusinessAccountSettingsVM == null) return;

#if DEBUG
            var uri = new Uri(string.Concat("https://localhost:44300/Quickbooks/GetAuthorization", "?roleId=", BusinessAccountSettingsVM.ContextManager.RoleId));
            RadHtmlPlaceholder1.SourceUrl = uri;
#else
            var uri = new Uri(string.Concat("http://www.foundops.com/Quickbooks/GetAuthorization", "?roleId=", BusinessAccountSettingsVM.ContextManager.RoleId));
            RadHtmlPlaceholder1.SourceUrl = uri;
#endif
        }

    }
}
