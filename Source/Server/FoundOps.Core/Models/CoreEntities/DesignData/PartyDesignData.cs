using System;
using System.Collections.Generic;
using System.Linq;

namespace FoundOps.Core.Models.CoreEntities.DesignData
{
    public class PartyDesignData
    {
#if SILVERLIGHT //To Prevent Design Data Issue
        public UserAccount DesignUserAccount { get; private set; }
#endif

        public Business DesignBusiness { get; private set; }
        public Business DesignBusinessTwo { get; private set; }
        public Business DesignBusinessThree { get; private set; }

        public Person OwnedPerson { get; private set; }
        public Person OwnedPersonTwo { get; private set; }
        public Person OwnedPersonThree { get; private set; }

        public BusinessAccount DesignBusinessAccount { get; private set; }


        public List<Party> DesignPartys { get; private set; }

        public PartyDesignData()
        {
            DesignBusiness = new Business { Name = "JD Cooling and Associates" };
            DesignBusinessTwo = new Business { Name = "Thirsty Bear" };
            DesignBusinessThree = new Business { Name = "Seltzer Factory" };
            
            DesignBusinessAccount = new BusinessAccount { Name = "GotGrease" };

            OwnedPerson = new Person { FirstName = "Bob", LastName = "Black", DateOfBirth = DateTime.UtcNow.Date.AddDays(-21), Gender = Gender.Male };
            OwnedPersonTwo = new Person { FirstName = "Susanne", LastName = "Greene", DateOfBirth = DateTime.UtcNow.Date.AddDays(-991), Gender = Gender.Female };
            OwnedPersonThree = new Person { FirstName = "Jim", LastName = "Boliath", DateOfBirth = DateTime.UtcNow.Date.AddDays(-501), Gender = Gender.Male };

            DesignPartys = new List<Party> { DesignBusiness, DesignBusinessTwo, DesignBusinessThree, DesignBusinessAccount, OwnedPerson, OwnedPersonTwo, OwnedPersonThree };

            #region Add ContactInfoSet

            //Add Business contact info
            foreach (var business in DesignPartys.OfType<Business>())
            {
                var designContactInfoData = new ContactInfoDesignData();
                foreach (var contactInfo in designContactInfoData.DesignBusinessContactInfoList)
                    business.ContactInfoSet.Add(contactInfo);
            }

            //Add Person contact info
            foreach (var person in DesignPartys.OfType<Person>())
            {
                var designContactInfoData = new ContactInfoDesignData();
                foreach (var contactInfo in designContactInfoData.DesignUserContactInfoList)
                    person.ContactInfoSet.Add(contactInfo);
            }

            #endregion

        }
    }
}