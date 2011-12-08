using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.DomainServices.Client;
using FoundOps.Common.Silverlight.Tools;
using FoundOps.Core.Context.Services.Interface;
using FoundOps.Core.Models.CoreEntities;

//This is a partial class and must be in the same namespace as the generated CoreDomainService
// ReSharper disable CheckNamespace
namespace FoundOps.Server.Services.CoreDomainService
// ReSharper restore CheckNamespace
{
    public partial class CoreDomainContext
    {
        private readonly List<string> _contactInfoTypes = new List<string> { "Phone Number", "Email Address", "Website", "Fax Number", "Other" };

        /// <summary>
        //  Revert all pending changes for this System.ServiceModel.DomainServices.Client.DomainContext.
        /// </summary>
        public new void RejectChanges()
        {
            //https://github.com/FoundOPS/FoundOPS/wiki/Files
            //Remove inserted files from cloud storage that are now going to be removed
            var filesToRemove =
                this.EntityContainer.GetChanges().Where(c => c as File != null && c.EntityState == EntityState.New).Cast<File>();

            foreach (var file in filesToRemove)
                FileManager.DeleteFile(file.PartyId, file.Id);

            base.RejectChanges();
        }

        partial void OnCreated()
        {
            this.PropertyChanged += (s, e) =>
                                        {
                                            if (e.PropertyName == "HasChanges")
                                            {

                                            }
                                        };

            this.ContactInfos.EntityAdded += ContactInfosCollectionChanged;
            this.ContactInfos.EntityRemoved += ContactInfosCollectionChanged;
        }

        void ContactInfosCollectionChanged(object sender, System.ServiceModel.DomainServices.Client.EntityCollectionChangedEventArgs<ContactInfo> e)
        {
            RaisePropertyChanged("LocationsContactInfoDataService");
            RaisePropertyChanged("PartyContactInfoDataService");
        }

        public IContactInfoDataService LocationsContactInfoDataService(Guid currentRoleId)
        {
            var locationContactInfos = this.ContactInfos.Where(ci => ci.Location != null && ci.Location.OwnerPartyId == currentRoleId);
            return new ContactInfoDataService(locationContactInfos.Select(ci => ci.Type).Union(_contactInfoTypes).Distinct().OrderBy(s => s), locationContactInfos.Select(ci => ci.Label).Distinct().OrderBy(s => s));
        }

        public IContactInfoDataService PartyContactInfoDataService(Guid currentRoleId)
        {
            var partyContactInfos = this.ContactInfos.Where(ci => ci.PartyId != Guid.Empty);
            return new ContactInfoDataService(partyContactInfos.Select(ci => ci.Type).Union(_contactInfoTypes).Distinct().OrderBy(s => s), partyContactInfos.Select(ci => ci.Label).Distinct().OrderBy(s => s));
        }
    }
}