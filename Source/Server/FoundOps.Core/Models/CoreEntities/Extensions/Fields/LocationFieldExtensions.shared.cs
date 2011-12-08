using System;

namespace FoundOps.Core.Models.CoreEntities
{
    public enum LocationFieldType
    {
        Destination = 0
    }

    public partial class LocationField
    {
        public LocationFieldType LocationFieldType
        {
            get
            {
                return (LocationFieldType)Convert.ToInt32(this.LocationFieldTypeInt);
            }
            set
            {
                this.LocationFieldTypeInt = Convert.ToInt16(value);
            }
        }

        partial void OnLocationFieldTypeIntChanged()
        {
            this.CompositeRaiseEntityPropertyChanged("LocationFieldType");
        }

        //The way to clone an entity on the server is different then the way to clone on the client
        //That is why this is split into !SILVERLIGHT and SILVERLIGHT
#if !SILVERLIGHT
        public override Field MakeChild()
        {
            var child = (LocationField)base.MakeChild();
            child.Value = this.Value;
            return child;
        }
#endif
    }
}