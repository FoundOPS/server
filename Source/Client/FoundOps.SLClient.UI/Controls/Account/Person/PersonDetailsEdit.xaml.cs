using System.Windows;
using System.Windows.Controls;
using FoundOps.SLClient.UI.ViewModels;

namespace FoundOps.SLClient.UI.Controls.Account.Person
{
    public partial class PersonDetailsEdit : UserControl
    {
        public PersonDetailsEdit()
        {
            InitializeComponent();
        }

        #region PersonVM Dependency Property

        /// <summary>
        /// PersonVM
        /// </summary>
        public PersonVM PersonVM
        {
            get { return (PersonVM)GetValue(PersonVMProperty); }
            set { SetValue(PersonVMProperty, value); }
        }

        /// <summary>
        /// PersonVM Dependency Property.
        /// </summary>
        public static readonly DependencyProperty PersonVMProperty =
            DependencyProperty.Register(
                "PersonVM",
                typeof(PersonVM),
                typeof(PersonDetailsEdit),
                new PropertyMetadata(null));

        #endregion
    }
}
