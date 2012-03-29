using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using FoundOps.Common.Composite.Entities;

//This is a partial class, must be in the same namespace so disable warning
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class ServiceHolder
    {
        /// <summary>
        /// DomainService operations require one public property marked with the KeyAttribute.
        /// This is a generated key, purely for the use of the DomainService. 
        /// NOTE: Do not reference it in code for any purpose.
        /// </summary>
        [Key]
        [DataMember]
        [Obsolete]
        public Guid Id { get; set; }
    }
}