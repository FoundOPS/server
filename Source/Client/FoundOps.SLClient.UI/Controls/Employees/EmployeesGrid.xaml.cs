using System.ComponentModel;
using FoundOps.SLClient.Data.Tools;
using FoundOps.SLClient.UI.ViewModels;

namespace FoundOps.SLClient.UI.Controls.Employees
{
    public partial class EmployeesGrid : INotifyPropertyChanged
    {
        public bool IsMainGrid { get; set; }
        public EmployeesGrid()
        {
            InitializeComponent();

            this.DependentWhenVisible(EmployeesVM);
        }

        public EmployeesVM EmployeesVM
        {
            get { return (EmployeesVM)this.DataContext; }
        }

        #region Implementation of INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
