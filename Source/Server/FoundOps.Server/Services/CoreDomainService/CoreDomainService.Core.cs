using FoundOps.Common.NET;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.CoreEntities.DesignData;
using FoundOps.Core.Tools;
using FoundOps.Server.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Security.Authentication;
using System.ServiceModel;
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
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public partial class CoreDomainService : LinqToEntitiesDomainService<CoreEntitiesContainer>
    {
        protected override bool PersistChangeSet()
        {
            //Get any files that were just deleted
            var deletedFiles = this.ChangeSet.ChangeSetEntries.Where(cse => cse.Entity is File && cse.Operation == DomainOperation.Delete)
                .Select(cse => cse.Entity as File).ToArray();

            var persistedChangeSet = base.PersistChangeSet();
            if (persistedChangeSet)
            {
                //Delete the files from cloud storage
                foreach (var fileToDelete in deletedFiles)
                    FileController.DeleteBlobHelper(fileToDelete.OwnerParty.Id, fileToDelete.Id);
            }

            return persistedChangeSet;
        }

        [Query]
        public IEnumerable<Address> GetAddresses()
        {
            throw new NotSupportedException("Exists solely to generate Address in the clients data project");
        }

        public IEnumerable<Block> GetBlocks()
        {
            throw new NotSupportedException("Exists solely to generate Blocks in the clients data project");
        }

        #region Businesses and BusinessAccounts

        /// <summary>
        /// Gets all the BusinessAccounts and their Depots
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public IQueryable<Party> GetBusinessAccountsForRole(Guid roleId)
        {
            var businessAccount = ObjectContext.Owner(roleId).First();

            //Make sure current account is a FoundOPS account
            return businessAccount.Id != BusinessAccountsDesignData.FoundOps.Id
                       ? null
                       : this.ObjectContext.Parties.OfType<BusinessAccount>().Include(ba => ba.Depots).OrderBy(b => b.Name);
        }

        public IQueryable<TaskStatus> GetTaskStatusesForBusinessAccount(Guid roleId)
        {
            var businessForRole = ObjectContext.Owner(roleId).FirstOrDefault();

            var taskStatuses = this.ObjectContext.TaskStatuses.Where(ts => ts.BusinessAccountId == businessForRole.Id);

            return taskStatuses.OrderBy(ts => ts.Name);
        }

        /// <summary>
        /// Gets the BusinessAccount details.
        /// It includes the ContactInfoSet, ServiceTemplates and Fields, OwnedRoles, OwnedRoles.MemberParties, and PartyImage.
        /// </summary>
        /// <param name="roleId">The current role id.</param>
        /// <param name="businessAccountId">The businessAccount id.</param>
        public Party GetBusinessAccountDetailsForRole(Guid roleId, Guid businessAccountId)
        {
            var businessForRole = ObjectContext.Owner(roleId).First();

            //Make sure current account is a FoundOPS account
            if (businessForRole.Id != BusinessAccountsDesignData.FoundOps.Id)
                return null;

            var businessAccountQueryable = this.ObjectContext.Parties.OfType<BusinessAccount>().Where(ba => ba.Id == businessAccountId)
                                    .Include(ba => ba.TaskStatuses).Include(ba => ba.ServiceTemplates).Include(ba => ba.OwnedRoles).Include("OwnedRoles.MemberParties");

            var a =
                (from businessAccount in businessAccountQueryable
                 //Force load the details
                 from serviceTemplate in businessAccount.ServiceTemplates
                 from options in serviceTemplate.Fields.OfType<OptionsField>().Select(of => of.Options).DefaultIfEmpty()
                 from locations in serviceTemplate.Fields.OfType<LocationField>().Select(lf => lf.Value).DefaultIfEmpty()
                 select new { businessAccount, serviceTemplate, serviceTemplate.OwnerClient, serviceTemplate.Fields, options, locations }).ToArray();

            var businessAccountWithDetails = businessAccountQueryable.FirstOrDefault();
            if (businessAccountWithDetails == null)
                return null;

            //Force load PartyImage
            businessAccountWithDetails.PartyImageReference.Load();

            return businessAccountWithDetails;
        }


        #region BusinessAccount

        private void InsertBusinessAccount(BusinessAccount account)
        {
            var defaultStatuses = AddDefaultTaskStatuses();

            //Add the default statuses to the BusinessAccount
            foreach (var status in defaultStatuses)
                account.TaskStatuses.Add(status);

            if ((account.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(account, EntityState.Added);
            }
            else
            {
                this.ObjectContext.Parties.AddObject(account);
            }
        }

        private IEnumerable<TaskStatus> AddDefaultTaskStatuses()
        {
            var taskStatuses = new List<TaskStatus> { };

            var taskStatus = new TaskStatus
                                 {
                                     Id = Guid.NewGuid(),
                                     Name = "Created",
                                     Color = "FFFF00",
                                     DefaultTypeInt = ((int)StatusDetail.CreatedDefault),
                                     RouteRequired = false
                                 };

            taskStatuses.Add(taskStatus);

            taskStatus = new TaskStatus
            {
                Id = Guid.NewGuid(),
                Name = "Routed",
                Color = "FFFFFF",
                DefaultTypeInt = ((int)StatusDetail.RoutedDefault),
                RouteRequired = true
            };

            taskStatuses.Add(taskStatus);

            taskStatus = new TaskStatus
            {
                Id = Guid.NewGuid(),
                Name = "Completed",
                Color = "32CD32",
                DefaultTypeInt = ((int)StatusDetail.CompletedDefault),
                RouteRequired = true
            };

            taskStatuses.Add(taskStatus);

            return taskStatuses.ToArray();
        }

        private void UpdateBusinessAccount(BusinessAccount account)
        {
            //Only FoundOPS admin accounts or a user with admin capabilities for the current account can update the account
            if (!this.ObjectContext.CanAdministerFoundOPS() && !this.ObjectContext.CanAdminister(account.Id))
                throw new AuthenticationException("Invalid attempted access logged for investigation.");

            this.ObjectContext.DetachExistingAndAttach(account);

            var originalBusinessAccount = this.ChangeSet.GetOriginal(account);

            //Only FoundOPS's admins can change the MaxRoutes
            if (originalBusinessAccount.MaxRoutes != account.MaxRoutes && !this.ObjectContext.CanAdministerFoundOPS())
                throw new AuthenticationException("Invalid attempted access logged for investigation.");

            this.ObjectContext.Parties.AttachAsModified(account);
        }

        private void DeleteBusinessAccount(BusinessAccount businessAccountToDelete)
        {
            if (businessAccountToDelete.Id == BusinessAccountsConstants.FoundOpsId)
                throw new InvalidOperationException("You are trying to delete the FoundOPS account?!");

            //Make sure current account is a FoundOPS account
            if (!this.ObjectContext.CanAdministerFoundOPS())
                throw new AuthenticationException("Invalid attempted access logged for investigation.");

            //Set timeout to 10 minutes
            ObjectContext.CommandTimeout = 600;

            //throw new InvalidOperationException("Deleting BusinessAccounts has been temporarily disabled...Sorry Patrick");
            ObjectContext.DeleteBusinessAccountBasedOnId(businessAccountToDelete.Id);

            //TODO: When quickbooks is setup
            //QuickBooksTools.DeleteAzureTable(businessAccountToDelete.Id);
        }

        #endregion

        #endregion

        #region ContactInfo

        public IEnumerable<string> ContactInfoLabelsForParty(Guid currentPartyId)
        {
            var contactInfoLabels = new List<string> { "Primary", "Mobile" };

            var currentParty = this.ObjectContext.Parties.FirstOrDefault(a => a.Id == currentPartyId);

            if (currentParty == null)
                return contactInfoLabels.OrderBy(s => s);

            //TODO?
            //Add all labels current user can administer that are the same account type. (Business Labels might be different than User Labels)
            var accountsCurrentUserCanAdminister = ObjectContext.RolesCurrentUserHasAccessTo(new[] { RoleType.Administrator }).Select(r => r.OwnerBusinessAccount);

            MethodInfo method = typeof(Queryable).GetMethod("OfType");
            MethodInfo generic = method.MakeGenericMethod(new Type[] { currentParty.GetType() });
            var accountsCurrentUserCanAdministerOfSameType = (IEnumerable<Party>)generic.Invoke(null, new object[] { accountsCurrentUserCanAdminister });

            //foreach (var accountCurrentUserCanAdministerOfSameType in accountsCurrentUserCanAdministerOfSameType)
            //{
            //accountCurrentUserCanAdministerOfSameType.ContactInfoSet.Load();
            //contactInfoLabels.AddRange(accountCurrentUserCanAdministerOfSameType.ContactInfoSet.Select(ci => ci.Label));
            //}

            contactInfoLabels = new List<string>(contactInfoLabels.Distinct());

            return contactInfoLabels.OrderBy(s => s);
        }

        public IEnumerable<string> ContactInfoTypesForParty(Guid currentPartyId)
        {
            var contactInfoTypes = new List<string> { "Phone Number", "Email Address", "Website", "Fax Number", "Other" };

            var currentParty = this.ObjectContext.Parties.FirstOrDefault(a => a.Id == currentPartyId);

            if (currentParty == null)
                return contactInfoTypes.OrderBy(s => s);

            //currentParty.ContactInfoSet.Load();

            //contactInfoTypes.AddRange(currentParty.ContactInfoSet.Select(ci => ci.Type));
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

        /// <summary>
        /// Returns the BusinessAccount for the current role
        /// </summary>
        public BusinessAccount BusinessAccountForRole(Guid roleId)
        {
            var businessAccount = ObjectContext.Owner(roleId).Include(ba => ba.Depots).First();

            return businessAccount;
        }

        public IQueryable<Party> GetPartys()
        {
            return this.ObjectContext.Parties;
        }

        public void InsertParty(Party account)
        {
            if (account is BusinessAccount)
                InsertBusinessAccount((BusinessAccount)account);
            else
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

            party.PartyImageReference.Load();

            if (party.PartyImage != null)
                this.DeleteFile(party.PartyImage);

            var businessAccountToDelete = party as BusinessAccount;
            var userAccountToDelete = party as UserAccount;

            if (businessAccountToDelete != null)
                DeleteBusinessAccount(businessAccountToDelete);
            else if (userAccountToDelete != null)
                ObjectContext.DeleteUserAccountBasedOnId(userAccountToDelete.Id);
        }

        #endregion

        #region Repeat

        public IQueryable<Repeat> GetRepeats()
        {
            throw new NotSupportedException("Exists solely to generate Repeats in the clients data project");
        }

        public void InsertRepeat(Repeat repeat)
        {
            repeat.StartDate = repeat.StartDate.Date;

            if (repeat.EndDate != null)
                repeat.EndDate = repeat.EndDate.Value.Date;

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
            var availableRoles = this.ObjectContext.RolesCurrentUserHasAccessTo(new[] { RoleType.Administrator });

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

        #endregion

        #region User Accounts

        /// <summary>
        /// Gets the current user account
        /// </summary>
        public UserAccount CurrentUserAccount()
        {
            return ObjectContext.CurrentUserAccount().First();
        }

        /// <summary>
        /// Returns the UserAccounts the current role has access to.
        /// </summary>
        /// <param name="roleId">The current role id.</param>
        /// <param name="serviceProviderId">(Optional) filter user accounts that are in a owned role of this service provider.</param>
        public IQueryable<Party> GetUserAccounts(Guid roleId, Guid serviceProviderId)
        {
            var businessAccount = ObjectContext.Owner(roleId).First();

            IQueryable<UserAccount> accesibleUserAccounts;

            //If the current account is a foundops account, return all user accounts
            if (businessAccount.Id == BusinessAccountsDesignData.FoundOps.Id)
            {
                accesibleUserAccounts = this.ObjectContext.Parties.OfType<UserAccount>();
            }
            else
            {
                //If not a FoundOPS account, return the current business's owned roles memberparties
                accesibleUserAccounts =
                    from role in this.ObjectContext.Roles.Where(r => r.OwnerBusinessAccountId == businessAccount.Id)
                    from userAccount in this.ObjectContext.Parties.OfType<UserAccount>()
                    where role.MemberParties.Any(p => p.Id == userAccount.Id)
                    select userAccount;
            }

            //If the serviceProviderId context is not empty
            //Filter only user accounts that are member parties of one of it's roles
            if (serviceProviderId != Guid.Empty)
            {
                return from role in this.ObjectContext.Roles.Where(r => r.OwnerBusinessAccountId == serviceProviderId)
                       from userAccount in accesibleUserAccounts
                       where role.MemberParties.Any(p => p.Id == userAccount.Id)
                       orderby userAccount.LastName + " " + userAccount.FirstName
                       select userAccount;
            }

            return accesibleUserAccounts.OrderBy(ua => ua.LastName + " " + ua.FirstName);
        }

        /// <summary>
        /// Gets the UserAccount details.
        /// It includes the PartyImage.
        /// </summary>
        /// <param name="roleId">The current role id.</param>
        /// <param name="userAccountId">The user account id.</param>
        public Party GetUserAccountDetailsForRole(Guid roleId, Guid userAccountId)
        {
            var userAccount = GetUserAccounts(roleId, Guid.Empty).FirstOrDefault(ua => ua.Id == userAccountId);

            //Force load PartyImage
            userAccount.PartyImageReference.Load();

            return userAccount;
        }

        /// <summary>
        /// Returns the UserAccounts the current role has access to filtered by the search text.
        /// </summary>
        /// <param name="roleId">The current role id.</param>
        /// <param name="searchText">The search text.</param>
        public IQueryable<Party> SearchUserAccountsForRole(Guid roleId, string searchText)
        {
            var userAccounts = GetUserAccounts(roleId, Guid.Empty).OfType<UserAccount>();

            if (!String.IsNullOrEmpty(searchText))
                userAccounts = userAccounts.Where(ua => ua.FirstName.StartsWith(searchText) || ua.LastName.StartsWith(searchText)
                                             || searchText.StartsWith(ua.FirstName) || searchText.Contains(ua.LastName));

            return userAccounts.OrderBy(ua => ua.LastName + " " + ua.FirstName);
        }

        public void InsertUserAccount(UserAccount userAccount)
        {
            if ((userAccount.EntityState != EntityState.Detached))
                this.ObjectContext.ObjectStateManager.ChangeObjectState(userAccount, EntityState.Added);
            else
                this.ObjectContext.Parties.AddObject(userAccount);

            //Set the password to the temporary password
            userAccount.PasswordHash = EncryptionTools.Hash(userAccount.TemporaryPassword);

            //Trim unwanted spaces from the email address
            userAccount.EmailAddress = userAccount.EmailAddress.Trim();
        }

        [Update]
        public void UpdateUserAccount(UserAccount currentUserAccount)
        {
            if (!ObjectContext.CanAdminister(currentUserAccount.Id) || !this.ObjectContext.CanAdministerFoundOPS())
                throw new AuthenticationException();

            //Check if there is a temporary password
            if (!string.IsNullOrEmpty(currentUserAccount.TemporaryPassword))
                currentUserAccount.PasswordHash = EncryptionTools.Hash(currentUserAccount.TemporaryPassword);

            //Trim unwanted spaces from the email address
            currentUserAccount.EmailAddress = currentUserAccount.EmailAddress.Trim();

            this.ObjectContext.Parties.AttachAsModified(currentUserAccount);
        }

        #endregion
    }
}
