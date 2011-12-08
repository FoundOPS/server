using System.Windows;
using FoundOps.SLClient.UI.ViewModels;

namespace FoundOps.SLClient.UI.Controls.ContactInfo
{
    public partial class ContactInfoView
    {
        public ContactInfoView()
        {
            InitializeComponent();
        }

        #region ContactInfoVM Dependency Property

        /// <summary>
        /// ContactInfoVM
        /// </summary>
        public ContactInfoVM ContactInfoVM
        {
            get { return (ContactInfoVM) GetValue(ContactInfoVMProperty); }
            set { SetValue(ContactInfoVMProperty, value); }
        }

        /// <summary>
        /// ContactInfoVM Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ContactInfoVMProperty =
            DependencyProperty.Register(
                "ContactInfoVM",
                typeof (ContactInfoVM),
                typeof (ContactInfoView),
                new PropertyMetadata(null));

        #endregion
    }
}
