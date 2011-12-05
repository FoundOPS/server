using System;
using FoundOps.Common.Silverlight.Blocks;

namespace FoundOps.SLClient.Navigator.Panes.Settings
{
    /// <summary>
    /// A page to setup User Account, Business Account, and QuickBooks Settings.
    /// </summary>
    [ExportPage("Settings")]
    public partial class SettingsPane
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsPane"/> class.
        /// </summary>
        public SettingsPane()
        {
            InitializeComponent();
        }
    }
}