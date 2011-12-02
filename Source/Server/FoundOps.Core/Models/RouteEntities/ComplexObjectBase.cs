using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace FoundOps.Core.Models.RouteEntities
{
    [DataContract]
    public class ComplexObjectBase
    {
        /// <summary>
        /// Gets or sets the unique id.
        /// </summary>
        /// <value>
        /// The unique id.
        /// </value>
        [Key] //Required for WCF Ria Services
        public Guid Id { get; set; }

        public ComplexObjectBase()
        {
            Id = Guid.NewGuid();
        }
    }
}