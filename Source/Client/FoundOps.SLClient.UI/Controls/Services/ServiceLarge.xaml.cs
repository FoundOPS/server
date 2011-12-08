using System.ComponentModel;

namespace FoundOps.Framework.Views.Controls.Services
{
    public partial class ServiceLarge
    {
        public ServiceLarge()
        {
            InitializeComponent();

#if DEBUG
            if (DesignerProperties.IsInDesignTool)
                return;
#endif
        }
    }
}
