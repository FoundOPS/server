using FoundOps.Common.Tools;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

//Need to manually link (share) this file, or else Option will be generated
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class OptionsField
    {
        private IDisposable _optionsStringTracker;

        /// <summary>
        /// Convert an OptionsString and Value to a concatenated string.
        /// For toString()
        /// </summary>
        /// <returns></returns>
        public static string SimpleValue(string optionsString, string value)
        {
            if (optionsString == null || value == null)
                return "";

            var names = optionsString.Split(',').Select(CsvWriter.Unescape).ToList();
            var options = names.Select((t, i) => new Option { Name = t }).ToArray();

            var selectedIndexes = value.Split(',').Select(CsvWriter.Unescape).Select(int.Parse);

            bool first = true;

            var result = selectedIndexes.Aggregate("", (current, index) =>
            {
                if (first)
                {
                    first = false;
                    return options.ElementAt(index).Name;
                }

                return current + "," + options.ElementAt(index).Name;
            });

            return result;
        }

        private ObservableCollection<Option> _options;
        public ObservableCollection<Option> Options
        {
            get
            {
                if (_options == null)
                    SetOptions(false);

                return _options;
            }
        }

        public override string ToString()
        {
            return SimpleValue(OptionsString, Value);
        }

        #region Helpers

        /// <summary>
        /// Get a static IEnumerable of Options from the OptionsString and Value
        /// </summary>
        private static IEnumerable<Option> GetOptions(string optionsString, string value, OptionsField parent)
        {
            if (String.IsNullOrEmpty(optionsString) || String.IsNullOrEmpty(value))
            {
                return new List<Option>();
            }

            var names = optionsString.Split(',').Select(CsvWriter.Unescape).ToList();

            var options = names.Select((t, i) => new Option { Name = t, Parent = parent }).ToArray();

            var selectedIndexes = value.Split(',').Select(CsvWriter.Unescape).Select(int.Parse);
            foreach (var index in selectedIndexes)
                options.ElementAt(index).IsChecked = true;

            return options;
        }

        /// <summary>
        /// Update the OptionsStrings to the current Options
        /// </summary>
        private void SyncOptionStrings()
        {
            string optionsString = "";
            string valueString = "";
            if (Options != null)
            {
                var orderedOptions = Options.OrderBy(o => o.Name).ToArray();

                //create an options string with the option's names alphabetically organized and CSVed
                //ex: "Option A,Option B,Option C"
                var optionsNames = orderedOptions.Select(o => o.Name);
                optionsString = string.Join(",", optionsNames.Select(CsvWriter.Escape));

                //the optionsfield value string is the indexes of the selected options concatenated
                //ex: "0,2" = First and third option are selected

                var indexesChecked = orderedOptions.Where(o => o.IsChecked)
                    .Select(option => Array.IndexOf(orderedOptions, option).ToString());

                valueString = string.Join(",", indexesChecked.Select(CsvWriter.Escape));
            }

            OptionsString = optionsString;
            Value = valueString;
        }

        /// <summary>
        /// Setup the Options based on the OptionsString and the Value string.
        /// It tracks changes and updates the collection accordingly.
        /// </summary>
        /// <param name="notifyPropertyChanged">Whether or not to trigger a property changed on "Options"</param>
        public void SetOptions(bool notifyPropertyChanged = true)
        {
            var options = GetOptions(OptionsString, Value, this);
            var result = new ObservableCollection<Option>(options);

            _options = result;

            //Track changes to the Options and update the Options strings
            if (_optionsStringTracker != null)
                _optionsStringTracker.Dispose();

            _optionsStringTracker = _options.FromCollectionChangedAndNow().SelectLatest(ops =>
                //whenever an option's property changes
                       _options.Select(o => o.FromAnyPropertyChanged()).Merge().AsGeneric()
                           //whenever the collection changes
                       .AndNow()).Synchronize()
#if SILVERLIGHT
                       .ObserveOnDispatcher()
#endif

.Subscribe(_ => SyncOptionStrings());

            if (notifyPropertyChanged)
                this.CompositeRaiseEntityPropertyChanged("Options");
        }

        #endregion
    }
}