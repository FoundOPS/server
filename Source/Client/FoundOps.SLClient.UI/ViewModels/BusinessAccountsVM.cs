using System;
using System.Collections;
using System.Reactive.Linq;
using System.Windows;
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
        IAddToDeleteFromDestination<BusinessAccount>, IAddNewExisting<UserAccount>, IRemoveDelete<UserAccount>,
        IAddNewExisting<ServiceTemplate>, IRemoveDelete<ServiceTemplate>
    {
        #region Public

        #region Implementation of IAddToDeleteFromProvider<BusinessAccount>

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
            if (sourceType == typeof(UserAccount))
            {
                control.AddExistingItem += (s, existingItem) => this.AddExistingItemUserAccount((UserAccount)existingItem);
                control.AddNewItem += (s, newItemText) => this.AddNewItemUserAccount(newItemText);
                control.RemoveItem += (s, e) => this.RemoveItemUserAccount();
                control.DeleteItem += (s, e) => this.DeleteItemUserAccount();
            }
        }

        #endregion

        #region Implementation of IAddNewExisting<ServiceTemplate> & IRemoveDelete<ServiceTemplate>

        public Func<string, ServiceTemplate> AddNewItemServiceTemplate { get; private set; }
        Func<string, ServiceTemplate> IAddNew<ServiceTemplate>.AddNewItem { get { return AddNewItemServiceTemplate; } }

        public Action<ServiceTemplate> AddExistingItemServiceTemplate { get; private set; }
        Action<ServiceTemplate> IAddNewExisting<ServiceTemplate>.AddExistingItem { get { return AddExistingItemServiceTemplate; } }

        public Func<ServiceTemplate> RemoveItemServiceTemplate { get; private set; }
        Func<ServiceTemplate> IRemove<ServiceTemplate>.RemoveItem { get { return RemoveItemServiceTemplate; } }

        public Func<ServiceTemplate> DeleteItemServiceTemplate { get; private set; }
        Func<ServiceTemplate> IRemoveDelete<ServiceTemplate>.DeleteItem { get { return DeleteItemServiceTemplate; } }

        #endregion

        #region Implementation of IAddNewExisting<UserAccount> & IRemoveDelete<UserAccount>

        public Func<string, UserAccount> AddNewItemUserAccount { get; private set; }
        Func<string, UserAccount> IAddNew<UserAccount>.AddNewItem { get { return AddNewItemUserAccount; } }

        public Action<UserAccount> AddExistingItemUserAccount { get; private set; }
        Action<UserAccount> IAddNewExisting<UserAccount>.AddExistingItem { get { return AddExistingItemUserAccount; } }

        public Func<UserAccount> RemoveItemUserAccount { get; private set; }
        Func<UserAccount> IRemove<UserAccount>.RemoveItem { get { return RemoveItemUserAccount; } }

        public Func<UserAccount> DeleteItemUserAccount { get; private set; }
        Func<UserAccount> IRemoveDelete<UserAccount>.DeleteItem { get { return DeleteItemUserAccount; } }

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

            #region Implementation of IAddToDeleteFromProvider<BusinessAccount>

            //Whenever the SelectedEntity changes, notify the DestinationItemsSource changed
            this.SelectedEntityObservable.ObserveOnDispatcher().Subscribe(
                _ => this.RaisePropertyChanged("DestinationItemsSource"));

            #endregion

            #region Implementation of IAddNewExisting<ServiceTemplate> & IRemoveDelete<ServiceTemplate>

            AddNewItemServiceTemplate = name =>
            {
                return VM.ServiceTemplates.CreateNewItem(name);
            };

            AddExistingItemServiceTemplate = existingItem => SelectedEntity.FirstOwnedRole.MemberParties.Add(existingItem);

            RemoveItemServiceTemplate = () =>
            {
                //TODO
                //MessageBox.Show("You cannot remove FoundOPS ServiceTemplates manually.");
                
                return null;
            };

            DeleteItemServiceTemplate = () =>
            {
                //TODO
                //MessageBox.Show("You cannot delete FoundOPS ServiceTemplates manually.");

                return null;
            };

            #endregion

            #region Implementation of IAddNewExisting<UserAccount> & IRemoveDelete<UserAccount>

            AddNewItemUserAccount = name =>
            {
                var newUserAccount = VM.UserAccounts.CreateNewItem(name);
                this.SelectedEntity.FirstOwnedRole.MemberParties.Add(newUserAccount);
                return (UserAccount)newUserAccount;
            };

            AddExistingItemUserAccount = existingItem => SelectedEntity.FirstOwnedRole.MemberParties.Add(existingItem);

            RemoveItemUserAccount = () =>
            {
                var selectedUserAccount = VM.UserAccounts.SelectedEntity;
                if (selectedUserAccount != null)
                    this.SelectedEntity.FirstOwnedRole.MemberParties.Remove(selectedUserAccount);

                return (UserAccount)selectedUserAccount;
            };

            DeleteItemUserAccount = () =>
            {
                var selectedUserAccount = RemoveItemUserAccount();
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
