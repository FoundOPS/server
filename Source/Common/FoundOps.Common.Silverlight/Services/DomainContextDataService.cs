using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using FoundOps.Common.Silverlight.MVVM.Models;
using FoundOps.Common.Silverlight.MVVM.Messages;
using System.ServiceModel.DomainServices.Client;
using FoundOps.Common.Silverlight.MVVM.Services.Interfaces;
using Microsoft.Windows.Data.DomainServices;

namespace ScalableCourier.Client.CommonResources.MVVM.Services
{
    public class DomainContextDataService<TDomainContext> : ThreadableNotifiableObject, IDataService where TDomainContext : DomainContext
    {
        [ImportingConstructor]
        public DomainContextDataService(TDomainContext context)
        {
            Context = context;
        }

        private TDomainContext _context;
        public TDomainContext Context
        {
            get { return _context; }
            set
            {
                _context = value;
                Context.PropertyChanged += ContextPropertyChanged;
            }
        }

        public bool HasChanges
        {
            get { return Context.HasChanges; }
            set { throw new EntryPointNotFoundException("This is tracked automatically"); }
        }

        public event EventHandler<HasChangesEventArgs> NotifyHasChanges;

        #region Implementation of IDataService

        public bool IsLoading { get { return Context.IsLoading; } }

        #endregion

        #region Implementation of IRejectAllChanges

        public void RejectAllChanges()
        {
            Context.RejectChanges();
        }

        #endregion

        public void Save(Action<SubmitOperation> submitCallback, object state)
        {
            if (Context.HasChanges)
            {
                //foreach(var entity in this.Context.EntityContainer.GetChanges())
                //{
                //    if (!entity.ValidateDisplayErrors())
                //        return;
                //}
                Context.SubmitChanges(submitCallback, state);
            }
        }
        public void Save(DomainCollectionView view, Action<SubmitOperation> submitCallback, object state)
        {
            view.Commit();
            Save(submitCallback, state);
        }

        public void CancelChanges()
        {
            Context.RejectChanges();
        }
        public void CancelChanges(DomainCollectionView view)
        {
            view.Commit();
            CancelChanges();
        }

        private void ContextPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (NotifyHasChanges != null)
            {
                NotifyHasChanges(this, new HasChangesEventArgs() { HasChanges = Context.HasChanges });
            }

            if (e.PropertyName == "HasChanges")
                RaisePropertyChanged("HasChanges");

            if (e.PropertyName == "IsLoading")
                RaisePropertyChanged("IsLoading");
        }

        public void LoadSingle<TEntity>(EntityQuery<TEntity> query, Action<TEntity> callback) where TEntity : Entity
        {
            Context.Load(query, loadOperation =>
                                    {
                                        if (loadOperation.HasError) return; //TODO Setup error callback
                                        callback(loadOperation.Entities.FirstOrDefault());
                                    }, null);
        }

        public void LoadCollection<TEntity>(EntityQuery<TEntity> query, Action<IEnumerable<TEntity>> callback) where TEntity : Entity
        {
            Context.Load(query, loadOperation =>
            {
                if (loadOperation.HasError) return; //TODO Setup error callback
                callback(loadOperation.Entities);
            }, null);
        }


        /// <summary>
        ///  Return an EntityList so List.Add and List.Remove are propogated back to the context
        /// </summary>
        public void LoadCollectionEntityList<TEntity>(EntityQuery<TEntity> query, EntitySet<TEntity> entitySet, Action<ObservableCollection<TEntity>> callback) where TEntity : Entity
        {
            Context.Load(query, loadOperation =>
            {
                if (loadOperation.HasError) return; //TODO Setup error callback
                callback(new EntityList<TEntity>(entitySet, loadOperation.Entities.OfType<TEntity>()));
            }, null);
        }
    }
}
