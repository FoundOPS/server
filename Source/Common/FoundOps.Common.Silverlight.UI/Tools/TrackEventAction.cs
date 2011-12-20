using System.Windows;
using System.Windows.Browser;
using System.Windows.Interactivity;

namespace FoundOps.Common.Silverlight.UI.Tools
{
    /// <summary>
    /// A class to set up event tracking for analytics
    /// </summary>
    public class TrackEventAction : TriggerAction<UIElement>
    {
        /// <summary>
        /// The analytics category.
        /// </summary>
        public static readonly DependencyProperty CategoryProperty =
            DependencyProperty.Register("Category", typeof(string),
                                        typeof(TrackEventAction),
                                        new PropertyMetadata("Silverlight.Event"));

        /// <summary>
        /// The analytics action.
        /// </summary>
        public static readonly DependencyProperty ActionProperty =
            DependencyProperty.Register("Action", typeof(string),
                                        typeof(TrackEventAction),
                                        new PropertyMetadata("Unknown Action"));

        /// <summary>
        /// The analytics label.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string),
                                        typeof(TrackEventAction),
                                        new PropertyMetadata("Unknown Action fired"));

        /// <summary>
        /// The analytics value.
        /// </summary>
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string),
                                        typeof(TrackEventAction),
                                        new PropertyMetadata("Unknown Action fired"));

        /// <summary>
        /// Gets or sets the analytics category.
        /// </summary>
        /// <value>
        /// The category.
        /// </value>
        public string Category
        {
            get { return (string)GetValue(CategoryProperty); }
            set { SetValue(CategoryProperty, value); }
        }

        /// <summary>
        /// Gets or sets the analytics action.
        /// </summary>
        /// <value>
        /// The action.
        /// </value>
        public string Action
        {
            get { return (string)GetValue(ActionProperty); }
            set { SetValue(ActionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the analytics label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        /// <summary>
        /// Gets or sets the analytics value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        protected override void Invoke(object parameter)
        {
            try
            {
                HtmlPage.Window.Invoke("trackEvent", new object[] { Category, Action, Label, Value });
            }
            catch
            {
            }
        }

        /// <summary>
        /// Tracks the specified event. Used for events called from code behind.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="action">The action.</param>
        /// <param name="label">The label.</param>
        /// /// <param name="value">The value.</param>
        public static void Track(string category, string action, string label, int value)
        {
            try
            {
                HtmlPage.Window.Invoke("trackEvent", new object[] { category, action, label, value });
            }
            catch
            {
            }
        }

        /// <summary>
        /// Tracks the specified category.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="action">The action.</param>
        /// <param name="value">The value.</param>
        public static void Track(string category, string action, int value)
        {
            try
            {
                HtmlPage.Window.Invoke("trackEvent", new object[] { category, action, value });
            }
            catch
            {
            }
        }
    }

    public class Totango
    {
        //SP-1268-01 for prodcution use and SP-12680-01 for test use
#if DEBUG
        private const string ServiceId = "SP-12680-01";
#else
        private const string ServiceId = "SP-1268-01";
#endif

        public static void Track(string org, string user, string activity, string module)
        {
            try
            {
                // create SDR string
                var url = string.Format("http://sdr.totango.com/pixel.gif/?sdr_s={0}&sdr_o={1}&sdr_u={2}&sdr_a={3}&sdr_m={4}", ServiceId,
                        HttpUtility.UrlEncode(org), HttpUtility.UrlEncode(user), HttpUtility.UrlEncode(activity), HttpUtility.UrlEncode(module));

                // API get call
                HtmlPage.Window.Invoke("httpGet", new object[] { url });
            }
            catch
            {
            }
        }
    }
}
