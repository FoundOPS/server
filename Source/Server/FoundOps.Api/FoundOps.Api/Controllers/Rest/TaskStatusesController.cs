using FoundOps.Api.Tools;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using System;
using System.Data.Entity;
using System.Linq;
using TaskStatus = FoundOps.Api.Models.TaskStatus;

namespace FoundOps.Api.Controllers.Rest
{
    public class TaskStatusesController : BaseApiController
    {
        /// <summary>
        /// Get task statuses for a business account
        /// </summary>
        /// <param name="roleId">The role</param>
        public IQueryable<TaskStatus> Get(Guid roleId)
        {
            var currentBusinessAccount = CoreEntitiesContainer.Owner(roleId, new[] { RoleType.Administrator, RoleType.Regular, RoleType.Mobile }).Include(ba => ba.TaskStatuses).FirstOrDefault();
            if (currentBusinessAccount == null)
                throw Request.NotAuthorized();

            var statuses = currentBusinessAccount.TaskStatuses.Select(TaskStatus.ConvertModel).AsQueryable();
            return statuses;
        }

        /// <summary>
        /// Insert a new task status
        /// REQUIRES: Admin access to role
        /// </summary>
        /// <param name="roleId">The role</param>
        /// <param name="taskStatus">The status</param>
        public void Post(Guid roleId, TaskStatus taskStatus)
        {
            var businessAccount = CoreEntitiesContainer.Owner(roleId, new[] { RoleType.Administrator }).Include(ba => ba.TaskStatuses).FirstOrDefault();
            if (businessAccount == null)
                throw Request.NotAuthorized();

            var status = TaskStatus.CreateFromModel(taskStatus);

            status.CreatedDate = DateTime.UtcNow;
            status.LastModifyingUserId = CoreEntitiesContainer.CurrentUserAccount().Id;

            businessAccount.TaskStatuses.Add(status);
            SaveWithRetry();
        }

        /// <summary>
        /// Update a task status
        /// REQUIRES: Admin access to role
        /// </summary>
        /// <param name="roleId">The role</param>
        /// <param name="taskStatus">The status</param>
        public void Put(Guid roleId, TaskStatus taskStatus)
        {
            var businessAccount = CoreEntitiesContainer.Owner(roleId, new[] { RoleType.Administrator }).Include(ba => ba.TaskStatuses).FirstOrDefault();
            if (businessAccount == null)
                throw Request.NotAuthorized();

            var original = CoreEntitiesContainer.TaskStatuses.FirstOrDefault(ts => ts.Id == taskStatus.Id);
            if (original == null)
                throw Request.NotFound();

            //update the status
            original.Color = taskStatus.Color;
            original.DefaultTypeInt = taskStatus.DefaultTypeInt;
            original.Name = taskStatus.Name;
            original.RemoveFromRoute = taskStatus.RemoveFromRoute;
            original.LastModified = DateTime.UtcNow;
            original.LastModifyingUserId = CoreEntitiesContainer.CurrentUserAccount().Id;

            SaveWithRetry();
        }

        /// <summary>
        /// Delete a task status
        /// </summary>
        /// <param name="roleId">The role</param>
        /// <param name="id">The task status id</param>
        public void Delete(Guid roleId, Guid id)
        {
            var businessAccount = CoreEntitiesContainer.Owner(roleId, new[] { RoleType.Administrator }).Include(ba => ba.TaskStatuses).FirstOrDefault();
            if (businessAccount == null)
                throw Request.NotAuthorized();

            var status = CoreEntitiesContainer.TaskStatuses.FirstOrDefault(ts => ts.Id == id);
            if (status == null)
                throw Request.NotFound();

            CoreEntitiesContainer.DeleteObject(status);

            SaveWithRetry();
        }
    }
}
