using System;
using System.Windows;

namespace FoundOps.Common.Silverlight.Tools
{
    public class AppendNumberToStringMarkupExtension : UpdatableMarkupExtension<string>
    {
        #region StringValue Dependency Property

        /// <summary>
        /// StringValue
        /// </summary>
        public string StringValue
        {
            get { return (string)GetValue(StringValueProperty); }
            set { SetValue(StringValueProperty, value); }
        }

        /// <summary>
        /// StringValue Dependency Property.
        /// </summary>
        public static readonly DependencyProperty StringValueProperty =
            DependencyProperty.Register(
                "StringValue",
                typeof(string),
                typeof(AppendNumberToStringMarkupExtension),
                new PropertyMetadata(new PropertyChangedCallback(StringValueChanged)));

        private static void StringValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AppendNumberToStringMarkupExtension c = d as AppendNumberToStringMarkupExtension;
            if (c != null)
            {
                c.UpdateValue(AppendNumberToStringHelper((string)e.NewValue, c.NumberValue));
            }
        }

        #endregion

        #region NumberValue Dependency Property

        /// <summary>
        /// NumberValue
        /// </summary>
        public int? NumberValue
        {
            get { return (int?)GetValue(NumberValueProperty); }
            set { SetValue(NumberValueProperty, value); }
        }

        /// <summary>
        /// NumberValue Dependency Property.
        /// </summary>
        public static readonly DependencyProperty NumberValueProperty =
            DependencyProperty.Register(
                "NumberValue",
                typeof(int?),
                typeof(AppendNumberToStringMarkupExtension),
                new PropertyMetadata(new PropertyChangedCallback(NumberValueChanged)));

        private static void NumberValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AppendNumberToStringMarkupExtension c = d as AppendNumberToStringMarkupExtension;
            if (c != null)
            {
                c.UpdateValue(AppendNumberToStringHelper(c.StringValue, (int?)e.NewValue));
            }
        }

        #endregion

        private static string AppendNumberToStringHelper(string stringValue, int? numberValue)
        {
            if (stringValue == null) return "";

            return numberValue.HasValue ? String.Format("{0}-{1}", stringValue, numberValue) : stringValue;
        }

        protected override object ProvideValueInternal(IServiceProvider serviceProvider)
        {
            return AppendNumberToStringHelper(StringValue, NumberValue);
        }
    }
}
