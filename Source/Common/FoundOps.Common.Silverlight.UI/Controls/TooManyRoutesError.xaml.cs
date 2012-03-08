using System.Windows;
using System.Windows.Controls;

namespace FoundOps.Common.Silverlight.UI.Controls
{
    public partial class TooManyRoutesError
    {
        #region MaxNumberOfRoutes
        /// <summary>
        /// MaxNumberofRoutes
        /// </summary>
        public int MaxNumberOfRoutes
        {
            get { return (int)GetValue(MaxNumberOfRoutesProperty); }
            set { SetValue(MaxNumberOfRoutesProperty, value); }
        }

        /// <summary>
        /// MaxNumberofRoutes Dependency Property.
        /// </summary>
        public static readonly DependencyProperty MaxNumberOfRoutesProperty =
            DependencyProperty.Register(
                "MaxNumberOfRoutes",
                typeof(int),
                typeof(TooManyRoutesError),
                new PropertyMetadata(null));
        #endregion

        public TooManyRoutesError()
        {
            InitializeComponent();
            this.Title = "Oops!";
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

