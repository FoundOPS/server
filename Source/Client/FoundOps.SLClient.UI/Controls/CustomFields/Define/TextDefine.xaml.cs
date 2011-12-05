using System.Windows;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.Framework.Views.Controls.CustomFields
{
    public partial class TextDefine
    {
        public TextDefine()
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
                typeof (TextDefine),
                new PropertyMetadata(null));

        #endregion
    }
}
