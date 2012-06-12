using System.Windows;

namespace FoundOps.SLClient.UI.Controls.Parties.Person
{
    /// <summary>
    /// The UI for viewing and editing a Person
    /// </summary>
    public partial class PersonDetailsEdit
    {
        #region Person Dependency Property

        /// <summary>
        /// Person
        /// </summary>
        public FoundOps.Core.Models.CoreEntities.Person Person
        {
            get { return (FoundOps.Core.Models.CoreEntities.Person)GetValue(PersonProperty); }
            set { SetValue(PersonProperty, value); }
        }

        /// <summary>
        /// SelectedClient Dependency Property.
        /// </summary>
        public static readonly DependencyProperty PersonProperty =
            DependencyProperty.Register(
                "Person",
                typeof(FoundOps.Core.Models.CoreEntities.Person),
                typeof(PersonDetailsEdit),
                new PropertyMetadata(null));

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="PersonDetailsEdit"/> class.
        /// </summary>
        public PersonDetailsEdit()
        {
            InitializeComponent();
        }
    }
}
