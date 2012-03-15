using FoundOps.Common.Silverlight.Interfaces;

//Partial class must be part of same namespace
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class ClientTitle : IReject, IRaiseValidationErrors
    {
        #region Logic

        #region Public Methods

        /// <summary>
        /// Rejects the changes of this individual entity.
        /// </summary>
        public void Reject()
        {
            this.RejectChanges();
        }

        /// <summary>
        /// Raises validation errors on this entity.
        /// </summary>
        public void RaiseValidationErrors()
        {
            this.BeginEdit();
            this.EndEdit();
        }

        #endregion

        #endregion
    }
}
