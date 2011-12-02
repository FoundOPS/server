using ReactiveUI.Xaml;

namespace FoundOps.Common.Silverlight.MVVM.VMs
{
    public interface IAddDeleteCommands
    {
        IReactiveCommand AddCommand { get; }
        IReactiveCommand DeleteCommand { get; }
    }
}
