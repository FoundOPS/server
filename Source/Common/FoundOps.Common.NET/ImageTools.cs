using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
namespace FoundOps.Common.NET
{
    public static class ImageTools
    {
        /// <summary>
        /// Find the right codec
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static ImageCodecInfo GetImageCodec(string extension)
        {
            extension = extension.ToUpperInvariant();
            var codecs = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FilenameExtension.Contains(extension))
                {
                    return codec;
                }
            }
            return codecs[1];
        }

        /// <summary>
        /// Converts an Image to a byte array.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="extension">The extension.</param>
        /// <param name="encoderParameters">The encoder parameters.</param>
        public static byte[] ImageToByteArray(System.Drawing.Image image, string extension, EncoderParameters encoderParameters)
        {
            var ms = new MemoryStream();
            if (!string.IsNullOrEmpty(extension) && encoderParameters != null)
                image.Save(ms, GetImageCodec(extension), encoderParameters);
            else
                image.Save(ms, image.RawFormat);
            return ms.ToArray();
        }

        /// <summary>
        /// Converts a byte array to an image.
        /// </summary>
        /// <param name="byteArrayIn">The byte array in.</param>
        /// <returns></returns>
        public static Image ByteArrayToImage(byte[] byteArrayIn)
        {
            var ms = new MemoryStream(byteArrayIn);
            var returnImage = Image.FromStream(ms);
            return returnImage;
        }

        /// <summary>
        /// Modifies an image image.
        /// </summary>
        /// <param name="data">The image data</param>
        /// <param name="extension">The image extension</param>
        /// <param name="x">The x</param>
        /// <param name="y">The y</param>
        /// <param name="w">The w</param>
        /// <param name="h">The h</param>
        public static byte[] CropImage(byte[] data, string extension, int x, int y, int w, int h)
        {
            var img = ByteArrayToImage(data);

            using (var bitmap = new Bitmap(w, h))
            {
                bitmap.SetResolution(img.HorizontalResolution, img.VerticalResolution);
                using (var graphic = Graphics.FromImage(bitmap))
                {
                    graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    graphic.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    graphic.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                    //Crop the image
                    graphic.DrawImage(img, 0, 0, w, h);
                    graphic.DrawImage(img, new Rectangle(0, 0, w, h), x, y, w, h, GraphicsUnit.Pixel);

                    //If the image is a gif file, change it into png
                    if (extension.EndsWith("gif", StringComparison.OrdinalIgnoreCase))
                        extension = ".png";

                    using (var encoderParameters = new EncoderParameters(1))
                    {
                        encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
                        return ImageToByteArray(bitmap, extension, encoderParameters);
                    }
                }
            }
        }
    }
}
