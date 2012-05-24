using System.Collections.Generic;

namespace FoundOPS.API.Models
{
    public class Client
    {
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
        }

        public static Client ConvertModel(FoundOps.Core.Models.CoreEntities.Client clientModel)
        {
            var client =  new Client {Name = clientModel.OwnedParty.DisplayName};

            foreach (var contactInfo in clientModel.OwnedParty.ContactInfoSet)
                client.ContactInfoSet.Add(ContactInfo.Convert(contactInfo));

            return client;
        }
    }
}
