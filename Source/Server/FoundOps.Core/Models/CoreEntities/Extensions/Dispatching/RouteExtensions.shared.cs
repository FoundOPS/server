using System;
using FoundOps.Common.Composite.Entities;
using FoundOps.Common.Composite;

namespace FoundOps.Core.Models.CoreEntities
{
    public partial class Route : IEntityDefaultCreation
    {
        #region Implementation of IEntityDefaultCreation

#if SILVERLIGHT
        partial void OnCreated()
        {
            ((IEntityDefaultCreation) this).OnCreate();
        }
#else
        public Route()
        {
            ((IEntityDefaultCreation)this).OnCreate();
        }
#endif

        partial void OnCreation(); //For Extensions on Silverlight Side

        void IEntityDefaultCreation.OnCreate()
        {
            Id = Guid.NewGuid();
            Date = DateTime.Now;
            StartTime = DateTime.Now.SetTime(9,0);
            EndTime = DateTime.Now.SetTime(17, 0);
            Name = "Route Name";
            OnCreation();
        }

        #endregion

        public int NumberOfRouteDestinations
        {
            get
            {
                return this.RouteDestinations.Count;
            }
        }
    }
}