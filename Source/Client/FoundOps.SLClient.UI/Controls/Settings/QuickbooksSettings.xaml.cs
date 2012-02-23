using System;
using System.Windows;
using FoundOps.SLClient.UI.ViewModels;
using FoundOps.Common.Tools.ExtensionMethods;

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

            var uri = new Uri(string.Format(@"{0}/Quickbooks/GetAuthorization?roleId={1}", UriExtensions.ThisRootUrl, BusinessAccountSettingsVM.ContextManager.RoleId));
            RadHtmlPlaceholder1.SourceUrl = uri;
        }

    }
}
