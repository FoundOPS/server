using MEFedMVVM.ViewModelLocator;

namespace FoundOps.SLClient.Data.Services
{
    /// <summary>
    /// An access point for the manager classes.
    /// </summary>
    public static class Manager
    {
        /// <summary>
        /// Gets the ContextManager.
        /// </summary>
        public static ContextManager Context { get { return ViewModelRepository.Instance.Resolver.Container.GetExportedValue<ContextManager>(); } }

        /// <summary>
        /// Gets the DataManager.
        /// </summary>
        public static DataManager Data {get{ return ViewModelRepository.Instance.Resolver.Container.GetExportedValue<DataManager>(); } }
    }
}
