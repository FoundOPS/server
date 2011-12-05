using System.ComponentModel;

namespace FoundOps.Common.Silverlight.Models.Import
{
    public class ValueWithOptionalAssociation : INotifyPropertyChanged
    {
        private object _value;
        public object Value
        {
            get { return _value; }
            set
            {
                _value = value;
                this.RaisePropertyChanged("Value");
            }
        }

        private object _optionalAssociation;
        public object OptionalAssociation
        {
            get { return _optionalAssociation; }
            set
            {
                _optionalAssociation = value;
                this.RaisePropertyChanged("OptionalAssociation");
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected void RaisePropertyChanged(params string[] propertyNames)
        {
            foreach (var name in propertyNames)
            {
                RaisePropertyChanged(name);
            }
        }
        #endregion
    }
}
