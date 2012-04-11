using System;
using System.Linq;
using System.Web.Security;
using FoundOps.Common.NET;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;

namespace FoundOps.Core.Models.Authentication
{
    public class CoreEntitiesMembershipProvider : MembershipProvider
    {
        readonly CoreEntitiesContainer _coreEntitiesContainer = new CoreEntitiesContainer();

        #region Overrides of MembershipProvider

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new NotImplementedException("Create user is done another");
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            var user = AuthenticationLogic.GetUserAccount(_coreEntitiesContainer, username);

            if (!ValidateUser(username, oldPassword) || user == null)
                return false;

            user.PasswordHash = EncryptionTools.Hash(newPassword);
            user.UserAccountLog.Add(new UserAccountLogEntry
            {
                UserAccountLogType = UserAccountLogType.PasswordChange,
                TimeStamp = DateTime.UtcNow
            });
            _coreEntitiesContainer.SaveChanges();

            return true;
        }

        public void ChangePassword(string username, string temporaryPassword)
        {
            var user = AuthenticationLogic.GetUserAccount(_coreEntitiesContainer, username);
            user.PasswordHash = EncryptionTools.Hash(temporaryPassword);
            user.UserAccountLog.Add(new UserAccountLogEntry
            {
                UserAccountLogType = UserAccountLogType.PasswordChange,
                TimeStamp = DateTime.UtcNow
            });

            _coreEntitiesContainer.SaveChanges();
        }

        public override bool ValidateUser(string username, string password)
        {
            var userParty = AuthenticationLogic.GetUserAccount(_coreEntitiesContainer, username);

            if (userParty == null || userParty.PasswordHash == null)
                return false;

            var actualHash = userParty.PasswordHash;
            var enteredHash = EncryptionTools.Hash(password);
            var hashesMatch = actualHash.SequenceEqual(enteredHash);

            return hashesMatch;
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            var userPartyId = (Guid)providerUserKey;

            var userParty = AuthenticationLogic.GetUserAccount(_coreEntitiesContainer, userPartyId);
            return userParty != null ? userParty.MembershipUser() : null;
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            var userParty = AuthenticationLogic.GetUserAccount(_coreEntitiesContainer, username);
            return userParty != null ? userParty.MembershipUser() : null;
        }

        public override string GetUserNameByEmail(string email)
        {
            return email;
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            var userParty = AuthenticationLogic.GetUserAccount(_coreEntitiesContainer, username);
            if (userParty == null)
                return false;

            _coreEntitiesContainer.Parties.DeleteObject(userParty);
            _coreEntitiesContainer.SaveChanges();

            return true;
        }

        public override bool EnablePasswordRetrieval
        {
            get { return true; }
        }

        public override bool EnablePasswordReset
        {
            get { return true; }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { return false; }
        }

        public override string ApplicationName
        {
            get { return "FoundOPS"; }
            set { throw new NotImplementedException(); }
        }

        public override bool RequiresUniqueEmail
        {
            get { return true; }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { return MembershipPasswordFormat.Hashed; }
        }

        public override int MinRequiredPasswordLength
        {
            get { return 8; }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return 1; }
        }

        //Is greater than seven characters.
        //Contains at least one digit.
        //Contains at least one special (non-alphanumeric) character.
        public override string PasswordStrengthRegularExpression
        {
            get { return @"(?=.{6,})(?=(.*\d){1,})(?=(.*\W){1,})"; }
        }


        #region NotImplemented

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            //Requires LastActivity to be tracked
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException("This is never allowed");
        }
        public override string GetPassword(string username, string answer)
        {
            var userParty = AuthenticationLogic.GetUserAccount(_coreEntitiesContainer, username);
            return userParty.PasswordHash;
        }
        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException("This is never allowed");
        }
        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException("This is never allowed");
        }
        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException("This is currently not supported");
        }
        public override int MaxInvalidPasswordAttempts
        {
            get { return 5; }
        }
        public override int PasswordAttemptWindow
        {
            get { return 5; }  //5 Minutes
        }

        #endregion

        #endregion
    }
}