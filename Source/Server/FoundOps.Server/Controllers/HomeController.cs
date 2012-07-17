using FoundOps.Core.Models.Azure;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using FoundOps.Server.Tools;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System;
using System.Web.Mvc;

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
                return Redirect(ServerConstants.RootFrontSiteUrl);
#endif

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

            var user = _coreEntitiesContainer.CurrentUserAccount().Include(ua => ua.PartyImage)
                        .Include(ua => ua.RoleMembership).Include("RoleMembership.Blocks").Include("RoleMembership.OwnerBusinessAccount")
                        .First();

            //Load all of the party images for the owner's of roles
            var businessOwnerIds = user.RoleMembership.Select(r => r.OwnerBusinessAccount).OfType<BusinessAccount>().Select(ba => ba.Id).Distinct();
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

            var roles = user.RoleMembership.Distinct().OrderBy(r => r.OwnerBusinessAccount.DisplayName);
            var sections = roles.SelectMany(r => r.Blocks).Where(s => !s.HideFromNavigation).Distinct().OrderBy(b => b.Name);

            //Build the initializeConfig model
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);

            using (JsonWriter jsonWriter = new JsonTextWriter(sw))
            {
#if DEBUG
                jsonWriter.Formatting = Newtonsoft.Json.Formatting.Indented;
#endif
                jsonWriter.WriteStartObject(); //navigatorConfig object
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
                    jsonWriter.WriteValue(role.OwnerBusinessAccount.Name);

                    //Set the business's logo
                    if (role.OwnerBusinessAccount.PartyImage != null)
                    {
                        jsonWriter.WritePropertyName("businessLogoUrl");
                        jsonWriter.WriteValue(partyImageUrls[role.OwnerBusinessAccount.PartyImage.Id]);
                    }

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

                    jsonWriter.WritePropertyName("color");
                    jsonWriter.WriteValue(section.Color);

                    if (section.Url != null)
                    {
                        jsonWriter.WritePropertyName("url");
                        jsonWriter.WriteValue(section.Url);
                    }

                    jsonWriter.WritePropertyName("iconUrl");
                    jsonWriter.WriteValue(section.IconUrl);

                    jsonWriter.WritePropertyName("hoverIconUrl");
                    jsonWriter.WriteValue(section.HoverIconUrl);

                    if (section.IsSilverlight.HasValue && section.IsSilverlight.Value)
                    {
                        jsonWriter.WritePropertyName("isSilverlight");
                        jsonWriter.WriteValue(true);
                    }

                    jsonWriter.WriteEnd(); //end section object
                }
                jsonWriter.WriteEnd(); //end section array

                jsonWriter.WriteEndObject(); //end initializeConfig object
            }

            var configData = sb.ToString();

            var model = new Dictionary<string, object> { { "NavigatorConfig", configData }, { "SilverlightVersion", version } };

            //Cast to object so the overload for model data is used
            //instead of it being confused as the view name
            return View(model);
        }

#if !RELEASE
        //Purely for debugging
        public ActionResult TestSilverlight()
        {
            var random = new Random();
            var version = random.Next(10000).ToString();
            var model = new Dictionary<string, object> { { "SilverlightVersion", version } };
            return View(model);
        }
#endif

        [AddTestUsersThenAuthorize]
        public ActionResult MapView()
        {
            return View();
        }
    }
}