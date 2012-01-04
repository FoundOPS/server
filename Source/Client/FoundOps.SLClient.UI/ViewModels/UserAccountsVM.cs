using System;
using System.Linq;
using System.Reactive.Linq;
using System.ComponentModel;
using FoundOps.Common.Tools;
using System.Collections.Generic;
using MEFedMVVM.ViewModelLocator;
using FoundOps.SLClient.Data.Services;
using FoundOps.Common.Silverlight.Tools;
using System.ComponentModel.Composition;
using FoundOps.SLClient.Data.ViewModels;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Common.Silverlight.Services;
using Microsoft.Windows.Data.DomainServices;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// A view model for all of the UserAccounts
    /// </summary>
    [ExportViewModel("UserAccountsVM")]
    public class UserAccountsVM : CoreEntityCollectionInfiniteAccordionVM<Party> //Base class is Party because DomainCollectionView does not work well with inheritance
    {
        /// <summary>
        /// Gets the object type provided (for the InfiniteAccordion). Overriden because base class is Party.
        /// </summary>
        public override Type ObjectTypeProvided
        {
            get { return typeof(UserAccount); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAccountsVM"/> class.
        /// </summary>
        /// <param name="dataManager">The data manager.</param>
        [ImportingConstructor]
        public UserAccountsVM(DataManager dataManager)
            : base(dataManager, true)
        {
            //Subscribe to the UserAccounts observable
            IsLoadingObservable = DataManager.Subscribe<UserAccount>(DataManager.Query.UserAccounts, this.ObservationState, null);

            #region DomainCollectionView

            var loadedUserAccounts = DataManager.GetEntityListObservable<Party>(DataManager.Query.UserAccounts);

            ////Whenever the BusinessAccount context changes, and when the loaded UserAccounts changes, update the DCV
            ContextManager.GetContextObservable<BusinessAccount>().AsGeneric().Merge(loadedUserAccounts.AsGeneric())
                .Throttle(TimeSpan.FromMilliseconds(200))
                .ObserveOnDispatcher().Subscribe(_ =>
                {
                    var businessAccountContext = this.ContextManager.GetContext<BusinessAccount>();

                    //If there is a businessAccount context then return the businessAccount's UserAccounts
                    //Otherwise return all the loaded UserAccounts
                    IEnumerable<Party> setOfUserAccounts = businessAccountContext != null
                                                        ? businessAccountContext.OwnedRoles.SelectMany(r => r.MemberParties.OfType<UserAccount>())
                                                        : this.Context.Parties.OfType<UserAccount>();

                    this.DomainCollectionViewObservable.OnNext(DomainCollectionViewFactory<Party>.GetDomainCollectionView(new EntityList<Party>(Context.Parties, setOfUserAccounts)));
                });

            //Whenever the DCV changes, sort by Name and select the first entity
            this.DomainCollectionViewObservable.Throttle(TimeSpan.FromMilliseconds(300)) //wait for UI to load
                .ObserveOnDispatcher().Subscribe(dcv =>
                {
                    dcv.SortDescriptions.Add(new SortDescription("DisplayName", ListSortDirection.Ascending));
                    this.SelectedEntity = this.DomainCollectionView.FirstOrDefault();
                });

            #endregion
        }

        #region Logic

        //Must override or else it will create a Party
        protected override Party AddNewEntity(object commandParameter)
        {
            var newUserAccount = new UserAccount { TemporaryPassword = PasswordTools.GeneratePassword() };
            ((EntityList<Party>)this.DomainCollectionView.SourceCollection).Add(newUserAccount);
            return newUserAccount;
        }

        protected override void OnAddEntity(Party newUserAccount)
        {
            var serviceProviderContext = ContextManager.GetContext<BusinessAccount>();
            if (serviceProviderContext == null) return;

            //Add the current user to the Administrator role of the ServiceProvider
            serviceProviderContext.OwnedRoles.First(r => r.Name == "Administrator").MemberParties.Add(newUserAccount);
        }

        #endregion
    }
}
