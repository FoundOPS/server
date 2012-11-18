﻿using System;

namespace FoundOps.Api.Models
{
    public class BusinessAccount : ITrackable
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }

        public DateTime CreatedDate { get; private set; }
        public DateTime? LastModifiedDate { get; private set; }
        public Guid? LastModifyingUserId { get; private set; }
        
        public void SetLastModified(DateTime? lastModified, Guid? userId)
        {
            LastModifiedDate = lastModified;
            LastModifyingUserId = userId;
        }

        public BusinessAccount(DateTime createdDate)
        {
            CreatedDate = createdDate;
        }
    }
}