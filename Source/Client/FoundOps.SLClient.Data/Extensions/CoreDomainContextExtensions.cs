using FoundOps.Common.Silverlight.Tools;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Context.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
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
        /// Clients' Contact Info
        /// </summary>
        Clients,
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
    /// The changes rejected event.
    /// </summary>
    public class ChangedRejectedEventArgs
    {
        /// <summary>
        /// The DomainContext sender.
        /// </summary>
        public DomainContext Sender { get; private set; }

        /// <summary>
        /// The rejected added entities.
        /// </summary>
        public Entity[] RejectedAddedEntities { get; private set; }

        /// <summary>
        /// The rejected modified entities.
        /// </summary>
        public Entity[] RejectedModifiedEntities { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangedRejectedEventArgs"/> class.
        /// </summary>
        /// <param name="sender">The DomainContext that rejected changes.</param>
        /// <param name="rejectedAddedEntities">The rejected added entities.</param>
        /// <param name="rejectedModifiedEntities">The rejected modified entities.</param>
        public ChangedRejectedEventArgs(DomainContext sender, Entity[] rejectedAddedEntities, Entity[] rejectedModifiedEntities)
        {
            Sender = sender;
            RejectedAddedEntities = rejectedAddedEntities;
            RejectedModifiedEntities = rejectedModifiedEntities;
        }
    }

    public partial class CoreDomainContext
    {
        /// <summary>
        /// An event handler for the ChangedRejected event.
        /// </summary>
        public delegate void ChangesRejectedHandler(ChangedRejectedEventArgs changedRejectedEventArgs);

        /// <summary>
        /// Called when changes are rejected.
        /// </summary>
        public event ChangesRejectedHandler ChangesRejected;

        private readonly Subject<ChangedRejectedEventArgs> _changesRejectedSubject = new Subject<ChangedRejectedEventArgs>();
        /// <summary>
        /// Pushes whenever the changes are rejected.
        /// </summary>
        public IObservable<ChangedRejectedEventArgs> ChangesRejectedObservable { get { return _changesRejectedSubject.AsObservable(); } }

        private readonly List<string> _contactInfoTypes = new List<string> { "Phone Number", "Email Address", "Website", "Fax Number", "Other" };

        partial void OnCreated()
        {
            Action contactInfoDataServicesChanged = () =>
            {
                RaisePropertyChanged("ClientsContactInfoDataService");
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
            var changes = GetChangeSet();

            var changesRejectedArgs = new ChangedRejectedEventArgs(this, changes.AddedEntities.ToArray(), changes.ModifiedEntities.ToArray());

            //https://github.com/FoundOPS/FoundOPS/wiki/Files
            //Remove inserted files from cloud storage that are now going to be removed
            var filesToRemove =
                changes.Where(c => c as File != null && c.EntityState == EntityState.New).Cast<File>();

            foreach (var file in filesToRemove)
                FileManager.DeleteFile(file.PartyId, file.Id);

            base.RejectChanges();

            _changesRejectedSubject.OnNext(changesRejectedArgs);

            if (ChangesRejected != null)
                ChangesRejected(changesRejectedArgs);
        }

        /// <summary>
        /// Returns the ChangeSet
        /// </summary>
        public EntityChangeSet GetChangeSet()
        {
            var changes = this.EntityContainer.GetChanges();
            return changes;
        }

        /// <summary>
        /// Gets the Clients ContactInfoDataService.
        /// </summary>
        /// <param name="currentBusinessAccountId">The current business account id.</param>
        /// <returns></returns>
        public IContactInfoDataService ClientsContactInfoDataService(Guid currentBusinessAccountId)
        {
            var clientContactInfos = this.ContactInfos.Where(ci => ci.Client != null && ci.Client.BusinessAccountId == currentBusinessAccountId);
            return
                new ContactInfoDataService(
                    clientContactInfos.Select(ci => ci.Type).Union(_contactInfoTypes).Distinct().Where(s => s != null).OrderBy(s => s),
                    clientContactInfos.Select(ci => ci.Label).Distinct().Where(s => s != null).OrderBy(s => s));
        }

        /// <summary>
        /// Gets the Locations ContactInfoDataService.
        /// </summary>
        /// <param name="currentBusinessAccountId">The current business account id.</param>
        public IContactInfoDataService LocationsContactInfoDataService(Guid currentBusinessAccountId)
        {
            var locationContactInfos = this.ContactInfos.Where(ci => ci.Location != null && ci.Location.BusinessAccountId == currentBusinessAccountId);
            return
                new ContactInfoDataService(
                    locationContactInfos.Select(ci => ci.Type).Union(_contactInfoTypes).Distinct().Where(s => s != null).OrderBy(s => s),
                    locationContactInfos.Select(ci => ci.Label).Distinct().Where(s => s != null).OrderBy(s => s));
        }

        /// <summary>
        /// Gets the Party ContactInfoDataService.
        /// </summary>
        /// <param name="currentBusinessAccountId">The current business account id.</param>
        public IContactInfoDataService PartyContactInfoDataService(Guid currentBusinessAccountId)
        {
            var partyContactInfos = this.ContactInfos.Where(ci => ci.PartyId != Guid.Empty);
            return
                new ContactInfoDataService(
                    partyContactInfos.Select(ci => ci.Type).Union(_contactInfoTypes).Distinct().Where(s => s != null).OrderBy(s => s),
                    partyContactInfos.Select(ci => ci.Label).Distinct().Where(s => s != null).OrderBy(s => s));
        }
    }
}