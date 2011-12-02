using System;
using System.Reflection;
using System.Windows.Markup;

namespace FoundOps.Common.Silverlight.Tools
{
    /// <summary>
    /// For calling Static Methods in XAML. 
    /// Ex. {Tools:StaticExtension TypeName=System:DateTime, PropertyName=Now}
    /// </summary>
    public class StaticExtension : MarkupExtension
    {
        public string TypeName { get; set; }
        public string PropertyName { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var t = Type.GetType(TypeName.Replace(':', '.'));

            var p = t.GetProperty(PropertyName, BindingFlags.Static | BindingFlags.Public);

            var v = p.GetValue(null, null);

            return v;
        }
    }

}
