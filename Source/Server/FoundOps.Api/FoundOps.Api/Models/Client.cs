using System;
using System.Collections.Generic;

namespace FoundOps.Api.Models
{
    public class Client : ITrackable
    {
        /// <summary>
        /// The Id
        /// </summary>
        public Guid Id { get; set; }

        public DateTime CreatedDate { get; private set; }
        public DateTime? LastModified { get; private set; }
        public Guid? LastModifyingUserId { get; private set; }

        /// <summary>
        /// The name of the Client
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The list of ContactInfo associated with this Client
        /// </summary>
        public List<ContactInfo> ContactInfoSet { get; set; }

        public Client()
        {
            ContactInfoSet = new List<ContactInfo>();
            CreatedDate = DateTime.UtcNow;
        }

        public static Client ConvertModel(FoundOps.Core.Models.CoreEntities.Client clientModel)
        {
            var client = new Client
            {
                Id = clientModel.Id,
                CreatedDate = clientModel.CreatedDate,
                Name = clientModel.Name
            };

            client.SetLastModified(clientModel.LastModified, clientModel.LastModifyingUserId);

            foreach (var contactInfo in clientModel.ContactInfoSet)
                client.ContactInfoSet.Add(ContactInfo.Convert(contactInfo));

            return client;
        }

        public void SetLastModified(DateTime? lastModified, Guid? userId)
        {
            LastModified = lastModified;
            LastModifyingUserId = userId;
        }
    }
}
