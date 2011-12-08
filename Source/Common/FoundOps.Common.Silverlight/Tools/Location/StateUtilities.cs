using System;
using System.Collections.Generic;
using System.Windows.Data;

namespace FoundOps.Common.Silverlight.Tools
{
    public static class StateUtilities
    {
        private static string[] _stateArray = {"AK", "AL", "AR", "AZ", "CA", "CO", "CT", "DC",
                                               "DE", "FL", "GA", "HI", "IA", "ID", "IL", "IN", "KS", "KY",
                                               "LA", "MA", "MD", "ME", "MI", "MN", "MO", "MS", "MT", "NC",
                                               "ND", "NE", "NH", "NJ", "NM", "NV", "NY", "OH", "OK", "OR",
                                               "PA", "RI", "SC", "SD", "TN", "TX", "UT", "VA", "VT", "WA",
                                               "WI", "WV", "WY"};
        private static List<string> _states = new List<string>(_stateArray);

        /// Returns a string array with the state abbreviations
        public static string[] GetStates()
        {
            return _stateArray;
        }


        /// Returns a boolean value on whether the specified string is a two letter state

        public static bool IsATwoLetterState(string input)
        {
            bool isState = false;

            if (_states.Contains(input))
            {
                isState = true;
            }

            return isState;
        }
    }
    public class StateGetValuesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return StateUtilities.GetStates();
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}


