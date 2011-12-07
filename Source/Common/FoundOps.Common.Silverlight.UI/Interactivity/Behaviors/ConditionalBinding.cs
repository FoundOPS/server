using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Data;
using FoundOps.Common.Composite.Tools;
using FoundOps.Common.Silverlight.UI.Tools;
using Telerik.Windows.Controls;
using System.Windows.Interactivity;
using FoundOps.Common.Silverlight.UI.Tools.ExtensionMethods;

namespace FoundOps.Common.Silverlight.UI.Interactivity.Behaviors
{
    /// <summary>
    /// Will only enable the SelectedValue binding whenever the ItemSource is not null.
    /// </summary>
    public class ConditionalBinding : Behavior<FrameworkElement>
    {
        /// <summary>
        /// The dependency property that must have a value.
        /// When the RequiredDependencyProperty has a value: UpdateSourceTrigger = Default, otherwise the UpdateSourceTrigger = Explicit.
        /// Ex. ItemsSource
        /// </summary>
        public string RequiredDependencyPropertyName { get; set; }

        /// <summary>
        /// The dependency property that is conditional on the RequiredDependencyProperty.
        /// </summary>
        public DependencyProperty ConditionalDependencyProperty { get; set; }

        protected override void OnAttached()
        {
            AssociatedObject.RegisterForNotification(RequiredDependencyPropertyName,
                                                     (dp, dpea) => CorrectConditionalDependencyPropertyBinding(dpea.NewValue != null));

            CorrectConditionalDependencyPropertyBinding(AssociatedObject.GetProperty<object>(RequiredDependencyPropertyName) != null);

            base.OnAttached();
        }

        /// <summary>
        /// Corrects the conditional dependency property binding.
        /// When the RequiredDependencyProperty has a value: UpdateSourceTrigger = Default, otherwise the UpdateSourceTrigger = Explicit.
        /// </summary>
        private void CorrectConditionalDependencyPropertyBinding(bool requiredDependencyPropertyHasValue)
        {
            var binding =
                AssociatedObject.GetBindingExpression(ConditionalDependencyProperty).ParentBinding;

            if (binding == null) return;

            var bindingCopy = binding.CopyBinding();

            // When the RequiredDependencyProperty has a value: UpdateSourceTrigger = Default, otherwise the UpdateSourceTrigger = Explicit.
            bindingCopy.UpdateSourceTrigger = requiredDependencyPropertyHasValue ? UpdateSourceTrigger.Explicit : UpdateSourceTrigger.Default;

            AssociatedObject.SetBinding(ConditionalDependencyProperty, bindingCopy);
        }
    }
}
