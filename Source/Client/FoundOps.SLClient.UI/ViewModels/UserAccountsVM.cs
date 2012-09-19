using System.Reactive.Linq;
using System.Reactive.Subjects;
using FoundOps.Common.Silverlight.Tools;
using FoundOps.Common.Silverlight.UI.Controls.AddEditDelete;
using FoundOps.Common.Tools;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.ViewModels;
using GalaSoft.MvvmLight.Command;
using MEFedMVVM.ViewModelLocator;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;
using ReactiveUI;
using ReactiveUI.Xaml;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// A view model for all of the UserAccounts
    /// </summary>
    [ExportViewModel("UserAccountsVM")]
    public class UserAccountsVM : InfiniteAccordionVM<Party, UserAccount>, //Base class is Party because DomainCollectionView does not work well with inheritance
        IAddToDeleteFromSource<Party> //Base class is Party because loadedUserAccounts is EntityList<Party>
    {
        #region Public Properties

        #region Implementation of IAddToDeleteFromSource

        public string MemberPath { get { return "DisplayName"; } }

        /// <summary>
        /// A function to create a new item from a string.
        /// </summary>
        public Func<string, Party> CreateNewItem { get; private set; }

        /// <summary>
        /// Gets the object type provided (for the InfiniteAccordion). Overriden because base class is Party.
        /// </summary>
        public override Type ObjectTypeProvided
        {
            get { return typeof(UserAccount); }
        }

        /// <summary>
        /// A method to update the ExistingItemsSource with UserAccount suggestions remotely loaded.
        /// </summary>
        public Action<AutoCompleteBox> ManuallyUpdateSuggestions { get; private set; }

        #endregion

        /// <summary>
        /// A property for whether the current user is mobile only for the business account context.
        /// </summary>
        public bool SelectedUserIsMobileOnly
        {
            get
            {
                var currentBusinessAccountContext = ContextManager.GetContext<BusinessAccount>();
                return currentBusinessAccountContext != null &&
                    currentBusinessAccountContext.MobileRole != null && currentBusinessAccountContext.MobileRole.MemberParties.Contains(SelectedEntity)
                    && !currentBusinessAccountContext.AdministratorRole.MemberParties.Contains(SelectedEntity);
            }
            // Only from the admin console. This will remove the user from the administrator role, and add them to the mobile only role.
            set
            {
                SetUserRole(value);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAccountsVM"/> class.
        /// </summary>
        [ImportingConstructor]
        public UserAccountsVM()
            : base(new[] { typeof(BusinessAccount) })
        {
            SetupDataLoading();
            SetupUserAccountAddToDeleteFromSource();

            #region Setup Mobile only command

            //Whenever the SelectedEntity changes or reject changes happens update SelectedUserIsMobileOnly
            SelectedEntityObservable.AsGeneric().Merge(DomainContext.ChangesRejectedObservable.AsGeneric())
                .Throttle(TimeSpan.FromMilliseconds(100)).ObserveOnDispatcher()
                .Subscribe(_ => this.RaisePropertyChanged("SelectedUserIsMobileOnly"));

            #endregion

            #region TODO Update the QCV when the BusinessAccount context OwnedRoles changes and the BusinessAccount context OwnedRoles' MemberParties changes

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

            #endregion
        }

        private void SetupDataLoading()
        {
            SetupContextDataLoading(roleId =>
                                        {
                                            var businessAccountContext = this.ContextManager.GetContext<BusinessAccount>();
                                            return DomainContext.GetUserAccountsQuery(roleId, businessAccountContext != null ? businessAccountContext.Id : Guid.Empty);
                                        }, null);


            //Whenever the user account changes load the details
            SetupDetailsLoading(selectedEntity => DomainContext.GetUserAccountDetailsForRoleQuery(ContextManager.RoleId, selectedEntity.Id));
        }

        /// <summary>
        /// Sets up the implementation of IAddToDeleteFromSource&lt;Party&gt;
        /// </summary>
        private void SetupUserAccountAddToDeleteFromSource()
        {
            CreateNewItem = name =>
            {
                //Jump to this details view
                NavigateToThis();

                //Use Temp salt
                var newUserAccount = new UserAccount
                    {
                        TemporaryPassword = PasswordTools.GeneratePassword(),
                        PasswordSalt = new byte[] {65, 0, 65},
                        DisplayName = name,
                        CreationDate = DateTime.UtcNow.Date
                    };

                //Add the new entity to the Context so it gets tracked/saved
                DomainContext.Parties.Add(newUserAccount);

                //Add the current user to the Administrator role of the ServiceProvider
                var serviceProviderContext = ContextManager.GetContext<BusinessAccount>();
                if (serviceProviderContext != null)
                    serviceProviderContext.OwnedRoles.First(r => r.Name == "Administrator").MemberParties.Add(newUserAccount);

                SelectedEntity = newUserAccount;

                return newUserAccount;
            };

            ManuallyUpdateSuggestions = autoCompleteBox =>
              SearchSuggestionsHelper(autoCompleteBox, () => Manager.Data.DomainContext.SearchUserAccountsForRoleQuery(Manager.Context.RoleId, autoCompleteBox.SearchText));
        }

        /// <summary>
        /// Set the user role.
        /// </summary>
        /// <param name="mobileOnly">If true add this use to mobile only. Otherwise make them an admin.</param>
        private void SetUserRole(bool mobileOnly)
        {
            if (SelectedEntity == null)
                return;

            var currentBusinessAccount = ContextManager.GetContext<BusinessAccount>();
            if (currentBusinessAccount == null)
                return;

            if (mobileOnly)
            {
                currentBusinessAccount.MobileRole.MemberParties.Add(SelectedEntity);
                currentBusinessAccount.AdministratorRole.MemberParties.Remove(SelectedEntity);
            }
            else
            {
                currentBusinessAccount.MobileRole.MemberParties.Remove(SelectedEntity);
                currentBusinessAccount.AdministratorRole.MemberParties.Add(SelectedEntity);
            }

            this.RaisePropertyChanged("SelectedUserIsMobileOnly");
        }

        #endregion

        #region Logic

        //Must override or else it will create a Party
        protected override Party AddNewEntity(object commandParameter)
        {
            //Reuse the CreateNewItem method
            return CreateNewItem("");
        }

        #endregion
    }
}
