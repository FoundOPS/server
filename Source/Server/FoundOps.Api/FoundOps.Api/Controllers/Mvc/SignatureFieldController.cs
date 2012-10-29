using ConsumedByCode.SignatureToImage;
using FoundOps.Core.Models.CoreEntities;
using System;
using System.Drawing;
using System.Linq;
using System.Web.Mvc;

namespace FoundOps.Api.Controllers.Mvc
{
    public class SignatureFieldController : Controller
    {
        /// <summary>
        /// Shows the signature image of a signature field
        /// </summary>
        /// <param name="signatureId">The id of the signature field</param>
        /// <returns>An image of the signature</returns>
        public Bitmap GetSigntureImage(Guid signatureId)
        {
            var coreEntitiesContainer = new CoreEntitiesContainer();
            
            var signatureField = coreEntitiesContainer.Fields.OfType<SignatureField>().FirstOrDefault(sf => sf.Id == signatureId);
            if (signatureField == null) return null;

            var sigToImage = new SignatureToImage();
            var signatureImage = sigToImage.SigJsonToImage(signatureField.Value);

            return signatureImage;
        }
    }
}
