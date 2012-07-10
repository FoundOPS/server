using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using FoundOps.Core.Models.Azure;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using FoundOps.Server.Tools;
using System;
using System.Web.Mvc;
using Newtonsoft.Json;

#if !DEBUG //RELEASE or TESTRELEASE
using System.IO;
using System.Net;
using FoundOps.Common.Tools;
#endif

namespace FoundOps.Server.Controllers
{
    public class HomeController : Controller
    {
        private readonly CoreEntitiesContainer _coreEntitiesContainer = new CoreEntitiesContainer();

        //Cannot require the page to be HTTPS until we have our own tile server
        //#if !DEBUG
        //        [RequireHttps]
        //#endif
        public ActionResult Index()
        {
#if !DEBUG
            if (!HttpContext.User.Identity.IsAuthenticated)
                return Redirect(Global.RootFrontSiteUrl);
#endif

            var user = _coreEntitiesContainer.CurrentUserAccount().Include(ua => ua.PartyImage)
                        .Include(ua => ua.RoleMembership).Include("RoleMembership.Blocks").Include("RoleMembership.OwnerParty")
                        .First();

            //Load all of the party images for the owner's of roles
            var businessOwnerIds = user.RoleMembership.Select(r => r.OwnerParty).OfType<BusinessAccount>().Select(ba => ba.Id).Distinct();
            var partyImages = _coreEntitiesContainer.Files.OfType<PartyImage>().Where(pi => businessOwnerIds.Contains(pi.PartyId)).Distinct();
            if (user.PartyImage != null)
                partyImages = partyImages.Union(new[] { user.PartyImage });

            //Go through each party image and get the url with the shared access key
            var partyImageUrls = new Dictionary<Guid, string>();
            foreach (var partyImage in partyImages)
            {
                var imageUrl = partyImage.RawUrl + AzureServerHelpers.GetBlobUrlHelper(partyImage.OwnerParty.Id, partyImage.Id);
                partyImageUrls.Add(partyImage.OwnerParty.Id, imageUrl);
            }

            var roles = user.RoleMembership.Distinct().OrderBy(r => r.OwnerParty.DisplayName);
            var sections = roles.SelectMany(r => r.Blocks).Where(s => !s.HideFromNavigation).Distinct().OrderBy(b => b.Name);

            //Build the initializeConfig model
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);

            using (JsonWriter jsonWriter = new JsonTextWriter(sw))
            {
#if DEBUG
                jsonWriter.Formatting = Newtonsoft.Json.Formatting.Indented;
#endif
                jsonWriter.WriteStartObject(); //initializeConfig object
                jsonWriter.WritePropertyName("name");
                jsonWriter.WriteValue(user.FirstName + " " + user.LastName);

                jsonWriter.WritePropertyName("avatarUrl");
                jsonWriter.WriteValue(user.PartyImage != null
                                          ? partyImageUrls[user.PartyImage.Id]
                                          : "img/emptyPerson.png");

                jsonWriter.WritePropertyName("roles");
                jsonWriter.WriteStartArray();

                //Go through each of the user's roles
                foreach (var role in roles)
                {
                    jsonWriter.WriteStartObject(); //start role object

                    //Set the id
                    jsonWriter.WritePropertyName("id");
                    jsonWriter.WriteValue(role.Id);

                    //Set the business account name
                    jsonWriter.WritePropertyName("name");
                    jsonWriter.WriteValue(role.OwnerParty.DisplayName);

                    //Set the business's logo
                    jsonWriter.WritePropertyName("businessLogoUrl");
                    jsonWriter.WriteValue(role.OwnerParty.PartyImage != null
                                              ? partyImageUrls[role.OwnerParty.PartyImage.Id]
                                              : "img/emptyBusiness.png");

                    //Add the available sections's names for the roles
                    jsonWriter.WritePropertyName("sections");
                    jsonWriter.WriteStartArray();
                    foreach (var section in role.Blocks.Where(s => !s.HideFromNavigation).OrderBy(r => r.Name))
                    {
                        jsonWriter.WriteValue(section.Name);
                    }
                    jsonWriter.WriteEnd(); //end sections array
                    jsonWriter.WriteEnd(); //end role object
                }

                jsonWriter.WriteEnd(); //end roles array


                //Add each of the available blocks (sections) and their details
                jsonWriter.WritePropertyName("sections");
                jsonWriter.WriteStartArray();
                foreach (var section in sections)
                {
                    jsonWriter.WriteStartObject(); //start section object

                    jsonWriter.WritePropertyName("name");
                    jsonWriter.WriteValue(section.Name);

                    jsonWriter.WritePropertyName("url");
                    jsonWriter.WriteValue(section.Url);

                    jsonWriter.WritePropertyName("iconUrl");
                    jsonWriter.WriteValue(section.IconUrl);

                    jsonWriter.WritePropertyName("hoverIconUrl");
                    jsonWriter.WriteValue(section.HoverIconUrl);

                    jsonWriter.WriteEnd(); //end section object
                }
                jsonWriter.WriteEnd(); //end section array

                jsonWriter.WriteEndObject(); //end initializeConfig object
            }

            var configData = sb.ToString();

            //Cast to object so the overload for model data is used
            //instead of it being confused as the view name
            return View((object)configData);
        }

        //Cannot require the page to be HTTPS until we have our own tile server
        //#if !DEBUG
        //        [RequireHttps]
        //#endif
        [AddTestUsersThenAuthorize]
        public ActionResult Silverlight()
        {
#if DEBUG
            var random = new Random();
            var version = random.Next(10000).ToString();
#else
            var request = (HttpWebRequest)WebRequest.Create(AzureTools.BlobStorageUrl + "xaps/version.txt");

            // *** Retrieve request info headers
            var response = (HttpWebResponse)request.GetResponse();
            var stream = new StreamReader(response.GetResponseStream());

            var version = stream.ReadToEnd();
            response.Close();
            stream.Close();
#endif

            //Must cast the string as an object so it uses the correct overloaded method
            return View((object)version);
        }

        [AddTestUsersThenAuthorize]
        public ActionResult MapView()
        {
            return View();
        }
    }
}