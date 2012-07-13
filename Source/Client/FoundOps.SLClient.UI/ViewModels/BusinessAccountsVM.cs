using FoundOps.Common.Silverlight.UI.Controls;
using FoundOps.Common.Silverlight.UI.Controls.AddEditDelete;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.CoreEntities.DesignData;
using FoundOps.Server.Services.CoreDomainService;
using FoundOps.SLClient.Data.ViewModels;
using FoundOps.SLClient.UI.Tools;
using MEFedMVVM.ViewModelLocator;
using System;
using System.Collections;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Windows;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Contains the logic for adding, editing, and removing BusinessAccounts in the admin console.
    /// </summary>
    [ExportViewModel("BusinessAccountsVM")]
    public class BusinessAccountsVM : InfiniteAccordionVM<Party, BusinessAccount>,
        IAddToDeleteFromDestination<UserAccount>, IAddNewExisting<UserAccount>, IRemoveDelete<UserAccount>,
        IAddToDeleteFromDestination<ServiceTemplate>, IAddNewExisting<ServiceTemplate>, IRemoveDelete<ServiceTemplate>
    {
        #region Public

        #region Implementation of IAddToDeleteFromDestination

        private List<IDisposable> addToDeleteFromSubscriptions = new List<IDisposable>();
        /// <summary>
        /// Links to the LinkToAddToDeleteFromControl observables.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="sourceType">Type of the source.</param>
        public void LinkToAddToDeleteFrom(AddToDeleteFrom control, Type sourceType)
        {
            if (sourceType == typeof(UserAccount))
            {
                addToDeleteFromSubscriptions.Add(control.AddExistingItem.Subscribe(existingItem => AddExistingItemUserAccount(existingItem)));
                addToDeleteFromSubscriptions.Add(control.AddNewItem.Subscribe(text => AddNewItemUserAccount(text)));
                addToDeleteFromSubscriptions.Add(control.RemoveItem.Subscribe(_ => RemoveItemUserAccount()));
                addToDeleteFromSubscriptions.Add(control.DeleteItem.Subscribe(_ => DeleteItemUserAccount()));
            }

            if (sourceType == typeof(ServiceTemplate))
            {
                addToDeleteFromSubscriptions.Add(control.AddExistingItem.Subscribe(existingItem => AddExistingItemServiceTemplate(existingItem)));
                addToDeleteFromSubscriptions.Add(control.AddNewItem.Subscribe(text => AddNewItemServiceTemplate(text)));
                addToDeleteFromSubscriptions.Add(control.RemoveItem.Subscribe(_ => RemoveItemServiceTemplate()));
                addToDeleteFromSubscriptions.Add(control.DeleteItem.Subscribe(_ => DeleteItemServiceTemplate()));
            }
        }

        /// <summary>
        /// Disposes the subscriptions to the add delete from control observables.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="sourceType">Type of the source.</param>
        public void UnlinkAddToDeleteFrom(AddToDeleteFrom c, Type type)
        {
            foreach (var subscription in addToDeleteFromSubscriptions)
                subscription.Dispose();

            addToDeleteFromSubscriptions.Clear();
        }

        /// <summary>
        /// Gets the user account destination items source.
        /// </summary>
        public IEnumerable UserAccountsDestinationItemsSource
        {
            get
            {
                if (SelectedEntity == null || SelectedEntity.AdministratorRole == null)
                    return null;

                return SelectedEntity.AdministratorRole.MemberParties;
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
        public Action<object> AddExistingItemServiceTemplate { get; private set; }
        Action<object> IAddNewExisting<ServiceTemplate>.AddExistingItem { get { return AddExistingItemServiceTemplate; } }

        /// <summary>
        /// NOT POSSIBLE
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
        public Action<object> AddExistingItemUserAccount { get; private set; }
        Action<object> IAddNewExisting<UserAccount>.AddExistingItem { get { return AddExistingItemUserAccount; } }

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

        private LocationVM _selectedDepotLocationVM;
        /// <summary>
        /// Gets or sets the selected depot location VM.
        /// </summary>
        public LocationVM SelectedDepotLocationVM
        {
            get { return _selectedDepotLocationVM; }
            set
            {
                _selectedDepotLocationVM = value;
                this.RaisePropertyChanged("SelectedDepotLocationVM");
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
            SetupDataLoading();

            #region Implementation of IAddToDeleteFromDestination<UserAccount> and Implementation of IAddToDeleteFromDestination<ServiceTemplate>

            //Whenever the SelectedEntity changes, notify the DestinationItemsSources changed
            this.SelectedEntityObservable.ObserveOnDispatcher().Subscribe(_ =>
            {
                this.RaisePropertyChanged("UserAccountsDestinationItemsSource");
                this.RaisePropertyChanged("ServiceTemplatesDestinationItemsSource");
            });

            #endregion

            #region Implementation of IAddNewExisting<ServiceTemplate> & IRemoveDelete<ServiceTemplate>

            //Used in the FoundOPS admin console to add a new FoundOPS service template
            AddNewItemServiceTemplate = name => VM.ServiceTemplates.CreateNewItem(name);

            //Used in the Service Providers admin console to add an existing (create a child) FoundOPS service template
            AddExistingItemServiceTemplate = foundOPSServiceTemplate =>
                VM.ServiceTemplates.CreateChildServiceTemplate((ServiceTemplate)foundOPSServiceTemplate);

            RemoveItemServiceTemplate = () => { throw new Exception("Cannot remove service templates. Can only delete them."); };

            DeleteItemServiceTemplate = () =>
            {
                var selectedServiceTemplate = VM.ServiceTemplates.SelectedEntity;

                if (selectedServiceTemplate == null)
                    return null;

                if (selectedServiceTemplate.ServiceTemplateLevel == ServiceTemplateLevel.FoundOpsDefined)
                {
                    MessageBox.Show("Cannot delete FoundOPS service templates, yet.");
                    return null;
                }

                VM.ServiceTemplates.DeleteEntity(selectedServiceTemplate);

                return selectedServiceTemplate;
            };

            #endregion

            #region Implementation of IAddNewExisting<UserAccount> & IRemoveDelete<UserAccount>

            AddNewItemUserAccount = name =>
            {
                var newUserAccount = VM.UserAccounts.CreateNewItem(name);
                //CreateNewItem automatically adds it to the Administator role, no need to do it here
                return (UserAccount)newUserAccount;
            };

            AddExistingItemUserAccount = existingItem => SelectedEntity.AdministratorRole.MemberParties.Add((UserAccount)existingItem);

            RemoveItemUserAccount = () =>
            {
                var selectedUserAccount = VM.UserAccounts.SelectedEntity;
                if (selectedUserAccount != null)
                {
                    var rolesToRemoveFrom = this.SelectedEntity.RoleMembership.Where(r => r.OwnerParty == this.SelectedEntity).ToArray();
                 
                    foreach (var role in rolesToRemoveFrom)
                        role.MemberParties.Remove(selectedUserAccount);
                }

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

        /// <summary>
        /// Used in the constructor to setup data loading.
        /// </summary>
        private void SetupDataLoading()
        {
            SetupTopEntityDataLoading(roleId => DomainContext.GetBusinessAccountsForRoleQuery(ContextManager.RoleId));

            //Whenever the business account changes load the ServiceTemplates (with Fields)
            //This is all done in one query, otherwise it would take a long time
            SetupDetailsLoading(selectedEntity => DomainContext.GetBusinessAccountDetailsForRoleQuery(ContextManager.RoleId, selectedEntity.Id));
        }

        #region Logic

        //Must override or else it will create a Party
        protected override Party AddNewEntity(object commandParameter)
        {
            var newBusinessAccount = new BusinessAccount();

            //Add the entity to the EntitySet so it is tracked by the DomainContext
            this.DomainContext.Parties.Add(newBusinessAccount);

            //Add administrator role
            var administratorRole = new Role { Id = Guid.NewGuid(), Name = "Administrator", OwnerParty = newBusinessAccount, RoleType = RoleType.Administrator };

            //Add the mobile and regular blocks to the administrator role
            foreach (var blockId in BlockConstants.RegularBlockIds.Union(BlockConstants.MobileBlockIds))
                administratorRole.RoleBlockToBlockSet.Add(new RoleBlock { BlockId = blockId });

            //Add mobile role
            var mobileRole = new Role { Id = Guid.NewGuid(), Name = "Mobile", OwnerParty = newBusinessAccount, RoleType = RoleType.Mobile };

            //Add the mobile blocks to the mobile only role
            foreach (var blockId in BlockConstants.MobileBlockIds)
                mobileRole.RoleBlockToBlockSet.Add(new RoleBlock { BlockId = blockId });

            return newBusinessAccount;
        }

        /// <summary>
        /// Before deleting make the user verify a string.
        /// </summary>
        /// <param name="checkCompleted">The action to call after checking.</param>
        protected override void CheckDelete(Action<bool> checkCompleted)
        {
            var stringVerifier = new StringVerifier();

            stringVerifier.Succeeded += (sender, args) => checkCompleted(true);
            stringVerifier.Cancelled += (sender, args) => checkCompleted(false);

            stringVerifier.Show();
        }

        /// <summary>
        /// Called when [selected entity changed]. 
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnSelectedEntityChanged(Party oldValue, Party newValue)
        {
            base.OnSelectedEntityChanged(oldValue, newValue);

            var businessAccount = newValue as BusinessAccount;

            if (businessAccount != null)
            {
                //Whenever the SelectedEntity changes setup the ContactInfoVM and DepotLocationVM for that entity

                //SelectedEntityContactInfoVM = new ContactInfoVM(ContactInfoType.OwnedParties, businessAccount.ContactInfoSet);

                //If there is not a depot: set one up
                if (!businessAccount.Depots.Any() && businessAccount.Id != BusinessAccountsConstants.FoundOpsId)
                    businessAccount.Depots.Add(new Location { BusinessAccount = ContextManager.ServiceProvider });

                //Now there will only be one depot per business account
                SelectedDepotLocationVM = new LocationVM(businessAccount.Depots.FirstOrDefault());
            }
        }

        #endregion
    }
}
