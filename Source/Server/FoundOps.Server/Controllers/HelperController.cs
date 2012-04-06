using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web.Mvc;
using com.google.zxing;
using FoundOps.Core.Server.Tools;

namespace FoundOps.Server.Controllers
{
    [Authorize]
    public class HelperController : Controller
    {
        //Look at http://code.google.com/p/zxing/wiki/BarcodeContents for formatting

        /// <summary>
        /// Returns a QRCode with the data embedded
        /// </summary>
        /// <param name="data">The data to embed in a QRCode.</param>
        /// <returns></returns>
        public ImageResult QRCode(string data)
        {
            var qrCode = new MultiFormatWriter();
            var byteIMG = qrCode.encode(data, BarcodeFormat.QR_CODE, 200, 200);
            sbyte[][] img = byteIMG.Array;
            var bmp = new Bitmap(200, 200);
            var g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            for (var y = 0; y <= img.Length - 1; y++)
            {
                for (var x = 0; x <= img[y].Length - 1; x++)
                {
                    g.FillRectangle(img[y][x] == 0 ? Brushes.Black : Brushes.White, x, y, 1, 1);
                }
            }

            var stream = new System.IO.MemoryStream();
            bmp.Save(stream, ImageFormat.Jpeg);
            var imageBytes = stream.ToArray();
            stream.Close();

            return this.Image(imageBytes, "image/jpeg");
        }

        /// <summary>
        /// Returns a QR code for the location.
        /// </summary>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        /// <returns></returns>
        public ImageResult LocationQRCode(double latitude, double longitude)
        {
            return QRCode(String.Format("geo:{0},{1},100", latitude, longitude));
        }
    }
}
