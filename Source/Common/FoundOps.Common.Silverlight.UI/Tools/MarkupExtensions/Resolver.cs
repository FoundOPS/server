using System;
using System.Windows;
using FoundOps.Common.Composite.Tools;

namespace FoundOps.Common.Silverlight.UI.Tools.MarkupExtensions
{
    /// <summary>
    /// Resolves a property on a source
    /// TODO: Change PropertyName->Path. Add property change tracking on PropertyName
    /// </summary>
    public class Resolver : UpdatableMarkupExtension<object>
    {
        #region PropertyName Dependency Property

        /// <summary>
        /// PropertyName
        /// </summary>
        public string PropertyName
        {
            get { return (string)GetValue(PropertyNameProperty); }
            set { SetValue(PropertyNameProperty, value); }
        }

        /// <summary>
        /// PropertyName Dependency Property.
        /// </summary>
        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.Register(
                "PropertyName",
                typeof(string),
                typeof(Resolver),
                new PropertyMetadata(PropertyNameChanged));

        private static void PropertyNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as Resolver;
            if (c == null) return;

            var value = c.ResolverHelper(c.Source, e.NewValue as string);
            if (value != null)
                c.UpdateValue(value);
        }

        #endregion

        #region Source Dependency Property

        /// <summary>
        /// Source
        /// </summary>
        public object Source
        {
            get { return GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        /// <summary>
        /// Source Dependency Property.
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(
                "Source",
                typeof(object),
                typeof(Resolver),
                new PropertyMetadata(SourceChanged));

        private static void SourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as Resolver;
            if (c == null) return;

            var value = c.ResolverHelper(e.NewValue, c.PropertyName);
            if(value!=null)
                c.UpdateValue(value);
        }

        #endregion

        protected override object ProvideValueInternal(IServiceProvider serviceProvider)
        {
            return ResolverHelper(Source, PropertyName);
        }

        private object ResolverHelper(object source, string propertyName)
        {
            if (source == null) return null;

            var value = source;
            var properties = propertyName.Split('.');

            foreach (var propName in properties)
            {
                value = value.GetProperty<object>(propName);
            }

            return value;
        }
    }
}