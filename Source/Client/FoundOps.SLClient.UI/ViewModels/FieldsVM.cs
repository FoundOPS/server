using FoundOps.Common.Silverlight.Services;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.CoreEntities.DesignData;
using FoundOps.SLClient.Data.ViewModels;
using MEFedMVVM.ViewModelLocator;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Contains the logic for displaying Fields
    /// </summary>
    [ExportViewModel("FieldsVM")]
    public class FieldsVM : InfiniteAccordionVM<Field>
    {
        # region Public Properties

        private static readonly IEnumerable<string> StandardFieldTypes = new List<string> { "Checkbox", "Checklist", "Combobox", "Currency", "Number", "Percentage", "Textbox Small", "Textbox Large", "Time" };

        private IEnumerable<string> _fieldTypes = StandardFieldTypes;
        /// <summary>
        /// Gets the field types.
        /// </summary>
        public IEnumerable<string> FieldTypes
        {
            get { return _fieldTypes; }
            private set
            {
                _fieldTypes = value;
                this.RaisePropertyChanged("FieldTypes");
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldsVM"/> class.
        /// </summary>
        [ImportingConstructor]
        public FieldsVM() : base(new[] { typeof(ServiceTemplate)}, false )
        {
            //Can only delete if the selected field is not required
            this.SelectedEntityObservable.Where(se => se != null)
                .Select(selectedField => !selectedField.Required).Subscribe(CanDeleteSubject);

            //Whenever the ServiceTemplateContext changes or the details are loaded
            //a) update the Fields DCV
            //b) Enable the Destination field option 
            //   if the current ServiceTemplate is a FoundOPS level or ServiceProvider level service template
            //   and does not have a destination field
            this.ContextManager.GetContextObservable<ServiceTemplate>().ObserveOnDispatcher()
                .Subscribe(serviceTemplateContext =>
                {
                    //a) update the Fields DCV
                    ViewObservable.OnNext(serviceTemplateContext != null ?
                        DomainCollectionViewFactory<Field>.GetDomainCollectionView(serviceTemplateContext.Fields) : null);
                    //b) Enable the Destination field option 
                    //   if the current ServiceTemplate is a FoundOPS level or ServiceProvider level service template
                    //   and does not have a destination field
                    if (serviceTemplateContext != null &&
                        (serviceTemplateContext.ServiceTemplateLevel == ServiceTemplateLevel.FoundOpsDefined || serviceTemplateContext.ServiceTemplateLevel == ServiceTemplateLevel.ServiceProviderDefined)
                        && serviceTemplateContext.Fields.OfType<LocationField>().FirstOrDefault(lf => lf.LocationFieldType == LocationFieldType.Destination) == null)
                        FieldTypes = StandardFieldTypes.Union(new[] { "Destination" });
                    else
                        FieldTypes = StandardFieldTypes;
                });
        }

        #region Logic

        protected override Field AddNewEntity(object commandParameter)
        {
            var serviceTemplateContext = ContextManager.GetContext<ServiceTemplate>();
            if (serviceTemplateContext == null)
                throw new NotSupportedException("FieldsVM is setup to work only when there is a ServiceTemplate Context");

            var param = (string)commandParameter;
            Field fieldToAdd;
            switch (param)
            {
                case "Checkbox":
                    fieldToAdd = new OptionsField { AllowMultipleSelection = false, OptionsType = OptionsType.Checkbox };
                    ((OptionsField)fieldToAdd).OptionsWrapper.Add(new Option { Name = "Default" });
                    break;
                case "Checklist":
                    fieldToAdd = new OptionsField { AllowMultipleSelection = true, OptionsType = OptionsType.Checklist };
                    break;
                case "Combobox":
                    fieldToAdd = new OptionsField { AllowMultipleSelection = false, OptionsType = OptionsType.Combobox };
                    break;
                case "Currency":
                    fieldToAdd = new NumericField { DecimalPlaces = 2, Mask = "c" };
                    break;
                case "Number":
                    fieldToAdd = new NumericField { DecimalPlaces = 2, Mask = "g" };
                    break;
                case "Percentage":
                    fieldToAdd = new NumericField { DecimalPlaces = 2, Mask = "p" };
                    break;
                case "Textbox Small":
                    fieldToAdd = new TextBoxField { IsMultiline = false };
                    break;
                case "Textbox Large":
                    fieldToAdd = new TextBoxField { IsMultiline = true };
                    break;
                case "Time":
                    fieldToAdd = new DateTimeField { DateTimeType = DateTimeType.TimeOnly };
                    //CurrentServiceTemplateContext.Values.Add(new DateTimeValue { Data = DateTime.Now }); //Setup a Default Value
                    break;
                //NOTE: For now this is only available on FoundOPS & ServiceProvider level service templates that do not have a destination field
                case "Destination":
                    if (serviceTemplateContext.Fields.OfType<LocationField>().FirstOrDefault(lf => lf.LocationFieldType == LocationFieldType.Destination) != null)
                    {
                        MessageBox.Show("There is already a destination field");
                        return null;
                    }
                    fieldToAdd = new LocationField
                    {
                        ParentFieldId = ServiceTemplateConstants.ServiceDestinationFieldId,
                        Group = "Location",
                        Name = "Service Destination",
                        Tooltip = "Enter the Service Destination here",
                        LocationFieldType = LocationFieldType.Destination,
                        Required = true
                    };
                    //Remove DestinationField from field options
                    FieldTypes = StandardFieldTypes;
                    break;
                default:
                    throw new NotImplementedException("Field Type not Recognized");
            }

            if (!(fieldToAdd is LocationField))
                fieldToAdd.Group = "Details";

            //Setup the ServiceTemplate Context
            serviceTemplateContext.Fields.Add(fieldToAdd);
            this.DomainContext.Fields.Add(fieldToAdd);

            return fieldToAdd;
        }

        #endregion
    }
}
