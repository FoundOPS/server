using System.Windows;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.SLClient.UI.Controls.CustomFields.Define
{
    public partial class TextLargeDefine
    {
        public TextLargeDefine()
        {
            InitializeComponent();
        }

        #region TextBoxField Dependency Property

        /// <summary>
        /// TextBoxField
        /// </summary>
        public TextBoxField TextBoxField
        {
            get { return (TextBoxField) GetValue(TextBoxFieldProperty); }
            set { SetValue(TextBoxFieldProperty, value); }
        }

        /// <summary>
        /// TextBoxField Dependency Property.
        /// </summary>
        public static readonly DependencyProperty TextBoxFieldProperty =
            DependencyProperty.Register(
                "TextBoxField",
                typeof (TextBoxField),
                typeof(TextLargeDefine),
                new PropertyMetadata(null));

        #endregion
    }
}
