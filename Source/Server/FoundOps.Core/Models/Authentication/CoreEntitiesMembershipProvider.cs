using System.IO;
using System.Xml;
using FoundOps.Common.NET;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using System;
using System.Linq;
using System.Text;
using System.Web.Security;

namespace FoundOps.Core.Models.Authentication
{
    public class CoreEntitiesMembershipProvider : MembershipProvider
    {
        private readonly CoreEntitiesContainer _coreEntitiesContainer;

        public CoreEntitiesMembershipProvider()
        {
            _coreEntitiesContainer = new CoreEntitiesContainer();
        }

        public CoreEntitiesMembershipProvider(CoreEntitiesContainer coreEntitiesContainer)
        {
            _coreEntitiesContainer = coreEntitiesContainer;
        }

        private static string GenerateRandomResetToken()
        {
            var builder = new StringBuilder();

            var random = new Random((int)DateTime.UtcNow.Ticks);

            for (var i = 0; i < 16; i++)
            {
                var ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            var randomString = builder.ToString();
            return randomString;
        }

        /// <summary>
        /// Set a one time reset code on the user account and send an email
        /// </summary>
        /// <param name="emailAddress">The account's email address</param>
        /// <param name="subject">The email subject line</param>
        /// <param name="body">The email body. {0} will be replaced with the link</param>
        /// <param name="expires">When to expire. Defaults to an hour</param>
        /// <returns>True if succesful, or false if it failed</returns>
        public bool ResetAccount(string emailAddress, string subject = null, string body = null, TimeSpan? expires = null)
        {
            var account = _coreEntitiesContainer.Parties.OfType<UserAccount>().FirstOrDefault(ua => ua.EmailAddress == emailAddress);

            if (account == null)
                return false;

            if (!expires.HasValue)
                expires = TimeSpan.FromHours(1);

            var expireTime = DateTime.UtcNow.Add(expires.Value);
            var token = GenerateRandomResetToken();

            account.TempTokenExpireTime = expireTime;
            account.TempResetToken = token;

            _coreEntitiesContainer.SaveChanges();

            var to = emailAddress;
            if (subject == null)
                subject = "FoundOPS Password Reset";

            if (body == null)
                body = "You can reset your password here: \r\n {0}";

            var link = AppConstants.RootApplicationUrl + "/Account/ResetPassword?resetCode=" + token;
            body = string.Format(body, link);

            EmailPasswordTools.SendEmail(to, subject, body);

            return true;
        }

        /// <summary>
        /// Set the password using a (one-time) reset code.
        /// Also authenticates the user.
        /// </summary>
        /// <param name="resetCode">The reset code</param>
        /// <param name="newPassword">The new password</param>
        /// <param name="autoLogin">(Optional) Defaults to true. Set the authentication cookie</param>
        /// <returns>True if succesful, or false if it failed</returns>
        public bool SetPassword(string resetCode, string newPassword, bool autoLogin = true)
        {
            var account = _coreEntitiesContainer.Parties.OfType<UserAccount>().First(ua => ua.TempResetToken == resetCode);

            if (resetCode != account.TempResetToken || DateTime.UtcNow > account.TempTokenExpireTime)
                return false;

            account.PasswordSalt = EncryptionTools.GenerateSalt();
            account.PasswordHash = EncryptionTools.Hash(newPassword, account.PasswordSalt);

            _coreEntitiesContainer.SaveChanges();

            if (autoLogin)
                FormsAuthentication.SetAuthCookie(account.EmailAddress, true);

            return true;
        }

        #region Overrides of MembershipProvider

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new NotImplementedException("Create user is done another");
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            var user = _coreEntitiesContainer.GetUserAccount(username);

            if (!ValidateUser(username, oldPassword) || user == null)
                return false;

            user.PasswordSalt = EncryptionTools.GenerateSalt();
            user.PasswordHash = EncryptionTools.Hash(newPassword, user.PasswordSalt);
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
            var user = _coreEntitiesContainer.GetUserAccount(username);
            user.PasswordSalt = EncryptionTools.GenerateSalt();

            user.PasswordHash = EncryptionTools.Hash(temporaryPassword, user.PasswordSalt);
            user.UserAccountLog.Add(new UserAccountLogEntry
            {
                UserAccountLogType = UserAccountLogType.PasswordChange,
                TimeStamp = DateTime.UtcNow
            });

            _coreEntitiesContainer.SaveChanges();
        }

        public override bool ValidateUser(string username, string password)
        {
            //trim the email address
            username = username.Trim();

            var userParty = _coreEntitiesContainer.GetUserAccount(username);

            if (userParty == null || userParty.PasswordHash == null)
                return false;

            var actualHash = userParty.PasswordHash;
            var enteredHash = EncryptionTools.Hash(password, userParty.PasswordSalt);
            var hashesMatch = actualHash.SequenceEqual(enteredHash);

            return hashesMatch;
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            var userParty = _coreEntitiesContainer.GetUserAccount(username);
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