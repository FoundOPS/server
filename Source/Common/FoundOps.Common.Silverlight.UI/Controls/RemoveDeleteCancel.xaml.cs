using System.Windows;
using System.Windows.Controls;

namespace FoundOps.Common.Silverlight.UI.Controls
{
    public partial class RemoveDeleteCancel
    {
        #region Properties and Variables

        #region ItemToRemoveString Dependency Property

        /// <summary>
        /// EntityToRemoveString
        /// </summary>
        public string ItemToRemoveString
        {
            get { return (string)GetValue(ItemToRemoveStringProperty); }
            set { SetValue(ItemToRemoveStringProperty, value); }
        }

        /// <summary>
        /// AddIsEnabled Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ItemToRemoveStringProperty =
            DependencyProperty.Register(
                "ItemToRemoveString",
                typeof(string),
                typeof(RemoveDeleteCancel),
                new PropertyMetadata(null));

        #endregion
        #region ItemToRemoveFromString Dependency Property

        /// <summary>
        /// EntityToRemoveFromString
        /// </summary>
        public string ItemToRemoveFromString
        {
            get { return (string)GetValue(ItemToRemoveFromStringProperty); }
            set { SetValue(ItemToRemoveFromStringProperty, value); }
        }

        /// <summary>
        /// AddIsEnabled Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ItemToRemoveFromStringProperty =
            DependencyProperty.Register(
                "ItemToRemoveFromString",
                typeof(string),
                typeof(RemoveDeleteCancel),
                new PropertyMetadata(null));

        #endregion

        #endregion

        public RemoveDeleteCancel()
        {
            InitializeComponent();
        }

        public Button RemoveButton
        {
            get { return Remove; }
        }
        public Button DeleteButton
        {
            get { return Delete; }
        }
        public Button CancelButton
        {
            get { return Cancel; }
        }
    }
}

