using System;
using FoundOps.Common.Silverlight.Tools;
using ReactiveUI;
using RiaServicesContrib;
using System.ComponentModel;
using FoundOps.Common.Silverlight.Interfaces;
using System.ServiceModel.DomainServices.Client;

//Partial class must be part of same namespace
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class Location : IRaiseValidationErrors, IReactiveNotifyPropertyChanged
    {
        #region IReactiveNotifyPropertyChanged

        public event PropertyChangingEventHandler PropertyChanging;

        MakeObjectReactiveHelper _reactiveHelper;

        public IObservable<IObservedChange<object, object>> Changed { get { return _reactiveHelper.Changed; } }

        public IObservable<IObservedChange<object, object>> Changing { get { return _reactiveHelper.Changing; } }

        public IDisposable SuppressChangeNotifications()
        {
            return _reactiveHelper.SuppressChangeNotifications();
        }

        #endregion

        partial void OnCreation()
        {
            //Setup IReactiveNotifyPropertyChanged
            _reactiveHelper = new MakeObjectReactiveHelper(this);

            SubLocationsListWrapper = new OrderedEntityCollection<SubLocation>(this.SubLocations, "Number", false);
        }

        protected override void OnLoaded(bool isInitialLoad)
        {
            //Setup IReactiveNotifyPropertyChanged
            _reactiveHelper = new MakeObjectReactiveHelper(this);

            base.OnLoaded(isInitialLoad);

            if (isInitialLoad)
            {
                SubLocationsListWrapper = new OrderedEntityCollection<SubLocation>(this.SubLocations, "Number", false);
            }
        }

        //private void InitializeHelper()
        //{
            ////Setup SubLocation automatic numbering operations
            //this.SubLocations.EntityAdded += (s, e) => e.Entity.Number = SubLocations.Count;
            //this.SubLocations.EntityRemoved += (s, e) =>
            //{
            //    foreach (var subLocation in this.SubLocations.Where(subLocation => subLocation.Number > e.Entity.Number))
            //        subLocation.Number = subLocation.Number - 1;
            //};
        //}

        private OrderedEntityCollection<SubLocation> _subLocationsListWrapper;
        public OrderedEntityCollection<SubLocation> SubLocationsListWrapper
        {
            get { return _subLocationsListWrapper; }
            private set
            {
                _subLocationsListWrapper = value;
                this.RaisePropertyChanged("SubLocationsListWrapper");
            }
        }

        /// <summary>
        /// Gets the telerik location.
        /// </summary>
        public Telerik.Windows.Controls.Map.Location? TelerikLocation
        {
            get
            {
                if (this.Latitude == null || this.Longitude == null)
                    return null;

                return new Telerik.Windows.Controls.Map.Location(System.Convert.ToDouble(this.Latitude),
                                                                 System.Convert.ToDouble(this.Longitude));
            }
        }

        /// <summary>
        /// Raises the validation errors.
        /// </summary>
        public void RaiseValidationErrors()
        {
            this.BeginEdit();
            this.EndEdit();
        }

        /// <summary>
        /// Gets the entity graph of Location to remove.
        /// </summary>
        public EntityGraph<Entity> EntityGraphToRemove
        {
            get
            {
                var graphShape = new EntityGraphShape().Edge<Location, ContactInfo>(location => location.ContactInfoSet);
                return new EntityGraph<Entity>(this, graphShape);
            }
        }
    }
}

