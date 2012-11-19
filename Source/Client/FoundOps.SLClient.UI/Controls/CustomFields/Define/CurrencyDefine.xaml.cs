using System.Windows;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.SLClient.UI.Controls.CustomFields.Define
{
    public partial class CurrencyDefine
    {
        public CurrencyDefine()
        {
            InitializeComponent();
        }

        #region NumericField Dependency Property

        /// <summary>
        /// NumericField
        /// </summary>
        public NumericField NumericField
        {
            get { return (NumericField)GetValue(NumericFieldProperty); }
            set { SetValue(NumericFieldProperty, value); }
        }

        /// <summary>
        /// NumericField Dependency Property.
        /// </summary>
        public static readonly DependencyProperty NumericFieldProperty =
            DependencyProperty.Register(
                "NumericField",
                typeof(NumericField),
                typeof(CurrencyDefine),
                new PropertyMetadata(null));

        #endregion
    }
}
