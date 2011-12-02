using System.ServiceModel.DomainServices.Client;
using FoundOps.Common.Silverlight.UI.ViewModels;
using FoundOps.Core.Context.Services;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.ViewModels;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Contains the logic for displaying Party
    /// </summary>
    public abstract class PartyVM : CoreEntityVM
    {
        #region Public Properties

        private Party _selectedParty;
        public Party SelectedParty
        {
            get
            {
                return _selectedParty;
            }
            set
            {
                if (SelectedParty == value) return;

                _selectedParty = value;
                ContactInfoVM = new ContactInfoVM(DataManager, ContactInfoType.OwnedParties, this.ContactInfoSet);
                this.RaisePropertyChanged("SelectedParty");
                if (SelectedParty == null) return;
            }
        }

        protected PartyVM(DataManager dataManager):base(dataManager)
        {
        }

        #region Implementation of IContactInfoDataService

        public EntityCollection<ContactInfo> ContactInfoSet
        {
            get
            {
                return SelectedParty == null ? null : SelectedParty.ContactInfoSet;
            }
        }

        #endregion

        private ContactInfoVM _contactInfoVM;
        public ContactInfoVM ContactInfoVM
        {
            get { return _contactInfoVM; }
            protected set
            {
                _contactInfoVM = value;
                this.RaisePropertyChanged("ContactInfoVM");
            }
        }

        #endregion
    }
}
