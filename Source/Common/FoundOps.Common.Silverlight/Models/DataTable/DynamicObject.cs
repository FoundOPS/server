using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.Generic;
using System.Reflection;

namespace Telerik.Data
{
    public abstract class DynamicObject : INotifyPropertyChanged
    {
        private readonly Dictionary<string, object> valuesStorage;

        public event PropertyChangedEventHandler PropertyChanged;

        protected DynamicObject()
        {
            this.valuesStorage = new Dictionary<string, object>();
        }

        protected internal virtual T GetValue<T>(string propertyName)
        {
            object value;
            if (!this.valuesStorage.TryGetValue(propertyName, out value))
            {
                return default(T);
            }

            return (T)value;
        }

        protected internal virtual void SetValue<T>(string propertyName, T value)
        {
            this.valuesStorage[propertyName] = value;

            this.RaisePropertyChanged(propertyName);
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            var hanlder = this.PropertyChanged;
            if (hanlder != null)
            {
                hanlder(this, args);
            }
        }
    }
}
