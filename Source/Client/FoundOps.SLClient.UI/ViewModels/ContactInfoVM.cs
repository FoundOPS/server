using System;
using System.Reactive.Linq;
using FoundOps.Common.Tools;
using GalaSoft.MvvmLight.Command;
using MEFedMVVM.ViewModelLocator;
using FoundOps.SLClient.Data.Services;
using System.ComponentModel.Composition;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Context.Services.Interface;
using System.ServiceModel.DomainServices.Client;
using FoundOps.Common.Silverlight.UI.ViewModels;
using FoundOps.Server.Services.CoreDomainService;

namespace FoundOps.SLClient.UI.ViewModels
{
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

        /// <summary>
        /// Gets the ContactInfoSet.
        /// </summary>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="ContactInfoVM"/> class.
        /// </summary>
        /// <param name="contactInfoType">Type of the contact info.</param>
        /// <param name="contactInfoSet">The contact info set.</param>
        [ImportingConstructor]
        public ContactInfoVM(ContactInfoType contactInfoType, EntityCollection<ContactInfo> contactInfoSet)
        {
            //Update the ContactInfoDataService whenever the LocationsContactInfoDataService or the PartyContactInfoDataService changes
            Manager.Data.Context.FromAnyPropertyChanged()
                .Where(pce => pce.PropertyName == "LocationsContactInfoDataService" || pce.PropertyName == "PartyContactInfoDataService")
                .AsGeneric().AndNow().SubscribeOnDispatcher().Subscribe(_ =>
                {
                    if (Manager.Context.OwnerAccount != null)
                        switch (contactInfoType)
                        {
                            case ContactInfoType.Locations:
                                ContactInfoDataService = Manager.Data.Context.LocationsContactInfoDataService(Manager.Context.OwnerAccount.Id);
                                break;
                            case ContactInfoType.OwnedParties:
                                ContactInfoDataService = Manager.Data.Context.PartyContactInfoDataService(Manager.Context.OwnerAccount.Id);
                                break;
                        }
                });

            ContactInfoSet = contactInfoSet;
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
