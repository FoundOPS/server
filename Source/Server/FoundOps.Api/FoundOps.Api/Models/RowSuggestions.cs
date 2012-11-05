using System;
using System.Collections.Generic;

namespace FoundOps.Api.Models
{
    public class RowSuggestions
    {
        public List<Guid> ClientSuggestions { get; set; }
        public List<Guid> LocationSuggestions { get; set; }
        public List<Repeat> Repeat { get; set; } 

        public RowSuggestions()
        {
            ClientSuggestions = new List<Guid>();
            LocationSuggestions = new List<Guid>();
            Repeat = new List<Repeat>();
        }
    }
}