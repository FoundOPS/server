using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel.DomainServices.Server.ApplicationServices;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.Core.Models.Account.Extensions
{
    public class WebContextUser : IUser
    {
        public WebContextUser()
        {
            this.Name = string.Empty;
            this.Roles = new string[0];
        }

        public WebContextUser(UserAccount currentUser)
        {
            if (currentUser != null)
                Name = String.Format("{0} {1}", currentUser.FirstName, currentUser.LastName);
        }

        [Key]
        public string Name { get; set; }

        [Editable(false)]
        public IEnumerable<string> Roles { get; set; }
    }
}