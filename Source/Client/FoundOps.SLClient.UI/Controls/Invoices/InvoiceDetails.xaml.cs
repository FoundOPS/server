using System.ComponentModel;
using System.Windows.Data;
using FoundOps.SLClient.UI.ViewModels;
using Telerik.Windows.Controls;
using FoundOps.SLClient.Data.Tools;

namespace FoundOps.SLClient.UI.Controls.Invoices
{
    public partial class InvoiceDetails
    {
        public InvoiceDetails()
        {
            InitializeComponent();

#if DEBUG 
            if(DesignerProperties.IsInDesignTool) return;
#endif

            LocationsVM.PropertyChanged += (sender, e) =>
            {
                //After the locations are loaded, setup two way binding
                if (e.PropertyName == "IsLoading")
                    if (LocationsVM.IsLoading)
                        ClearTwoWayBinding();
                    else
                        SetupTwoWayBinding();
            };

            this.DependentWhenVisible(LocationsVM);
        }

        private void ClearTwoWayBinding()
        {
            LocationsComboBox.SetBinding(Selector.SelectedValueProperty, new Binding("BillToLocation"));
        }

        private void SetupTwoWayBinding()
        {
            LocationsComboBox.SetBinding(Selector.SelectedValueProperty, new Binding("BillToLocation") { Mode = BindingMode.TwoWay });
        }

        public LocationsVM LocationsVM
        {
            get { return (LocationsVM)LocationsVMHolderStackPanel.DataContext; }
        }
    }
}
