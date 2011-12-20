using System.Linq;
using System.Windows;
using FoundOps.Common.Silverlight.Interfaces;
using System.ServiceModel.DomainServices.Client;

namespace FoundOps.Common.Silverlight.Tools.ExtensionMethods
{
    public static class ValidationExtensions
    {
        public static bool ValidateDisplayErrors(this Entity entity)
        {
            if (typeof(IRaiseValidationErrors).IsAssignableFrom(entity.GetType()))
            {
                ((IRaiseValidationErrors)entity).RaiseValidationErrors();
            }

            for (var i = 0; i < entity.ValidationErrors.Count; i++)
            {
                var error = entity.ValidationErrors.ElementAt(i).ErrorMessage;

                if ((i + 1) < entity.ValidationErrors.Count)
                    error += " and ";

                MessageBox.Show(error, "Please Fix Errors", MessageBoxButton.OK);
            }

            return entity.ValidationErrors.Count <= 0;
        }
    }
}
