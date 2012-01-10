using System;
using System.ComponentModel;
using System.Reactive.Linq;
using FoundOps.Common.Silverlight.Services;
using FoundOps.SLClient.Data.Services;
using System.Collections.Generic;
using FoundOps.Core.Context.Services;
using System.ComponentModel.Composition;
using FoundOps.SLClient.Data.ViewModels;
using FoundOps.Core.Models.CoreEntities;
using MEFedMVVM.ViewModelLocator;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Contains the logic for displaying Fields
    /// </summary>
    [ExportViewModel("FieldsVM")]
    public class FieldsVM : CoreEntityCollectionInfiniteAccordionVM<Field>
    {
        # region Public Properties

        private readonly List<string> _fieldTypes = new List<string> { "Checkbox", "Checklist", "Combobox", "Currency", "Number", "Percentage", "Textbox Small", "Textbox Large", "Time" };
        /// <summary>
        /// Gets the field types.
        /// </summary>
        public List<string> FieldTypes { get { return _fieldTypes; } }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldsVM"/> class.
        /// </summary>
        /// <param name="dataManager">The data manager.</param>
        [ImportingConstructor]
        public FieldsVM(DataManager dataManager)
            : base(dataManager, false)
        {
            //Can only delete if the selected field is not required
            this.SelectedEntityObservable.Where(se => se != null)
                .Select(selectedField => !selectedField.Required).Subscribe(CanDeleteSubject);

            //Whenever the ServiceTemplateContext changes update the Fields DCV
            this.ContextManager.GetContextObservable<ServiceTemplate>().ObserveOnDispatcher()
                .Subscribe(serviceTemplateContext =>
                    DomainCollectionViewObservable.OnNext(serviceTemplateContext != null
                    ? DomainCollectionViewFactory<Field>.GetDomainCollectionView(serviceTemplateContext.Fields)
                    : null));
        }

        #region Logic

        protected override Field AddNewEntity(object commandParameter)
        {
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
                default:
                    throw new NotImplementedException("Field Type not Recognized");
            }

            fieldToAdd.Group = "Details";

            //Setup the ServiceTemplate Context

            var serviceTemplateContext = ContextManager.GetContext<ServiceTemplate>();
            if (serviceTemplateContext == null)
                throw new NotSupportedException("FieldsVM is setup to work only when there is a ServiceTemplate Context");

            fieldToAdd.OwnerServiceTemplate = serviceTemplateContext;

            this.Context.Fields.Add(fieldToAdd);

            return fieldToAdd;
        }

        #endregion
    }
}
