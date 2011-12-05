using ReactiveUI.Xaml;

namespace FoundOps.Common.Silverlight.UI.ViewModels
{
    public interface ISaveDiscardChangesCommands
    {
        IReactiveCommand DiscardCommand { get; }
        IReactiveCommand SaveCommand { get; }
    }
}
