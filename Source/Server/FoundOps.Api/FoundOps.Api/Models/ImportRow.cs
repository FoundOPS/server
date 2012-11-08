using System.Collections.Generic;

namespace FoundOps.Api.Models
{
    public class ImportRow
    {
        public Client Client { get; set; }

        public Location Location { get; set; }

        public Repeat Repeat { get; set; }

        public List<ContactInfo> ContactInfoSet { get; set; }

        public ImportRow()
        {
            ContactInfoSet = new List<ContactInfo>();
        }
    }
}