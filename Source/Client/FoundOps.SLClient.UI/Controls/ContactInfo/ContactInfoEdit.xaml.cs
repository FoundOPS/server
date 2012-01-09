using System.Windows;
using FoundOps.SLClient.UI.ViewModels;

namespace FoundOps.SLClient.UI.Controls.ContactInfo
{
    /// <summary>
    /// A control to allow adding and editing contact information.
    /// </summary>
    public partial class ContactInfoEdit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContactInfoEdit"/> class.
        /// </summary>
        public ContactInfoEdit()
        {
            // Required to initialize variables
            InitializeComponent();
        }

        #region ContactInfoVM Dependency Property

        /// <summary>
        /// ContactInfoVM
        /// </summary>
        public ContactInfoVM ContactInfoVM
        {
            get { return (ContactInfoVM)GetValue(ContactInfoVMProperty); }
            set { SetValue(ContactInfoVMProperty, value); }
        }

        /// <summary>
        /// ContactInfoVM Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ContactInfoVMProperty =
            DependencyProperty.Register(
                "ContactInfoVM",
                typeof(ContactInfoVM),
                typeof(ContactInfoEdit),
                new PropertyMetadata(null));

        #endregion
    }
}