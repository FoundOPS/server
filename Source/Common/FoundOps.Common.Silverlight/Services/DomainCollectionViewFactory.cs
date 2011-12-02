using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ServiceModel.DomainServices.Client;
using Microsoft.Windows.Data.DomainServices;

namespace FoundOps.Common.Silverlight.Services
{
    /// <summary>
    /// Creates the DomainCollectionView. Pages data on Load based on View. Allows DesignTime Data.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public class DomainCollectionViewFactory<TEntity> where TEntity : Entity
    {
        public DomainCollectionView<TEntity> View { get; private set; }

        private readonly EntityQuery<TEntity> _query;
        private readonly DomainContext _context;
        private readonly Action<LoadOperation<TEntity>> _loadCompleted;
        private readonly EntityList<TEntity> _entityList;
        public readonly NotifiableDomainCollectionViewLoader<TEntity> _loader;

        private LoadOperation<TEntity> _lastLoadOperation;

        public DomainCollectionViewFactory(IEnumerable<TEntity> source)
        {
            var loader = new NotifiableLoader();
            View = new DomainCollectionView<TEntity>(loader, source);

            ((INotifyCollectionChanged)View.SortDescriptions).CollectionChanged +=
                (sender, e) => loader.RaiseLoadCompleted(); //Fix Sorting

            View.Refresh();
        }

        public DomainCollectionViewFactory(IEnumerable<TEntity> source, NotifiableLoader loader)
        {
            View = new DomainCollectionView<TEntity>(loader, source);

            ((INotifyCollectionChanged)View.SortDescriptions).CollectionChanged +=
                (sender, e) => loader.RaiseLoadCompleted(); //Fix Sorting

            View.Refresh();
        }

        public DomainCollectionViewFactory(EntityQuery<TEntity> query, EntitySet<TEntity> entitySet, DomainContext context, Action<LoadOperation<TEntity>> loadCompleted)
        {
            _query = query;
            _context = context;
            _loadCompleted = loadCompleted;

            _loader = new NotifiableDomainCollectionViewLoader<TEntity>(Load, CancelCurrentLoad, OnLoadCompleted);
            _entityList = new EntityList<TEntity>(entitySet);

            View = new DomainCollectionView<TEntity>(_loader, _entityList);

            ((INotifyCollectionChanged)View.SortDescriptions).CollectionChanged +=
                (sender, e) => _loader.RaiseLoadCompleted(); //Fix Sorting

            View.Refresh();

            _loader.IsBusy = true;
        }

        ///<summary>
        /// Builds a DomainCollectionView from an EntityList
        ///</summary>
        ///<param name="source">The source to wrap</param>
        ///<typeparam name="T">The type of Entity</typeparam>
        ///<returns></returns>
        public static DomainCollectionView<T> GetDomainCollectionView<T>(IEnumerable<T> source) where T : Entity
        {
            return new DomainCollectionViewFactory<T>(source).View;
        }

        private LoadOperation<TEntity> Load()
        {
            _lastLoadOperation = this._context.Load(_query.PageBy(View));
            return _lastLoadOperation;
        }

        public void CancelCurrentLoad()
        {
            if (_lastLoadOperation != null && !_lastLoadOperation.IsComplete && !_lastLoadOperation.IsCanceled)
            {
                _lastLoadOperation.Cancel();
            }
        }

        private void OnLoadCompleted(LoadOperation<TEntity> loadOperation)
        {
            if (loadOperation.HasError)
            {
                // TODO: handle errors
                loadOperation.MarkErrorAsHandled();
            }
            else if (!loadOperation.IsCanceled)
            {
                this._entityList.Source = loadOperation.Entities;

                if (loadOperation.TotalEntityCount != -1)
                {
                    this.View.SetTotalItemCount(loadOperation.TotalEntityCount);
                }

                if (_loadCompleted != null)
                    _loadCompleted(loadOperation);
            }

            if (_loader != null)
                _loader.IsBusy = false;
        }
    }


    public class NotifiableDomainCollectionViewLoader<TEntity> : DomainCollectionViewLoader<TEntity> where TEntity : Entity
    {
        private readonly Action _cancel;

        public NotifiableDomainCollectionViewLoader(Func<LoadOperation<TEntity>> load, Action cancel)
            : base(load)
        {
            _cancel = cancel;
        }

        public NotifiableDomainCollectionViewLoader(Func<LoadOperation<TEntity>> load, Action cancel, Action<LoadOperation<TEntity>> onLoadCompleted)
            : base(load, onLoadCompleted)
        {
            _cancel = cancel;
        }

        public void RaiseLoadCompleted()
        {
            this.OnLoadCompleted(new AsyncCompletedEventArgs(null, false, null));
        }

        public void CancelLoad()
        {
            _cancel();
        }
    }

    public class NotifiableLoader : CollectionViewLoader
    {
        public override bool CanLoad { get { return true; } }
        public override void Load(object userState)
        {
            this.OnLoadCompleted(
              new AsyncCompletedEventArgs(null, false, userState));
        }

        public void RaiseLoadCompleted()
        {
            this.OnLoadCompleted(new AsyncCompletedEventArgs(null, false, null));
        }
    }
}
