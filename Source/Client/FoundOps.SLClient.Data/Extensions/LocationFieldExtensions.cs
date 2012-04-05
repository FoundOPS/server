using System;
using System.Reactive.Linq;
using RiaServicesContrib.DataValidation;
using System.ComponentModel.DataAnnotations;
using RiaServicesContrib.DomainServices.Client.DataValidation;

// Needs to be in the same namespace, because it is a partial class
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class LocationField
    {
        partial void OnCreation()
        {
            InitializeHelper();
        }
        protected override void OnLoaded(bool isInitialLoad)
        {
            if (isInitialLoad)
                InitializeHelper();

            base.OnLoaded(isInitialLoad);
        }

        private void InitializeHelper()
        {
            //Whenever the Value changes, validate this entity
            //DistinctUntilChanged to prevent StackOverflow on continuous validation
            Observable2.FromPropertyChangedPattern(this, x => x.LocationId).DistinctUntilChanged().ObserveOnDispatcher()
                .Subscribe(_ =>
                {
                    this.ValidationErrors.Clear();
                    var validator = new ValidationEngine { RulesProvider = new MEFValidationRulesProvider<ValidationResult>() };
                    validator.Validate(this);
                });

            Observable2.FromPropertyChangedPattern(this, x => x.Value).DistinctUntilChanged().ObserveOnDispatcher()
                 .Subscribe(_ =>
                 {
                     //When a LocationField's value is set automatically set the parent Service's Client
                     if (Value != null && Value.Party != null && Value.Party.ClientOwner != null &&
                         OwnerServiceTemplate != null && OwnerServiceTemplate.OwnerService != null)
                     {
                         OwnerServiceTemplate.OwnerService.Client = Value.Party.ClientOwner;
                     }
                 });
        }

        protected override Field MakeChildSilverlight()
        {
            //Clone using RIA Services Contrib's Entity Graph
            var child = (LocationField)base.MakeChildSilverlight();

            //Update the value
            child.Value = this.Value;

            return child;
        }
    }
}
