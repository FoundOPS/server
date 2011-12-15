using System;
using System.Xaml;
using System.Windows;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Markup;

namespace FoundOps.Common.Silverlight.Tools
{
    public abstract class UpdatableMarkupExtension<TE> : FrameworkElement, IMarkupExtension<TE> where TE : class
    {
        private object _targetObject;
        private object _targetProperty;

        protected object TargetObject
        {
            get { return _targetObject; }
        }

        protected object TargetProperty
        {
            get { return _targetProperty; }
        }

        protected void UpdateValue(object value)
        {
            //Convert the value if there is a converter
            if (Converter != null)
                value = Converter.Convert(value, null, null, null);

            if (_targetObject != null)
            {
                if (_targetProperty as DependencyProperty != null)
                {
                    var obj = _targetObject as DependencyObject;
                    var prop = _targetProperty as DependencyProperty;

                    Action updateAction = () => obj.SetValue(prop, value);

                    // Check whether the target object can be accessed from the
                    // current thread, and use Dispatcher.Invoke if it can't

                    if (obj.CheckAccess())
                        updateAction();
                    else
                        updateAction.BeginInvoke(null, null);
                }
                else // _targetProperty is PropertyInfo
                {
                    var prop = _targetProperty as PropertyInfo;
                    prop.SetValue(_targetObject, value, null);
                }
            }
        }

        protected abstract object ProvideValueInternal(IServiceProvider serviceProvider);

        public IValueConverter Converter { get; set; }

        TE IMarkupExtension<TE>.ProvideValue(IServiceProvider serviceProvider)
        {
            var target = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (target != null)
            {
                _targetObject = target.TargetObject;
                _targetProperty = target.TargetProperty;
            }

            var value = ProvideValueInternal(serviceProvider);

            //Convert the value if there is a converter
            if (Converter != null)
                value = Converter.Convert(value, null, null, null);

            return (TE) value;
        }
    }
}
