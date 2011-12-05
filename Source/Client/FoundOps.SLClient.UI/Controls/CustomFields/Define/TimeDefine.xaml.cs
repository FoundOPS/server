using System.Windows;
using FoundOps.Core.Models.CoreEntities;
namespace FoundOps.Framework.Views.Controls.CustomFields
{
    public partial class TimeDefine
    {
        public TimeDefine()
        {
            InitializeComponent();
        }

        #region DateTimeField Dependency Property

        /// <summary>
        /// DateTimeField
        /// </summary>
        public DateTimeField DateTimeField
        {
            get { return (DateTimeField) GetValue(DateTimeFieldProperty); }
            set { SetValue(DateTimeFieldProperty, value); }
        }

        /// <summary>
        /// DateTimeField Dependency Property.
        /// </summary>
        public static readonly DependencyProperty DateTimeFieldProperty =
            DependencyProperty.Register(
                "DateTimeField",
                typeof (DateTimeField),
                typeof (TimeDefine),
                new PropertyMetadata(null));

        #endregion
    }
}
