using FoundOps.Common.Composite.Entities;

//This is a partial class, must be in the same namespace so disable warning
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
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
            get { return Name; }
            set { Name = value; }
        }
    }
}