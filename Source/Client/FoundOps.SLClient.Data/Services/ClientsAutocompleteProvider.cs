using FoundOps.Common.Silverlight.UI.Controls.AutoComplete;
using FoundOps.Core.Models.CoreEntities;
using System.ServiceModel.DomainServices.Client;
using Telerik.Windows.Controls;

namespace FoundOps.SLClient.Data.Services
{
    /// <summary>
    /// Autocomplete provider for Clients
    /// </summary>
    public class ClientsAutocompleteProvider : IAutocompleteProvider
    {
        /// <summary>
        /// An event to call when [items received].
        /// </summary>
        public event ItemsReceivedEventHandler ItemsReceived;

        /// <summary>
        /// Bound to the TextSearchMode from the attached property of the AutocompleteBehavior.
        /// </summary>
        public TextSearchMode TextSearchMode { get; set; }

        /// <summary>
        /// Bound to the TextPath from the ComboBox.
        /// </summary>
        public string TextPath { get; set; }

        private LoadOperation<Client> _lastLoad;

        /// <summary>
        /// A method to gets items based on text.
        /// </summary>
        /// <param name="text">The ComboBox's text.</param>
        public void GetItems(string text)
        {
            if (_lastLoad != null && _lastLoad.CanCancel)
                _lastLoad.Cancel();

            _lastLoad = Manager.Data.Context.Load(Manager.Data.Context.GetClientsForRoleQuery(Manager.Context.RoleId),
                                      clientsLoadOperation =>
                                      {
                                          if (clientsLoadOperation.IsCanceled || clientsLoadOperation.HasError) return;

                                          if (this.ItemsReceived != null)
                                              this.ItemsReceived(this, new ItemsReceivedEventArgs(clientsLoadOperation.Entities));
                                      }, null);
        }
    }
}