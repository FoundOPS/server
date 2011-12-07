using System.Reflection;
using System.Data.Objects;
using FoundOps.Common.NET;
using System.Collections.Generic;
using FoundOps.Server.Controllers;
using System.Security.Authentication;
using FoundOps.Server.Authentication;
using FoundOps.Core.Models.CoreEntities;
using System.ServiceModel.DomainServices.Server;
using FoundOps.Core.Models.CoreEntities.DesignData;

namespace FoundOps.Server.Services.CoreDomainService
{
    using System;
    using System.Data;
    using System.Linq;
    using System.ServiceModel.DomainServices.EntityFramework;
    using System.ServiceModel.DomainServices.Hosting;

    /// <summary>
    /// Holds the domain service operations for any core entities:
    /// Businesses, ContactInfo, Files,
    /// Parties, Repeats, Roles, User Accounts
    /// </summary>
    //TODO: Secure
    [EnableClientAccess]
    public partial class CoreDomainService : LinqToEntitiesDomainService<CoreEntitiesContainer>
    {
        protected override bool PersistChangeSet()
        {
            //Get any files that were just deleted
            var deletedFiles =
                this.ChangeSet.ChangeSetEntries.Where(cse => cse.Entity is File && cse.Operation == DomainOperation.Delete)
                .Select(cse => cse.Entity as File);

            //Delete the files from cloud storage
            foreach (var fileToDelete in deletedFiles)
                FileController.DeleteBlobHelper(fileToDelete.OwnerParty.Id, fileToDelete.Id);

            return base.PersistChangeSet();
        }

        [Query]
        public IEnumerable<Address> GetAddresses()
        {
            throw new NotSupportedException("Exists solely to generate Address in the clients data project");
        }

        public IEnumerable<Block> GetPublicBlocks()
        {
            return ObjectContext.Blocks.Where(block => block.LoginNotRequired);
        }

        #region Businesses

        public IQueryable<Party> GetBusinessAccountsForRole(Guid roleId)
        {
            var businessForRole = ObjectContext.BusinessForRole(roleId);

            //make sure current account is a foundops account
            if (businessForRole.Id == BusinessAccountsDesignData.FoundOps.Id)
                return this.ObjectContext.Parties.OfType<BusinessAccount>().Include("ServiceTemplates").Include("ContactInfoSet").Include("PartyImage");
            return null;
        }

        public BusinessAccount BusinessAccountWithClientsForRole(Guid roleId)
        {
            var businessForRole = ObjectContext.BusinessForRole(roleId);
            return businessForRole == null
                       ? null
                       : this.ObjectContext.Parties.OfType<BusinessAccount>().Include("Clients")
                       .FirstOrDefault(ba => ba.Id == businessForRole.Id);
        }

        public Business BusinessForRole(Guid roleId)
        {
            return ObjectContext.BusinessForRole(roleId);
        }

        #endregion

        #region ContactInfo

        public IEnumerable<string> ContactInfoLabelsForParty(Guid currentPartyId)
        {
            var contactInfoLabels = new List<string> { "Primary", "Mobile" };

            var currentParty = this.ObjectContext.Parties.FirstOrDefault(a => a.Id == currentPartyId);

            if (currentParty == null)
                return contactInfoLabels.OrderBy(s => s);

            //Add all labels current user can administer that are the same account type. (Business Labels might be different than User Labels)
            var accountsCurrentUserCanAdminister = AuthenticationLogic.AdministratorRoles(ObjectContext.RolesCurrentUserHasAccessTo()).Select(r => r.OwnerParty);

            MethodInfo method = typeof(Queryable).GetMethod("OfType");
            MethodInfo generic = method.MakeGenericMethod(new Type[] { currentParty.GetType() });
            var accountsCurrentUserCanAdministerOfSameType = (IEnumerable<Party>)generic.Invoke
                  (null, new object[] { accountsCurrentUserCanAdminister });

            foreach (var accountCurrentUserCanAdministerOfSameType in accountsCurrentUserCanAdministerOfSameType)
            {
                accountCurrentUserCanAdministerOfSameType.ContactInfoSet.Load();
                contactInfoLabels.AddRange(accountCurrentUserCanAdministerOfSameType.ContactInfoSet.Select(ci => ci.Label));
            }

            contactInfoLabels = new List<string>(contactInfoLabels.Distinct());

            return contactInfoLabels.OrderBy(s => s);
        }

        public IEnumerable<string> ContactInfoTypesForParty(Guid currentPartyId)
        {
            var contactInfoTypes = new List<string> { "Phone Number", "Email Address", "Website", "Fax Number", "Other" };

            var currentParty = this.ObjectContext.Parties.FirstOrDefault(a => a.Id == currentPartyId);

            if (currentParty == null)
                return contactInfoTypes.OrderBy(s => s);

            currentParty.ContactInfoSet.Load();

            contactInfoTypes.AddRange(currentParty.ContactInfoSet.Select(ci => ci.Type));
            contactInfoTypes = new List<string>(contactInfoTypes.Distinct());

            return contactInfoTypes.OrderBy(s => s);
        }

        public IQueryable<ContactInfo> GetContactInfoSet()
        {
            return this.ObjectContext.ContactInfoSet;
        }

        [Insert]
        public void InsertContactInfo(ContactInfo contactInfo)
        {
            if ((contactInfo.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(contactInfo, EntityState.Added);
            }
            else
            {
                this.ObjectContext.ContactInfoSet.AddObject(contactInfo);
            }
        }

        public void UpdateContactInfo(ContactInfo contactInfo)
        {
            this.ObjectContext.ContactInfoSet.AttachAsModified(contactInfo);
        }

        public void DeleteContactInfo(ContactInfo contactInfo)
        {
            var loadedContactInfo = this.ObjectContext.ContactInfoSet.FirstOrDefault(ci => ci.Id == contactInfo.Id);

            if (loadedContactInfo != null)
                this.ObjectContext.Detach(contactInfo);

            if ((contactInfo.EntityState == EntityState.Detached))
                this.ObjectContext.ContactInfoSet.Attach(contactInfo);

            this.ObjectContext.ContactInfoSet.DeleteObject(contactInfo);
        }

        #endregion

        #region File

        public IQueryable<File> GetFilesForParty(Guid roleId)
        {
            //return FileController.GetBlobUri(roleId.ToString(), );
            return this.ObjectContext.Files.Where(f => f.OwnerParty.Id == roleId);
        }

        public void InsertFile(File file)
        {
            if ((file.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(file, EntityState.Added);
            }
            else
            {
                this.ObjectContext.Files.AddObject(file);
            }
        }

        public void UpdateFile(File file)
        {
            this.ObjectContext.Files.AttachAsModified(file);
        }

        public void DeleteFile(File file)
        {
            if ((file.EntityState == EntityState.Detached))
            {
                this.ObjectContext.Files.Attach(file);
            }

            this.ObjectContext.Files.DeleteObject(file);
        }

        #endregion

        #region Party

        public Party PartyForRole(Guid roleId)
        {
            return this.ObjectContext.PartyForRole(roleId);
        }

        public Party PartyToAdministerForRole(Guid roleId)
        {
            var role =
                AuthenticationLogic.AdministratorRoles(this.ObjectContext.RolesCurrentUserHasAccessTo()).FirstOrDefault(r => r.Id == roleId);

            var ownerParty = role.OwnerParty;
            ownerParty.ContactInfoSet.Load();
            return role.OwnerParty;
        }

        public IQueryable<Party> GetPartys()
        {
            return this.ObjectContext.Parties;
        }

        public void InsertParty(Party account)
        {
            if ((account.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(account, EntityState.Added);
            }
            else
            {
                this.ObjectContext.Parties.AddObject(account);
            }
        }

        public void UpdateParty(Party account)
        {
            this.ObjectContext.Parties.AttachAsModified(account);
        }

        public void DeleteParty(Party party)
        {
            var loadedParty = this.ObjectContext.Parties.FirstOrDefault(p => p.Id == party.Id);
            if (loadedParty != null)
                this.ObjectContext.Detach(loadedParty);

            if ((party.EntityState == EntityState.Detached))
                this.ObjectContext.Parties.Attach(party);

            party.OwnedLocations.Load();
            party.OfClients.Load();
            party.ContactInfoSet.Load();
            party.PartyImageReference.Load();
            
            if(party.PartyImage!=null)
                this.DeleteFile(party.PartyImage);

            var partyLocations = party.Locations.ToList();
            foreach (var location in partyLocations)
                this.DeleteLocation(location);

            var accountLocations = party.OwnedLocations.ToList();
            foreach (var location in accountLocations)
                this.DeleteLocation(location);

            var ofClients = party.OfClients.ToList();
            foreach (var ofClient in ofClients)
                this.DeleteClient(ofClient);

            var contactInfoSetToRemove = party.ContactInfoSet.ToList();
            foreach (var contactInfo in contactInfoSetToRemove)
            {
                this.DeleteContactInfo(contactInfo);
            }

            if ((party.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(party, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.Parties.Attach(party);
                this.ObjectContext.Parties.DeleteObject(party);
            }
        }

        #endregion

        #region Repeat

        public IQueryable<Repeat> GetRepeats()
        {
            return this.ObjectContext.Repeats;
        }

        public void InsertRepeat(Repeat repeat)
        {
            repeat.StartDate = repeat.StartDate.Date;

            if (repeat.EndDate != null)
                repeat.StartDate = repeat.EndDate.Value.Date;

            if ((repeat.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(repeat, EntityState.Added);
            }
            else
            {
                this.ObjectContext.Repeats.AddObject(repeat);
            }
        }

        public void UpdateRepeat(Repeat currentRepeat)
        {
            currentRepeat.StartDate = currentRepeat.StartDate.Date; //Remove time

            if (currentRepeat.EndDate != null)
                currentRepeat.StartDate = currentRepeat.EndDate.Value.Date; //Remove time

            this.ObjectContext.Repeats.AttachAsModified(currentRepeat);
        }

        public void DeleteRepeat(Repeat repeat)
        {
            if ((repeat.EntityState == EntityState.Detached))
                this.ObjectContext.Repeats.Attach(repeat);

            this.ObjectContext.Repeats.DeleteObject(repeat);
        }

        #endregion

        #region Roles

        public IEnumerable<Role> GetRoles()
        {
            var availableRoles = this.ObjectContext.RolesCurrentUserHasAccessTo();

            return availableRoles;
        }

        #endregion

        #region User Accounts

        public UserAccount CurrentUserAccount()
        {
            var currentUserAccount = ((ObjectQuery<UserAccount>)AuthenticationLogic.CurrentUserAccountQueryable(this.ObjectContext))
                 .Include("RoleMembership").Include("RoleMembership.OwnerParty").Include("RoleMembership.Blocks").Include("OwnedRoles").Include("OwnedRoles.Blocks")
                 .FirstOrDefault();

            if (currentUserAccount != null)
                currentUserAccount.PartyImageReference.Load();

            return currentUserAccount;
        }

        public IQueryable<Party> GetAllUserAccounts(Guid roleId)
        {
            var businessForRole = ObjectContext.BusinessForRole(roleId);

            //make sure current account is a foundops account
            if (businessForRole.Id == BusinessAccountsDesignData.FoundOps.Id)
                return this.ObjectContext.Parties.OfType<UserAccount>();

            return this.ObjectContext.Parties.OfType<UserAccount>();
        }

        [Update]
        public void UpdateUserAccount(UserAccount currentUserAccount)
        {
            if (!ObjectContext.CurrentUserCanAdministerThisParty(currentUserAccount.Id))
                throw new AuthenticationException();

            this.ObjectContext.Parties.AttachAsModified(currentUserAccount);
        }

        #endregion
    }
}


