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
                    var names = this.OptionsString.Split(',').Select(CsvWriter.Unescape).ToList();
                    var values = this.Value.Split(',').Select(CsvWriter.Unescape).Select(int.Parse).ToList();

                    var options = names.Select((t, i) => new Option { Name = t, IsChecked = values.Contains(i), Parent = this });
                    _options = new ObservableCollection<Option>(options);

                    _options.FromCollectionChanged().SelectLatest(ops =>
                        //whenever an option's property changes
                        _options.Select(o => o.FromAnyPropertyChanged()).Merge().AsGeneric()
                            //whenever the collection changes
                        .AndNow())
#if SILVERLIGHT
.ObserveOnDispatcher()
#else
.ObserveOn(SynchronizationContext.Current)
#endif
.Subscribe(_ =>
                    {
                        OptionsString = "TODO";
                        Value = "TODO";
                    });
                }

                return _options;
            }
        }
    }
}