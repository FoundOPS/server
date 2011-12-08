using System;
using System.ComponentModel;
using System.Windows;
using GalaSoft.MvvmLight;
using System.Windows.Threading;

namespace FoundOps.Common.Silverlight.UI.ViewModels
{
    public abstract class ThreadableVM : ViewModelBase, INotifyPropertyChanged
    {
        #region Public Properties

        public new event PropertyChangedEventHandler PropertyChanged;

        public Guid Id { get; private set; }

        #endregion

        #region Protected (Extendable) Features

        protected abstract void RegisterCommands();
        protected abstract void RegisterMessages();

        protected override void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                CurrentDispatcher.BeginInvoke(new Action<string>(OnPropertyChanged), propertyName);
        }
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                //check if we are on the UI thread if not switch
                if (CurrentDispatcher.CheckAccess())
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                else
                    CurrentDispatcher.BeginInvoke(new Action<string>(OnPropertyChanged), propertyName);
            }
        }
        #endregion

        #region Local Variables

        public Dispatcher CurrentDispatcher;

        #endregion

        protected ThreadableVM()
        {
            if (Application.Current != null && Deployment.Current.Dispatcher != null && Deployment.Current.Dispatcher != null)
            {
                CurrentDispatcher = Deployment.Current.Dispatcher;
            }
            else // if we did not get the Dispatcher throw an exception
            {
                throw new InvalidOperationException("This object must be initialized after that the RootVisual has been loaded");
            }

            Id = Guid.NewGuid();

            RegisterCommands();
            RegisterMessages();
        }

        #region ThreadableVM's Logic

        public void DispatchMethod(Action method)
        {
            if (CurrentDispatcher.CheckAccess())
                method.DynamicInvoke();
            else
                CurrentDispatcher.BeginInvoke(method);
        }

        #endregion
    }
}
