using System;
using System.Linq;
using System.ComponentModel;
using FoundOps.Common.Composite.Entities;

//Partial class must be part of same namespace
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class RouteDestination : IEntityDefaultCreation, ICompositeRaiseEntityPropertyChanged
    {
        #region Public Properties

        #region Implementation of IEntityDefaultCreation

#if SILVERLIGHT

        protected override void OnLoaded(bool isInitialLoad)
        {
            if (isInitialLoad)
                OnInitialization();

            base.OnLoaded(isInitialLoad);
        }

        partial void OnCreated()
        {
            ((IEntityDefaultCreation) this).OnCreate();
        }

        partial void OnInitializedSilverlight();

        partial void OnInitialization()
        {
            this.RouteTasks.EntityAdded += RouteTasksEntityAdded;

            this.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "Location")
                    this.RaisePropertyChanged("Name");
            };

            OnInitializedSilverlight();
        }

        void RouteTasksEntityAdded(object sender, System.ServiceModel.DomainServices.Client.EntityCollectionChangedEventArgs<RouteTask> e)
        {
            RouteTasksEntityAddedComposite(e.Entity);
        }
#else
        public RouteDestination()
        {
            ((IEntityDefaultCreation)this).OnCreate();
            this.RouteTasks.AssociationChanged += RouteTasksAssociationChanged;
        }

        void RouteTasksAssociationChanged(object sender, System.ComponentModel.CollectionChangeEventArgs e)
        {
            if (e.Action == CollectionChangeAction.Add)
                RouteTasksEntityAddedComposite((RouteTask)e.Element);
        }
#endif

        public void RouteTasksEntityAddedComposite(RouteTask routeTask)
        {
            //If this RouteDestination does not already have a Location update it with the task's location
            if (this.Location == null)
            {
                if (routeTask.Location != null)
                    this.Location = routeTask.Location;
            }

            //If this RouteDestination does not already have a Client update it with the task's client
            if (this.Client == null)
            {
                if (routeTask.Client != null)
                    this.Client = routeTask.Client;
            }
        }

        partial void OnInitialization(); //For Extensions on Silverlight Side

        void IEntityDefaultCreation.OnCreate()
        {
            Id = Guid.NewGuid();
            OnInitialization();
        }

        #endregion

        #region Implementation of ICompositeRaiseEntityPropertyChanged

#if SILVERLIGHT
        public void CompositeRaiseEntityPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
#else
        public void CompositeRaiseEntityPropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }
#endif
        #endregion

        public TimeSpan? TravelTimeFromLast
        {
            get
            {
                var random = new Random();
                return new TimeSpan(0, 0, random.Next(5, 31), 0);
            }
        }

        public DateTime StartTime
        {
            get
            {
                if (this.Route == null || OrderInRoute <= 0)
                    return new DateTime();

                if (OrderInRoute == 1)
                    return this.Route.StartTime;

                //Calculate the start time
                var previousDestination =
                    this.Route.RouteDestinations.FirstOrDefault(rd => rd.OrderInRoute == (this.OrderInRoute - 1));
                if (TravelTimeFromLast != null)
                    return previousDestination.EndTime.Add((TimeSpan)TravelTimeFromLast);
                return previousDestination.EndTime;
            }
        }

        public DateTime EndTime
        {
            get
            {
                var endTime = this.StartTime;
                return this.RouteTasks.Aggregate(endTime, (current, routeTask) => current + routeTask.EstimatedDuration);
            }
        }

        #endregion
    }
}