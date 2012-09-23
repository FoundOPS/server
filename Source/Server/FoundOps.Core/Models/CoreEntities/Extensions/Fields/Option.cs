using System.ComponentModel;
using System.Runtime.Serialization;

// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class Option : INotifyPropertyChanged
    {
        private string _name;
        [DataMember]
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged("IsChecked");
            }
        }

        private bool _isChecked;
        [DataMember]
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                RaisePropertyChanged("IsChecked");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
#if SILVERLIGHT
                handler(propertyName);
#else
                handler(this, new PropertyChangedEventArgs(propertyName));
#endif
            }

        }
    }
}