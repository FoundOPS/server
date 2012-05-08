using System.Data;
using System.ServiceModel.DomainServices.EntityFramework;
using FoundOps.Common.NET;
using FoundOps.Core.Models.CoreEntities;
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
            var status = TaskStatus.ConvertModel(taskStatus);

            if (status.EntityState == EntityState.Detached)
                _coreEntitiesContainer.ObjectStateManager.ChangeObjectState(status, EntityState.Added);
            else
                _coreEntitiesContainer.TaskStatus.AddObject(status);       
        }

        #endregion

        #region UPDATE

        public void UpdateTaskStatus(TaskStatus taskStatus)
        {
            _coreEntitiesContainer.TaskStatus.AttachAsModified(TaskStatus.ConvertModel(taskStatus));
        }

        #endregion

        #region DELETE

        public void DeleteTaskStatus(TaskStatus taskStatus)
        {
            var status = TaskStatus.ConvertModel(taskStatus);

            _coreEntitiesContainer.DetachExistingAndAttach(status); 
            _coreEntitiesContainer.TaskStatus.DeleteObject(status);
        }

        #endregion
    }
}
