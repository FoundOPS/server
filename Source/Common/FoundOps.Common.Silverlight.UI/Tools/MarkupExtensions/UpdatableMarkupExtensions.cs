using System;
using System.Xaml;
using System.Windows;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Markup;
using FoundOps.Common.Silverlight.UI.Tools.ExtensionMethods;

namespace FoundOps.Common.Silverlight.UI.Tools.MarkupExtensions
{
    public abstract class UpdatableMarkupExtension<TE> : FrameworkElement, IMarkupExtension<TE> where TE : class
    {
        public object TargetObject { get; private set; }

        public string TargetPropertyName { get; private set; }
        public object TargetProperty { get; private set; }

        protected void UpdateValue(object value)
        {
            //Convert the value if there is a converter
            if (Converter != null)
                value = Converter.Convert(value, null, ConverterParameter, null);

            if (TargetObject == null) return;
            if (TargetProperty as DependencyProperty != null)
            {
                var obj = TargetObject as DependencyObject;
                var prop = TargetProperty as DependencyProperty;

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
                var prop = TargetProperty as PropertyInfo;
                prop.SetValue(TargetObject, value, null);
            }
        }

        protected abstract object ProvideValueInternal(IServiceProvider serviceProvider);

        public IValueConverter Converter { get; set; }
        public object ConverterParameter { get; set; }

        public TE ProvideValue(IServiceProvider serviceProvider)
        {
            var target = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (target != null)
            {
                TargetObject = target.TargetObject;
                TargetProperty = target.TargetProperty;
                if (TargetObject != null && TargetProperty != null)
                    TargetPropertyName = TargetProperty is DependencyProperty
                                             ? ((DependencyProperty)TargetProperty).GetDependencyPropertyName(target.TargetObject.GetType())
                                             : ((PropertyInfo)TargetProperty).Name;
            }

            var value = ProvideValueInternal(serviceProvider);

            //Convert the value if there is a converter
            if (Converter != null)
                value = Converter.Convert(value, null, ConverterParameter, null);

            return (TE)value;
        }
    }
}
