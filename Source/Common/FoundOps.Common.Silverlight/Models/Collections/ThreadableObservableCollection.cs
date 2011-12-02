using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace FoundOps.Common.Silverlight.MVVM.Models
{
    /// <summary>
    /// Collection that supports INotifyCollectionChanged notification from seperate threads
    /// </summary>
    /// <typeparam name="T">The type of object to store in the collections</typeparam>
    public class ThreadableObservableCollection<T> : ObservableCollection<T>
    {
        private Dispatcher currentDispatcher;
        /// <summary>
        /// default constructor
        /// Here we must get the Dispatcher object
        /// </summary>
        public ThreadableObservableCollection()
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

        protected override void InsertItem(int index, T item)
        {
            if (currentDispatcher.CheckAccess())
                base.InsertItem(index, item);
            else
                currentDispatcher.BeginInvoke(new Action<int, T>(InsertItem), index, item);
        }

        protected override void ClearItems()
        {
            if (currentDispatcher.CheckAccess())
                base.ClearItems();
            else
                currentDispatcher.BeginInvoke(new Action(ClearItems));
        }

        protected override void RemoveItem(int index)
        {
            if (currentDispatcher.CheckAccess())
                base.RemoveItem(index);
            else
                currentDispatcher.BeginInvoke(new Action<int>(RemoveItem), index);
        }

        protected override void SetItem(int index, T item)
        {
            if (currentDispatcher.CheckAccess())
                base.SetItem(index, item);
            else
                currentDispatcher.BeginInvoke(new Action<int, T>(SetItem), index, item);
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (currentDispatcher.CheckAccess())
                base.OnPropertyChanged(e);
            else
                currentDispatcher.BeginInvoke(new Action<PropertyChangedEventArgs>(OnPropertyChanged), e);
        }
    }
}


