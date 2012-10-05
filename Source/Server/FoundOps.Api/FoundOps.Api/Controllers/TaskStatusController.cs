using System;
using System.Data;
using System.Data.Objects;
using System.Linq;
using System.ServiceModel.DomainServices.EntityFramework;
using FoundOps.Common.NET;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using TaskStatus = FoundOPS.API.Models.TaskStatus;
using System.Web.Http;

namespace FoundOPS.API.Controllers
{
#if !DEBUG
    [Authorize]
#endif

    /// <summary>
    /// An API controller that exposes the Task Status'
    /// </summary>
    public class TaskStatusController : ApiController
    {
        private readonly CoreEntitiesContainer _coreEntitiesContainer = new CoreEntitiesContainer();

        #region INSERT

        public void InsertTaskStatus(TaskStatus taskStatus)
        {
            var status = TaskStatus.ConvertFromModel(taskStatus);

            _coreEntitiesContainer.TaskStatus.AddObject(status);

            _coreEntitiesContainer.SaveChanges();
        }

        #endregion

        #region UPDATE

        public void UpdateTaskStatus(TaskStatus taskStatus)
        {
            var original = _coreEntitiesContainer.TaskStatus.FirstOrDefault(ts => ts.Id == taskStatus.Id);

            if(original == null)
                throw new UpdateException("This is bad. Somehow you updated an object that doesn't exist yet...");

            var convertedTaskStatus = TaskStatus.ConvertFromModel(taskStatus);

            original.BusinessAccountId = convertedTaskStatus.BusinessAccountId;
            original.Color = convertedTaskStatus.Color;
            original.DefaultTypeInt = convertedTaskStatus.DefaultTypeInt;
            original.Name = convertedTaskStatus.Name;
            original.RouteRequired = convertedTaskStatus.RouteRequired;

            if (convertedTaskStatus.RouteTasks != original.RouteTasks)
            {
                original.RouteTasks.Clear();

                foreach (var routeTask in convertedTaskStatus.RouteTasks)
                    original.RouteTasks.Add(routeTask);
            
            }
            _coreEntitiesContainer.SaveChanges();
        }

        #endregion

        #region DELETE

        public void DeleteTaskStatus(TaskStatus taskStatus)
        {
            var status = TaskStatus.ConvertFromModel(taskStatus);

            _coreEntitiesContainer.DetachExistingAndAttach(status);
            _coreEntitiesContainer.TaskStatus.DeleteObject(status);

            _coreEntitiesContainer.SaveChanges();
        }

        #endregion

        #region GET

        public IQueryable<TaskStatus> GetStatuses(Guid roleId)
        {
            var currentBusinessAccount = _coreEntitiesContainer.BusinessAccountOwnerOfRoleQueryable(roleId).FirstOrDefault();

            if (currentBusinessAccount == null)
                ExceptionHelper.ThrowNotAuthorizedBusinessAccount();

            return currentBusinessAccount.TaskStatuses.Select(TaskStatus.ConvertModel).AsQueryable();
        }

        #endregion
    }
}
