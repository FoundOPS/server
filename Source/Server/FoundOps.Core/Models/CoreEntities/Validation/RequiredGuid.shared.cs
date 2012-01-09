using System;
using System.ComponentModel.DataAnnotations;

namespace FoundOps.Core.Models.Validation
{
    public class RequiredGuid : ValidationAttribute  
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return ((Guid) value != Guid.Empty)
                       ? ValidationResult.Success
                       : new ValidationResult(this.ErrorMessage);
        }  
    }
}
