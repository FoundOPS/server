using FoundOps.Common.Silverlight.MVVM.Interfaces;
using FoundOps.Common.Silverlight.MVVM.Validation;

namespace FoundOps.Core.Models.CoreEntities
{
    public partial class ClientTitle : IReject, IRaiseValidationErrors
    {
        public void Reject()
        {
            this.RejectChanges();
        }

        public void RaiseValidationErrors()
        {
            this.BeginEdit();
            this.EndEdit();
        }
    }
}
