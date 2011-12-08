using FoundOps.SLClient.UI.ViewModels;
using FoundOps.Common.Silverlight.UI.Controls.InfiniteAccordion;

namespace FoundOps.SLClient.UI.Controls.Services.RecurringServices
{
    public partial class RecurringServiceLarge
    {
        public RecurringServiceLarge()
        {
            InitializeComponent();
        }

        private void EditServiceTemplateButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            //Select the ServiceTemplate before switching to DetailsView to give the proper context
            ((ServiceTemplatesVM)this.ServiceTemplateStackPanel.DataContext).SelectedEntity = ((RecurringServicesVM)this.DataContext).SelectedEntity.ServiceTemplate;

            ((IProvideContext)this.ServiceTemplateStackPanel.DataContext).MoveToDetailsView.Execute(null);
        }
    }
}
