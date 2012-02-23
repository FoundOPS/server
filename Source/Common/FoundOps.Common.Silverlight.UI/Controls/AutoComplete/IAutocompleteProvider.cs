using Telerik.Windows.Controls;

namespace FoundOps.Common.Silverlight.UI.Controls.AutoComplete
{
	public interface IAutocompleteProvider
	{
		event ItemsReceivedEventHandler ItemsReceived;

		string TextPath { get; set; }

		TextSearchMode TextSearchMode { get; set; }

		void GetItems(string text);
	}
}