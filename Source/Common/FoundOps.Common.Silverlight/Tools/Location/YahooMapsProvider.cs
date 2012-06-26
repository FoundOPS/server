using Telerik.Windows.Controls.Map;
using FoundOps.Common.Silverlight.Extensions.Telerik;

namespace FoundOps.Common.Silverlight.Tools.Location
{
    public class YahooMapsProvider : MapProviderBase
    {
        public YahooMapsProvider(MapMode mode)
        {
            var source = new YahooMapsSource(mode);
            this.MapSources.Add(source.UniqueId, source);
        }

        public YahooMapsProvider()
        {
            var source = new YahooMapsSource(MapMode.Aerial);
            this.MapSources.Add(source.UniqueId, source);
        }

        public override ISpatialReference SpatialReference
        {
            get
            {
                return new MercatorProjection();
            }
        }
    }

}
