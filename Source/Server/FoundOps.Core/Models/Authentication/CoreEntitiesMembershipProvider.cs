using System.Net;
using System.Net.Mail;
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
        /// Set a one time reset code on the user account.
        /// </summary>
        /// <param name="emailAddress">The account's email address</param>
        /// <param name="expires">When to expire. Defaults to an hour</param>
        /// <returns>True if succesful, or false if it failed</returns>
        public bool ResetAccount(string emailAddress, TimeSpan? expires = null)
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

            //Send email
            var ss = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Timeout = 10000,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("info@foundops.com", "6Neoy1KRjSVQV6sCk6ax")
            };

            var to = emailAddress;
            const string from = "info@foundops.com";
            const string subject = "FoundOPS Password Reset";
            //TODO
            var body = "You can reset your password here: app.foundops.com/Account/ResetPassword?resetCode=" + token;
            var mm = new MailMessage(from, to, subject, body)
            {
                BodyEncoding = Encoding.UTF8,
                DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure
            };
            ss.Send(mm);

            return true;
        }

        /// <summary>
        /// Set the password using a (one-time) reset code.
        /// </summary>
        /// <returns>True if succesful, or false if it failed</returns>
        public bool SetPassword(string resetCode, string newPassword)
        {
            var account = _coreEntitiesContainer.Parties.OfType<UserAccount>().First(ua => ua.TempResetToken == resetCode);

            if (resetCode != account.TempResetToken || DateTime.UtcNow > account.TempTokenExpireTime)
                return false;

            account.PasswordSalt = EncryptionTools.GenerateSalt();
            account.PasswordHash = EncryptionTools.Hash(newPassword, account.PasswordSalt);

            _coreEntitiesContainer.SaveChanges();

            return true;
        }

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

        //public interface IMembershipService
        //{
        //    int MinPasswordLength { get; }
        //    bool ValidateReset(string resetCode);
        //    bool ValidateUser(string email, string password);
        //    MembershipCreateStatus CreateUser(string email, string password, string linkCode);
        //    bool ChangePassword(string email, string oldPassword, string newPassword);
        //    int MaxInvalidPasswordAttempts { get; set; }
        //}

        //public class PartyMembershipService : IMembershipService
        //{
        //    private readonly MembershipProvider _provider;

        //    public PartyMembershipService()
        //        : this(null)
        //    {
        //    }

        //    public PartyMembershipService(MembershipProvider provider)
        //    {
        //        _provider = provider ?? Membership.Provider;
        //    }

        //    public int MinPasswordLength
        //    {
        //        get
        //        {
        //            return _provider.MinRequiredPasswordLength;
        //        }
        //    }

        //    public int MaxInvalidPasswordAttempts
        //    {
        //        get
        //        {
        //            return _provider.MaxInvalidPasswordAttempts;
        //        }
        //        set { }
        //    }

        //    public bool ValidateReset(string resetCode)
        //    {


        //        if (String.IsNullOrEmpty(email)) throw new ArgumentException("  Value cannot be null or empty.", "email");
        //        return _provider.GetUser(email, false) != null;
        //    }

        //    public bool ValidateUser(string email, string password)
        //    {
        //        if (String.IsNullOrEmpty(email)) throw new ArgumentException("  Value cannot be null or empty.", "email");
        //        if (String.IsNullOrEmpty(password)) throw new ArgumentException("  Value cannot be null or empty.", "password");
        //        return _provider.ValidateUser(email, password);
        //    }

        //    public MembershipCreateStatus CreateUser(string email, string password, string temporaryPassword)
        //    {
        //        if (_provider.GetUser(email, false) == null)
        //            return MembershipCreateStatus.InvalidEmail;

        //        if (!_provider.ValidateUser(email, temporaryPassword))
        //            return MembershipCreateStatus.InvalidPassword;

        //        ChangePassword(email, temporaryPassword, password);

        //        return MembershipCreateStatus.Success;
        //    }

        //    public bool ChangePassword(string userName, string oldPassword, string newPassword)
        //    {
        //        if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");
        //        if (String.IsNullOrEmpty(oldPassword)) throw new ArgumentException("Value cannot be null or empty.", "oldPassword");
        //        if (String.IsNullOrEmpty(newPassword)) throw new ArgumentException("Value cannot be null or empty.", "newPassword");

        //        // The underlying ChangePassword() will throw an exception rather
        //        // than return false in certain failure scenarios.
        //        try
        //        {
        //            MembershipUser currentUser = _provider.GetUser(userName, true /* userIsOnline */);
        //            var worked = currentUser.ChangePassword(oldPassword, newPassword);

        //            return worked;
        //        }
        //        catch (ArgumentException)
        //        {
        //            return false;
        //        }
        //        catch (MembershipPasswordException)
        //        {
        //            return false;
        //        }
        //    }
        //}
    }
}