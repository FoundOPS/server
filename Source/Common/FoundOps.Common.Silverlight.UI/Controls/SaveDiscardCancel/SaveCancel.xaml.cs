using System.Windows;
using FoundOps.Common.Silverlight.UI.ViewModels;

namespace FoundOps.Common.Silverlight.UI.Controls.SaveDiscardCancel
{
    public partial class SaveCancel
	{
		public SaveCancel()
		{
			// Required to initialize variables
			InitializeComponent();
		}

        #region ISaveDiscardChangesCommands Dependency Property

        /// <summary>
        /// ISaveDiscardChangesCommands
        /// </summary>
        public ISaveDiscardChangesCommands ISaveDiscardChangesCommands
        {
            get { return (ISaveDiscardChangesCommands) GetValue(ISaveDiscardChangesCommandsProperty); }
            set { SetValue(ISaveDiscardChangesCommandsProperty, value); }
        }

        /// <summary>
        /// ISaveDiscardChangesCommands Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ISaveDiscardChangesCommandsProperty =
            DependencyProperty.Register(
                "ISaveDiscardChangesCommands",
                typeof (ISaveDiscardChangesCommands),
                typeof (SaveCancel),
                new PropertyMetadata(null));

        #endregion
	}
}