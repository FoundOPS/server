using System;
using System.Collections.Generic;

namespace FoundOps.Api.Models
{
    public class Suggestions
    {
        public List<Client> Clients { get; set; }
        public List<Location> Locations { get; set; }
        public List<ContactInfo> ContactInfoSet { get; set; } 
        public List<RowSuggestions> RowSuggestions { get; set; }

        public Suggestions()
        {
            Clients = new List<Client>();
            Locations = new List<Location>();
            ContactInfoSet = new List<ContactInfo>();
            RowSuggestions = new List<RowSuggestions>();
        }
    }
}