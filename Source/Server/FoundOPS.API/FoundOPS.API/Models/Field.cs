using System;

namespace FoundOPS.API.Models
{
    public class Field
    {
        public Guid Id { get; set; }

        public string Group { get; set; }

        public string Name { get; set; }

        public bool Required { get; set; }

        public string ToolTip { get; set; }

        public Guid? ParentFieldId { get; set; }

        public Guid? ServiceTemplateId { get; set; }

        public static Field ConvertModel(object fieldModel)
        {
            //If the field is a TextBoxField, convert the field to a API TextBoxField and return
            if (fieldModel is FoundOps.Core.Models.CoreEntities.TextBoxField)
                return TextBoxField.ConvertTextBoxFieldModel((FoundOps.Core.Models.CoreEntities.TextBoxField) fieldModel);

            //If the field is a LocationField, convert the field to a API LocationField and return
            if (fieldModel is FoundOps.Core.Models.CoreEntities.LocationField)
                return LocationField.ConvertLocationFieldModel((FoundOps.Core.Models.CoreEntities.LocationField)fieldModel);

            //If the field is a NumericField, convert the field to a API NumericField and return
            if (fieldModel is FoundOps.Core.Models.CoreEntities.NumericField)
                return NumericField.ConvertNumericFieldModel((FoundOps.Core.Models.CoreEntities.NumericField)fieldModel);

            //If the field is a OptionsField, convert the field to a API OptionsField and the Options to API Options and return
            if (fieldModel is FoundOps.Core.Models.CoreEntities.OptionsField)
                return OptionsField.ConvertOptionsFieldModel((FoundOps.Core.Models.CoreEntities.OptionsField)fieldModel);
            
            //If the field is a DateTimeField, convert the field to a API DateTimeField and return
            if (fieldModel is FoundOps.Core.Models.CoreEntities.DateTimeField)
                return DateTimeField.ConvertDateTimeFieldModel((FoundOps.Core.Models.CoreEntities.DateTimeField)fieldModel);

            throw new Exception("Field does not exist");
        }
    }
}