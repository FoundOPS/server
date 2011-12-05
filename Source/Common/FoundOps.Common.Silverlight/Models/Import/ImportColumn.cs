using System.ComponentModel;
using FoundOps.Common.Silverlight.Models.Import;
using FoundOps.Common.Silverlight.Models.DataTable;

namespace FoundOps.Framework.Views.Models.Import
{
    public class ImportColumn : DataColumn<ValueWithOptionalAssociation>, INotifyPropertyChanged
    {
        public ImportColumnType ImportColumnType { get; set; }

        //TODO
        private int? _multiplicity;
        public int? Multiplicity
        {
            get { return _multiplicity; }
            set
            {
                _multiplicity = value;
                this.RaisePropertyChanged("Multiplicity");
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
