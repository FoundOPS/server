using System;
using System.ComponentModel;
using System.Linq;
using FoundOps.Common.Silverlight.MVVM.Validation;
using ReactiveUI;

namespace FoundOps.Core.Models.CoreEntities
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
                InitializeHelper();

            base.OnLoaded(isInitialLoad);
        }

        private void InitializeHelper()
        {
            //Setup IReactiveNotifyPropertyChanged
            _reactiveHelper = new MakeObjectReactiveHelper(this);
            this.SubLocations.EntityAdded += SubLocationsEntityAdded;
            this.SubLocations.EntityRemoved += SubLocationsEntityRemoved;
        }

        void SubLocationsEntityAdded(object sender, System.ServiceModel.DomainServices.Client.EntityCollectionChangedEventArgs<SubLocation> e)
        {
            e.Entity.Number = SubLocations.Count;
        }

        void SubLocationsEntityRemoved(object sender, System.ServiceModel.DomainServices.Client.EntityCollectionChangedEventArgs<SubLocation> e)
        {
            foreach (var subLocation in
                this.SubLocations.Where(subLocation => subLocation.Number > e.Entity.Number))
            {
                subLocation.Number = subLocation.Number - 1;
            }
        }

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

        public void RaiseValidationErrors()
        {
            this.BeginEdit();
            this.EndEdit();
        }
    }
}

