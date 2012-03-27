using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

//This is a partial class, must be in the same namespace so disable warning
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class ServiceHolder
    {
        /// <summary>
        /// DomainService operations require one public property marked with the KeyAttribute.
        /// This is a generated key, purely for the use of the DomainService. 
        /// NOTE: Do not reference it in code for any purpose.
        /// </summary>
        [Key]
        [DataMember]
        [Obsolete]
        public Guid Id { get; set; }

        public ServiceHolder()
        {
            // ReSharper disable CSharpWarnings::CS0612
            Id = Guid.NewGuid();
            // ReSharper restore CSharpWarnings::CS0612
        }

        #region Overridden Methods

        public override bool Equals(Object obj)
        {
            return Equals(obj as ServiceHolder);
        }

        protected bool Equals(ServiceHolder other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            //They are equal if the ServiceIds are equal or if the OccurDate and the RecurringServiceIds are equal
            return (ServiceId.HasValue && ServiceId.Equals(other.ServiceId)) ||
                (OccurDate.Equals(other.OccurDate) &&
                RecurringServiceId.HasValue && RecurringServiceId.Equals(other.RecurringServiceId));
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = RecurringServiceId.GetHashCode();
                hashCode = (hashCode * 397) ^ ServiceId.GetHashCode();
                hashCode = (hashCode * 397) ^ OccurDate.GetHashCode();
                hashCode = (hashCode * 397) ^ Id.GetHashCode();
                return hashCode;
            }
        }

        #endregion
    }
}