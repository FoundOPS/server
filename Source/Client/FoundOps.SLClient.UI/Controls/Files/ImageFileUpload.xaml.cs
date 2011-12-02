using System;
using System.Windows;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.SLClient.UI.Controls.Files
{
    /// <summary>
    /// A class to hold ImageUpload
    /// </summary>
    public partial class ImageFileUpload
    {
        #region File Dependency Property

        /// <summary>
        /// File
        /// </summary>
        public File File
        {
            get { return (File)GetValue(FileProperty); }
            set { SetValue(FileProperty, value); }
        }

        /// <summary>
        /// File Dependency Property.
        /// </summary>
        public static readonly DependencyProperty FileProperty =
            DependencyProperty.Register(
                "File",
                typeof(File),
                typeof(ImageFileUpload),
                new PropertyMetadata(null));

        #endregion

        #region OwnerParty Dependency Property

        /// <summary>
        /// The File's OwnerParty.
        /// NOTE: Required for creating PartyImages
        /// </summary>
        public Party OwnerParty
        {
            get { return (Party)GetValue(OwnerPartyProperty); }
            set { SetValue(OwnerPartyProperty, value); }
        }

        /// <summary>
        /// OwnerParty Dependency Property.
        /// </summary>
        public static readonly DependencyProperty OwnerPartyProperty =
            DependencyProperty.Register(
                "OwnerParty",
                typeof(Party),
                typeof(ImageFileUpload),
                new PropertyMetadata(null));

        #endregion

        /// <summary>
        /// Gets or sets the default file type to create.
        /// It will create an instance of the type when the image is set & File is null
        /// NOTE: Must be or inherit from File.
        /// </summary>
        public Type DefaultFileTypeToCreate { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageFileUpload"/> class.
        /// </summary>
        public ImageFileUpload()
        {
            InitializeComponent();

            DefaultFileTypeToCreate = typeof(File);
        }

        #region Logic

        //Whenever an Image is set, if there is not a file but there is a valid binding:
        //a) Create the proper file
        //b) Set the Name & ByteData
        private void ImageUpload_OnImageSet(string imageName, byte[] data)
        {
            if (File != null)
                return;

            //a) Create the proper file
            var newFile = (File)Activator.CreateInstance(DefaultFileTypeToCreate);
            if (OwnerParty != null)
                newFile.OwnerParty = OwnerParty;

            //Set the File to the newFile
            File = newFile;

            //b) Set the Name & ByteData
            newFile.Name = imageName;
            newFile.ByteData = data;
        }

        #endregion
    }
}
