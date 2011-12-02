using System.Linq;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

namespace FoundOps.Core.Navigator.Loader
{
    public interface IAggregateCatalogService
    {
        AggregateCatalog AggregateCatalog { get; }
        CompositionContainer Container { get; }
        void AddCatalog(ComposablePartCatalog composablePartCatalog);
    }
    [Export(typeof(IAggregateCatalogService))]
    public class AggregateCatalogService : IAggregateCatalogService
    {
        public AggregateCatalog AggregateCatalog { get; private set; }
        public CompositionContainer Container { get; private set; }

        public AggregateCatalogService(AggregateCatalog aggregateCatalog, CompositionContainer container)
        {
            this.AggregateCatalog = aggregateCatalog;
            Container = container;
        }

        public void AddCatalog(ComposablePartCatalog catalogToAdd)
        {
            //If there is a duplicate catalog do not add this catalog

            if (this.AggregateCatalog.Catalogs.Any(existingCatalog => 
                existingCatalog.Parts.All(existingPart => catalogToAdd.Parts.Any(newCatalogPart => newCatalogPart.ToString() == existingPart.ToString()))))
                return;

            AggregateCatalog.Catalogs.Add(catalogToAdd);
        }
    }
}

