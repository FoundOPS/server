using System.ComponentModel.Composition;
using System.ServiceModel.DomainServices.Client;
using FoundOps.Common.Silverlight.UI.ViewModels;
using FoundOps.Core.Context.Services;
using FoundOps.Core.Context.Services.Interface;
using FoundOps.SLClient.Data.Services;
using FoundOps.Core.Models.CoreEntities;
using GalaSoft.MvvmLight.Command;
using MEFedMVVM.ViewModelLocator;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// The different ContactInfoTypes
    /// </summary>
    public enum ContactInfoType
    {
        /// <summary>
        /// Locations' Contact Info
        /// </summary>
        Locations,
        /// <summary>
        /// OwnedParties' Contact Info
        /// </summary>
        OwnedParties
    }

    /// <summary>
    /// Contains the logic for displaying ContactInfo
    /// </summary>
    [ExportViewModel("ContactInfoVM")]
    public class ContactInfoVM : ThreadableVM
    {
        #region Public Properties

        private IContactInfoDataService _contactInfoDataService;
        /// <summary>
        /// Gets the contact info data service.
        /// </summary>
        public IContactInfoDataService ContactInfoDataService
        {
            get { return _contactInfoDataService; }
            private set
            {
                _contactInfoDataService = value;
                RaisePropertyChanged("ContactInfoDataService");
            }
        }

        public EntityCollection<ContactInfo> ContactInfoSet { get; private set; }

        private RelayCommand _addNewContactInfoCommand;
        /// <summary>
        /// Gets the add new contact info command.
        /// </summary>
        public RelayCommand AddNewContactInfoCommand
        {
            get { return _addNewContactInfoCommand; }
            private set
            {
                _addNewContactInfoCommand = value;
                this.RaisePropertyChanged("AddNewContactInfoCommand");
            }
        }

        private RelayCommand<ContactInfo> _deleteContactInfoCommand;
        /// <summary>
        /// Gets the delete contact info command.
        /// </summary>
        public RelayCommand<ContactInfo> DeleteContactInfoCommand
        {
            get { return _deleteContactInfoCommand; }
            private set
            {
                _deleteContactInfoCommand = value;
                this.RaisePropertyChanged("DeleteContactInfoCommand");
            }
        }

        #endregion

        //Locals
        private readonly DataManager _dataManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContactInfoVM"/> class.
        /// </summary>
        /// <param name="contactInfoDataService">The contact info data service.</param>
        /// <param name="contactInfoSet">The contact info set.</param>
        [ImportingConstructor]
        public ContactInfoVM(IContactInfoDataService contactInfoDataService, EntityCollection<ContactInfo> contactInfoSet)
        {
            ContactInfoSet = contactInfoSet;
            ContactInfoDataService = contactInfoDataService;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContactInfoVM"/> class.
        /// </summary>
        /// <param name="dataManager">The data manager.</param>
        /// <param name="contactInfoType">Type of the contact info.</param>
        /// <param name="contactInfoSet">The contact info set.</param>
        [ImportingConstructor]
        public ContactInfoVM(DataManager dataManager, ContactInfoType contactInfoType, EntityCollection<ContactInfo> contactInfoSet)
        {
            _dataManager = dataManager;
            dataManager.Context.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "LocationsContactInfoDataService" || e.PropertyName == "PartyContactInfoDataService")
                    UpdateContactInfoDataService(contactInfoType);
            };
            ContactInfoSet = contactInfoSet;
            UpdateContactInfoDataService(contactInfoType);
        }

        private void UpdateContactInfoDataService(ContactInfoType contactInfoType)
        {
            if (_dataManager.ContextManager.OwnerAccount != null)
                if (contactInfoType == ContactInfoType.Locations)
                    ContactInfoDataService = _dataManager.Context.LocationsContactInfoDataService(_dataManager.ContextManager.OwnerAccount.Id);
                else if (contactInfoType == ContactInfoType.OwnedParties)
                    ContactInfoDataService = _dataManager.Context.PartyContactInfoDataService(_dataManager.ContextManager.OwnerAccount.Id);
        }

        #region Logic

        /// <summary>
        /// Called when [add new contact info].
        /// </summary>
        public void OnAddNewContactInfo()
        {
            ContactInfoSet.Add(new ContactInfo { Type = "Phone Number", Label = "" });
            RaisePropertyChanged("ContactInfoSet");
        }

        /// <summary>
        /// Called when [delete contact info command].
        /// </summary>
        /// <param name="contactInfoToDelete">The contact info to delete.</param>
        public void OnDeleteContactInfoCommand(ContactInfo contactInfoToDelete)
        {
            ContactInfoSet.Remove(contactInfoToDelete);
            RaisePropertyChanged("ContactInfoSet");
        }

        protected override void RegisterCommands()
        {
            AddNewContactInfoCommand = new RelayCommand(OnAddNewContactInfo);
            DeleteContactInfoCommand = new RelayCommand<ContactInfo>(OnDeleteContactInfoCommand);
        }

        protected override void RegisterMessages()
        {
        }

        #endregion
    }
}
