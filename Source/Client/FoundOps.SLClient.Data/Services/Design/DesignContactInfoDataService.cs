using System.ComponentModel;
using MEFedMVVM.ViewModelLocator;
using System.Collections.Generic;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Context.Services.Interface;
using System.ServiceModel.DomainServices.Client;
using FoundOps.Core.Models.CoreEntities.DesignData;

namespace FoundOps.Core.Context.Services.Design
{
    [ExportService(ServiceType.DesignTime, typeof(IContactInfoDataService))]
    public class DesignContactInfoDataService :  IContactInfoDataService
    {
        public EntityCollection<ContactInfo> ContactInfoSet
        {
            get
            {
                var contactInfoData = new ContactInfoDesignData();
                var account = new Party();
                foreach (var contactInfo in contactInfoData.DesignUserContactInfoList)
                    account.ContactInfoSet.Add(contactInfo);
                return account.ContactInfoSet;
            }
        }

        public IEnumerable<string> ContactInfoTypes
        {
            get { return ContactInfoDesignData.DesignContactInfoTypes; }
        }

        public IEnumerable<string> ContactInfoLabels
        {
            get { return ContactInfoDesignData.DesignContactInfoLabels; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
