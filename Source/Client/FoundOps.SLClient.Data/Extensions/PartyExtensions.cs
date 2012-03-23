using FoundOps.Common.Silverlight.UI.Interfaces;
using ReactiveUI;
using System;
using System.ComponentModel;
using System.Linq;

//This is a partial class, must be in the same namespace so disable warning
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class Party : IReactiveNotifyPropertyChanged, ILoadDetails
    {
        #region Public Properties

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

        #region Implementation of ILoadDetails

        private bool _detailsLoaded;
        /// <summary>
        /// Gets or sets a value indicating whether [details loaded].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [details loading]; otherwise, <c>false</c>.
        /// </value>
        public bool DetailsLoaded
        {
            get { return _detailsLoaded; }
            set
            {
                _detailsLoaded = value;
                this.RaisePropertyChanged("DetailsLoaded");
            }
        }

        #endregion

        /// <summary>
        /// Returns the first owned role.
        /// </summary>
        public Role FirstOwnedRole { get { return this.OwnedRoles.FirstOrDefault(); } }

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
        }
    }
}
