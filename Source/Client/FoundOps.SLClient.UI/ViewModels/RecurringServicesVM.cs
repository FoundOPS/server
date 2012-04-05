using System.ServiceModel.DomainServices.Client;
using FoundOps.Core.Models.CoreEntities.Extensions.Services;
using FoundOps.SLClient.Data.ViewModels;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.UI.Tools;
using MEFedMVVM.ViewModelLocator;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Contains the logic for displaying RecurringServices
    /// </summary>
    [ExportViewModel("RecurringServicesVM")]
    public class RecurringServicesVM : InfiniteAccordionVM<RecurringService, RecurringService>
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RecurringServicesVM"/> class.
        /// </summary>
        [ImportingConstructor]
        public RecurringServicesVM()
            : base(new[] { typeof(Client) })
        {
            SetupDataLoading();
        }

        /// <summary>
        /// Used in the constructor to setup data loading.
        /// </summary>
        private void SetupDataLoading()
        {
            SetupContextDataLoading(roleId =>
                                        {
                                            //Manually filter as workaround to a randomly occuring problem
                                            //where they are all getting loaded when there is no context
                                            var clientContext = ContextManager.GetContext<Client>();
                                            return clientContext == null
                                                       ? null
                                                       : DomainContext.GetRecurringServicesForClientQuery(roleId, clientContext.Id);
                                        }, new[] { new ContextRelationshipFilter("ClientId", typeof(Client), v => ((Client)v).Id) });

            //Whenever the selected RecurringService changes load the details
            SetupDetailsLoading(selectedEntity => DomainContext.GetRecurringServiceDetailsForRoleQuery(ContextManager.RoleId, selectedEntity.Id));
        }

        #endregion

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
            DomainContext.RecurringServices.Remove(entityToDelete);
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

        protected override void OnSave(System.ServiceModel.DomainServices.Client.SubmitOperation submitOperation)
        {
            base.OnSave(submitOperation);

            //Refresh Services
            VM.Services.ForceRefresh();
        }

        #endregion
    }
}
