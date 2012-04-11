using System;
using FoundOps.Common.Composite.Entities;
using System.Collections.Generic;

namespace FoundOps.Core.Models.CoreEntities
{
    public partial class Service : IEntityDefaultCreation, IComparable<Service>, IEqualityComparer<Service>
    {
        #region Implementation of IEntityDefaultCreation

#if SILVERLIGHT

        partial void OnCreated()
        {
            ((IEntityDefaultCreation) this).OnCreate();
        }
#else

        public Service()
        {
            ((IEntityDefaultCreation)this).OnCreate();
        }

#endif
        partial void OnCreation(); //For Extensions on Silverlight Side

        void IEntityDefaultCreation.OnCreate()
        {
            Id = Guid.NewGuid();
            ServiceDate = DateTime.UtcNow.Date;
            OnCreation();
        }

        #endregion

        public int CompareTo(Service other)
        {
            //Compare Dates first
            var compare = this.ServiceDate.CompareTo(other.ServiceDate);

            //Then compare the ServiceTemplate.Name (if it exists)
            if (compare == 0 && this.ServiceTemplate != null && other.ServiceTemplate != null)
                compare = this.ServiceTemplate.Name.CompareTo(other.ServiceTemplate.Name);

            //Then compare the Client
            if (compare == 0 && this.Client != null && other.Client != null)
                compare = this.Client.DisplayName.CompareTo(other.Client.DisplayName);

            //If it is a generated service compare the recurring service parent's id
            if (compare == 0 && this.RecurringServiceId.HasValue && other.RecurringServiceId.HasValue)
                return compare + this.RecurringServiceId.Value.CompareTo(other.RecurringServiceId);
            else
                //Otherwise compare the Service Ids
                return compare != 0 ? compare : this.Id.CompareTo(other.Id);
        }


        public bool Equals(Service x, Service y)
        {
            return x.CompareTo(y) == 0;
        }

        public int GetHashCode(Service obj)
        {
            unchecked
            {
                int result = (obj != null ? obj.GetHashCode() : 0);
                result = (result * 397) ^ (ServiceDate.GetHashCode());
                result = (result * 397) ^ (ServiceProviderId.GetHashCode());
                result = (result * 397) ^ (ClientId.GetHashCode());
                return result;
            }
        }
    }
}