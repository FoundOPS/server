using System;

namespace FoundOps.Api.Models
{
    public class BusinessAccount
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public Guid? LastModifyingUserId { get; set; }
    }
}