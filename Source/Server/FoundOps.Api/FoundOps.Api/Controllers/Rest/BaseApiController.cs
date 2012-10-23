using FoundOps.Api.Tools;
using FoundOps.Core.Models.Authentication;
using FoundOps.Core.Models.CoreEntities;
using System;
using System.Web.Http;

namespace FoundOps.Api.Controllers.Rest
{
    public abstract class BaseApiController : ApiController
    {
        public readonly CoreEntitiesContainer CoreEntitiesContainer;

        protected readonly CoreEntitiesMembershipProvider CoreEntitiesMembershipProvider;
        protected BaseApiController()
        {
            CoreEntitiesContainer = new CoreEntitiesContainer();
            CoreEntitiesContainer.ContextOptions.LazyLoadingEnabled = false;
            CoreEntitiesMembershipProvider = new CoreEntitiesMembershipProvider(CoreEntitiesContainer);
        }

        protected void SaveWithRetry()
        {
            try
            {
                CoreEntitiesContainer.SaveChanges();
            }
            catch (Exception)
            {
                try
                {
                    //try one more time
                    CoreEntitiesContainer.SaveChanges();
                }
                catch (Exception)
                {
                    throw Request.NotSaving();
                }
            }
        }
    }
}