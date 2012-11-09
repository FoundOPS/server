using System;
using System.Collections.Generic;

namespace FoundOps.Api.Models
{
    public class RowSuggestions
    {
        public List<Guid> ClientSuggestions { get; set; }
        public List<Guid> LocationSuggestions { get; set; }
        public List<Repeat> Repeats { get; set; }
        public List<Guid> ContactInfoSuggestions { get; set; }

        public RowSuggestions()
        {
            ClientSuggestions = new List<Guid>();
            LocationSuggestions = new List<Guid>();
            ContactInfoSuggestions = new List<Guid>();
            Repeats = new List<Repeat>();
        }
    }
}