using System;
using System.Globalization;
using Telerik.Windows.Controls.Map;

namespace FoundOps.Common.Silverlight.Extensions.Telerik
{
    public class YahooMapsSource : TiledMapSource
    {
        private const string YahooMapsAerialUrlFormat = @"http://us.maps3.yimg.com/aerial.maps.yimg.com/tile?v=1.7&t=a&x={x}&y={y}&z={zoom}";
        private const string YahooMapsRoadUrlFormat = @"http://us.maps2.yimg.com/us.png.maps.yimg.com/png?v=3.52&t=m&x={x}&y={y}&z={zoom}";

        private readonly MapMode _mapMode;

        public YahooMapsSource(MapMode mode)
            : base(1, 20, 256, 256)
        {
            _mapMode = mode;
        }

        public override void Initialize()
        {
            // Raise provider initialized event.
            this.RaiseIntializeCompleted();
        }

        /// <summary>
        /// Gets the image URI.
        /// </summary>
        /// <param name="tileLevel">Tile level.</param>
        /// <param name="tilePositionX">Tile X.</param>
        /// <param name="tilePositionY">Tile Y.</param>
        /// <returns>URI of image.</returns>
        protected override Uri GetTile(int tileLevel, int tilePositionX, int tilePositionY)
        {
            int zoom = this.ConvertTileToZoomLevel(tileLevel);

            int zoomLevel = 18 - zoom;
            double num4 = Math.Pow(2.0, zoom) / 2.0;
            double y;
            if (tilePositionY < num4)
            {
                y = (num4 - tilePositionY) - 1.0;
            }
            else
            {
                y = ((tilePositionY + 1) - num4) * -1.0;
            }

            string url = string.Empty;

            switch (_mapMode)
            {
                case MapMode.Road:
                    url = YahooMapsRoadUrlFormat;
                    break;

                case MapMode.Aerial:
                    url = YahooMapsAerialUrlFormat;
                    break;
            }

            url = ProtocolHelper.SetScheme(url);
            url = url.Replace("{zoom}", zoomLevel.ToString(CultureInfo.InvariantCulture));
            url = url.Replace("{x}", tilePositionX.ToString(CultureInfo.InvariantCulture));
            url = url.Replace("{y}", y.ToString(CultureInfo.InvariantCulture));

            return new Uri(url);
        }
    }
}
