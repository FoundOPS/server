﻿using System;

namespace FoundOps.Api.Models
 {
     interface ITrackable
     {
         DateTime CreatedDate { get; }
         DateTime? LastModifiedDate { get; set; }
         Guid? LastModifyingUserId { get; set; }
     }
 }