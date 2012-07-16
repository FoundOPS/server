using System;
using System.Linq;
using System.Net.Http;
using FoundOPS.API.Controllers;
using FoundOPS.API.Models;
using FoundOPS.API.Api;
using FoundOps.Core.Models.CoreEntities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DateTimeField = FoundOPS.API.Models.DateTimeField;
using NumericField = FoundOPS.API.Models.NumericField;
using OptionsField = FoundOPS.API.Models.OptionsField;
using TextBoxField = FoundOPS.API.Models.TextBoxField;

namespace API.Tests.Controllers
{
    [TestClass]
    public class ServiceControllerTest
    {
        [TestMethod]
        public void GetTaskDetails()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost");

            var controller = new ServiceController() { Request = request };

            var service = controller.GetTaskDetails(new Guid("8D6DA3D0-FFC5-4E21-8C06-516878366E65")).FirstOrDefault();

            var field = service.Fields.FirstOrDefault(f => f is OptionsField);

            var optionsField1 = (OptionsField) field;
            if (optionsField1 != null)
            {
                var options = optionsField1.Options;

                foreach (var option in options)
                {
                    option.IsChecked = false;
                }
            }

            var optionsField = (OptionsField) field;
            if (optionsField != null)
                optionsField.Options.FirstOrDefault(o => o.Index == 2).IsChecked = true;

            field = service.Fields.FirstOrDefault(f => f is TextBoxField);

            var textBoxField = (TextBoxField) field;
            if (textBoxField != null) textBoxField.Value = "No longer long"; //Notes

            field = service.Fields.LastOrDefault(f => f is TextBoxField);

            var boxField = (TextBoxField) field;
            if (boxField != null) boxField.Value = "asdfg";

            field = service.Fields.FirstOrDefault(f => f is NumericField);

            var numericField = (NumericField) field;
            if (numericField != null) numericField.Value = (decimal?) 3.22;

            field = service.Fields.FirstOrDefault(f => f is DateTimeField);

            var dateTimeField = (DateTimeField) field;
            if (dateTimeField != null)
                dateTimeField.Value = dateTimeField.Value + new TimeSpan(0, 3, 0, 0);

            var response = controller.UpdateServiceDetails(service);
        }
    }
}
