using FoundOps.Core.Models.CoreEntities;
using System;
using System.Linq;
using System.Web.Mvc;

namespace FoundOps.Api.Controllers.Mvc
{
    public class SignatureFieldController : Controller
    {
        /// <summary>
        /// Return the signature svg of a signature field
        /// </summary>
        /// <param name="id">The id of the signature field</param>
        /// <returns>An image of the signature</returns>
        public string Image(Guid id)
        {
            var coreEntitiesContainer = new CoreEntitiesContainer();
            
            var signatureField = coreEntitiesContainer.Fields.OfType<SignatureField>().FirstOrDefault(sf => sf.Id == id);
            if (signatureField == null) return null;

            //Data must be stored using https://github.com/brinley/jSignature base30 format
            var data = new jSignature.Tools.Base30Converter().GetData(signatureField.Value);
            
            string svg = jSignature.Tools.SVGConverter.ToSVG(data);
            return svg;
        }
    }
}
