using System;
using System.Collections;
using System.Reactive.Linq;
using FoundOps.SLClient.UI.Tools;
using MEFedMVVM.ViewModelLocator;
using FoundOps.SLClient.Data.Services;
using System.ComponentModel.Composition;
using FoundOps.SLClient.Data.ViewModels;
using FoundOps.Core.Models.CoreEntities;
using Microsoft.Windows.Data.DomainServices;
using FoundOps.Server.Services.CoreDomainService;
using FoundOps.Common.Silverlight.UI.Controls.AddEditDelete;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Contains the logic for displaying BusinessAccounts
    /// </summary>
    [ExportViewModel("BusinessAccountsVM")]
    public class BusinessAccountsVM : CoreEntityCollectionInfiniteAccordionVM<Party>, //Base class is Party instead of BusinessAccount because DomainCollectionView does not work well with inheritance
        IAddToDeleteFromDestination<BusinessAccount>, IAddNewExisting<UserAccount>, IRemoveDelete<UserAccount>  
    {
        #region Public

        #region Implementation of IAddToDeleteFromProvider

        public IEnumerable DestinationItemsSource
        {
            get
            {
                if (SelectedEntity == null || SelectedEntity.FirstOwnedRole == null)
                    return null;

                return SelectedEntity.FirstOwnedRole.MemberParties;
            }
        }

        /// <summary>
        /// Links to the LinkToAddToDeleteFromControl events.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="sourceType">Type of the source.</param>
        public void LinkToAddToDeleteFromEvents(AddToDeleteFrom control, Type sourceType)
        {
            if (sourceType == typeof (UserAccount))
            {
                control.AddExistingItem += (s, existingItem) => this.AddExistingItem((UserAccount) existingItem);
                control.AddNewItem += (s, newItemText) => this.AddNewItem(newItemText);
                control.RemoveItem += (s, e) => this.RemoveItem();
                control.DeleteItem += (s, e) => this.DeleteItem();
            }
        }

        #endregion

        #region Implementation of IAddNewExisting<in Party>

        public Action<string> AddNewItem { get; private set; }
        public Action<UserAccount> AddExistingItem { get; private set; }

        #endregion

        #region Implementation of IRemoveDelete<in Party>

        public Func<UserAccount> RemoveItem { get; private set; }
        public Func<UserAccount> DeleteItem { get; private set; }

        #endregion

        /// <summary>
        /// Gets the object type provided (for the InfiniteAccordion). Overriden because base class is Party.
        /// </summary>
        public override Type ObjectTypeProvided
        {
            get { return typeof(BusinessAccount); }
        }

        private ContactInfoVM _selectedEntityContactInfoVM;
        /// <summary>
        /// Gets or sets the selected entity contact info VM.
        /// </summary>
        /// <value>
        /// The selected entity contact info VM.
        /// </value>
        public ContactInfoVM SelectedEntityContactInfoVM
        {
            get { return _selectedEntityContactInfoVM; }
            set
            {
                _selectedEntityContactInfoVM = value;
                this.RaisePropertyChanged("SelectedEntityContactInfoVM");
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessAccountsVM"/> class.
        /// </summary>
        /// <param name="dataManager">The data manager.</param>
        [ImportingConstructor]
        public BusinessAccountsVM(DataManager dataManager)
            : base(dataManager, true)
        {
            SetupMainQuery(DataManager.Query.BusinessAccounts);

            #region Implementation of IAddToDeleteFromSource

            //Whenever the SelectedEntity changes, notify the DestinationItemsSource changed
            this.SelectedEntityObservable.ObserveOnDispatcher().Subscribe(
                _ => this.RaisePropertyChanged("DestinationItemsSource"));

            AddNewItem = name =>
            {
                var newUserAccount = VM.UserAccounts.CreateNewItem(name);
                this.SelectedEntity.FirstOwnedRole.MemberParties.Add(newUserAccount);
            };

            AddExistingItem = existingItem => SelectedEntity.FirstOwnedRole.MemberParties.Add(existingItem);

            RemoveItem = () =>
            {
                var selectedUserAccount = VM.UserAccounts.SelectedEntity;
                if (selectedUserAccount != null)
                    this.SelectedEntity.FirstOwnedRole.MemberParties.Remove(selectedUserAccount);

                return (UserAccount) selectedUserAccount;
            };

            DeleteItem = () =>
            {
                var selectedUserAccount = RemoveItem();
                if (selectedUserAccount != null)
                    VM.UserAccounts.DeleteEntity(selectedUserAccount);

                return selectedUserAccount;
            };

            #endregion

        }

        #region Logic

        //Must override or else it will create a Party
        protected override Party AddNewEntity(object commandParameter)
        {
            var newBusinessAccount = new BusinessAccount();
            ((EntityList<Party>)this.DomainCollectionView.SourceCollection).Add(newBusinessAccount);
            return newBusinessAccount;
        }

        /// <summary>
        /// Called when [selected entity changed]. 
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnSelectedEntityChanged(Party oldValue, Party newValue)
        {
            base.OnSelectedEntityChanged(oldValue, newValue);

            if (newValue != null)
                //Whenever the SelectedEntity changes setup the ContactInfoVM for that entity
                SelectedEntityContactInfoVM = new ContactInfoVM(DataManager, ContactInfoType.OwnedParties, newValue.ContactInfoSet);
        }

        #endregion
    }
}
