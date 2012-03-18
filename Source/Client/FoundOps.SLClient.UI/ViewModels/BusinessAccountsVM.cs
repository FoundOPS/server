using System;
using System.Linq;
using System.Windows;
using System.Collections;
using System.Reactive.Linq;
using System.Collections.Generic;
using FoundOps.SLClient.UI.Tools;
using MEFedMVVM.ViewModelLocator;
using FoundOps.SLClient.Data.Services;
using System.ComponentModel.Composition;
using FoundOps.SLClient.Data.ViewModels;
using FoundOps.Core.Models.CoreEntities;
using Microsoft.Windows.Data.DomainServices;
using FoundOps.Common.Silverlight.UI.Controls;
using FoundOps.Server.Services.CoreDomainService;
using FoundOps.Common.Silverlight.UI.Controls.AddEditDelete;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Contains the logic for displaying BusinessAccounts
    /// </summary>
    [ExportViewModel("BusinessAccountsVM")]
    public class BusinessAccountsVM : InfiniteAccordionVM<Party>, //Base class is Party instead of BusinessAccount because DomainCollectionView does not work well with inheritance
        IAddToDeleteFromDestination<UserAccount>, IAddNewExisting<UserAccount>, IRemoveDelete<UserAccount>,
        IAddToDeleteFromDestination<ServiceTemplate>, IAddNewExisting<ServiceTemplate>, IRemoveDelete<ServiceTemplate>
    {
        #region Public

        #region Implementation of IAddToDeleteFromDestination

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

            if (sourceType == typeof(ServiceTemplate))
            {
                control.AddExistingItem += (s, existingItem) => this.AddExistingItemServiceTemplate((ServiceTemplate)existingItem);
                control.AddNewItem += (s, newItemText) => this.AddNewItemServiceTemplate(newItemText);
                control.RemoveItem += (s, e) => this.RemoveItemServiceTemplate();
                control.DeleteItem += (s, itemToDelete) => this.DeleteItemServiceTemplate();
            }
        }

        /// <summary>
        /// Gets the user account destination items source.
        /// </summary>
        public IEnumerable UserAccountsDestinationItemsSource
        {
            get
            {
                if (SelectedEntity == null || SelectedEntity.FirstOwnedRole == null)
                    return null;

                return SelectedEntity.FirstOwnedRole.MemberParties;
            }
        }

        /// <summary>
        /// Gets the service templates destination items source.
        /// </summary>
        public IEnumerable ServiceTemplatesDestinationItemsSource
        {
            get { return SelectedEntity == null ? null : ((BusinessAccount)SelectedEntity).ServiceTemplates; }
        }

        #endregion

        #region Implementation of IAddNewExisting<ServiceTemplate> & IRemoveDelete<ServiceTemplate>

        /// <summary>
        /// An action to add a new ServiceTemplate to the current BusinessAccount.
        /// </summary>
        public Func<string, ServiceTemplate> AddNewItemServiceTemplate { get; private set; }
        Func<string, ServiceTemplate> IAddNew<ServiceTemplate>.AddNewItem { get { return AddNewItemServiceTemplate; } }

        /// <summary>
        /// An action to add an existing ServiceTemplate to the current BusinessAccount.
        /// </summary>
        public Action<ServiceTemplate> AddExistingItemServiceTemplate { get; private set; }
        Action<ServiceTemplate> IAddNewExisting<ServiceTemplate>.AddExistingItem { get { return AddExistingItemServiceTemplate; } }

        /// <summary>
        /// An action to remove a ServiceTemplate from the current BusinessAccount.
        /// </summary>
        public Func<ServiceTemplate> RemoveItemServiceTemplate { get; private set; }
        Func<ServiceTemplate> IRemove<ServiceTemplate>.RemoveItem { get { return RemoveItemServiceTemplate; } }

        /// <summary>
        /// An action to remove a ServiceTemplate from the current BusinessAccount and delete it.
        /// </summary>
        public Func<ServiceTemplate> DeleteItemServiceTemplate { get; private set; }
        Func<ServiceTemplate> IRemoveDelete<ServiceTemplate>.DeleteItem { get { return DeleteItemServiceTemplate; } }

        #endregion
        #region Implementation of IAddNewExisting<UserAccount> & IRemoveDelete<UserAccount>

        /// <summary>
        /// An action to add a new UserAccount to the current BusinessAccount.
        /// </summary>
        public Func<string, UserAccount> AddNewItemUserAccount { get; private set; }
        Func<string, UserAccount> IAddNew<UserAccount>.AddNewItem { get { return AddNewItemUserAccount; } }

        /// <summary>
        /// Gets the add existing item user account.
        /// </summary>
        public Action<UserAccount> AddExistingItemUserAccount { get; private set; }
        Action<UserAccount> IAddNewExisting<UserAccount>.AddExistingItem { get { return AddExistingItemUserAccount; } }

        /// <summary>
        /// An action to remove a UserAccount from the current BusinessAccount.
        /// </summary>
        public Func<UserAccount> RemoveItemUserAccount { get; private set; }
        Func<UserAccount> IRemove<UserAccount>.RemoveItem { get { return RemoveItemUserAccount; } }

        /// <summary>
        /// An action to remove a UserAccount from the current BusinessAccount and delete it.
        /// </summary>
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

        /// <summary>
        /// String to be verified
        /// </summary>
        private String _verificationString;
        /// <summary>
        /// Gets or sets the verification string used to prevent accidental deletion of a business account.
        /// </summary>
        /// <value>
        /// The string to be verified.
        /// </value>
        public string VerificationString
        {
            get { return _verificationString; }
            set
            {
                _verificationString = value;
                this.RaisePropertyChanged("VerificationString");
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessAccountsVM"/> class.
        /// </summary>
        [ImportingConstructor]
        public BusinessAccountsVM()
        {
            SetupMainQuery(DataManager.Query.BusinessAccounts);

            #region Implementation of IAddToDeleteFromDestination<UserAccount> and Implementation of IAddToDeleteFromDestination<ServiceTemplate>

            //Whenever the SelectedEntity changes, notify the DestinationItemsSources changed
            this.SelectedEntityObservable.ObserveOnDispatcher().Subscribe(_ =>
            {
                this.RaisePropertyChanged("UserAccountsDestinationItemsSource");
                this.RaisePropertyChanged("ServiceTemplatesDestinationItemsSource");
            });

            #endregion

            #region Implementation of IAddNewExisting<ServiceTemplate> & IRemoveDelete<ServiceTemplate>

            AddNewItemServiceTemplate = name => VM.ServiceTemplates.CreateNewItem(name);

            AddExistingItemServiceTemplate = existingItem =>
            {
                var serviceTemplateChild = existingItem.MakeChild(ServiceTemplateLevel.ServiceProviderDefined);
                ((BusinessAccount)SelectedEntity).ServiceTemplates.Add(serviceTemplateChild);
            };

            RemoveItemServiceTemplate = () =>
            {
                var selectedServiceTemplate = VM.ServiceTemplates.SelectedEntity;
                if (selectedServiceTemplate != null)
                    ((BusinessAccount)this.SelectedEntity).ServiceTemplates.Remove(selectedServiceTemplate);

                return selectedServiceTemplate;

                //MessageBox.Show("You cannot remove FoundOPS ServiceTemplates manually yet.");
                //return null;
            };

            DeleteItemServiceTemplate = () =>
            {
                //Add logic to remove a Service template here
                var selectedServiceTemplate = VM.ServiceTemplates.SelectedEntity;
                if (selectedServiceTemplate != null)
                    VM.ServiceTemplates.DeleteEntity(selectedServiceTemplate);

                return selectedServiceTemplate;

                //MessageBox.Show("You cannot delete FoundOPS ServiceTemplates manually yet.");
                //return null;
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

            //Add default role
            var role = new Role { Id = Guid.NewGuid(), Name = "Administrator", OwnerParty = newBusinessAccount, RoleType = RoleType.Administrator };

            //Add the manager and business administrator blocks to the role
            foreach (var blockId in BlockConstants.ManagerBlockIds.Union(BlockConstants.BusinessAdministratorBlockIds))
                role.RoleBlockToBlockSet.Add(new RoleBlock { BlockId = blockId });

            ((EntityList<Party>)this.CollectionView.SourceCollection).Add(newBusinessAccount);

            return newBusinessAccount;
        }

        protected override void CheckDelete(Action<bool> checkCompleted)
        {
            var stringVerifier = new StringVerifier();
            
            stringVerifier.Succeeded += (sender, args) => checkCompleted(true);

            stringVerifier.Cancelled += (sender, args) => checkCompleted(false);

            stringVerifier.Show();
        }

        public override void DeleteEntity(Party entityToDelete)
        {
            var entityCollection = new List<BusinessAccount> { (BusinessAccount)entityToDelete };

            //MessageBox.Show("Cannot manually delete Service Providers.");

            DataManager.RemoveEntities(entityCollection);
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
                SelectedEntityContactInfoVM = new ContactInfoVM(ContactInfoType.OwnedParties, newValue.ContactInfoSet);
        }

        #endregion
    }
}
