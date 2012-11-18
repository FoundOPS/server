﻿using System;

namespace FoundOps.Api.Models
 {
     interface ITrackable
     {
         DateTime CreatedDate { get; }
         DateTime? LastModifiedDate { get; }
         Guid? LastModifyingUserId { get; }

         void SetLastModified(DateTime? lastModified, Guid? userId);
     }
 }