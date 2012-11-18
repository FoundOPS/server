using System;

namespace FoundOps.Api.Models
{
    public class BusinessAccount : ITrackable
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime? LastModifiedDate { get; set; }
        public Guid? LastModifyingUserId { get; set; }

        public BusinessAccount(DateTime createdDate)
        {
            CreatedDate = createdDate;
        }
    }
}