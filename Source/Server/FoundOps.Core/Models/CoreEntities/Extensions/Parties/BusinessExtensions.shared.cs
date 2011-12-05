using System;
using FoundOps.Common.Composite.Entities;

namespace FoundOps.Core.Models.CoreEntities
{
    public partial class Business : ICompositeRaiseEntityPropertyChanged
    {
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

        partial void OnNameChanged()
        {
            this.CompositeRaiseEntityPropertyChanged("DisplayName");
        }

        public override string DisplayName
        {
            get
            {
                return Name;
            }
        }
    }
}