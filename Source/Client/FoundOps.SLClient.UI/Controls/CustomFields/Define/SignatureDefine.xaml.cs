using System.Windows;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.SLClient.UI.Controls.CustomFields.Define
{
    public partial class SignatureDefine
    {
        public SignatureDefine()
        {
            InitializeComponent();
        }

        #region SignatureField Dependency Property

        /// <summary>
        /// SignatureField
        /// </summary>
        public SignatureField SignatureField
        {
            get { return (SignatureField)GetValue(SignatureFieldProperty); }
            set { SetValue(SignatureFieldProperty, value); }
        }

        /// <summary>
        /// TextBoxField Dependency Property.
        /// </summary>
        public static readonly DependencyProperty SignatureFieldProperty =
            DependencyProperty.Register(
                "SignatureField",
                typeof(SignatureField),
                typeof(SignatureDefine),
                new PropertyMetadata(null));

        #endregion
    }
}
