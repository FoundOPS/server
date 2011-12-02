using System;

//The available infinite accordion messages for use.
namespace FoundOps.Common.Silverlight.UI.Controls.InfiniteAccordion
{
    /// <summary>
    /// The strategy for moving in the InfiniteAccordion
    /// </summary>
    public enum MoveStrategy
    {
        /// <summary>
        /// Adds the context to the existing contexts.
        /// </summary>
        AddContextToExisting,
        /// <summary>
        /// Clear the context, then move to the DetailsView.
        /// </summary>
        StartFresh,
        /// <summary>
        /// Remove the contexts in front of this context. If this context does not exist, start fresh.
        /// </summary>
        MoveBackwards
    }

    /// <summary>
    /// A message to trigger moving to a typeOfDetailsView
    /// </summary>
    public class MoveToDetailsViewMessage
    {
        public string TypeOfDetailsView { get; private set; }
        public MoveStrategy Strategy { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveToDetailsViewMessage"/> class.
        /// </summary>
        /// <param name="typeOfDetailsView">The type of details view to move to.</param>
        /// <param name="moveStrategy">The move strategy.</param>
        public MoveToDetailsViewMessage(Type typeOfDetailsView, MoveStrategy moveStrategy)
            : this(typeOfDetailsView.ToString(), moveStrategy) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveToDetailsViewMessage"/> class.
        /// </summary>
        /// <param name="typeOfDetailsView">The type of details view.</param>
        /// <param name="moveStrategy">The move strategy.</param>
        public MoveToDetailsViewMessage(string typeOfDetailsView, MoveStrategy moveStrategy)
        {
            TypeOfDetailsView = typeOfDetailsView;
            Strategy = moveStrategy;
        }
    }

    /// <summary>
    /// Sent whenever the context provider changed. For example: the display switches to Clients, the ContextProvider would now be ClientsVM.
    /// </summary>
    public class ContextProviderChangedMessage
    {
        /// <summary>
        /// Gets the new context provider.
        /// </summary>
        public IProvideContext CurrentContextProvider { get; private set; }

        public ContextProviderChangedMessage(IProvideContext contextProvider)
        {
            CurrentContextProvider = contextProvider;
        }
    }

    /// <summary>
    /// A message to add a context to the InfiniteAccordion
    /// </summary>
    public class AddContextMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddContextMessage"/> class.
        /// </summary>
        /// <param name="context">The context to add.</param>
        public AddContextMessage(object context)
        {
            Context = context;
        }

        public object Context { get; set; }
    }

    /// <summary>
    /// A message to remove a context from the InfiniteAccordion
    /// </summary>
    public class RemoveContextMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveContextMessage"/> class.
        /// </summary>
        /// <param name="context">The context to remove.</param>
        public RemoveContextMessage(object context)
        {
            Context = context;
        }

        public object Context { get; set; }
    }
}
