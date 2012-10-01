using FoundOps.Common.NET;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TaskStatus = FoundOPS.API.Models.TaskStatus;

namespace FoundOPS.API.Api
{
    public class TaskStatusesController : ApiController
    {
        private readonly CoreEntitiesContainer _coreEntitiesContainer;

        public TaskStatusesController()
        {
            _coreEntitiesContainer = new CoreEntitiesContainer();
            _coreEntitiesContainer.ContextOptions.LazyLoadingEnabled = false;
        }

        [AcceptVerbs("GET", "POST")]
        public IQueryable<TaskStatus> GetStatuses(Guid roleId)
        {
            var currentBusinessAccount = _coreEntitiesContainer.Owner(roleId).Include(ba => ba.TaskStatuses).FirstOrDefault();

            if (currentBusinessAccount == null)
                ExceptionHelper.ThrowNotAuthorizedBusinessAccount();

            var statuses = currentBusinessAccount.TaskStatuses.OrderBy(s => s.Name)
                .Select(TaskStatus.ConvertModel).AsQueryable();

            return statuses;
        }

        [AcceptVerbs("POST")]
        public HttpResponseMessage InsertTaskStatus(TaskStatus taskStatus, Guid roleId)
        {
            var status = TaskStatus.CreateFromModel(taskStatus);

            var businessAccount = _coreEntitiesContainer.Owner(roleId, new[] { RoleType.Administrator }).Include(ba => ba.TaskStatuses).FirstOrDefault();

            if (businessAccount == null)
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "User does not have Admin abilities");

            businessAccount.TaskStatuses.Add(status);

            try
            {
                _coreEntitiesContainer.SaveChanges();
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.NotAcceptable, "There was an error while saving. Please check your inputs");
            }

            return Request.CreateResponse(HttpStatusCode.Accepted);
        }

        [AcceptVerbs("POST")]
        public HttpResponseMessage UpdateTaskStatus(TaskStatus taskStatus, Guid roleId)
        {
            var businessAccount = _coreEntitiesContainer.Owner(roleId, new[] { RoleType.Administrator }).Include(ba => ba.TaskStatuses).FirstOrDefault();

            if (businessAccount == null)
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "User does not have Admin abilities");

            var original = _coreEntitiesContainer.TaskStatuses.FirstOrDefault(ts => ts.Id == taskStatus.Id);

            if (original == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "You are trying to update an object the does not exist yet!");

            //update the original
            original.Color = taskStatus.Color;
            original.DefaultTypeInt = taskStatus.DefaultTypeInt;
            original.Name = taskStatus.Name;
            original.RemoveFromRoute = taskStatus.RemoveFromRoute;

            try
            {
                _coreEntitiesContainer.SaveChanges();
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.NotAcceptable, "There was an error while saving. Please check your inputs");
            }

            return Request.CreateResponse(HttpStatusCode.Accepted);
        }

        [AcceptVerbs("POST")]
        public HttpResponseMessage DeleteTaskStatus(TaskStatus taskStatus, Guid roleId)
        {
            var businessAccount = _coreEntitiesContainer.Owner(roleId, new[] { RoleType.Administrator }).Include(ba => ba.TaskStatuses).FirstOrDefault();

            if (businessAccount == null)
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "User does not have Admin abilities");

            var status = TaskStatus.CreateFromModel(taskStatus);

            _coreEntitiesContainer.DetachExistingAndAttach(status);
            _coreEntitiesContainer.TaskStatuses.DeleteObject(status);

            try
            {
                _coreEntitiesContainer.SaveChanges();
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.NotAcceptable, "There was an error while saving. Please check your inputs");
            }

            return Request.CreateResponse(HttpStatusCode.Accepted);
        }
    }
}
