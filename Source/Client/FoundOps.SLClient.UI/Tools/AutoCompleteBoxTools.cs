using System;
using System.Windows.Controls;

namespace FoundOps.SLClient.UI.Tools
{
    /// <summary>
    /// Tools for use with the AutoCompleteBox.
    /// </summary>
    public static class AutoCompleteBoxTools
    {
        /// <summary>
        /// A method to automatically hookup search suggestions.
        /// The suggestions will populate whenever the AutoCompleteBox gets focus 
        /// and when the user types in text.
        /// NOTE: If you change this logic, you should probably change AddToDeleteFrom's code behind as well. (It manually implements this).
        /// </summary>
        /// <param name="autoCompleteBox">The autocompletebox to hookup.</param>
        /// <param name="manuallyUpdateSuggestions">An action</param>
        public static void HookupSearchSuggestions(this AutoCompleteBox autoCompleteBox,
            Action<AutoCompleteBox> manuallyUpdateSuggestions)
        {
            autoCompleteBox.MinimumPrefixLength = 0;
            autoCompleteBox.MinimumPopulateDelay = 200;

            autoCompleteBox.Populating += (sender, e) =>
            {
                //Allow us to wait for the response
                e.Cancel = true;
                manuallyUpdateSuggestions((AutoCompleteBox)sender);
            };

            autoCompleteBox.GotFocus += (sender, e) => ((AutoCompleteBox)sender).IsDropDownOpen = true;
        }
    }
}
