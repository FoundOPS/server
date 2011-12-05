using System.ComponentModel;
using System.Windows.Controls;

namespace FoundOps.Framework.Views.Controls.Locations
{
    public partial class LocationLarge : UserControl
    {
        public LocationLarge()
        {
            InitializeComponent();
#if DEBUG
            if (DesignerProperties.IsInDesignTool)
                return;
#endif
        }
    }
}