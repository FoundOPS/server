using System.Windows;
using FoundOps.Framework.Views.Controls.Regions;
using FoundOps.SLClient.UI.ViewModels;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.SLClient.UI.Controls.Regions
{
    public partial class RegionLinkSmall
    {
        public RegionLinkSmall()
        {
            InitializeComponent();
         }

        public RegionsVM RegionsVM
        {
            get { return (RegionsVM)this.DataContext; }
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
                new PropertyMetadata(null));

        #endregion

    }
}
