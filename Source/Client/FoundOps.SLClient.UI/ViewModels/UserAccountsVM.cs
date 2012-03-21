﻿using FoundOps.Common.Silverlight.Tools;
using FoundOps.Common.Silverlight.Services;
using FoundOps.Common.Silverlight.UI.Controls.AddEditDelete;
using FoundOps.Common.Tools;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.ViewModels;
using MEFedMVVM.ViewModelLocator;
using Microsoft.Windows.Data.DomainServices;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Controls;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// A view model for all of the UserAccounts
    /// </summary>
    [ExportViewModel("UserAccountsVM")]
    public class UserAccountsVM : InfiniteAccordionVM<Party>, //Base class is Party because DomainCollectionView does not work well with inheritance
        IAddToDeleteFromSource<Party> //Base class is Party because loadedUserAccounts is EntityList<Party>
    {
        #region Public Properties and Variables

        #region Implementation of IAddToDeleteFromSource

        //Want to use the default comparer. So this has need to be set.
        public IEqualityComparer<object> CustomComparer { get; set; }

        public IEnumerable ExistingItemsSource { get { return Context.Parties.OfType<UserAccount>(); } }

        public string MemberPath { get { return "DisplayName"; } }

        /// <summary>
        /// A function to create a new item from a string.
        /// </summary>
        public Func<string, Party> CreateNewItem { get; private set; }

        #endregion

        /// <summary>
        /// Gets the object type provided (for the InfiniteAccordion). Overriden because base class is Party.
        /// </summary>
        public override Type ObjectTypeProvided
        {
            get { return typeof(UserAccount); }
        }

        //TODO
        public Action<string, AutoCompleteBox> ManuallyUpdateSuggestions
        {
            get { return null; }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAccountsVM"/> class.
        /// </summary>
        [ImportingConstructor]
        public UserAccountsVM()
            : base(new[] { typeof(BusinessAccount) })
        {
            SetupDataLoading();

            //Subscribe to the UserAccounts observable
            //IsLoadingObservable = DataManager.Subscribe<UserAccount>(DataManager.Query.UserAccounts, this.ObservationState, null);
            //#region DomainCollectionView

            //var loadedUserAccounts = DataManager.GetEntityListObservable<Party>(DataManager.Query.UserAccounts);

            ////Update the DCV
            ////a) the loaded UserAccounts changes
            ////b) whenever the BusinessAccount context changes
            ////c) //TODO the BusinessAccount context OwnedRoles changes
            ////d) //TODO the BusinessAccount context OwnedRoles' MemberParties changes

            //loadedUserAccounts.AsGeneric().Merge(
            //    ContextManager.GetContextObservable<BusinessAccount>().WhereNotNull()
            //    .SelectLatest(ba =>
            //                    ba.OwnedRoles.FromCollectionChangedAndNow()
            //                    .SelectLatest(_ => //d) the BusinessAccount context OwnedRoles' MemberParties changes
            //                                    ba.OwnedRoles.Select(or => or.MemberParties.FromCollectionChangedGeneric()).Merge()
            //                                        //c) the BusinessAccount context's OwnedRoles changes
            //                                    .AndNow())
            //                        //b) the BusinessAccount context changes
            //                                    .AndNow()))
            //    //a) the loaded UserAccounts changes
            //                                    .AndNow()
            //    .Throttle(TimeSpan.FromMilliseconds(200))
            //    .ObserveOnDispatcher().Subscribe(_ =>
            //    {
            //        var businessAccountContext = this.ContextManager.GetContext<BusinessAccount>();

            //        //If there is a businessAccount context then return the businessAccount's UserAccounts
            //        //Otherwise return all the loaded UserAccounts
            //        IEnumerable<Party> setOfUserAccounts = businessAccountContext != null
            //                                            ? businessAccountContext.OwnedRoles.SelectMany(r => r.MemberParties.OfType<UserAccount>())
            //                                            : this.Context.Parties.OfType<UserAccount>();

            //        this.CollectionViewObservable.OnNext(DomainCollectionViewFactory<Party>.GetDomainCollectionView(new EntityList<Party>(Context.Parties, setOfUserAccounts)));
            //    });

            //#endregion

            //#region Implementation of IAddToDeleteFromSource<Party>

            ////Whenever the _loadedUserAccounts changes notify ExistingItemsSource changed
            //_loadedUserAccounts = loadedUserAccounts.ToProperty(this, x => x.ExistingItemsSource);

            CreateNewItem = name =>
            {
                var newUserAccount = new UserAccount { TemporaryPassword = PasswordTools.GeneratePassword(), DisplayName = name };

                //Add the new entity to the Context so it gets tracked/saved
                Context.Parties.Add(newUserAccount);

                return newUserAccount;
            };

            //#endregion
        }

        #region Logic

        private void SetupDataLoading()
        {
            //Force load the entities when in a related types view
            //this is because VDCV will only normally load when a virtual item is loaded onto the screen
            //virtual items will not always load because in clients context the gridview does not always show (sometimes it is in single view)
            SetupContextDataLoading(roleId =>
                                        {
                                            var businessAccountContext = this.ContextManager.GetContext<BusinessAccount>();
                                            return Context.GetUserAccountsQuery(roleId, businessAccountContext != null ? businessAccountContext.Id : Guid.Empty);
                                        }, null, false, VirtualItemCountLoadBehavior.LoadAfterManyRelationContextChanges);


            //Whenever the user account changes load the details
            SetupDetailsLoading(selectedEntity => Context.GetUserAccountDetailsForRoleQuery(ContextManager.RoleId, selectedEntity.Id));
        }

        //Must override or else it will create a Party
        protected override Party AddNewEntity(object commandParameter)
        {
            //Reuse the CreateNewItem method
            return CreateNewItem("");
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
