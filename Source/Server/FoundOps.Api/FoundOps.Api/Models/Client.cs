using System;
using System.Collections.Generic;
using FoundOps.Api.Models;

namespace FoundOps.Api.Models
{
    public class Client : ITrackable
    {
        /// <summary>
        /// The Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The name of the Client
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The list of ContactInfo associated with this Client
        /// </summary>
        public List<ContactInfo> ContactInfoSet { get; set; }

        public DateTime CreatedDate { get; private set; }
        public DateTime? LastModifiedDate { get; private set; }
        public Guid? LastModifyingUserId { get; private set; }

        public void SetLastModified(DateTime? lastModified, Guid? userId)
        {
            LastModifiedDate = lastModified;
            LastModifyingUserId = userId;
        }

        public Client(DateTime createdDate)
        {
            ContactInfoSet = new List<ContactInfo>();
            CreatedDate = createdDate;
        }

        public static Client ConvertModel(FoundOps.Core.Models.CoreEntities.Client clientModel)
        {
            var client = new Client(clientModel.CreatedDate)
                {
                    Id = clientModel.Id, 
                    Name = clientModel.Name
                };

            client.SetLastModified(clientModel.LastModifiedDate, clientModel.LastModifyingUserId);

            foreach (var contactInfo in clientModel.ContactInfoSet)
                client.ContactInfoSet.Add(ContactInfo.Convert(contactInfo));

            return client;
        }
    }
}
