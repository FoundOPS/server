using System.Runtime.Serialization;

namespace FoundOps.Core.Models.CoreEntities
{
    public partial class Service
    {
        /// <summary>
        /// Gets or sets a value indicating whether this RouteTask was generated (and not inserted yet)
        /// </summary>
        [DataMember]
        public bool Generated { get; set; }
    }
}