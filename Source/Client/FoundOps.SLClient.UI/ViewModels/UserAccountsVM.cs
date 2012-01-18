using System;
using FoundOps.Common.Tools;
using ReactiveUI;
using System.Linq;
using System.Collections;
using System.Reactive.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using MEFedMVVM.ViewModelLocator;
using FoundOps.SLClient.Data.Services;
using FoundOps.Common.Silverlight.Tools;
using System.ComponentModel.Composition;
using FoundOps.SLClient.Data.ViewModels;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Common.Silverlight.Services;
using Microsoft.Windows.Data.DomainServices;
using FoundOps.Common.Silverlight.Tools.ExtensionMethods;
using FoundOps.Common.Silverlight.UI.Controls.AddEditDelete;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// A view model for all of the UserAccounts
    /// </summary>
    [ExportViewModel("UserAccountsVM")]
    public class UserAccountsVM : CoreEntityCollectionInfiniteAccordionVM<Party>, //Base class is Party because DomainCollectionView does not work well with inheritance
        IAddToDeleteFromSource<Party> //Base class is Party because loadedUserAccounts is EntityList<Party>
    {
        #region Properties and Variables

        #region Public

        #region Implementation of IAddToDeleteFromSource

        //Want to use the default comparer. So this has need to be set.
        public IEqualityComparer<object> CustomComparer { get; set; }

        private readonly ObservableAsPropertyHelper<IEnumerable> _loadedUserAccounts;
        public IEnumerable ExistingItemsSource { get { return _loadedUserAccounts.Value; } }

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

        #endregion

        #endregion

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

            //Update the DCV
            //a) the loaded UserAccounts changes
            //b) whenever the BusinessAccount context changes
            //c) the BusinessAccount context OwnedRoles changes
            //d) the BusinessAccount context OwnedRoles' MemberParties changes

            //a) the loaded UserAccounts changes
            loadedUserAccounts.AsGeneric().Merge(
                ContextManager.GetContextObservable<BusinessAccount>().WhereNotNull()
                .SelectLatest(ba =>//c) the BusinessAccount context OwnedRoles changes
                                ba.OwnedRoles.FromCollectionChangedAndNow()
                                .SelectLatest(_ => //d) the BusinessAccount context OwnedRoles' MemberParties changes
                                                ba.OwnedRoles.Select(or => or.MemberParties.FromCollectionChangedGenericAndNow()).Merge()))
                //b) whenever the BusinessAccount context changes
                 .AndNow())
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

            //Whenever the DCV changes:
            //a) sort by Name 
            //b) select the first entity
            this.DomainCollectionViewObservable.Throttle(TimeSpan.FromMilliseconds(300)) //wait for UI to load
                .ObserveOnDispatcher().Subscribe(dcv =>
                {
                    //a) sort by Name 
                    dcv.SortDescriptions.Add(new SortDescription("DisplayName", ListSortDirection.Ascending));
                    //b) select the first entity
                    this.SelectedEntity = this.DomainCollectionView.FirstOrDefault();
                });

            #endregion

            #region Implementation of IAddToDeleteFromSource<Party>

            //Whenever the _loadedUserAccounts changes notify ExistingItemsSource changed
            _loadedUserAccounts = loadedUserAccounts.ToProperty(this, x => x.ExistingItemsSource);

            CreateNewItem = name =>
            {
                var newUserAccount = new UserAccount { TemporaryPassword = PasswordTools.GeneratePassword() };

                //Try to guess the name
                if (!String.IsNullOrEmpty(name))
                {
                    var firstLastName = name.Split(' ');
                    if (firstLastName.Count() == 2)
                    {
                        newUserAccount.FirstName = firstLastName.First();
                        newUserAccount.LastName = firstLastName.Last();
                    }
                    else
                        newUserAccount.FirstName = name;
                }

                //Add the new entity to the EntityList so it gets tracked/saved
                ((EntityList<Party>)ExistingItemsSource).Add(newUserAccount);

                return newUserAccount;
            };

            #endregion
        }

        #region Logic

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
