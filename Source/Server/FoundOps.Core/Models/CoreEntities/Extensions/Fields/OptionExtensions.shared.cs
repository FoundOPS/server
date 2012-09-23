using System.Runtime.Serialization;

// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class Option
    {
        [DataMember]
        public OptionsField Parent { get; set; }
    }
}