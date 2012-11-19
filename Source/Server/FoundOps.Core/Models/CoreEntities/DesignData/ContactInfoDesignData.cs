using System;
using System.Collections.Generic;

namespace FoundOps.Core.Models.CoreEntities.DesignData
{
    public class ContactInfoDesignData
    {
        public static List<string> DesignContactInfoTypes = new List<string> { "Phone", "Email", "Website", "Fax", "Other" };
        public static List<string> DesignContactInfoLabels = new List<string> { "Primary", "Home", "Cell" };

        public List<ContactInfo> DesignUserContactInfoList = new List<ContactInfo>
        {
            new ContactInfo
            {
                Label = "Home",
                Data = "703-582-1810",
                Type = "Phone",
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            },
            new ContactInfo
            {
                Label = "Primary Email",
                Data = "jrice@purdue.edu",
                Type = "Email",
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            },
            new ContactInfo
            {
                Label = "Blog",
                Data = "2020vision.com",
                Type = "Website",
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            }
        };

        public List<ContactInfo> DesignBusinessContactInfoList = new List<ContactInfo>
        {
            new ContactInfo
            {
                Label = "Zach Bright",
                Data = "703-582-1810",
                Type = "Phone",
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            },
            new ContactInfo
            {
                Label = "Jason Rice",
                Data = "jrice@purdue.edu",
                Type = "Email",
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            },
            new ContactInfo
            {
                Label = "Oren Shatken",
                Data = "2020vision.com",
                Type = "Website",
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            }
};
    }
}
