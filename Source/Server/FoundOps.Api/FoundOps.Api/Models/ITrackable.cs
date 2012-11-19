﻿using System;

namespace FoundOps.Api.Models
 {
     interface ITrackable
     {
         DateTime CreatedDate { get; }
         /// <summary>
         /// The last time the entity was modified. Null if it has not been updated since creation
         /// </summary>
         DateTime? LastModified { get; }
         
         Guid? LastModifyingUserId { get; }

         void SetLastModified(DateTime? lastModified, Guid? userId);
     }
 }