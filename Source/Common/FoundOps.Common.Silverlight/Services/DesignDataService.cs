using System;
using System.ComponentModel;
using System.ServiceModel.DomainServices.Client;
using FoundOps.Common.Silverlight.MVVM.Messages;
using FoundOps.Common.Silverlight.MVVM.Services.Interfaces;

namespace FoundOps.Common.Silverlight.MVVM.Services
{
    public abstract class DesignDataService : IDataService 
    {
        public bool HasChanges
        {
            get { return false; }
            set { throw new NotImplementedException(); }
        }

        public event EventHandler<HasChangesEventArgs> NotifyHasChanges;
        public void CancelChanges()
        {
        }

        public void Save(Action<SubmitOperation> submitCallback, object state)
        {
            submitCallback(null);
        }

        #region Implementation of IDataService

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsLoading
        {
            get { return false; }
        }

        #endregion

        #region Implementation of IRejectAllChanges

        public void RejectAllChanges()
        {
        }

        #endregion
    }
}
