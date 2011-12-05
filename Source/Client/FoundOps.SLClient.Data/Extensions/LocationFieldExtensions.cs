// Needs to be in the same namespace, because it is a partial class
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class LocationField
    {
        protected override Field MakeChildSilverlight()
        {
            //Clone using RIA Services Contrib's Entity Graph
            var child = (LocationField)base.MakeChildSilverlight();

            //Update the value
            child.Value = this.Value;

            return child;
        }
    }
}
