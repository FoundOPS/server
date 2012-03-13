using System;
using System.Windows;
using System.Windows.Data;
using FoundOps.Common.Silverlight.UI.Tools.ExtensionMethods;

namespace FoundOps.Common.Silverlight.UI.Tools.MarkupExtensions
{
    //From http://stackoverflow.com/questions/777991/how-do-i-bind-the-binding-path-property-to-underlying-data
    /// <summary>
    /// Allows for a bindable path.
    /// </summary>
    public class IndirectBinder : UpdatableMarkupExtension<object>
    {
        #region Path Dependency Property

        /// <summary>
        /// Source
        /// </summary>
        public string Path
        {
            get { return (string)GetValue(PathProperty); }
            set { SetValue(PathProperty, value); }
        }

        /// <summary>
        /// Path Dependency Property.
        /// </summary>
        public static readonly DependencyProperty PathProperty =
            DependencyProperty.Register(
                "Path",
                typeof(string),
                typeof(IndirectBinder),
                new PropertyMetadata(PathChanged));

        private static void PathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as IndirectBinder;
            if (c == null) return;

            if (String.IsNullOrEmpty((string)e.NewValue))
                return;

            c.UpdateBindingHelper((string)e.NewValue);
        }

        #endregion
        public BindingMode Mode { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndirectBinder"/> class.
        /// </summary>
        public IndirectBinder()
        {
            Mode = BindingMode.OneWay;
        }

        protected override object ProvideValueInternal(IServiceProvider serviceProvider)
        {
            return UpdateBindingHelper(Path);
        }

        private string UpdateBindingHelper(string path)
        {
            var targetFrameworkElement = TargetObject as FrameworkElement;
            if (String.IsNullOrEmpty(path) || TargetPropertyName == null || targetFrameworkElement == null)
                return null;

            //Try to find the dependencyproperty based off the TargetPropertyName
            var targetDependencyProperty = TargetObject.GetType().GetDependencyProperty(this.TargetPropertyName + "Property");

            if (targetDependencyProperty == null)
                return null;

            var binding = new Binding(Path) { Mode = Mode, Source = targetFrameworkElement.DataContext };
            BindingOperations.SetBinding(targetFrameworkElement, targetDependencyProperty, binding);

            //Try to return the initial value of the binding
            var dataContext = targetFrameworkElement.DataContext;
            if (dataContext == null)
                return null;

            var realPi = dataContext.GetType().GetProperty(Path);
            return realPi.GetValue(dataContext, null) as String;
        }
    }
}
