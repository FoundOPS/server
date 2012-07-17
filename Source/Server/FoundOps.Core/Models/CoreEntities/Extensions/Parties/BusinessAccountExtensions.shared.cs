//Partial class must be part of same namespace
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
{
// ReSharper restore CheckNamespace
    public partial class BusinessAccount
    {
        public override string DisplayName
        {
            get { return this.Name; }
            set { this.Name = value; }
        }

        partial void OnNameChanged()
        {
#if SILVERLIGHT
            this.OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("DisplayName"));      
#else
            this.OnPropertyChanged("DisplayName");
#endif
        }
    }
}