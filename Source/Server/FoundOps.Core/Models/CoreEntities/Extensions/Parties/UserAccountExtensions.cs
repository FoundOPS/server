using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Web.Security;

// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class UserAccount
    {
        public MembershipUser MembershipUser()
        {
            var orderedUserAccountLog = this.UserAccountLog.OrderByDescending(entry => entry.TimeStamp);

            var lastLoginDateEntry =
                orderedUserAccountLog.FirstOrDefault(entry => entry.UserAccountLogType == UserAccountLogType.Login);

            var lastPasswordChangeDateEntry =
                orderedUserAccountLog.FirstOrDefault(entry => entry.UserAccountLogType == UserAccountLogType.PasswordChange);

            var lastLockoutDateEntry =
               orderedUserAccountLog.FirstOrDefault(entry => entry.UserAccountLogType == UserAccountLogType.PasswordChange);

            var lastActivityDateEntry =
                orderedUserAccountLog.FirstOrDefault();

            //Must have log entries loaded)
            this.UserAccountLog.Load();

            return new MembershipUser("CoreEntitiesProvider", this.EmailAddress,
                                      this.Id, this.EmailAddress, "", "", true,
                                      false, this.CreationDate,
                                      lastLoginDateEntry != null ? lastLoginDateEntry.TimeStamp : DateTime.MinValue,
                                      lastActivityDateEntry != null ? lastActivityDateEntry.TimeStamp : DateTime.MinValue,
                                      lastPasswordChangeDateEntry != null
                                          ? lastPasswordChangeDateEntry.TimeStamp
                                          : DateTime.MinValue,
                                      lastLockoutDateEntry != null ? lastLockoutDateEntry.TimeStamp : DateTime.MinValue);
        }

        /// <summary>
        /// Gets or sets a temporary password for new user accounts.
        /// It's not shared so that it doesn't get generated twice on the client.
        /// </summary>
        [DataMember]
        public string TemporaryPassword { get; set; }

        /// <summary>
        /// Get the system's TimeZoneInfo for the current users TimeZone
        /// </summary>
        public TimeZoneInfo TimeZoneInfo
        {
            get
            {
                var timeZone = string.IsNullOrEmpty(TimeZone) ? "Eastern Standard Time" : TimeZone;
                return TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            }
        }

        private TimeSpan _userTimeZoneOffset;
        /// <summary>
        /// The user's timezone offset (depends on their TimeZone settings)
        /// It's not shared so that it doesn't get generated twice on the client.
        /// </summary>
        [DataMember]
        public TimeSpan UserTimeZoneOffset
        {
            get
            {
#if !SILVERLIGHT
                //get the timezone offset (pass UtcNow so it considers daylight savings)
                _userTimeZoneOffset = TimeZoneInfo.GetUtcOffset(DateTime.UtcNow);
#endif
                return _userTimeZoneOffset;
            }
            set
            {
                _userTimeZoneOffset = value;
            }
        }
    }
}