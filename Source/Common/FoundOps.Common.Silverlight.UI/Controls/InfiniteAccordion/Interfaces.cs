using System;
using System.Windows;
using System.ComponentModel;
using GalaSoft.MvvmLight.Command;

namespace FoundOps.Common.Silverlight.UI.Controls.InfiniteAccordion
{
    /// <summary>
    /// Implemented by the main Entity ViewModels which provide context to the InfiniteAccordion.
    /// </summary>
    public interface IProvideContext : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the currently selected context. (Ex. If Client "Thirsty Bear" is in the DetailsView, it would be the SelectedContext of ClientsVM) 
        /// </summary>
        object SelectedContext { get; }

        /// <summary>
        /// An Observable of the selected context.
        /// </summary>
        IObservable<object> SelectedContextObservable { get; }

        /// <summary>
        /// A command which takes the InfiniteAccordion to this ViewModel's object type's details view.
        /// </summary>
        RelayCommand MoveToDetailsView { get; }

        /// <summary>
        /// Gets the object type provided.
        /// </summary>
        Type ObjectTypeProvided { get; }
    }

    /// <summary>
    /// Implemented by controls to Display a certain ObjectType
    /// </summary>
    public interface IObjectTypeDisplay
    {
        /// <summary>
        /// Gets the context provider view model for this object.
        /// </summary>
        IProvideContext ContextProvider { get; }

        /// <summary>
        /// The control displaying the ObjectType.
        /// </summary>
        FrameworkElement Display { get; }

        /// <summary>
        /// Gets the object type to display.
        /// </summary>
        string ObjectTypeToDisplay { get; }
    }
}
