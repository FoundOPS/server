using System;
using System.Collections.Generic;
using FoundOps.Api.Tools;

namespace FoundOps.Api.Models
{
    public class Client : IImportable
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

        public int? StatusInt { get; set; }

        public Client()
        {
            ContactInfoSet = new List<ContactInfo>();
        }

        public static Client ConvertModel(FoundOps.Core.Models.CoreEntities.Client clientModel)
        {
            var client = new Client { Id = clientModel.Id, Name = clientModel.Name };

            foreach (var contactInfo in clientModel.ContactInfoSet)
                client.ContactInfoSet.Add(ContactInfo.Convert(contactInfo));

            return client;
        }

        public static Core.Models.CoreEntities.Client ConvertBack(Client clientModel)
        {
            var client = new Core.Models.CoreEntities.Client { Id = clientModel.Id, Name = clientModel.Name };

            foreach (var contactInfo in clientModel.ContactInfoSet)
                client.ContactInfoSet.Add(ContactInfo.ConvertBack(contactInfo));

            return client;
        }
    }
}
