using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using FoundOps.SLClient.UI.ViewModels;
using FoundOps.Core.Models.CoreEntities;
using Telerik.Windows.Data;

namespace FoundOps.SLClient.UI.Controls.CustomFields.Define
{
    public partial class OptionsFieldDefine
    {
        private readonly OptionsFieldVM _optionsFieldVM = new OptionsFieldVM();
        public OptionsFieldVM OptionsFieldVM { get { return _optionsFieldVM; } }

        public OptionsFieldDefine()
        {
            InitializeComponent();
            OptionsFieldRadGridView.SortDescriptors.Add(new SortDescriptor { Member = "Index", SortDirection = ListSortDirection.Ascending });
        }

        #region OptionsField Dependency Property

        /// <summary>
        /// OptionsField
        /// </summary>
        public OptionsField OptionsField
        {
            get { return (OptionsField)GetValue(OptionsFieldProperty); }
            set { SetValue(OptionsFieldProperty, value); }
        }

        /// <summary>
        /// OptionsField Dependency Property.
        /// </summary>
        public static readonly DependencyProperty OptionsFieldProperty =
            DependencyProperty.Register(
                "OptionsField",
                typeof(OptionsField),
                typeof(OptionsFieldDefine),
                new PropertyMetadata(new PropertyChangedCallback(OptionsFieldChanged)));

        private static void OptionsFieldChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OptionsFieldDefine c = d as OptionsFieldDefine;
            if (c != null)
            {
                c.OptionsFieldVM.OptionsField = e.NewValue as OptionsField;
            }
        }

        #endregion

        private void OptionCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;

            var currentOption = checkBox.DataContext as Option;
            if (currentOption == null)
                return;

            foreach (Option item in currentOption.OptionsField.Options.Where(item => item != currentOption))
            {
                item.IsChecked = false;
            }
        }
    }
}
