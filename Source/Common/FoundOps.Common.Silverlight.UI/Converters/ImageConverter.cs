using System;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace FoundOps.Common.Silverlight.Converters
{
    public class ImageConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            byte[] data = value as byte[];

            BitmapImage image;

            if (data == null)
            {
                image = new BitmapImage(new Uri("/FoundOps.Common.Silverlight;component/Resources/noimageavailable.png", UriKind.RelativeOrAbsolute));
            }
            else
            {
                image = new BitmapImage();
                try
                {
                    using (var stream = new MemoryStream(data))
                        image.SetSource(stream);
                }
                catch
                {
                    //Set the image to the default image
                    image = new BitmapImage(new Uri("/FoundOps.Common.Silverlight;component/Resources/noimageavailable.png", UriKind.RelativeOrAbsolute));
                }
            }
            return image;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
