using System.Windows.Data;
using FoundOps.Common.Composite.Tools;

namespace FoundOps.Common.Silverlight.UI.Tools
{
    public static class BindingTools
    {
        private static readonly string[] BindingPropertiesToCopy = {
                                                        "BindsDirectlyToSource", "Converter", "ConverterCulture",
                                                        "ConverterParameter", "ElementName", "FallbackValue",
                                                        "Mode", "NotifyOnValidationError", "Path", "RelativeSource",
                                                        "Source", "StringFormat", "TargetNullValue",
                                                        "ValidatesOnDataErrors",
                                                        "ValidatesOnExceptions", "ValidatesOnNotifyDataErrors"
                                                    };

        /// <summary>
        /// Copies a binding.
        /// </summary>
        /// <param name="bindingToCopy">The binding to copy.</param>
        public static Binding CopyBinding(this Binding bindingToCopy)
        {
            var copy = new Binding();

            foreach (var propertyName in BindingPropertiesToCopy)
            {
                var propertyValue = bindingToCopy.GetProperty<object>(propertyName);
                if (propertyValue != null)
                    copy.SetProperty(propertyName, propertyValue);
            }

            return copy;
        }
    }
}
