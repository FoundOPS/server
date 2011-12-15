using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using FoundOps.SLClient.UI.Tools;
using FoundOps.SLClient.Data.Tools;
using FoundOps.SLClient.UI.ViewModels;

namespace FoundOps.SLClient.UI.Controls.Contacts
{
    public partial class ClientTitleGrid : UserControl, INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientTitleGrid"/> class.
        /// </summary>
        public ClientTitleGrid()
        {
            InitializeComponent();

            this.DependentWhenVisible(ClientTitlesVM);
            this.DependentWhenVisible(ClientsVM);

            AddDeleteClientTitle.CreateNewItem = () => ClientTitlesVM.StartCreationOfClientTitle();
            AddDeleteClientTitle.RemoveCurrentItem = (item) => ClientTitlesVM.DeleteClientTitleInCreation();
        }

        /// <summary>
        /// Gets the ClientsVM.
        /// </summary>
        public ClientsVM ClientsVM{get{return VM.Clients;}}

        /// <summary>
        /// Gets the ClientTitlesVM.
        /// </summary>
        public ClientTitlesVM ClientTitlesVM{get{return VM.ClientTitles;}}


        #region ParentContextVM Dependency Property

        /// <summary>
        /// ParentContextVM
        /// </summary>
        public IAddDeleteSelectedClient ParentContextVM
        {
            get { return (IAddDeleteSelectedClient)GetValue(ParentContextVMProperty); }
            set { SetValue(ParentContextVMProperty, value); }
        }

        /// <summary>
        /// ParentContextVM Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ParentContextVMProperty =
            DependencyProperty.Register(
                "ParentContextVM",
                typeof(IAddDeleteSelectedClient),
                typeof(ClientTitleGrid),
                new PropertyMetadata(null));

        #endregion

        #region Implementation of INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private void AddClientTitleButtonClicked(object sender, RoutedEventArgs e)
        {
            AddDeleteClientTitle.AddedCurrentItem();
        }
    }
}