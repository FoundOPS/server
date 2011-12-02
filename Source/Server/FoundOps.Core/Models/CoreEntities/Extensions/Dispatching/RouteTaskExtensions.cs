using System.Runtime.Serialization;
using FoundOps.Common.Composite.Entities;

namespace FoundOps.Core.Models.CoreEntities
{
    public partial class RouteTask
    {
        /// <summary>
        /// Gets or sets a value indicating whether this RouteTask was generated (and not inserted yet) on the server.
        /// </summary>
        [DataMember]
        public bool GeneratedOnServer { get; set; }
    }
}