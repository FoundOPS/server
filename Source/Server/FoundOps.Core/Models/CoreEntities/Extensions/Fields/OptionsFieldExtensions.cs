﻿using System.Reactive.Linq;
using FoundOps.Common.Tools;
using System;
using System.Collections.ObjectModel;
using System.Linq;

//Need to manually link (share) this file, or else Option will be generated
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class OptionsField
    {
        private IDisposable _optionsStringTracker;

        /// <summary>
        /// Updates the Options based on the OptionsString and the Value string
        /// </summary>
        /// <param name="notifyPropertyChanged">Whether or not to trigger a property changed on "Options"</param>
        public void UpdateOptions(bool notifyPropertyChanged = true)
        {
            ObservableCollection<Option> result;

            if (String.IsNullOrEmpty(OptionsString))
            {
                result = new ObservableCollection<Option>();
            }
            else
            {
                var names = this.OptionsString.Split(',').Select(CsvWriter.Unescape).ToList();

                var options = names.Select((t, i) => new Option { Name = t, Parent = this }).ToArray();

                if (!String.IsNullOrEmpty(Value))
                {
                    var selectedIndexes = this.Value.Split(',').Select(CsvWriter.Unescape).Select(int.Parse);
                    foreach (var index in selectedIndexes)
                        options.ElementAt(index).IsChecked = true;
                }

                result = new ObservableCollection<Option>(options);
            }

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

.Subscribe(_ => UpdateOptionStrings());

            if (notifyPropertyChanged)
                this.CompositeRaiseEntityPropertyChanged("Options");
        }

        private void UpdateOptionStrings()
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

        private ObservableCollection<Option> _options;
        public ObservableCollection<Option> Options
        {
            get
            {
                if (_options == null)
                    UpdateOptions(false);

                return _options;
            }
        }
    }
}