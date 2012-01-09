using System.Windows;
using System.Windows.Controls;

namespace FoundOps.Common.Silverlight.UI.Controls
{
    public partial class RemoveDeleteCancel
    {
        #region Properties and Variables

        #region EntityToRemoveString Dependency Property

        /// <summary>
        /// EntityToRemoveString
        /// </summary>
        public string EntityToRemoveString
        {
            get { return (string)GetValue(EntityToRemoveStringProperty); }
            set { SetValue(EntityToRemoveStringProperty, value); }
        }

        /// <summary>
        /// AddIsEnabled Dependency Property.
        /// </summary>
        public static readonly DependencyProperty EntityToRemoveStringProperty =
            DependencyProperty.Register(
                "EntityToRemoveString",
                typeof(string),
                typeof(RemoveDeleteCancel),
                new PropertyMetadata(null));

        #endregion
        #region EntityToRemoveFromString Dependency Property

        /// <summary>
        /// EntityToRemoveFromString
        /// </summary>
        public string EntityToRemoveFromString
        {
            get { return (string)GetValue(EntityToRemoveFromStringProperty); }
            set { SetValue(EntityToRemoveFromStringProperty, value); }
        }

        /// <summary>
        /// AddIsEnabled Dependency Property.
        /// </summary>
        public static readonly DependencyProperty EntityToRemoveFromStringProperty =
            DependencyProperty.Register(
                "EntityToRemoveFromString",
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

