﻿using System.IO;
using System.Reactive.Subjects;
using System.Windows.Controls;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.CoreEntities.Extensions.Services;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.ViewModels;
using FoundOps.SLClient.UI.Tools;
using MEFedMVVM.ViewModelLocator;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Linq;
using System.ServiceModel.DomainServices.Client;
using System.Windows;
using Telerik.Windows.Data;

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

            //Cannot currently delete recurring services. Waiting on bug fix.
            //this.CanDeleteSubject.OnNext(false);
        }

        /// <summary>
        /// Used in the constructor to setup data loading.
        /// </summary>
        private void SetupDataLoading()
        {
            //Update when the ClientContext changes
            ContextManager.GetContextObservable<Client>().ObserveOnDispatcher()
                .Subscribe(clientContext =>
                    QueryableCollectionView = clientContext == null
                                                                    ? new QueryableCollectionView(new Client[] { })
                                                                    : new QueryableCollectionView(clientContext.RecurringServices));

            //Whenever the selected RecurringService changes load the details (ServiceTemplate and fields)
            SetupDetailsLoading(selectedEntity => DomainContext.GetRecurringServiceDetailsForRoleQuery(ContextManager.RoleId, selectedEntity.Id));
        }

        #endregion

        #region Logic

        #region Add Delete

        protected override RecurringService AddNewEntity(object commandParameter)
        {
            var newRecurringService = new RecurringService();
            newRecurringService.AddRepeat();

            newRecurringService.Repeat.StartDate = Manager.Context.UserAccount.Now().Date;

            //The RecurringServices Add Button will pass a ClientLevel ServiceTemplate (Available Service)
            var clientLevelServiceTemplate = (ServiceTemplate)commandParameter;

            //Copy the template from the ClientLevel ServiceTemplate
            var copiedTemplate = clientLevelServiceTemplate.MakeChild(ServiceTemplateLevel.RecurringServiceDefined);
            copiedTemplate.Id = newRecurringService.Id;
            newRecurringService.ServiceTemplate = copiedTemplate;

            var clientContext = ContextManager.GetContext<Client>();

            newRecurringService.Client = clientContext;

            //If there is only one location, try to set the destination (assuming this Service Template has a destination field)
            if (clientContext.Locations.Count == 1)
                newRecurringService.ServiceTemplate.SetDestination(clientContext.Locations.First());

            return newRecurringService;
        }

        protected override void OnAddEntity(RecurringService recurringService)
        {
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

        protected override void OnSave(SubmitOperation submitOperation)
        {
            base.OnSave(submitOperation);

            //Refresh Services
            VM.Services.ForceRefresh();
        }

        #endregion

        #region Export to CSV

        /// <summary>
        /// Exports to a CSV file.
        /// NOTE: Must be called directly in a user initiated event handler (like a click) for security purposes for SaveFileDialog. Therefore it cannot be executed from a command.
        /// </summary>
        public void ExportToCSV()
        {
            var csvLoadedObservable = new ReplaySubject<byte[]>();

            //Load the CSV
            DomainContext.GetRecurringServicesCSVForRole(ContextManager.RoleId, 
                loadedCSV => csvLoadedObservable.OnNext(loadedCSV.Value), null);

            var fileName = String.Format("RecurringServicesExport {0}.csv", Manager.Context.UserAccount.Now().ToString("MM'-'dd'-'yyyy"));
            var saveFileDialog = new SaveFileDialog { DefaultFileName = fileName, DefaultExt = ".csv", Filter = "CSV File|*.csv" };

            if (saveFileDialog.ShowDialog() != true) return;

            csvLoadedObservable.Take(1).ObserveOnDispatcher().Subscribe(csvByteArray =>
            {
                var fileWriter = new BinaryWriter(saveFileDialog.OpenFile());
                fileWriter.Write(csvByteArray);
                fileWriter.Close();
            });
        }

        #endregion
    }
}
