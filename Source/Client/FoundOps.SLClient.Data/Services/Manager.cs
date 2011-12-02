using FoundOps.Core.Navigator.Loader;
using System.ComponentModel.Composition;
using FoundOps.SLClient.Data.Services.Analytics;

namespace FoundOps.SLClient.Data.Services
{
    /// <summary>
    /// Imports all the manager classes
    /// </summary>
    public class Manager
    {
        /// <summary>
        /// Gets or sets the analytics.
        /// </summary>
        //[Import]
        public AnalyticsManager Analytics { get; set; }

        /// <summary>
        /// Gets or sets the context manager.
        /// </summary>
        //[Import]
        public ContextManager Context { get; set; }

        /// <summary>
        /// Gets or sets the aggregate catalog service.
        /// </summary>
        //[Import]
        public IAggregateCatalogService AggregateCatalogService { get; set; }

        /// <summary>
        /// Gets or sets the data manager.
        /// </summary>
        //[Import]
        public DataManager Data { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Manager"/> class.
        /// </summary>
        public Manager()
        {
            CompositionInitializer.SatisfyImports(this);

            //var contextManager = AggregateCatalogService.AggregateCatalog.GetExports(
            //   new ImportDefinition((exportDef) => exportDef.ContractName == typeof(ContextManager).ToString(),
            //                         typeof(ContextManager).ToString(), ImportCardinality.ExactlyOne, false, false)).ElementAt(0);

            //var part = contextManager.Item1.CreatePart();
            //var contextManagerValue = part.GetExportedValue(contextManager.Item2);
        }
    }
}
