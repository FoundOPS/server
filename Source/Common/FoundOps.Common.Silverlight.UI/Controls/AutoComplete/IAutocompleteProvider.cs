using Telerik.Windows.Controls;

namespace FoundOps.Common.Silverlight.UI.Controls.AutoComplete
{
	public interface IAutocompleteProvider
	{
        /// <summary>
        /// An event to call when [items received].
        /// </summary>
		event ItemsReceivedEventHandler ItemsReceived;

        /// <summary>
        /// Bound to the TextSearchMode from the attached property of the AutocompleteBehavior.
        /// </summary>
		string TextPath { get; set; }

        /// <summary>
        /// Bound to the TextPath from the ComboBox.
        /// </summary>
		TextSearchMode TextSearchMode { get; set; }

        /// <summary>
        /// A method to gets items based on text.
        /// </summary>
        /// <param name="text">The ComboBox's text.</param>
		void GetItems(string text);
	}
}