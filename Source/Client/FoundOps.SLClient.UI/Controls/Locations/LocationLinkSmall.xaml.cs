using System.Windows;
using System.Collections.Generic;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.Framework.Views.Controls.Locations
{
    public partial class LocationLinkSmall
    {
        public LocationLinkSmall()
        {
            InitializeComponent();
         }

        #region Locations Dependency Property

        /// <summary>
        /// Locations
        /// </summary>
        public IEnumerable<Location> Locations
        {
            get { return (IEnumerable<Location>) GetValue(LocationsProperty); }
            set { SetValue(LocationsProperty, value); }
        }

        /// <summary>
        /// Locations Dependency Property.
        /// </summary>
        public static readonly DependencyProperty LocationsProperty =
            DependencyProperty.Register(
                "Locations",
                typeof (IEnumerable<Location>),
                typeof (LocationLinkSmall),
                new PropertyMetadata(null));

        #endregion
    }
}
