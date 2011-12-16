using System;
using EntityGraph;
using System.Linq;
using EntityGraph.RIA;
using System.ComponentModel;
using System.Reactive.Linq;
using FoundOps.Common.Tools;
using FoundOps.Common.Silverlight.Tools;

//Partial class must be part of same namespace
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class OptionsField
    {
        protected override Field MakeChildSilverlight()
        {
            var entityGraph = new EntityGraph<OptionsField>(this, new EntityGraphShape().Edge<OptionsField, Option>(of => of.Options));

            //Clone using RIA Services Contrib's Entity Graph
            var childOptionsField = (OptionsField)entityGraph.Clone();

            //Update the Id
            childOptionsField.Id = Guid.NewGuid();

            //Clear ServiceTemplate to prevent overriding ParentField's ServiceTemplate
            childOptionsField.OwnerServiceTemplate = null;

            //Update the parent of the field
            childOptionsField.ParentFieldId = this.Id;

            //Update the options' Ids
            foreach (var option in childOptionsField.Options)
            {
                option.Id = Guid.NewGuid();
                option.OptionsField = childOptionsField;
            }

            return childOptionsField;
        }

        private OrderedEntityCollection<Option> _optionsWrapper;
        /// <summary>
        /// Wraps the Options and orders by the Option.Index
        /// </summary>
        public OrderedEntityCollection<Option> OptionsWrapper
        {
            get { return _optionsWrapper; }
            private set
            {
                _optionsWrapper = value;
                this.RaisePropertyChanged("OptionsWrapper");

                //Whenever an option changes, notify that ThisField changed
                OptionsWrapper.FromCollectionChangedEvent().Where(e=>e.NewItems!=null).SelectMany(e =>
                {
                    var newItems = new object[e.NewItems.Count]; e.NewItems.CopyTo(newItems, 0);
                    return newItems.Select(ni => ni as INotifyPropertyChanged).Where(ni => ni != null)
                        .Select(ni => ni.FromAnyPropertyChanged()).Merge();
                }).Throttle(new TimeSpan(0, 0, 0, 0, 250)).ObserveOnDispatcher()
               .Subscribe(_ => this.CompositeRaiseEntityPropertyChanged("ThisField"));
            }
        }

        partial void OnCreation()
        {
            OptionsWrapper = new OrderedEntityCollection<Option>(this.Options, "Index", true);
        }

        protected override void OnLoaded(bool isInitialLoad)
        {
            if (isInitialLoad)
                OptionsWrapper = new OrderedEntityCollection<Option>(this.Options, "Index", true);
        }
    }
}
