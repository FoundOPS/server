using System.Collections.Generic;

namespace FoundOps.Core.Models.CoreEntities.DesignData
{
    public class ContactsDesignData
    {
        public Contact DesignContact { get; private set; }
        public Contact DesignContactTwo { get; private set; }
        public Contact DesignContactThree { get; private set; }

        public IEnumerable<Contact> DesignContacts { get; private set; }

        public ClientTitle DesignClientTitle { get; private set; }
        public ClientTitle DesignClientTitleTwo { get; private set; }
        public ClientTitle DesignClientTitleThree { get; private set; }

        public IEnumerable<ClientTitle> DesignClientTitles { get; private set; }

        public ContactsDesignData():this(new ClientsDesignData()) { }

        public ContactsDesignData(ClientsDesignData clientsDesignData)
        {
            var partyDesignData = new PartyDesignData();
            
            //Setup Contacts
            DesignContact = new Contact
            {
                Notes = "Important",
                OwnedPerson = partyDesignData.OwnedPerson
            };

            DesignContactTwo = new Contact
            {
                Notes = "Notes about this contact",
                OwnedPerson = partyDesignData.OwnedPersonTwo
            };

            DesignContactThree = new Contact
            {
                Notes = "A few insignificant notes",
                OwnedPerson = partyDesignData.OwnedPersonThree
            };

            DesignContacts = new List<Contact> { DesignContact, DesignContactTwo, DesignContactThree };


            //Setup ClientTitles
            DesignClientTitle = new ClientTitle
            {
                Title = "Decision Maker",
                Contact =  DesignContact,
                Client =  clientsDesignData.DesignClient
            };

            DesignClientTitleTwo = new ClientTitle
            {
                Title = "Chef",
                Contact =  DesignContact,
                Client = clientsDesignData.DesignClientTwo
            };

            DesignClientTitleThree = new ClientTitle
            {
                Title = "Accountant",
                Contact =  DesignContactTwo,
                Client = clientsDesignData.DesignClientThree
            };

            DesignClientTitles = new List<ClientTitle> { DesignClientTitle, DesignClientTitleTwo, DesignClientTitleThree };
        }
    }
}
