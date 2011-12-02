using System;
using System.Reactive.Linq;
using System.Windows;
using FoundOps.Common.Silverlight.Services;
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
    public class RecurringServicesVM : CoreEntityCollectionInfiniteAccordionVM<RecurringService>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecurringServicesVM"/> class.
        /// </summary>
        /// <param name="dataManager">The data manager.</param>
        [ImportingConstructor]
        public RecurringServicesVM(DataManager dataManager)
            : base(dataManager)
        {
            //Subscribe to the RecurringServices query
            IsLoadingObservable = DataManager.Subscribe<RecurringService>(DataManager.Query.RecurringServices, ObservationState, null);

            //Setup DomainCollectionView based on the current Client context

            var clientContext = ContextManager.GetContextObservable<Client>();
            clientContext.ObserveOnDispatcher().Subscribe(client =>
            {
                if (client != null)
                    DomainCollectionViewObservable.OnNext(
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
