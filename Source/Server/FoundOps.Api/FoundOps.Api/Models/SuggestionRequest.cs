using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoundOps.Api.Models
{
    /// <summary>
    /// Contains the inputs for either ValidateInput or SuggestEntites
    /// </summary>
    public class SuggestionsRequest
    {
        /// <summary>
        ///Used for ValidateInput
        /// </summary>
        public List<string[]> RowsWithHeaders { get; set; }
        
        /// <summary>
        /// Used for SuggestEntites
        /// </summary>
        public ImportRow[] Rows { get; set; }
    }
}