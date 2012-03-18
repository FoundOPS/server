using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using FoundOps.Common.Silverlight.Services;
using FoundOps.Core.Models.CoreEntities.Extensions.Services;
using MEFedMVVM.ViewModelLocator;
using FoundOps.SLClient.Data.Services;
using System.ComponentModel.Composition;
using FoundOps.SLClient.Data.ViewModels;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Contains the logic for displaying RecurringServices
    /// </summary>
    [ExportViewModel("RecurringServicesVM")]
    public class RecurringServicesVM : InfiniteAccordionVM<RecurringService>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecurringServicesVM"/> class.
        /// </summary>
        [ImportingConstructor]
        public RecurringServicesVM()
            : base(new[] { typeof(Client) })
        {
            //Subscribe to the RecurringServices query
            IsLoadingObservable = DataManager.Subscribe<RecurringService>(DataManager.Query.RecurringServices, ObservationState, null);

            //Setup DomainCollectionView based on the current Client context

            var clientContext = ContextManager.GetContextObservable<Client>();
            clientContext.ObserveOnDispatcher().Subscribe(client =>
            {
                if (client != null)
                    CollectionViewObservable.OnNext(
                        DomainCollectionViewFactory<ServiceTemplate>.GetDomainCollectionView(client.RecurringServices));
            });
        }

        #region Logic

        #region Add Delete

        protected override RecurringService AddNewEntity(object commandParameter)
        {
            var newRecurringService = base.AddNewEntity(commandParameter);

            //The RecurringServices Add Button will pass a ClientLevel ServiceTemplate (Available Service)
            var clientLevelServiceTemplate = (ServiceTemplate)commandParameter;

            //Copy the template from the ClientLevel ServiceTemplate
            var copiedTemplate = clientLevelServiceTemplate.MakeChild(ServiceTemplateLevel.RecurringServiceDefined);
            copiedTemplate.Id = newRecurringService.Id;
            newRecurringService.ServiceTemplate = copiedTemplate;

            var currentClient = ContextManager.GetContext<Client>();

            //If there is only one location, try to set the destination (assuming this Service Template has a destination field)
            if (currentClient.OwnedParty.Locations.Count == 1)
                newRecurringService.ServiceTemplate.SetDestination(currentClient.OwnedParty.Locations.First());

            return newRecurringService;
        }

        protected override void OnAddEntity(RecurringService recurringService)
        {
            //Find if there is a client context
            var clientContext = ContextManager.GetContext<Client>();

            recurringService.Client = clientContext;

            //Move to the RecurringServices Details View if not already
            if (!IsInDetailsView)
                MoveToDetailsView.Execute(null);
        }

        public override void DeleteEntity(RecurringService entityToDelete)
        {
            //Make sure the recurring service is removed from the Context
            //Instead of it being removed from the Client (the backing DCV)
            Context.RecurringServices.Remove(entityToDelete);
        }

        #endregion

        protected override bool BeforeSaveCommand()
        {
            if (SelectedEntity == null) return true; //If deleting

            var hasServiceTemplate = SelectedEntity.ServiceTemplate != null;
            if (!hasServiceTemplate)
            {
                MessageBox.Show("Please setup a Service Template");
                return false;
            }
            return true;
        }

        #endregion
    }
}
