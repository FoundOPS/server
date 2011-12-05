using System.Windows;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.Framework.Views.Controls.CustomFields
{
    public partial class NumberDefine
    {
        public NumberDefine()
        {
            InitializeComponent();
        }

        #region NumericField Dependency Property

        /// <summary>
        /// NumericField
        /// </summary>
        public NumericField NumericField
        {
            get { return (NumericField) GetValue(NumericFieldProperty); }
            set { SetValue(NumericFieldProperty, value); }
        }

        /// <summary>
        /// NumericField Dependency Property.
        /// </summary>
        public static readonly DependencyProperty NumericFieldProperty =
            DependencyProperty.Register(
                "NumericField",
                typeof (NumericField),
                typeof (NumberDefine),
                new PropertyMetadata(null));

        #endregion
    }
}
