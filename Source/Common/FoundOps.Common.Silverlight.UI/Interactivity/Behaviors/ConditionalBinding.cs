using System.Windows;
using System.Windows.Data;
using System.Windows.Interactivity;
using FoundOps.Common.Composite.Tools;
using FoundOps.Common.Silverlight.UI.Tools;
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
            //Correct the binding initially 
            CorrectConditionalDependencyPropertyBinding(AssociatedObject.GetProperty<object>(RequiredDependencyPropertyName) != null);

            //and correct the binding whenever the RequiredDepenendencyProperty's value changes
            AssociatedObject.RegisterForNotification(RequiredDependencyPropertyName,
                                                     (dp, dpea) =>
                                                     {
                                                         if (dpea.NewValue != dpea.OldValue)
                                                             CorrectConditionalDependencyPropertyBinding(dpea.NewValue != null);
                                                     });

            base.OnAttached();
        }

        private Binding _lastBinding;

        /// <summary>
        /// Corrects the conditional dependency property binding.
        /// When the RequiredDependencyProperty has a value: UpdateSourceTrigger = Default, otherwise the UpdateSourceTrigger = Explicit.
        /// </summary>
        private void CorrectConditionalDependencyPropertyBinding(bool requiredDependencyPropertyHasValue)
        {
            var bindingExpression = AssociatedObject.GetBindingExpression(ConditionalDependencyProperty);

            if (bindingExpression == null || bindingExpression.ParentBinding == null) return;

            _lastBinding = bindingExpression.ParentBinding;
            var bindingCopy = _lastBinding.CopyBinding();

            //When the RequiredDependencyProperty has a value set the UpdateSourceTrigger = Default, otherwise the UpdateSourceTrigger = Explicit.
            bindingCopy.UpdateSourceTrigger = requiredDependencyPropertyHasValue ? UpdateSourceTrigger.Default : UpdateSourceTrigger.Explicit;

            AssociatedObject.SetBinding(ConditionalDependencyProperty, bindingCopy);
        }
    }
}
