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

        //public Business DesignBusiness { get; private set; }
        //public Business DesignBusinessTwo { get; private set; }
        //public Business DesignBusinessThree { get; private set; }
        //public Business DesignBusinessFour { get; private set; }
        //public Business DesignBusinessFive { get; private set; }
        //public Business DesignBusinessSix { get; private set; }
        //public Business DesignBusinessSeven { get; private set; }
        //public Business DesignBusinessEight { get; private set; }
        //public Business DesignBusinessNine { get; private set; }
        //public Business DesignBusinessTen { get; private set; }
        //public Business DesignBusinessEleven { get; private set; }
        //public Business DesignBusinessTwelve { get; private set; }
        //public Business DesignBusinessThirteen { get; private set; }
        //public Business DesignBusinessFourteen { get; private set; }
        //public Business DesignBusinessFifteen { get; private set; }
        //public Business DesignBusinessSixteen { get; private set; }
        //public Business DesignBusinessSeventeen { get; private set; }
        //public Business DesignBusinessEighteen { get; private set; }
        //public Business DesignBusinessNineteen { get; private set; }
        //public Business DesignBusinessTwenty { get; private set; }
        //public Business DesignBusinessTwentyOne { get; private set; }
        //public Business DesignBusinessTwentyTwo { get; private set; }
        //public Business DesignBusinessTwentyThree { get; private set; }
        //public Business DesignBusinessTwentyFour { get; private set; }

        public Person OwnedPerson { get; private set; }
        public Person OwnedPersonTwo { get; private set; }
        public Person OwnedPersonThree { get; private set; }

        public BusinessAccount DesignBusinessAccount { get; private set; }


        public List<Party> DesignPartys { get; private set; }

        public PartyDesignData()
        {
            //DesignBusiness = new Business { Name = "Adelino's Old World Kitchen" };
            //DesignBusinessTwo = new Business { Name = "Akropolis Restaraunt" };
            //DesignBusinessThree = new Business { Name = "Apollo Lounge" };
            //DesignBusinessFour = new Business { Name = "Applebee's Neighborhood Grill" };
            //DesignBusinessFive = new Business { Name = "Arby's" };
            //DesignBusinessSix = new Business { Name = "Arctic White Soft Serve Ice" };
            //DesignBusinessSeven = new Business { Name = "Arthur's" };
            //DesignBusinessEight = new Business { Name = "Artie's Tenderloin" };
            //DesignBusinessNine = new Business { Name = "Barcelona Tapas" };
            //DesignBusinessTen = new Business { Name = "Bistro 501" };
            //DesignBusinessEleven = new Business { Name = "Black Sparrow" };
            //DesignBusinessTwelve = new Business { Name = "Blue Fin Bistro Sushi Lafayette" };
            //DesignBusinessThirteen = new Business { Name = "Bob Evans Restaurant" };
            //DesignBusinessFourteen = new Business { Name = "Bruno's Pizza and Big O's Sports Room" };
            //DesignBusinessFifteen = new Business { Name = "Buca di Beppo - Downtown Indianapolis" };
            //DesignBusinessSixteen = new Business { Name = "Buffalo Wild Wings Grill & Bar" };
            //DesignBusinessSeventeen = new Business { Name = "Burger King" };
            //DesignBusinessEighteen = new Business { Name = "Campbell's On Main Street" };
            //DesignBusinessNineteen = new Business { Name = "Covered Bridge Restaurant" };
            //DesignBusinessTwenty = new Business { Name = "Crawfordsville Forum Family" };
            //DesignBusinessTwentyOne = new Business { Name = "Dairy Queen" };
            //DesignBusinessTwentyTwo = new Business { Name = "Diamond Coffee Company" };
            //DesignBusinessTwentyThree = new Business { Name = "Culver's" };
            //DesignBusinessTwentyFour = new Business { Name = "El Rodeo" };
            
            DesignBusinessAccount = new BusinessAccount { Name = "GotGrease" };

            OwnedPerson = new Person { FirstName = "Bob", LastName = "Black", DateOfBirth = DateTime.UtcNow.Date.AddDays(-21), Gender = Gender.Male };
            OwnedPersonTwo = new Person { FirstName = "Susanne", LastName = "Greene", DateOfBirth = DateTime.UtcNow.Date.AddDays(-991), Gender = Gender.Female };
            OwnedPersonThree = new Person { FirstName = "Jim", LastName = "Boliath", DateOfBirth = DateTime.UtcNow.Date.AddDays(-501), Gender = Gender.Male };

            DesignPartys = new List<Party>  { 
                                            //DesignBusiness, 
                                            //DesignBusinessTwo, 
                                            //DesignBusinessThree, 
                                            DesignBusinessAccount, 
                                            OwnedPerson, 
                                            OwnedPersonTwo, 
                                            OwnedPersonThree 
                                            };
        }
    }
}