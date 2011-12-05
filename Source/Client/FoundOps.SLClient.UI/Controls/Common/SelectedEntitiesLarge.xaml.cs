using System.Windows;
using FoundOps.Common.Silverlight.UI.Selectors;

namespace FoundOps.SLClient.UI.Controls.Common
{
    /// <summary>
    /// A control to display and edit SelectedEntities. Related to CoreSelectedEntitiesCollectionVM.
    /// </summary>
    public partial class SelectedEntitiesLarge
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedEntitiesLarge"/> class.
        /// </summary>
        public SelectedEntitiesLarge()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets the new entity template.
        /// </summary>
        public DataTemplate NewEntityTemplate
        {
            get { return null; } //Required to prevent XAML compilation issue
            set { ((EntityTemplateSelector)this.Resources["EntityTemplateSelector"]).NewItemTemplate = value; }
        }
        /// <summary>
        /// Sets the existing entity template.
        /// </summary>
        public DataTemplate ExistingEntityTemplate
        {
            get { return null; } //Required to prevent XAML compilation issue
            set
            {
                ((EntityTemplateSelector)this.Resources["EntityTemplateSelector"]).ExistingItemTemplate = value;
                AddDelete.ItemTemplate = value;
            }
        }

        #region CoreSelectedEntitiesCollectionVM Dependency Property

        /// <summary>
        /// The CoreSelectedEntitiesCollectionVM. NOTE: This is an object, because SL XAML does not support generics :(
        /// </summary>
        public object CoreSelectedEntitiesCollectionVM
        {
            get { return (object)GetValue(CoreSelectedEntitiesCollectionVMProperty); }
            set { SetValue(CoreSelectedEntitiesCollectionVMProperty, value); }
        }

        /// <summary>
        /// CoreSelectedEntitiesCollectionVM Dependency Property.
        /// </summary>
        public static readonly DependencyProperty CoreSelectedEntitiesCollectionVMProperty =
            DependencyProperty.Register(
                "CoreSelectedEntitiesCollectionVM",
                typeof(object),
                typeof(SelectedEntitiesLarge),
                new PropertyMetadata(new PropertyChangedCallback(CoreSelectedEntitiesCollectionVMChanged)));

        private static void CoreSelectedEntitiesCollectionVMChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as SelectedEntitiesLarge;
            if (c != null)
            {
                //Set the DataContext to the ViewModel
                c.DataContext = e.NewValue;
            }
        }

        #endregion



        #region EntityText Dependency Property

        /// <summary>
        /// EntityText
        /// </summary>
        public string EntityText
        {
            get { return (string)GetValue(EntityTextProperty); }
            set { SetValue(EntityTextProperty, value); }
        }

        /// <summary>
        /// EntityText Dependency Property.
        /// </summary>
        public static readonly DependencyProperty EntityTextProperty =
            DependencyProperty.Register(
                "EntityText",
                typeof(string),
                typeof(SelectedEntitiesLarge),
                new PropertyMetadata(null));

        #endregion
    }
}
