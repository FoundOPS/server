using FoundOps.Api.Tools;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using System;
using System.Data.Entity;
using System.Linq;
using ContactInfo = FoundOps.Api.Models.ContactInfo;

namespace FoundOps.Api.Controllers.Rest
{
    public class ContactInfoController : BaseApiController
    {
        public void Post(Guid roleId, ContactInfo contactInfo)
        {
            var businessAccount = CoreEntitiesContainer.Owner(roleId, new[] { RoleType.Administrator, RoleType.Regular, RoleType.Mobile }).FirstOrDefault();
            if (businessAccount == null)
                throw Request.NotAuthorized();

            var foundOpsContactInfo = ContactInfo.ConvertBack(contactInfo);
            if (contactInfo.LocationId.HasValue)
            {
                var location = CoreEntitiesContainer.Locations.FirstOrDefault(l => l.Id == contactInfo.LocationId);
                if (location == null)
                    throw Request.BadRequest();

                location.ContactInfoSet.Add(foundOpsContactInfo);
            }
            else if (contactInfo.ClientId.HasValue)
            {
                var client = CoreEntitiesContainer.Clients.FirstOrDefault(c => c.Id == contactInfo.ClientId);
                if (client == null)
                    throw Request.BadRequest();

                client.ContactInfoSet.Add(foundOpsContactInfo);
            }

            SaveWithRetry();
        }

        public void Put(Guid roleId, ContactInfo contactInfo)
        {
            var businessAccount = CoreEntitiesContainer.Owner(roleId, new[] { RoleType.Administrator, RoleType.Regular, RoleType.Mobile }).FirstOrDefault();
            if (businessAccount == null)
                throw Request.NotAuthorized();

            var original = CoreEntitiesContainer.ContactInfoSet.FirstOrDefault(ci => ci.Id == contactInfo.Id);
            if (original == null)
                throw Request.NotFound();

            original.Type = contactInfo.Type;
            original.Label = contactInfo.Label;
            original.Data = contactInfo.Data;
            original.LocationId = contactInfo.LocationId;
            original.ClientId = contactInfo.ClientId;

            SaveWithRetry();
        }

        public void Delete(Guid contactInfoId)
        {
            var contactInfo = CoreEntitiesContainer.ContactInfoSet.FirstOrDefault(ci => ci.Id == contactInfoId);
            if (contactInfo == null)
                throw Request.BadRequest();

            CoreEntitiesContainer.ContactInfoSet.DeleteObject(contactInfo);

            SaveWithRetry();
        }
    }
}