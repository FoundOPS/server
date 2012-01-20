using System;
using System.Linq;
using System.Reactive.Linq;
using System.ServiceModel.DomainServices.Client;
using ReactiveUI;
using RiaServicesContrib;
using System.ComponentModel;
using FoundOps.Common.Silverlight.Interfaces;

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

        public IObservable<IObservedChange<object, object>> Changed
        {
            get { return _reactiveHelper.Changed; }
        }
        public IObservable<IObservedChange<object, object>> Changing
        {
            get { return _reactiveHelper.Changing; }
        }
        public IDisposable SuppressChangeNotifications()
        {
            return _reactiveHelper.SuppressChangeNotifications();
        }

        #endregion

        partial void OnCreation()
        {
            InitializeHelper();
        }

        protected override void OnLoaded(bool isInitialLoad)
        {
            if (isInitialLoad)
                //Wait for associations to set (after the load) before setting up automatic numbering
                Observable.Interval(TimeSpan.FromMilliseconds(300)).ObserveOnDispatcher().Subscribe(_ => InitializeHelper());

            base.OnLoaded(isInitialLoad);
        }

        private void InitializeHelper()
        {
            //Setup IReactiveNotifyPropertyChanged
            _reactiveHelper = new MakeObjectReactiveHelper(this);

            //Setup SubLocation automatic numbering operations
            this.SubLocations.EntityAdded += (s, e) => e.Entity.Number = SubLocations.Count;
            this.SubLocations.EntityRemoved += (s, e) =>
            {
                foreach (var subLocation in this.SubLocations.Where(subLocation => subLocation.Number > e.Entity.Number))
                    subLocation.Number = subLocation.Number - 1;
            };
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

