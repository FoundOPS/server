using System.Windows;
using System.Windows.Controls;

namespace FoundOps.SLClient.UI.Controls.Account.Business
{
    public partial class BusinessDetailsView : UserControl
    {
        public BusinessDetailsView()
        {
            InitializeComponent();
        }

        #region SelectedBusiness Dependency Property

        /// <summary>
        /// SelectedBusiness
        /// </summary>
        public FoundOps.Core.Models.CoreEntities.Business SelectedBusiness
        {
            get { return (FoundOps.Core.Models.CoreEntities.Business) GetValue(SelectedBusinessProperty); }
            set { SetValue(SelectedBusinessProperty, value); }
        }

        /// <summary>
        /// SelectedBusiness Dependency Property.
        /// </summary>
        public static readonly DependencyProperty SelectedBusinessProperty =
            DependencyProperty.Register(
                "SelectedBusiness",
                typeof (FoundOps.Core.Models.CoreEntities.Business),
                typeof (BusinessDetailsView),
                new PropertyMetadata(null));

        #endregion
    }
}
