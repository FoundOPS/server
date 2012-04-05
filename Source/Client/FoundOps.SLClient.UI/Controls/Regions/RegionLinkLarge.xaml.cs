using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.Tools;
using FoundOps.SLClient.UI.Tools;
using System.Windows;

namespace FoundOps.SLClient.UI.Controls.Regions
{
    /// <summary>
    /// UI to select a single region from the loaded regions.
    /// </summary>
    public partial class RegionLinkLarge
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegionLinkLarge"/> class.
        /// </summary>
        public RegionLinkLarge()
        {
            InitializeComponent();

            this.DependentWhenVisible(VM.Regions);
            RegionsAutoCompleteBox.HookupSearchSuggestions(VM.Regions.ManuallyUpdateSuggestions);
        }

        #region SelectedRegion Dependency Property

        /// <summary>
        /// SelectedRegion
        /// </summary>
        public Region SelectedRegion
        {
            get { return (Region)GetValue(SelectedRegionProperty); }
            set { SetValue(SelectedRegionProperty, value); }
        }

        /// <summary>
        /// SelectedRegion Dependency Property.
        /// </summary>
        public static readonly DependencyProperty SelectedRegionProperty =
            DependencyProperty.Register(
                "SelectedRegion",
                typeof(Region),
                typeof(RegionLinkLarge),
                new PropertyMetadata(null));

        #endregion

        #region IsReadOnly Dependency Property

        /// <summary>
        /// IsReadOnly
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>
        /// IsReadOnly Dependency Property.
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(
                "IsReadOnly",
                typeof(bool),
                typeof(RegionLinkLarge),
                new PropertyMetadata(IsReadOnlyChanged));

        private static void IsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as RegionLinkLarge;
            if (c != null)
                c.RegionsAutoCompleteBox.IsEnabled = !((bool)e.NewValue);
        }

        #endregion
    }
}
