using System;
using System.ServiceModel.DomainServices.Client;

namespace FoundOps.Common.Silverlight.MVVM.Services.Interfaces
{
    public interface ISaveChanges
    {
        void Save(Action<SubmitOperation> submitCallback, object state);
    }
}
