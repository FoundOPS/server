using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;


namespace FoundOps.Common.Silverlight.Models.DataTable
{
    public class DataTable<T> : IEnumerable, INotifyCollectionChanged
    {
        private IList<DataColumn<T>> _columns;
        private ObservableCollection<DataRow<T>> _rows;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public IList<DataColumn<T>> Columns
        {
            get
            {
                if (_columns == null)
                {
                    _columns = new List<DataColumn<T>>();
                }

                return _columns;
            }
        }

        public IList<DataRow<T>> Rows
        {
            get
            {
                if (this._rows == null)
                {
                    this._rows = new ObservableCollection<DataRow<T>>();
                    this._rows.CollectionChanged += OnRowsCollectionChanged;
                }

                return _rows;
            }
        }

        private void OnRowsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.OnCollectionChanged(e);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    this.OnCollectionChanged(e);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    this.OnCollectionChanged(e);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.OnCollectionChanged(e);
                    break;
                default:
                    this.OnCollectionChanged(e);
                    break;
            }
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var handler = this.CollectionChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public IEnumerator GetEnumerator()
        {
            return this.Rows.GetEnumerator();
        }
    }

}