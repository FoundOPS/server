using System.Collections.Generic;

namespace FoundOps.Core.Models.CoreEntities.DesignData
{
    public class ContactInfoDesignData
    {
        public static List<string> DesignContactInfoTypes = new List<string> { "Phone Number", "Email Address", "Website", "Fax Number", "Other" };
        public static List<string> DesignContactInfoLabels = new List<string> { "Primary", "Home", "Cell Phone" };

        public List<ContactInfo> DesignUserContactInfoList = new List<ContactInfo>
                                                                    {
                                                                        new ContactInfo
                                                                            {
                                                                                Label = "Home",
                                                                                Data = "703-582-1810",
                                                                                Type = "Phone Number"
                                                                            },
                                                                        new ContactInfo
                                                                            {
                                                                                Label = "Primary Email",
                                                                                Data = "jrice@purdue.edu",
                                                                                Type = "Email Address"
                                                                            },
                                                                        new ContactInfo
                                                                            {
                                                                                Label = "Blog",
                                                                                Data = "2020vision.com",
                                                                                Type = "Website"
                                                                            }
                                                                    };

        public List<ContactInfo> DesignBusinessContactInfoList = new List<ContactInfo>
                                                                    {
                                                                        new ContactInfo
                                                                            {
                                                                                Label = "Zach Bright",
                                                                                Data = "703-582-1810",
                                                                                Type = "Phone Number"
                                                                            },
                                                                        new ContactInfo
                                                                            {
                                                                                Label = "Jason Rice",
                                                                                Data = "jrice@purdue.edu",
                                                                                Type = "Email Address"
                                                                            },
                                                                        new ContactInfo
                                                                            {
                                                                                Label = "Oren Shatken",
                                                                                Data = "2020vision.com",
                                                                                Type = "Website"
                                                                            }
                                                                    };
    }
}
