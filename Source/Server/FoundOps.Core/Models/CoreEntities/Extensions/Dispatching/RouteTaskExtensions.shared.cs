using System;
using FoundOps.Common.Composite.Entities;

//Partial class must be part of same namespace
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    /// <summary>
    /// An enum for task statuses.
    /// </summary>
    public enum Status
    {
        Unrouted = 1,
        Routed = 2,
        InProgress = 3,
        OnHold = 4,
        Completed = 5,
        Incomplete = 6
    }

    public partial class RouteTask : IEntityDefaultCreation, ICompositeRaiseEntityPropertyChanged
    {
        #region Implementation of IEntityDefaultCreation

#if SILVERLIGHT
        partial void OnCreated()
        {
            ((IEntityDefaultCreation) this).OnCreate();
        }
#else
        public RouteTask()
        {
            ((IEntityDefaultCreation)this).OnCreate();
        }
#endif

        partial void OnCreation(); //For Extensions on Silverlight Side

        void IEntityDefaultCreation.OnCreate()
        {
            Id = Guid.NewGuid();
            Name = "";
            OnCreation();
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

        //public Status Status
        //{
        //    get { return (Status)StatusInt; }
        //    set
        //    {
        //        StatusInt = (int)value;
        //        CompositeRaiseEntityPropertyChanged("Status");
        //    }
        //}
    }
}