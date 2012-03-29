using System;
using FoundOps.Common.Composite.Entities;

//This is a partial class, must be in the same namespace so disable warning
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class ServiceHolder : IEntityDefaultCreation
    {
        /// <summary>
        /// The ExistingServiceId. (Used instead of ServiceId so change tracking does not kick in).
        /// </summary>
        public Guid? ExistingServiceId { get; set; }

        #region Implementation of IEntityDefaultCreation

#if SILVERLIGHT
        partial void OnCreated()
        {
            ((IEntityDefaultCreation) this).OnCreate();
        }
#else
        public ServiceHolder()
        {
            ((IEntityDefaultCreation)this).OnCreate();
        }
#endif

        public void OnCreate()
        {
            // ReSharper disable CSharpWarnings::CS0612
            Id = Guid.NewGuid();
            // ReSharper restore CSharpWarnings::CS0612

            OnCreation();
        }

        partial void OnCreation(); //For Extensions on Silverlight Side

        #endregion

        #region Overriden Methods

        /// <summary>
        /// Use ExistingServiceId instead of ServiceId to prevent change tracking.
        /// </summary>
        partial void OnServiceIdChanged()
        {
            ExistingServiceId = ServiceId;
        }

        public override bool Equals(Object obj)
        {
            return Equals(obj as ServiceHolder);
        }

        protected bool Equals(ServiceHolder other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            //They are equal if the ServiceIds are equal or if the OccurDate and the RecurringServiceIds are equal
            return (ExistingServiceId.HasValue && ExistingServiceId.Equals(other.ExistingServiceId)) ||
                (OccurDate.Equals(other.OccurDate) &&
                RecurringServiceId.HasValue && RecurringServiceId.Equals(other.RecurringServiceId));
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = RecurringServiceId.GetHashCode();
                hashCode = (hashCode * 397) ^ ExistingServiceId.GetHashCode();
                hashCode = (hashCode * 397) ^ OccurDate.GetHashCode();
                hashCode = (hashCode * 397) ^ Id.GetHashCode();
                return hashCode;
            }
        }

        #endregion
    }
}