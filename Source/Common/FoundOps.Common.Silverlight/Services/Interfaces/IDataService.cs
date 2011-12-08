using System.ComponentModel;

namespace FoundOps.Common.Silverlight.MVVM.Services.Interfaces
{
    public interface IDataService : INotifyPropertyChanged, ISaveChanges, IRejectAllChanges
    {
        bool IsLoading { get; }
    }

    public interface IRejectAllChanges
    {
        void RejectAllChanges();
    }
}
