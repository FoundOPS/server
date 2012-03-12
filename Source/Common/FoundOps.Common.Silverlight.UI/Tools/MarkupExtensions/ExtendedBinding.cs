using FoundOps.Common.Silverlight.UI.Tools.ExtensionMethods;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace FoundOps.Common.Silverlight.UI.Tools.MarkupExtensions
{
    /// <summary>
    /// Extends a binding by wrapping it with ExternalValue.
    /// Has an optional Function to prevent binding update.
    /// Should only be used on Dependency Properties.
    /// </summary>
    public class ExtendedBinding : UpdatableMarkupExtension<object>, INotifyPropertyChanged
    {
        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        /// <summary>
        /// The binding to extend.
        /// </summary>
        public Binding Binding { get; set; }

        /// <summary>
        /// An action to determine if the ExternalValue should be updated.
        /// </summary>
        public Func<object, bool> ShouldBindingUpdate { get; set; }

        #region Private Properties

        #region ExternalValue Dependency Property

        /// <summary>
        /// The value the Binding is bound to.
        /// </summary>
        public object ExternalValue
        {
            get { return GetValue(ExternalValueProperty); }
            set { SetValue(ExternalValueProperty, value); }
        }

        /// <summary>
        /// ExternalValue Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ExternalValueProperty =
            DependencyProperty.Register(
                "ExternalValue",
                typeof(object),
                typeof(ExtendedBinding),
                new PropertyMetadata(ExternalValueChanged));

        private static void ExternalValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as ExtendedBinding;
            if (c != null)
            {
                //This will automatically update the InternalValue
                c.UpdateValue(e.NewValue);
            }
        }

        #endregion

        private object _internalValue;
        /// <summary>
        /// The internal value (bound to the markup extension's parent dependency property)
        /// </summary>
        public object InternalValue
        {
            get { return _internalValue; }
            set
            {
                if (ExternalValue == value)
                {
                    _internalValue = value;
                    this.RaisePropertyChanged("InternalValue");
                    return;
                }

                //Check if the binding should update
                if (ShouldBindingUpdate != null && !ShouldBindingUpdate(value))
                    return;

                _internalValue = value;
                this.RaisePropertyChanged("InternalValue");

                //Update the value
                ExternalValue = InternalValue;
            }
        }

        #endregion

        protected override object ProvideValueInternal(IServiceProvider serviceProvider)
        {
            //Setup the binding to the ExternalValue
            this.SetBinding(ExternalValueProperty, Binding);

            //Setup a binding between the Target dependency property and the InternalValue
            //this.SetBinding(InternalValueProperty, new Binding(TargetPropertyName) { Mode = BindingMode.TwoWay, Source = TargetObject });
            var targetDependencyProperty = TargetObject.GetType().GetDependencyProperty(this.TargetPropertyName + "Property");
            ((FrameworkElement)TargetObject).SetBinding(targetDependencyProperty, new Binding("InternalValue") { Source = this, Mode = BindingMode.TwoWay });

            return InternalValue;
        }
    }
}
