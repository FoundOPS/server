using System.Windows;
using System.ComponentModel;

namespace FoundOps.Common.Silverlight.UI.Controls.InfiniteAccordion
{
    /// <summary>
    /// A control which displays a certain ObjectType and has a ContextProvider.
    /// The ContextProvider is the DataContext of the Display's FrameworkElement.
    /// </summary>
    public partial class ObjectTypeDisplay : IObjectTypeDisplay, INotifyPropertyChanged
    {
        /// <value>
        /// The related context provider.
        /// </value>
        public IProvideContext ContextProvider { get; set; }

        /// <value>
        /// The object type to display.
        /// </value>
        public string ObjectTypeToDisplay { get; set; }

        public ObjectTypeDisplay()
        {
            InitializeComponent();
        }

        private FrameworkElement _display;
        /// <summary>
        /// Gets or sets the Display FrameworkElement. The ContextProvider is the DataContext of this FrameworkElement
        /// </summary>
        public FrameworkElement Display
        {
            get { return _display; }
            set
            {
                _display = value;
                this.RaisePropertyChanged("Display");
                this.ContextProvider = value.DataContext as IProvideContext;
            }
        }

        #region Implementation of INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
