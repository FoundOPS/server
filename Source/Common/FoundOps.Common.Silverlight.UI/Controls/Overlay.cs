using System.Windows;
using System.Windows.Controls;

namespace FoundOps.Common.Silverlight.UI.Controls
{
    public class Overlay : ContentControl
    {
        #region Message Dependency Property

        /// <summary>
        /// Message
        /// </summary>
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        /// <summary>
        /// Message Dependency Property.
        /// </summary>
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(
                "Message",
                typeof(string),
                typeof(Overlay), 
                null);

        #endregion

        public Overlay()
        {
            this.DefaultStyleKey = typeof(Overlay);
        }
    }
}
