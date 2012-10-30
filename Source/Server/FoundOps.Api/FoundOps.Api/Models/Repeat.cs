using System;
using FoundOps.Api.Tools;

namespace FoundOps.Api.Models
{
    public class Repeat : IImportable
    {
        public Guid Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? EndAfterTimes { get; set; }
        public int FrequencyInt { get; set; }
        public int FrequencyDetailInt { get; set; }
        public Guid? RecurringServiceId { get; set; }
        public int StatusInt { get; set; }
    }
}