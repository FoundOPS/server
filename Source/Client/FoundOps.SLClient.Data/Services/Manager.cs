using MEFedMVVM.ViewModelLocator;

namespace FoundOps.SLClient.Data.Services
{
    /// <summary>
    /// An access point for the manager classes.
    /// </summary>
    public static class Manager
    {
        private static ContextManager _contextManager;
        /// <summary>
        /// Gets the ContextManager.
        /// </summary>
        public static ContextManager Context
        {
            get
            {
                return _contextManager ??
                       (_contextManager = ViewModelRepository.Instance.Resolver.Container.GetExportedValue<ContextManager>());
            }
        }

        private static DataManager _dataManager;
        /// <summary>
        /// Gets the DataManager.
        /// </summary>
        public static DataManager Data
        {
            get
            {
                return _dataManager ??
                       (_dataManager = ViewModelRepository.Instance.Resolver.Container.GetExportedValue<DataManager>());
            }
        }
    }
}
