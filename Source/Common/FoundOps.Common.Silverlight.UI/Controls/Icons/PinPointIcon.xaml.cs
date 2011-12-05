using System.Windows;
using System.Windows.Media;

namespace FoundOps.Common.Silverlight.Controls.Icons
{
    public partial class PinPointIcon
    {
        public PinPointIcon()
        {
            InitializeComponent();
        }

        #region Fill Dependency Property

        /// <summary>
        /// Fill
        /// </summary>
        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        /// <summary>
        /// Fill Dependency Property.
        /// </summary>
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register(
                "Fill",
                typeof(Brush),
                typeof(PinPointIcon),
                new PropertyMetadata(new SolidColorBrush(Colors.Red)));

        #endregion

        #region Text Dependency Property

        /// <summary>
        /// Text
        /// </summary>
        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Text Dependency Property.
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",
                typeof (string),
                typeof (PinPointIcon),
                new PropertyMetadata(null));

        #endregion
    }
}
