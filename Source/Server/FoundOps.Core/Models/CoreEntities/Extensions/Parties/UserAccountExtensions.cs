using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Web.Security;

namespace FoundOps.Core.Models.CoreEntities
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
                                      lastActivityDateEntry != null ? lastActivityDateEntry.TimeStamp: DateTime.MinValue,
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
    }
}