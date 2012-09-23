using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using FoundOps.Common.Tools;
using FoundOps.Common.Composite.Entities;
using System;
using System.Collections.ObjectModel;
using System.Linq;

// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public enum OptionsType
    {
        Combobox = 0,
        Checkbox = 1,
        Checklist = 2
    }

    public partial class OptionsField : IEntityDefaultCreation
    {
        #region Implementation of IEntityDefaultCreation

#if SILVERLIGHT
        partial void OnCreated()
        {
            ((IEntityDefaultCreation) this).OnCreate();
        }
#else
        public OptionsField()
        {
            ((IEntityDefaultCreation)this).OnCreate();
        }

#endif

        partial void OnCreation(); //For Extensions on Silverlight Side

        void IEntityDefaultCreation.OnCreate()
        {
            Id = Guid.NewGuid();
            OnCreation();
        }

        #endregion

        public OptionsType OptionsType
        {
            get
            {
                return (OptionsType)Convert.ToInt32(this.TypeInt);
            }
            set
            {
                this.TypeInt = Convert.ToInt16(value);
            }
        }

        partial void OnTypeIntChanged()
        {
            this.CompositeRaiseEntityPropertyChanged("OptionsType");
        }

        private ObservableCollection<Option> _options;
        public ObservableCollection<Option> Options
        {
            get
            {
                if (_options == null)
                {
                    if (OptionsString == null)
                    {
                        _options = new ObservableCollection<Option>();
                    }
                    else
                    {
                        var names = this.OptionsString.Split(',').Select(CsvWriter.Unescape).ToList();
                        var values = this.Value.Split(',').Select(CsvWriter.Unescape).Select(int.Parse).ToList();

                        var options = names.Select((t, i) => new Option { Name = t, IsChecked = values.Contains(i), Parent = this });
                        _options = new ObservableCollection<Option>(options);
                    }

                    _options.FromCollectionChanged().SelectLatest(ops =>
                        //whenever an option's property changes
                        _options.Select(o => o.FromAnyPropertyChanged()).Merge().AsGeneric()
                            //whenever the collection changes
                        .AndNow())
#if SILVERLIGHT
                        .ObserveOnDispatcher()
#endif
.Subscribe(_ =>
                    {
                        OptionsString = CreateOptionsString(Options);
                        Value = CreateValueString(Options);
                    });
                }

                return _options;
            }
        }

        private string CreateOptionsString(IEnumerable<Option> optionsForField)
        {
            if (optionsForField == null)
                return "";

            //create an options string with the option's names alphabetically organized and CSVed
            //ex: "Option A,Option B,Option C"
            var optionsNames = optionsForField.Select(o => o.Name).OrderBy(name => name);
            var csv = string.Join(",", optionsNames.Select(CsvWriter.Escape));

            return csv;
        }

        private string CreateValueString(IEnumerable<Option> optionsForField)
        {
            if (optionsForField == null)
                return "";

            //the optionsfield value string is the indexes of the selected options concatenated
            //ex: "0,2" = First and third option are selected
            var orderedOptions = optionsForField.OrderBy(o => o.Name).ToArray();

            var indexesChecked = orderedOptions.Where(o => o.IsChecked)
                .Select(option => Array.IndexOf(orderedOptions, option).ToString());

            var csv = string.Join(",", indexesChecked.Select(CsvWriter.Escape));
            return csv;
        }
    }
}