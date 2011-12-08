using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using FoundOps.Common.Composite;

namespace FoundOps.Common.Silverlight.UI.Controls
{
    public partial class ImageUpload
    {
        public delegate void ImageSetHandler(string imageName, byte[] data);

        /// <summary>
        /// Occurs when the [image is set].
        /// </summary>
        public event ImageSetHandler ImageSet;

        public ImageUpload()
        {
            // Required to initialize variables
            InitializeComponent();
        }

        #region ImageData Dependency Property

        /// <summary>
        /// ImageData
        /// </summary>
        public byte[] ImageData
        {
            get { return (byte[])GetValue(ImageDataProperty); }
            set { SetValue(ImageDataProperty, value); }
        }

        /// <summary>
        /// ImageData Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ImageDataProperty =
            DependencyProperty.Register(
                "ImageData",
                typeof(byte[]),
                typeof(ImageUpload),
                new PropertyMetadata(FileTools.ReadBytesFromResource(@"/FoundOps.Common.Silverlight;component/Resources/noimageavailable.png")));

        #endregion

        #region ImageName Dependency Property

        /// <summary>
        /// ImageName
        /// </summary>
        public string ImageName
        {
            get { return (string)GetValue(ImageNameProperty); }
            set { SetValue(ImageNameProperty, value); }
        }

        /// <summary>
        /// ImageName Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ImageNameProperty =
            DependencyProperty.Register(
                "ImageName",
                typeof(string),
                typeof(ImageUpload),
                new PropertyMetadata(""));

        #endregion

        #region Image Operations
        private static bool IsSupportedImageFile(string extension)
        {
            switch (extension.Trim().ToLower())
            {
                case ".jpg":
                case ".png":
                    return true;
                default: break;
            }
            return false;
        }

        private void UploadImageButtonClick(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Multiselect = false, Filter = "Image Files (*.jpg, *.png )|*.jpg;*.png;" };

            var retval = dlg.ShowDialog();
            if (retval == null || retval != true) return;

            ImageName = dlg.File.Name;
            SetPicture(dlg.File.Name, dlg.File.OpenRead());
        }

        private void ImageDrop(object sender, DragEventArgs e) //Does not work for some reason
        {
            var droppedFiles = e.Data.GetData(DataFormats.FileDrop) as FileInfo[];

            foreach (var droppedFile in droppedFiles.Where(droppedFile => IsSupportedImageFile(droppedFile.Extension)))
                SetPicture(droppedFile.Name, droppedFile.OpenRead());
        }

        private void SetPicture(string fileName, Stream photoFileStream)
        {
            var photoBytes = new byte[photoFileStream.Length];
            photoFileStream.Read(photoBytes, 0, (int)photoFileStream.Length);
            ImageData = photoBytes;

            //Call the image set event. Pass it the fileName and photoBytes directly (not through a DependencyProperty) so there is no lag
            if (ImageSet != null)
                ImageSet(fileName, photoBytes);
        }

        #endregion
    }
}