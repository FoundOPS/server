using System;
using ReactiveUI;
using System.Linq;
using ReactiveUI.Xaml;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using FoundOps.Common.Tools;
using System.Collections.Generic;
using FoundOps.SLClient.Data.Services;
using Microsoft.Windows.Data.DomainServices;
using FoundOps.Core.Models.CoreEntities.M2M4Ria;
using System.ServiceModel.DomainServices.Client;

namespace FoundOps.SLClient.Data.ViewModels
{
    /// <summary>
    /// A view model which encapsulates some standard selected entities collection logic/properties for an entity type.
    /// On top of the <see cref="CoreSelectedEntitiesCollectionVM&lt;TEntity&gt;"/> Properties and Commands. This also includes:
    /// Properties: SelectedEntities, NotSelectedEntities
    /// Commands: AddExistingEntity
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// </summary>
    public class CoreSelectedEntitiesCollectionVM<TEntity> : CoreEntityCollectionVM<TEntity> where TEntity : Entity
    {
        #region Public

        /// <summary>
        /// The selected entities.
        /// </summary>
        public IEntityCollection<TEntity> SelectedEntities { get; private set; }

        private readonly Subject<IEnumerable<TEntity>> _notSelectedEntitiesObservable = new Subject<IEnumerable<TEntity>>();
        public ObservableAsPropertyHelper<IEnumerable<TEntity>> _notSelectedEntities;
        /// <summary>
        /// Gets the not selected entities.
        /// </summary>
        public IEnumerable<TEntity> NotSelectedEntities { get { return _notSelectedEntities.Value; } }

        public ObservableAsPropertyHelper<IEnumerable<TEntity>> _loadedEntities;
        /// <summary>
        /// Gets the loaded entities.
        /// </summary>
        public IEnumerable<TEntity> LoadedEntities { get { return _loadedEntities.Value; } }

        //Commands

        /// <summary>
        /// A command to add an existing entity to SelectedEntities.
        /// </summary>
        public IReactiveCommand AddExistingEntityCommand { get; private set; }

        #endregion

        //Locals

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreSelectedEntitiesCollectionVM&lt;TEntity&gt;"/> class.
        /// </summary>
        /// <param name="selectedEntities">The selected entities collection.</param>
        /// <param name="loadedEntities">An Observable of the loaded entities.</param>
        public CoreSelectedEntitiesCollectionVM(IEntityCollection<TEntity> selectedEntities, IObservable<EntityList<TEntity>> loadedEntities) 
            : base(false)
        {
            SelectedEntities = selectedEntities;

            //Setup the LoadedEntities
            _loadedEntities = loadedEntities.ToProperty(this, x => x.LoadedEntities);

            //Setup NotSelectedEntities
            _notSelectedEntities = _notSelectedEntitiesObservable.ToProperty(this, x => x.NotSelectedEntities);

            //NotSelectedEntities are the loaded entities without the SelectedEntities
            loadedEntities.FromCollectionChangedOrSet().Merge(SelectedEntities.FromCollectionChanged().Select(a => LoadedEntities)) //Whenever loadedEntities or SelectedEntities changes
               .Select(loadEnts => loadEnts.Except(SelectedEntities))  //Select the loadedEntities except the SelectedEntities
               .Subscribe(_notSelectedEntitiesObservable);

            #region Register Commands

            //Setup AddExistingEntityCommand

            var oneExistingEntity =
                loadedEntities.FromCollectionChangedOrSet().Select(le => le.Count > 0);

            //AddExistingEntityCommand can happen anytime there is at least 1 existing entity
            AddExistingEntityCommand = new ReactiveCommand(oneExistingEntity);

            AddExistingEntityCommand.Subscribe(existingEntityToAdd => SelectedEntities.Add((TEntity)existingEntityToAdd));

            //Override default Delete logic. We don't actually want to delete, we want to remove from the SelectedEntities
            var canDelete = this.WhenAny(x => x.SelectedEntity, selectedEntity => selectedEntity.Value != null);

            DeleteCommand = new ReactiveCommand(canDelete);

            DeleteCommand.Throttle(TimeSpan.FromMilliseconds(500)).ObserveOnDispatcher().Subscribe(param =>
            {
                var entityToRemove = SelectedEntity;

                //Clear the SelectedEntity
                SelectedEntity = null;

                //Remove from SelectedEntities
                this.SelectedEntities.Remove(entityToRemove);

                //If the Entity is new delete it
                if (entityToRemove.EntityState == EntityState.New)
                    Context.EntityContainer.GetEntitySet(typeof(TEntity)).Remove(entityToRemove);
            });

            #endregion
        }
    }
}
