using System.Runtime.Serialization;

namespace FoundOps.Core.Models.RouteEntities
{
    [DataContract]
    public class Location
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}