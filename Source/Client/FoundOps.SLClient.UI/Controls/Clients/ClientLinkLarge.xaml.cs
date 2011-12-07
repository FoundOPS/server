using System.Windows;
using FoundOps.SLClient.Data.Tools;
using FoundOps.SLClient.UI.ViewModels;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.SLClient.UI.Controls.Clients
{
    /// <summary>
    /// The UI for selecting a single Client from the loaded Clients.
    /// </summary>
    public partial class ClientLinkLarge
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientLinkLarge"/> class.
        /// </summary>
        public ClientLinkLarge()
        {
            InitializeComponent();

            this.DependentWhenVisible(ClientsVM);
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
