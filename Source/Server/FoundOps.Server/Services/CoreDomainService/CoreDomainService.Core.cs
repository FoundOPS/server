using FoundOps.Common.NET;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.CoreEntities.DesignData;
using FoundOps.Core.Models.QuickBooks;
using FoundOps.Server.Authentication;
using FoundOps.Server.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Objects;
using System.Linq;
using System.Reflection;
using System.Security.Authentication;
using System.ServiceModel.DomainServices.EntityFramework;
using System.ServiceModel.DomainServices.Hosting;
using System.ServiceModel.DomainServices.Server;

namespace FoundOps.Server.Services.CoreDomainService
{
    /// <summary>
    /// Holds the domain service operations for any core entities:
    /// Businesses, ContactInfo, Files,
    /// Parties, Repeats, Roles, User Accounts
    /// </summary>
#if DEBUG
    [EnableClientAccess]
#else
    [EnableClientAccess(RequiresSecureEndpoint = true)]
#endif
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

        /// <summary>
        /// Gets all the BusinessAccounts
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public IQueryable<Party> GetBusinessAccountsForRole(Guid roleId)
        {
            var businessForRole = ObjectContext.BusinessOwnerOfRole(roleId);

            //Make sure current account is a FoundOPS account
            return businessForRole.Id != BusinessAccountsDesignData.FoundOps.Id
                       ? null
                       : this.ObjectContext.Parties.OfType<BusinessAccount>().OrderBy(b => b.Name);
        }

        /// <summary>
        /// Gets the BusinessAccount details.
        /// It includes the ContactInfoSet, ServiceTemplates, OwnedRoles, OwnedRoles.MemberParties, and PartyImage.
        /// </summary>
        /// <param name="roleId">The current role id.</param>
        /// <param name="businessAccountId">The businessAccount id.</param>
        public Party GetBusinessAccountDetailsForRole(Guid roleId, Guid businessAccountId)
        {
            var businessForRole = ObjectContext.BusinessOwnerOfRole(roleId);

            //Make sure current account is a FoundOPS account
            if (businessForRole.Id != BusinessAccountsDesignData.FoundOps.Id)
                return null;

            var businessAccount = this.ObjectContext.Parties.OfType<BusinessAccount>().Where(ba => ba.Id == businessAccountId)
                .Include("ContactInfoSet").Include("ServiceTemplates").Include("OwnedRoles").Include("OwnedRoles.MemberParties").FirstOrDefault();

            //Force load PartyImage
            businessAccount.PartyImageReference.Load();

            return businessAccount;
        }

        private void UpdateBusinessAccount(BusinessAccount account)
        {
            //Only FoundOPS admin accounts or a user with admin capabilities for the current account can update the account
            if (!this.ObjectContext.CurrentUserCanAdministerFoundOPS() && !this.ObjectContext.CurrentUserCanAdministerParty(account.Id))
                throw new AuthenticationException("Invalid attempted access logged for investigation.");

            this.ObjectContext.DetachExistingAndAttach(account);

            var originalBusinessAccount = this.ChangeSet.GetOriginal(account);

            //Only FoundOPS's admins can change the MaxRoutes
            if (originalBusinessAccount.MaxRoutes != account.MaxRoutes && !this.ObjectContext.CurrentUserCanAdministerFoundOPS())
                throw new AuthenticationException("Invalid attempted access logged for investigation.");

            this.ObjectContext.Parties.AttachAsModified(account);
        }

        private void DeleteBusinessAccount(BusinessAccount businessAccountToDelete)
        {
            if (businessAccountToDelete.Id == BusinessAccountsConstants.FoundOpsId)
                throw new InvalidOperationException("You are trying to delete the FoundOPS account?!");

            //Make sure current account is a FoundOPS account
            if (!this.ObjectContext.CurrentUserCanAdministerFoundOPS())
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
            //Delete all ServiceTemplates that do not have delete ChangeSetEntries
            var serviceTemplatesToDelete = businessAccountToDelete.ServiceTemplates.Where(st =>
                        !ChangeSet.ChangeSetEntries.Any(cse => cse.Operation == DomainOperation.Delete &&
                        cse.Entity is ServiceTemplate && ((ServiceTemplate)cse.Entity).Id == st.Id))
                        .ToArray();

            foreach (var serviceTemplate in serviceTemplatesToDelete)
                this.DeleteServiceTemplate(serviceTemplate);

            businessAccountToDelete.Invoices.Load();
            foreach (var invoice in businessAccountToDelete.Invoices.ToList())
                this.DeleteInvoice(invoice);

            QuickBooksTools.DeleteAzureTable(businessAccountToDelete.Id);
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
            return this.ObjectContext.OwnerPartyOfRole(roleId);
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
            if (!(account is BusinessAccount))
            {
                this.ObjectContext.Parties.AttachAsModified(account);
            }
            else
                UpdateBusinessAccount((BusinessAccount)account);
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

        /// <summary>
        /// Gets the current user account. Includes RoleMembership, RoleMembership.OwnerParty, OwnedRoles, OwnedRoles.Blocks, LinkedEmployees
        /// and PartyImage.
        /// TODO optimize
        /// </summary>
        public UserAccount CurrentUserAccount()
        {
            var currentUserAccount = ((ObjectQuery<UserAccount>)AuthenticationLogic.CurrentUserAccountQueryable(this.ObjectContext))
                 .Include("RoleMembership").Include("RoleMembership.OwnerParty").Include("RoleMembership.Blocks").Include("OwnedRoles").Include("OwnedRoles.Blocks").Include("LinkedEmployees")
                 .FirstOrDefault();

            if (currentUserAccount != null)
                currentUserAccount.PartyImageReference.Load();

            return currentUserAccount;
        }

        /// <summary>
        /// Returns the UserAccounts the current role has access to.
        /// </summary>
        /// <param name="roleId">The current role id.</param>
        /// <param name="serviceProviderId">(Optional) filter user accounts that are in a owned role of this service provider.</param>
        public IQueryable<Party> GetUserAccounts(Guid roleId, Guid serviceProviderId)
        {
            var businessForRole = ObjectContext.BusinessOwnerOfRole(roleId);

            IQueryable<UserAccount> accesibleUserAccounts;

            //If the current account is a foundops account, return all user accounts
            if (businessForRole.Id == BusinessAccountsDesignData.FoundOps.Id)
            {
                accesibleUserAccounts = this.ObjectContext.Parties.OfType<UserAccount>();
            }
            else
            {
                //If not a FoundOPS account, return the current business's owned roles memberparties
                accesibleUserAccounts =
                    from role in this.ObjectContext.Roles.Where(r => r.OwnerPartyId == businessForRole.Id)
                    from userAccount in this.ObjectContext.Parties.OfType<UserAccount>()
                    where role.MemberParties.Any(p => p.Id == userAccount.Id)
                    select userAccount;
            }

            //If the serviceProviderId context is not empty
            //Filter only user accounts that are member parties of one of it's roles
            if (serviceProviderId != Guid.Empty)
            {
                return from role in this.ObjectContext.Roles.Where(r => r.OwnerPartyId == serviceProviderId)
                       from userAccount in accesibleUserAccounts
                       where role.MemberParties.Any(p => p.Id == userAccount.Id)
                       orderby userAccount.LastName + " " + userAccount.FirstName
                       select userAccount;
            }

            return accesibleUserAccounts.OrderBy(ua => ua.LastName + " " + ua.FirstName);
        }

        /// <summary>
        /// Gets the UserAccount details.
        /// It includes the ContactInfoSet and PartyImage.
        /// </summary>
        /// <param name="roleId">The current role id.</param>
        /// <param name="userAccountId">The user account id.</param>
        public Party GetUserAccountDetailsForRole(Guid roleId, Guid userAccountId)
        {
            var userAccount = GetUserAccounts(roleId, Guid.Empty).Include("ContactInfoSet").FirstOrDefault(ua => ua.Id == userAccountId);

            //Force load PartyImage
            userAccount.PartyImageReference.Load();

            return userAccount;
        }

        /// <summary>
        /// Returns the UserAccounts the current role has access to filtered by the search text.
        /// </summary>
        /// <param name="roleId">The current role id.</param>
        /// <param name="searchText">The search text.</param>
        /// <returns></returns>
        public IQueryable<Party> SearchUserAccountsForRole(Guid roleId, string searchText)
        {
            var userAccounts = GetUserAccounts(roleId, Guid.Empty).OfType<UserAccount>();

            if (!String.IsNullOrEmpty(searchText))
                userAccounts = userAccounts.Where(ua => ua.FirstName.StartsWith(searchText) || ua.LastName.StartsWith(searchText));

            return userAccounts.OrderBy(ua => ua.LastName + " " + ua.FirstName);
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
            if (!ObjectContext.CurrentUserCanAdministerParty(currentUserAccount.Id))
                throw new AuthenticationException();

            //Check if there is a temporary password
            if (string.IsNullOrEmpty(currentUserAccount.TemporaryPassword))
                currentUserAccount.PasswordHash = EncryptionTools.Hash(currentUserAccount.TemporaryPassword);

            this.ObjectContext.Parties.AttachAsModified(currentUserAccount);
        }

        #endregion
    }
}
