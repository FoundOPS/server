using System.Collections.Generic;

namespace FoundOPS.API.Models
{
    public class Client
    {
        public string Name { get; set; }
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
