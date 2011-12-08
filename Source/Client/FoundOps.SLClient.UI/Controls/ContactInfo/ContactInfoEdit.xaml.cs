using System;
using System.Windows;
using System.Windows.Input;
using FoundOps.SLClient.UI.ViewModels;
using Telerik.Windows.Controls;

namespace FoundOps.SLClient.UI.Controls.ContactInfo
{
    public partial class ContactInfoEdit
    {
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

        private void LabelRadComboBox_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count <= 0) return;

            var contactInfo = (Core.Models.CoreEntities.ContactInfo)((FrameworkElement)sender).DataContext;
            if (contactInfo != null)
                contactInfo.Label = (string)e.AddedItems[0];
        }

        //Manually update label
        private void LabelRadComboBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var contactInfo = (Core.Models.CoreEntities.ContactInfo)((FrameworkElement)sender).DataContext;
            var comboBoxText = ((RadComboBox)sender).Text;
            if (contactInfo != null && !String.IsNullOrEmpty(comboBoxText))
            {
                if (contactInfo.Label != comboBoxText) //To fix CancelChanges
                    contactInfo.Label = comboBoxText;
            }
        }

        private void RadComboBox_KeyDown(object sender, KeyEventArgs e) //To continually trigger update
        {
            LabelRadComboBox_LostFocus(sender, null);
        }
    }
}