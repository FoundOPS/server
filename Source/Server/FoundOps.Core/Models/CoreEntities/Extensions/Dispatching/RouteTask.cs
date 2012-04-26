using System;
using System.Runtime.Serialization;

namespace FoundOps.Core.Models.CoreEntities.Extensions.Dispatching
{
    class RouteTask
    {
        /// <summary>
        /// Gets or sets the Guid of a RouteTask that this was generated off of.
        /// It will be set if a RouteTask was left with a Status of "OnHold"
        /// Used to set the DelayedChildId property on RouteTask
        /// See: https://github.com/FoundOPS/FoundOPS/wiki/Route-Tasks for further detail
        /// </summary>
        [DataMember]
        public Guid DelayedParentId { get; set; }
    }
}
