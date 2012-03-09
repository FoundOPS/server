using System.ServiceModel.DomainServices.Client;
using System.Windows;
using System.Windows.Controls;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.Tools;
using FoundOps.SLClient.UI.Tools;
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

            this.DependentWhenVisible(VM.Clients);
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
                new PropertyMetadata(IsReadOnlyChanged));

        private static void IsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as ClientLinkLarge;
            if (c != null)
                c.ClientsAutoCompleteBox.IsEnabled = !((bool)e.NewValue);
        }

        #endregion

        private void ClientsAutoCompleteBox_OnPopulating(object sender, PopulatingEventArgs e)
        {
            // Allow us to wait for the response
            e.Cancel = true;

            UpdateSuggestions();
        }

        private LoadOperation<Client> _lastSuggestionQuery;
        /// <summary>
        /// Updates the client suggestions.
        /// </summary>
        private void UpdateSuggestions()
        {
            if (_lastSuggestionQuery != null && _lastSuggestionQuery.CanCancel)
                _lastSuggestionQuery.Cancel();

            _lastSuggestionQuery = Manager.Data.Context.Load(Manager.Data.Context.SearchClientsForRoleQuery(Manager.Context.RoleId, ClientsAutoCompleteBox.Text).Take(10),
                                clientsLoadOperation =>
                                {
                                    if (clientsLoadOperation.IsCanceled || clientsLoadOperation.HasError) return;

                                    ClientsAutoCompleteBox.ItemsSource = clientsLoadOperation.Entities;
                                    ClientsAutoCompleteBox.PopulateComplete();
                                }, null);
        }
    }
}
