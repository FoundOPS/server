using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoundOPS.API.Models
{
    public class NumericField
    {
        public string Mask { get; set; }

        public int DecimalPlaces { get; set; }

        public decimal Minimum { get; set; }

        public decimal Maximum { get; set; }

        public decimal Value { get; set; }
    }
}