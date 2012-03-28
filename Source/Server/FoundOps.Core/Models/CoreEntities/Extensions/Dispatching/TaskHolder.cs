using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class TaskHolder
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

        public TaskHolder()
        {
            // ReSharper disable CSharpWarnings::CS0612
            Id = Guid.NewGuid();
            // ReSharper restore CSharpWarnings::CS0612
        }
    }
}
