using FoundOps.Common.Silverlight.Models.Collections;
using FoundOps.Common.Tools;
using RiaServicesContrib;
using RiaServicesContrib.DomainServices.Client;
using System;
using System.Linq;
using System.Reactive.Linq;

//Partial class must be part of same namespace
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class OptionsField
    {
        protected override Field MakeChildSilverlight()
        {
            var entityGraphShape = new EntityGraphShape().Edge<OptionsField, Option>(of => of.Options);

            //Clone using RIA Services Contrib's Entity Graph
            var childOptionsField = this.Clone(entityGraphShape);

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

                //Everytime the OptionsWrapper collection changes, select all the options' property changed events
                OptionsWrapper.FromCollectionChanged()
                    .SelectLatest(e =>
                        ((OrderedEntityCollection<Option>)e.Sender).Where(o => o != null)
                        .Select(option => option.FromAnyPropertyChanged()).Merge().Throttle(TimeSpan.FromMilliseconds(250)))
                    //Notify that ThisField changed
                        .ObserveOnDispatcher().Subscribe(_ => this.CompositeRaiseEntityPropertyChanged("ThisField"));
            }
        }

        private Func<Option, OrderedEntityCollection<Option>> GetOptionsWrapper
        {
            get
            {
                return option => option.OptionsField == null ? null : option.OptionsField.OptionsWrapper;
            }
        }

        partial void OnCreation()
        {
            OptionsWrapper = new OrderedEntityCollection<Option>(this.Options, "Index", true, GetOptionsWrapper);
        }

        protected override void OnLoaded(bool isInitialLoad)
        {
            if (isInitialLoad)
                OptionsWrapper = new OrderedEntityCollection<Option>(this.Options, "Index", true, GetOptionsWrapper);
        }
    }
}
