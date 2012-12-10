using System;

namespace FoundOps.Api.Models
{
    public class BusinessAccount : ITrackable
    {
        public Guid Id { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? LastModified { get; private set; }
        public Guid? LastModifyingUserId { get; private set; }

        public string Name { get; set; }
        public string ImageUrl { get; set; }

        public BusinessAccount()
        {
            CreatedDate = DateTime.UtcNow;
        }

        public void SetLastModified(DateTime? lastModified, Guid? userId)
        {
            LastModified = lastModified;
            LastModifyingUserId = userId;
        }
    }
}