using System.Windows.Controls;
using System.Windows.Interactivity;

namespace FoundOps.Common.Silverlight.UI.Converters
{
    public class TextBoxUpdateBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.TextChanged += AssociatedObjectOnTextChanged;
        }

        private void AssociatedObjectOnTextChanged(object sender, TextChangedEventArgs args)
        {
            var bindingExpression = AssociatedObject.GetBindingExpression(TextBox.TextProperty);
            bindingExpression.UpdateSource();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.TextChanged -= AssociatedObjectOnTextChanged;
        }
    }
}