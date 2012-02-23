using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.ServiceModel.DomainServices.Client;
using FoundOps.Common.Silverlight.UI.Controls.AutoComplete;
using System;
using System.Collections.Generic;
using System.Net;
using FoundOps.Core.Models.CoreEntities;
using Telerik.Windows.Controls;

namespace FoundOps.SLClient.Data.Services
{
    /// <summary>
    /// Autocomplete provider for Clients
    /// </summary>
    public class ClientsAutocompleteProvider : IAutocompleteProvider
    {
        /// <summary>
        /// 
        /// </summary>
        public event ItemsReceivedEventHandler ItemsReceived;

        public TextSearchMode TextSearchMode { get; set; }

        public string TextPath { get; set; }

        private LoadOperation<Client> _lastLoad;

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <param name="text">The searched text.</param>
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