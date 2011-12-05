using FoundOps.SLClient.Data.Tools;
using FoundOps.SLClient.UI.ViewModels;

namespace FoundOps.SLClient.UI.Controls.Services.ServiceTemplate
{
    public partial class ServiceTemplatesLink
    {
        public ServiceTemplatesLink()
        {
            InitializeComponent();

            this.DependentWhenVisible(ServiceTemplatesVM);
        }

        public ServiceTemplatesVM ServiceTemplatesVM
        {
            get { return (ServiceTemplatesVM)this.DataContext; }
        }
    }
}
