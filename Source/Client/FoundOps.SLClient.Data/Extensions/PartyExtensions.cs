using System;
using ReactiveUI;
using System.ComponentModel;

//This is a partial class, must be in the same namespace so disable warning
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class Party : IReactiveNotifyPropertyChanged
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
        }
    }
}
