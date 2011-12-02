using System.Windows;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.Framework.Views.Controls.CustomFields
{
    public partial class CheckBoxDefine
    {
        public CheckBoxDefine()
        {
            InitializeComponent();
        }

        #region OptionsField Dependency Property

        /// <summary>
        /// OptionsField
        /// </summary>
        public OptionsField OptionsField
        {
            get { return (OptionsField) GetValue(OptionsFieldProperty); }
            set { SetValue(OptionsFieldProperty, value); }
        }

        /// <summary>
        /// OptionsField Dependency Property.
        /// </summary>
        public static readonly DependencyProperty OptionsFieldProperty =
            DependencyProperty.Register(
                "OptionsField",
                typeof (OptionsField),
                typeof (CheckBoxDefine),
                new PropertyMetadata(null));

        #endregion
    }
}
