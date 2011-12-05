using System.Windows;
using System.Windows.Data;
using FoundOps.SLClient.UI.ViewModels;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.Tools;
using Telerik.Windows.Controls;

namespace FoundOps.SLClient.UI.Controls.Clients
{
    public partial class ClientLinkLarge
    {
        public ClientLinkLarge()
        {
            InitializeComponent();

            if (!ClientsVM.IsLoading)
                SetupTwoWayBinding();

            ClientsVM.PropertyChanged += (sender, e) =>
            {
                //After the clients are loaded, setup two way binding
                if (e.PropertyName == "IsLoading")
                    if (ClientsVM.IsLoading)
                        ClearTwoWayBinding();
                    else
                        SetupTwoWayBinding();
            };

            this.DependentWhenVisible(ClientsVM);
        }

        private void ClearTwoWayBinding()
        {
            ClientsRadComboBox.SetBinding(Selector.SelectedValueProperty, new Binding("SelectedClient") { Source = this });
        }
        private void SetupTwoWayBinding()
        {
            ClientsRadComboBox.SetBinding(Selector.SelectedValueProperty, new Binding("SelectedClient") { Source = this, Mode = BindingMode.TwoWay });
        }

        public ClientsVM ClientsVM
        {
            get { return (ClientsVM)this.DataContext; }
        }

        #region SelectedClient Dependency Property

        /// <summary>
        /// SelectedClient
        /// </summary>
        public Client SelectedClient
        {
            get { return (Client)GetValue(SelectedClientProperty); }
            set { SetValue(SelectedClientProperty, value); }
        }

        /// <summary>
        /// SelectedClient Dependency Property.
        /// </summary>
        public static readonly DependencyProperty SelectedClientProperty =
            DependencyProperty.Register(
                "SelectedClient",
                typeof(Client),
                typeof(ClientLinkLarge),
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
                typeof(ClientLinkLarge),
                new PropertyMetadata(new PropertyChangedCallback(IsReadOnlyChanged)));

        private static void IsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ClientLinkLarge c = d as ClientLinkLarge;
            if (c != null)
            {
                c.ClientsRadComboBox.IsEnabled = !((bool)e.NewValue);
            }
        }

        #endregion

    }
}
