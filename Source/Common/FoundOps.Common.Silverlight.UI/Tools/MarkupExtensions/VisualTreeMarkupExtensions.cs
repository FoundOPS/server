using System;
using FoundOps.Common.Silverlight.Tools;

namespace FoundOps.Common.Silverlight.UI.Tools.MarkupExtensions
{
    /// <summary>
    /// Returns the root element wherever the markup extension is called.
    /// </summary>
    public class RootElement : UpdatableMarkupExtension<object>
    {
        protected override object ProvideValueInternal(IServiceProvider serviceProvider)
        {
            var provider = serviceProvider as System.Xaml.IRootObjectProvider;

            //This will only work if the provider != null
            if (provider == null) return null;

            return provider.RootObject;
        }
    }
}