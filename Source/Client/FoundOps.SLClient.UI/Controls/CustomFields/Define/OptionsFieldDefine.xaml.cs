using System.Linq;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using FoundOps.SLClient.UI.ViewModels;
using FoundOps.Core.Models.CoreEntities;
using SortDescriptor = Telerik.Windows.Data.SortDescriptor;

namespace FoundOps.SLClient.UI.Controls.CustomFields.Define
{
    /// <summary>
    /// The UI for defining an OptionsField.
    /// </summary>
    public partial class OptionsFieldDefine
    {
        private readonly OptionsFieldVM _optionsFieldVM = new OptionsFieldVM();
        public OptionsFieldVM OptionsFieldVM { get { return _optionsFieldVM; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsFieldDefine"/> class.
        /// </summary>
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

            foreach (Option item in currentOption.Parent.Options.Where(item => item != currentOption))
            {
                item.IsChecked = false;
            }
        }
    }
}
