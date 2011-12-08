using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using FoundOps.Server.Authentication;
using FoundOps.Core.Models.CoreEntities;
using System.ServiceModel.DomainServices.Server;
using System.ServiceModel.DomainServices.Hosting;
using FoundOps.Core.Models.CoreEntities.DesignData;
using System.ServiceModel.DomainServices.EntityFramework;

namespace FoundOps.Server.Services
{
    // Implements application logic using the CoreEntitiesContainer context.
    // TODO: Add your application logic to these methods or in additional methods.
    // TODO: Wire up authentication (Windows/ASP.NET Forms) and uncomment the following to disable anonymous access
    // Also consider adding roles to restrict access as appropriate.
    // [RequiresAuthentication]
    [EnableClientAccess()]
    public class TechnicianDomainService : LinqToEntitiesDomainService<CoreEntitiesContainer>
    {
        readonly LocationsDesignData _locationsDesignData = new LocationsDesignData();

        [RequiresAuthentication]
        public IQueryable<Party> GetParties()
        {
            return this.ObjectContext.Parties;
        }

        [RequiresAuthentication]
        public IQueryable<Location> GetLocations()
        {
            return this.ObjectContext.Locations;
        }

        //[RequiresAuthentication]
        public IQueryable<Route> GetRoutes()
        {
            return this.ObjectContext.Routes;
        }

        [RequiresAuthentication]
        public void InsertRoute(Route route)
        {
            if ((route.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(route, EntityState.Added);
            }
            else
            {
                this.ObjectContext.Routes.AddObject(route);
            }
        }

        [RequiresAuthentication]
        public void UpdateRoute(Route currentRoute)
        {
            this.ObjectContext.Routes.AttachAsModified(currentRoute, this.ChangeSet.GetOriginal(currentRoute));
        }

        [RequiresAuthentication]
        public void DeleteRoute(Route route)
        {
            if ((route.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(route, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.Routes.Attach(route);
                this.ObjectContext.Routes.DeleteObject(route);
            }
        }

        public IQueryable<Route> GetTestTechniciansRoutes()
        {
            //TODO FIX DESIGN DATA
            //TODO ADD RouteDestinations.Location.CONTACTINFOSET

            //var routeDestination = new RouteDestination();
            //routeDestination.Client.OwnedParty.ContactInfoSet

            //var location = new Location();
            //location.ContactInfoSet.ElementAt(0).

            var routes = ObjectContext.Routes.Include("RouteDestinations").Include("RouteDestinations.Location").Include("RouteDestinations.Client").Include("RouteDestinations.Client.OwnedParty").Include("RouteDestinations.Location.ContactInfoSet")
                .Include("RouteDestinations.Client.OwnedParty.ContactInfoSet").Take(3);

            var contactInfoSets =
                routes.First().RouteDestinations.Select(rd => new[] {rd.Client.OwnedParty.ContactInfoSet, rd.Location.ContactInfoSet}.ToArray());

            return routes;
        }

        [RequiresAuthentication]
        public IEnumerable<RouteDestination> GetTechniciansRouteDestinationsAuthorized()
        {
            var route = new Route();

            var routeStop = new RouteDestination { Location = _locationsDesignData.DesignLocation, OrderInRoute = 1 };

            route.RouteDestinations.Add(routeStop);
            routeStop = new RouteDestination { Location = _locationsDesignData.DesignLocationTwo, OrderInRoute = 2, };
            route.RouteDestinations.Add(routeStop);
            routeStop = new RouteDestination { Location = _locationsDesignData.DesignLocationThree, OrderInRoute = 3 };
            route.RouteDestinations.Add(routeStop);

            return route.RouteDestinations;
        }


        public IEnumerable<RouteDestination> GetRouteDestinations()
        {
            return this.ObjectContext.RouteDestinations;
        }

        public void InsertRouteDestination(RouteDestination routeDestination)
        {
            if ((routeDestination.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(routeDestination, EntityState.Added);
            }
            else
            {
                this.ObjectContext.RouteDestinations.AddObject(routeDestination);
            }
        }

        public void UpdateRouteDestination(RouteDestination currentRouteDestination)
        {
            this.ObjectContext.RouteDestinations.AttachAsModified(currentRouteDestination, this.ChangeSet.GetOriginal(currentRouteDestination));
        }

        public void DeleteRouteDestination(RouteDestination routeDestination)
        {
            if ((routeDestination.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(routeDestination, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.RouteDestinations.Attach(routeDestination);
                this.ObjectContext.RouteDestinations.DeleteObject(routeDestination);
            }
        }

        [Query(IsDefault = true)]
        public IQueryable<TrackPoint> GetTrackPoints()
        {
            return this.ObjectContext.TrackPoints;
        }

        [Invoke]
        [RequiresAuthentication]
        public void InsertTrackPoint(long latitude, long longitude, DateTime timeStamp)
        {
            this.ObjectContext.TrackPoints.AddObject(new TrackPoint
            {
                Timestamp = timeStamp,
                Latitude = latitude,
                Longitude = longitude,
            });

            this.ObjectContext.SaveChanges();
        }

        public IQueryable<File> GetFiles()
        {
            return null;
        }
    }
}


