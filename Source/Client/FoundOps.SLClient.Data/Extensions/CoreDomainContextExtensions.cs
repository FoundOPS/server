using FoundOps.Common.Silverlight.Tools;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Context.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.DomainServices.Client;

//This is a partial class and must be in the same namespace as the generated CoreDomainService
// ReSharper disable CheckNamespace
namespace FoundOps.Server.Services.CoreDomainService
// ReSharper restore CheckNamespace
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

    public partial class CoreDomainContext
    {
        /// <summary>
        /// An event handler for the ChangedRejected event.
        /// </summary>
        /// <param name="sender">The DomainContext that rejected changes.</param>
        /// <param name="rejectedAddedEntities">The rejected added entities.</param>
        /// <param name="rejectedModifiedEntities">The rejected modified entities.</param>
        /// <returns></returns>
        public delegate void ChangesRejectedHandler(DomainContext sender, Entity[] rejectedAddedEntities, Entity[] rejectedModifiedEntities);

        /// <summary>
        /// Called when changes are rejected.
        /// </summary>
        public event ChangesRejectedHandler ChangesRejected;

        private readonly List<string> _contactInfoTypes = new List<string> { "Phone Number", "Email Address", "Website", "Fax Number", "Other" };

        partial void OnCreated()
        {
            Action contactInfoDataServicesChanged = () =>
            {
                RaisePropertyChanged("LocationsContactInfoDataService");
                RaisePropertyChanged("PartyContactInfoDataService");
            };

            this.ContactInfos.EntityAdded += (_, __) => contactInfoDataServicesChanged();
            this.ContactInfos.EntityRemoved += (_, __) => contactInfoDataServicesChanged();
        }

        /// <summary>
        //  Revert all pending changes for this System.ServiceModel.DomainServices.Client.DomainContext.
        /// </summary>
        public new void RejectChanges()
        {
            var changes = this.EntityContainer.GetChanges();

            var addedEntities = changes.AddedEntities.ToArray();
            var modifiedEntities = changes.ModifiedEntities.ToArray();

            //https://github.com/FoundOPS/FoundOPS/wiki/Files
            //Remove inserted files from cloud storage that are now going to be removed
            var filesToRemove =
                changes.Where(c => c as File != null && c.EntityState == EntityState.New).Cast<File>();

            foreach (var file in filesToRemove)
                FileManager.DeleteFile(file.PartyId, file.Id);

            base.RejectChanges();

            if (ChangesRejected != null)
                ChangesRejected(this, addedEntities, modifiedEntities);
        }

        /// <summary>
        /// Gets the Locations ContactInfoDataService.
        /// </summary>
        /// <param name="currentRoleId">The current role id.</param>
        /// <returns></returns>
        public IContactInfoDataService LocationsContactInfoDataService(Guid currentRoleId)
        {
            var locationContactInfos = this.ContactInfos.Where(ci => ci.Location != null && ci.Location.OwnerPartyId == currentRoleId);
            return
                new ContactInfoDataService(
                    locationContactInfos.Select(ci => ci.Type).Union(_contactInfoTypes).Distinct().Where(s => s != null).OrderBy(s => s),
                    locationContactInfos.Select(ci => ci.Label).Distinct().Where(s => s != null).OrderBy(s => s));
        }

        /// <summary>
        /// Gets the Party ContactInfoDataService.
        /// </summary>
        /// <param name="currentRoleId">The current role id.</param>
        /// <returns></returns>
        public IContactInfoDataService PartyContactInfoDataService(Guid currentRoleId)
        {
            var partyContactInfos = this.ContactInfos.Where(ci => ci.PartyId != Guid.Empty);
            return
                new ContactInfoDataService(
                    partyContactInfos.Select(ci => ci.Type).Union(_contactInfoTypes).Distinct().Where(s => s != null).OrderBy(s => s),
                    partyContactInfos.Select(ci => ci.Label).Distinct().Where(s => s != null).OrderBy(s => s));
        }
    }
}