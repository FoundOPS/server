using FoundOps.Common.Silverlight.Interfaces;

//Partial class must be part of same namespace
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
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
