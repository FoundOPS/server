using System.Windows;
using FoundOps.SLClient.UI.ViewModels;

namespace FoundOps.SLClient.UI.Controls.Account.Person
{
    /// <summary>
    /// The UI for viewing and editing a Person
    /// </summary>
    public partial class PersonDetailsEdit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PersonDetailsEdit"/> class.
        /// </summary>
        public PersonDetailsEdit()
        {
            InitializeComponent();
        }

        #region PersonVM Dependency Property

        /// <summary>
        /// PersonVM
        /// </summary>
        public PartyVM PersonVM
        {
            get { return (PartyVM)GetValue(PersonVMProperty); }
            set { SetValue(PersonVMProperty, value); }
        }

        /// <summary>
        /// PersonVM Dependency Property.
        /// </summary>
        public static readonly DependencyProperty PersonVMProperty =
            DependencyProperty.Register(
                "PersonVM",
                typeof(PartyVM),
                typeof(PersonDetailsEdit),
                new PropertyMetadata(null));

        #endregion
    }
}
