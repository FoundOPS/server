using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace FoundOps.Common.Silverlight.MVVM.Models
{
    public class ThreadableNotifiableObject : INotifyPropertyChanged
    {
        private Dispatcher currentDispatcher;
        public event PropertyChangedEventHandler PropertyChanged;

        public ThreadableNotifiableObject()
        {
            if (Deployment.Current != null && Deployment.Current.Dispatcher != null)
            {
                currentDispatcher = Deployment.Current.Dispatcher;
            }
            else // if we did not get the Dispatcher throw an exception
            {
                throw new InvalidOperationException("This object must be initialized after that the RootVisual has been loaded");
            }
        }

        public void DispatchMethod(Action method)
        {
            if (currentDispatcher.CheckAccess())
                method.DynamicInvoke();
            else
                currentDispatcher.BeginInvoke(method);
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                //check if we are on the UI thread if not switch
                if (currentDispatcher.CheckAccess())
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                else
                    currentDispatcher.BeginInvoke(new Action<string>(OnPropertyChanged), propertyName);
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                //check if we are on the UI thread if not switch
                if (currentDispatcher.CheckAccess())
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                else
                    currentDispatcher.BeginInvoke(new Action<string>(OnPropertyChanged), propertyName);
            }
        }
    }
}