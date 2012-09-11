using System.Windows.Controls;
using FoundOps.Common.Silverlight.Services;
using FoundOps.Common.Silverlight.UI.Controls.AddEditDelete;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.CoreEntities.DesignData;
using FoundOps.SLClient.Data.Services;
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
    public class FieldsVM : InfiniteAccordionVM<Field, Field>, IAddToDeleteFromSource<Field>
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

        #region Implementation of IAddToDeleteFromSource<ServiceTemplate>

        public Func<string, Field> CreateNewItem { get; private set; }

        public string MemberPath { get; private set; }

        /// <summary>
        /// A method to update the AddToDeleteFrom's AutoCompleteBox with suggestions remotely loaded.
        /// </summary>
        public Action<AutoCompleteBox> ManuallyUpdateSuggestions { get; private set; }

        #endregion

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldsVM"/> class.
        /// </summary>
        [ImportingConstructor]
        public FieldsVM()
            : base(new[] { typeof(ServiceTemplate) }, false)
        {
            //TODO Setup deleting..
            CanDeleteSubject.OnNext(false);
            //// Can only delete if the selected field is not required
            //this.SelectedEntityObservable.Where(se => se != null)
            //    .Select(selectedField => !selectedField.Required).Subscribe(CanDeleteSubject);

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


            #region IAddToDeleteFromSource<Field> Implementation

            MemberPath = "Name";

            //In the Administrative Console, creating a new FoundOPS template
            CreateNewItem = name =>
            {
                throw new Exception("Should not add new items through AddToDeleteFrom control. Still using old Add/Delete");
            };

            ManuallyUpdateSuggestions = autoCompleteBox =>
            {
                // Make sure there is a current serviceProvider context and it is not FoundOPS
                // and there is a service template context
                var businessAccountContext = ContextManager.GetContext<BusinessAccount>();
                if (businessAccountContext == null || businessAccountContext.Id == BusinessAccountsConstants.FoundOpsId)
                    return;

                var serviceTemplateContext = ContextManager.GetContext<ServiceTemplate>();

                // Search the parent FoundOPS ServiceTemplate for Fields the current service template does not have yet
                SearchSuggestionsHelper(autoCompleteBox, () =>
                     Manager.Data.DomainContext.SearchFieldsForServiceProviderQuery(Manager.Context.RoleId, serviceTemplateContext.ParentServiceTemplate.Id, serviceTemplateContext.Id, autoCompleteBox.SearchText));
            };

            #endregion
        }

        #region Logic

        protected override Field AddNewEntity(object commandParameter)
        {
            var serviceTemplateContext = ContextManager.GetContext<ServiceTemplate>();
            if (serviceTemplateContext == null)
                throw new NotSupportedException("FieldsVM is setup to work only when there is a ServiceTemplate Context");

            if (serviceTemplateContext.ServiceTemplateLevel != ServiceTemplateLevel.FoundOpsDefined)
                throw new NotSupportedException("Cannot add new Fields to a non FoundOPS level Service Template");

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
                    fieldToAdd = new NumericField { DecimalPlaces = 2, Mask = "c", Minimum = 0, Maximum = 999 };
                    break;
                case "Number":
                    fieldToAdd = new NumericField { DecimalPlaces = 2, Mask = "g", Minimum = -999, Maximum = 999 };
                    break;
                case "Percentage":
                    fieldToAdd = new NumericField { DecimalPlaces = 2, Mask = "p", Minimum = 0, Maximum = 1 };
                    break;
                case "Textbox Small":
                    fieldToAdd = new TextBoxField { IsMultiline = false };
                    break;
                case "Textbox Large":
                    fieldToAdd = new TextBoxField { IsMultiline = true };
                    break;
                case "Time":
                    fieldToAdd = new DateTimeField { DateTimeType = DateTimeType.TimeOnly };
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

            //Setup the ServiceTemplate Context
            serviceTemplateContext.Fields.Add(fieldToAdd);
            this.DomainContext.Fields.Add(fieldToAdd);

            return fieldToAdd;
        }

        public override void DeleteEntity(Field entityToDelete)
        {
            var serviceTemplate = entityToDelete.OwnerServiceTemplate;
            if (serviceTemplate.ServiceTemplateLevel == ServiceTemplateLevel.ServiceProviderDefined)
            {
                var result =
                    MessageBox.Show(
                        "Doing this will delete this Field and all of its children fields as well as any data assiciated with them. Are you sure this is what you want to do?",
                        "Stop and Think!", MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.Cancel)
                    return;
            }

            base.DeleteEntity(entityToDelete);
        }

        #endregion
    }
}
