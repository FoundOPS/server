using System;
using System.Runtime.Serialization;

namespace FoundOps.Core.Models.CoreEntities
{
    public partial class Service
    {
        /// <summary>
        /// Gets or sets a value indicating whether this RouteTask was generated (and not inserted yet)
        /// </summary>
        [Obsolete("Should use Service holder instead")]
        [DataMember]
        public bool Generated { get; set; }
    }
}