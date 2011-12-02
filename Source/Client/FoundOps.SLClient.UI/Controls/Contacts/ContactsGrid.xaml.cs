using System.ComponentModel;
using FoundOps.SLClient.Data.Tools;
using FoundOps.SLClient.UI.ViewModels;

namespace FoundOps.SLClient.UI.Controls.Contacts
{
    public partial class ContactsGrid : INotifyPropertyChanged
    {
        public bool IsMainGrid { get; set; }
        public ContactsGrid()
        {
            InitializeComponent();

            this.DependentWhenVisible(ContactsVM);
        }

        public ContactsVM ContactsVM
        {
            get { return (ContactsVM)this.DataContext; }
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
