using System;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Data.Objects;
using FoundOps.Common.NET;
using System.Collections.Generic;
using FoundOps.Core.Server.Blocks;
using FoundOps.Server.Controllers;
using System.Security.Authentication;
using FoundOps.Server.Authentication;
using FoundOps.Core.Models.CoreEntities;
using System.ServiceModel.DomainServices.Server;
using System.ServiceModel.DomainServices.Hosting;
using FoundOps.Core.Models.CoreEntities.DesignData;
using System.ServiceModel.DomainServices.EntityFramework;

namespace FoundOps.Server.Services.CoreDomainService
{
    /// <summary>
    /// Holds the domain service operations for any core entities:
    /// Businesses, ContactInfo, Files,
    /// Parties, Repeats, Roles, User Accounts
    /// </summary>
    //TODO: Secure
    //(RequiresSecureEndpoint = true)
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

        public IEnumerable<Block> GetBlocks()
        {
            return ObjectContext.Blocks;
        }

        #region Businesses

        public IQueryable<Party> GetBusinessAccountsForRole(Guid roleId)
        {
            var businessForRole = ObjectContext.BusinessForRole(roleId);

            //Make sure current account is a FoundOPS account
            if (businessForRole.Id != BusinessAccountsDesignData.FoundOps.Id)
                return null;

            var businessAccounts = this.ObjectContext.Parties.OfType<BusinessAccount>().Include("ServiceTemplates")
                .Include("ContactInfoSet").Include("OwnedRoles").Include("OwnedRoles.MemberParties");

            //Force load PartyImage
            var t = (from ba in businessAccounts
                     join pi in this.ObjectContext.Files.OfType<PartyImage>()
                         on ba.Id equals pi.Id
                     select pi).ToArray();
            var count = t.Count();

            return businessAccounts;
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

        #region BusinessAccount

        private void DeleteBusinessAccount(BusinessAccount businessAccountToDelete)
        {
            if (businessAccountToDelete.Id == BusinessAccountsConstants.FoundOpsId)
                throw new InvalidOperationException("You are trying to delete the FoundOPS account?!");

            //Make sure current account is a FoundOPS account
            if (!this.ObjectContext.CurrentUserHasFoundOPSAdminAccess())
                throw new AuthenticationException("Invalid attempted access logged for investigation.");

            businessAccountToDelete.Employees.Load();
            foreach (var employee in businessAccountToDelete.Employees.ToList())
                this.DeleteEmployee(employee);

            businessAccountToDelete.Regions.Load();
            foreach (var region in businessAccountToDelete.Regions.ToList())
                this.DeleteRegion(region);

            businessAccountToDelete.RouteTasks.Load();
            foreach (var routeTask in businessAccountToDelete.RouteTasks.ToList())
                this.DeleteRouteTask(routeTask);

            businessAccountToDelete.Routes.Load();
            foreach (var route in businessAccountToDelete.Routes.ToList())
                this.DeleteRoute(route);

            businessAccountToDelete.ServicesToProvide.Load();
            foreach (var service in businessAccountToDelete.ServicesToProvide.ToList())
                this.DeleteService(service);

            businessAccountToDelete.Clients.Load();
            foreach (var client in businessAccountToDelete.Clients.ToList())
                this.DeleteClient(client);

            //This must be after Services
            //Delete all ServiceTemplates that do not have delete ChangeSetEntries.ServiceTemplates.Load();
            var serviceTemplatesToDelete = businessAccountToDelete.ServiceTemplates.Where(st =>
                        !ChangeSet.ChangeSetEntries.Any(cse => cse.Operation == DomainOperation.Delete &&
                        cse.Entity is ServiceTemplate && ((ServiceTemplate)cse.Entity).Id == st.Id))
                        .ToArray();

            foreach (var serviceTemplate in serviceTemplatesToDelete)
                this.DeleteServiceTemplate(serviceTemplate);

            businessAccountToDelete.Invoices.Load();
            foreach (var invoice in businessAccountToDelete.Invoices.ToList())
                this.DeleteInvoice(invoice);
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
            if (!ObjectContext.PreDelete(loadedContactInfo))
                return;

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

            party.Locations.Load();
            party.OwnedLocations.Load();
            party.ClientOwnerReference.Load();
            party.Contacts.Load();
            party.ContactInfoSet.Load();
            party.PartyImageReference.Load();
            party.OwnedRoles.Load();
            party.Vehicles.Load();

            party.RoleMembership.Load();
            party.RoleMembership.Clear();

            if (party.PartyImage != null)
                this.DeleteFile(party.PartyImage);

            var accountLocations = party.OwnedLocations.ToArray();
            foreach (var location in accountLocations)
                this.DeleteLocation(location);

            var partyLocations = party.Locations.ToArray();
            foreach (var location in partyLocations)
                this.DeleteLocation(location);

            var contacts = party.Contacts.ToArray();
            foreach (var contact in contacts)
                this.DeleteContact(contact);

            var contactInfoSetToRemove = party.ContactInfoSet.ToArray();
            foreach (var contactInfo in contactInfoSetToRemove)
                this.DeleteContactInfo(contactInfo);

            var ownedRolesToDelete = party.OwnedRoles.ToArray();
            foreach (var ownedRole in ownedRolesToDelete)
                this.DeleteRole(ownedRole);

            var vehicles = party.Vehicles.ToArray();
            foreach (var vehicle in vehicles)
                this.DeleteVehicle(vehicle);

            var businessAccountToDelete = party as BusinessAccount;
            if (businessAccountToDelete != null)
                DeleteBusinessAccount(businessAccountToDelete);

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
                currentRepeat.EndDate = currentRepeat.EndDate.Value.Date; //Remove time

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

        public void InsertRole(Role role)
        {
            if ((role.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(role, EntityState.Added);
            }
            else
            {
                this.ObjectContext.Roles.AddObject(role);
            }
        }

        private void DeleteRole(Role ownedRole)
        {
            if ((ownedRole.EntityState == EntityState.Detached))
                this.ObjectContext.Roles.Attach(ownedRole);

            //Load and remove the blocks
            ownedRole.Blocks.Load();
            foreach (var block in ownedRole.Blocks.ToArray())
                ownedRole.Blocks.Remove(block);

            //Load and remove the member parties
            ownedRole.MemberParties.Load();
            foreach (var member in ownedRole.MemberParties.ToArray())
                ownedRole.MemberParties.Remove(member);

            this.ObjectContext.Roles.DeleteObject(ownedRole);
        }

        #endregion

        #region User Accounts

        public UserAccount CurrentUserAccount()
        {
            var currentUserAccount = ((ObjectQuery<UserAccount>)AuthenticationLogic.CurrentUserAccountQueryable(this.ObjectContext))
                 .Include("RoleMembership").Include("RoleMembership.OwnerParty").Include("RoleMembership.Blocks").Include("OwnedRoles").Include("OwnedRoles.Blocks").Include("LinkedEmployees")
                 .FirstOrDefault();

            if (currentUserAccount != null)
                currentUserAccount.PartyImageReference.Load();

            return currentUserAccount;
        }

        public IEnumerable<Party> GetAllUserAccounts(Guid roleId)
        {
            var businessForRole = ObjectContext.BusinessForRole(roleId);

            //make sure current account is a foundops account
            if (businessForRole.Id == BusinessAccountsDesignData.FoundOps.Id)
                return this.ObjectContext.Parties.OfType<UserAccount>();

            //Filter by current businesses role
            return businessForRole.OwnedRoles.SelectMany(or => or.MemberParties);
        }

        public void InsertUserAccount(UserAccount userAccount)
        {
            //Set the password to the temporary password
            userAccount.PasswordHash = EncryptionTools.Hash(userAccount.TemporaryPassword);

            //Setup the Default UserAccount role
            var userAccountBlocks = this.ObjectContext.Blocks.Where(block => BlockConstants.UserAccountBlockIds.Any(userAccountBlockId => block.Id == userAccountBlockId));

            RolesDesignData.SetupDefaultUserAccountRole(userAccount, userAccountBlocks);

            if ((userAccount.EntityState != EntityState.Detached))
                this.ObjectContext.ObjectStateManager.ChangeObjectState(userAccount, EntityState.Added);
            else
                this.ObjectContext.Parties.AddObject(userAccount);
        }

        [Update]
        public void UpdateUserAccount(UserAccount currentUserAccount)
        {
            if (!ObjectContext.CurrentUserCanAdministerThisParty(currentUserAccount.Id))
                throw new AuthenticationException();

            //Check if there is a temporary password
            if (string.IsNullOrEmpty(currentUserAccount.TemporaryPassword))
            {
                currentUserAccount.PasswordHash = EncryptionTools.Hash(currentUserAccount.TemporaryPassword);
            }

            this.ObjectContext.Parties.AttachAsModified(currentUserAccount);
        }

        #endregion
    }
}
