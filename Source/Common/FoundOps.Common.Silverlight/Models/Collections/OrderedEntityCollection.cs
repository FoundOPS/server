using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.ServiceModel.DomainServices.Client;
using FoundOps.Common.Composite.Tools;

namespace FoundOps.Common.Silverlight.Tools
{
    /// <summary>
    /// Keeps an ordered representation of a entity collection.
    /// </summary>
    /// <typeparam name="TEntity">The entity collection's type</typeparam>
    public class OrderedEntityCollection<TEntity> : ObservableCollection<TEntity> where TEntity : Entity
    {
        private readonly EntityCollection<TEntity> _entityCollection;
        private readonly string _indexPropertyName;
        private readonly bool _zeroIndexed;
        private bool _thisMakingChangesToEntityCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedEntityCollection&lt;TEntity&gt;"/> class.
        /// </summary>
        /// <param name="entityCollection">The entity collection to order.</param>
        /// <param name="indexPropertyName">Name of the indexing property. Ex. OrderInRoute</param>
        /// <param name="zeroIndexed">A flag signaling if the collection is zero or one indexed.</param>
        public OrderedEntityCollection(EntityCollection<TEntity> entityCollection, string indexPropertyName, bool zeroIndexed)
        {
            _entityCollection = entityCollection;
            _indexPropertyName = indexPropertyName;
            _zeroIndexed = zeroIndexed;

            //Listen to external changes of the entity collection
            _entityCollection.EntityAdded += EntityCollectionEntityAdded;
            _entityCollection.EntityRemoved += EntityCollectionEntityRemoved;

            int index = zeroIndexed ? 0 : 1;

            //Add all of the items to this in order of the _indexPropertyName
            foreach (var element in entityCollection.OrderBy(e => e.GetProperty<int>(_indexPropertyName)))
            {
                this.Add(element);

                //Update the index in case there are skipped numbers
                //This ensure the correctness of the ObservableCollection
                element.SetProperty(_indexPropertyName, index);
                index++;
            }
        }

        //This method takes care of entities added to the EntityCollection not through this class
        //It will add the entity to the ordered collection based on the index
        void EntityCollectionEntityAdded(object sender, EntityCollectionChangedEventArgs<TEntity> e)
        {
            var entityToAdd = e.Entity;

            //If an entity was added directly to this it has already be handled
            if (_thisMakingChangesToEntityCollection)
            {
                _thisMakingChangesToEntityCollection = false;
                return;
            }

            //Add the entity to this backing collection
            base.Add(entityToAdd);

            //Reorganize the backing collection
            var ordered = this.OrderBy(o => o.GetProperty<int>(_indexPropertyName)).ToArray();
            
            base.Clear();

            foreach (var orderedItem in ordered)
                base.Add(orderedItem);
        }

        //This method takes care of entities removed from the EntityCollection not through this class
        void EntityCollectionEntityRemoved(object sender, EntityCollectionChangedEventArgs<TEntity> e)
        {
            var entityToRemove = e.Entity;

            //If an entity was removed directly from this it has already be handled
            if (_thisMakingChangesToEntityCollection)
            {
                _thisMakingChangesToEntityCollection = false;
                return;
            }

            //Remove the entity from this backing collection
            base.Remove(entityToRemove);

            //Go through each entity that was after this element and update it's index to one less than before
            var entitiesToUpdate = this.Where(el => el.GetProperty<int>(_indexPropertyName) > entityToRemove.GetProperty<int>(_indexPropertyName));

            foreach (var entityToUpdate in entitiesToUpdate)
                entityToUpdate.SetProperty(_indexPropertyName, entityToUpdate.GetProperty<int>(_indexPropertyName) - 1);
        }

        public new void Insert(int index, TEntity itemToInsert)
        {
            //Update the inserted entity's index property
            itemToInsert.SetProperty(_indexPropertyName, _zeroIndexed ? index : index + 1);

            //Go through each entity that was on or after the index and update it's index to one more than before
            var entitiesToUpdate = this.Where(entity => entity.GetProperty<int>(_indexPropertyName) >= itemToInsert.GetProperty<int>(_indexPropertyName));

            foreach (var entityToUpdate in entitiesToUpdate)
                entityToUpdate.SetProperty(_indexPropertyName, entityToUpdate.GetProperty<int>(_indexPropertyName) + 1);

            //Add the entity to this backing collection
            base.Insert(index, itemToInsert);

            _thisMakingChangesToEntityCollection = true; //Prevent an incorrect operation

            //Add the entity to the entity collection
            _entityCollection.Add(itemToInsert);
        }

        public new void Add(TEntity item)
        {
            this.Insert(this.Count, item);
        }

        public new void Clear()
        {
            base.Clear();

            foreach (var entity in _entityCollection.ToArray())
            {
                _thisMakingChangesToEntityCollection = true; //Prevent an incorrect operation
                _entityCollection.Remove(entity);
            }
        }

        public new bool Remove(TEntity item)
        {
            if (!Contains(item))
                return false;

            //Go through each entity that was on or after the removed entity's index and update it's index to one less than before
            var entitiesToUpdate = this.Where(entity => entity.GetProperty<int>(_indexPropertyName) > item.GetProperty<int>(_indexPropertyName));

            foreach (var entityToUpdate in entitiesToUpdate)
                entityToUpdate.SetProperty(_indexPropertyName, entityToUpdate.GetProperty<int>(_indexPropertyName) - 1);

            base.Remove(item);

            _thisMakingChangesToEntityCollection = true; //Prevent an incorrect operation
            _entityCollection.Remove(item);

            return true;
        }

        public new void RemoveAt(int index)
        {
            if (index >= this.Count())
                throw new ArgumentOutOfRangeException();

            var item = this.ElementAt(index);
            this.Remove(item);
        }
    }
}
