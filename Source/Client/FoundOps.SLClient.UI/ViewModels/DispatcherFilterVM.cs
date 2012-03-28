﻿using System.ServiceModel.DomainServices.Client;
using FoundOps.Common.Silverlight.MVVM.Messages;
using FoundOps.Common.Silverlight.MVVM.Models;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.ViewModels;
using MEFedMVVM.ViewModelLocator;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Reactive.Linq;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// A VM for filtering various parts of the dispatcher
    /// </summary>
    [ExportViewModel("DispatcherFilterVM")]
    public class DispatcherFilterVM : DataFedVM
    {
        #region Public Properties

        private ObservableCollection<Region> _loadedRegions;
        /// <summary>
        /// Collection of loaded Regions for the ServiceProvider.
        /// </summary>
        public ObservableCollection<Region> LoadedRegions
        {
            get { return _loadedRegions; }
            private set
            {
                _loadedRegions = value;
                this.RaisePropertyChanged("LoadedRegions");
            }
        }

        private readonly ObservableCollection<EntityOption> _serviceTemplateOptions = new ObservableCollection<EntityOption>();
        /// <summary>
        /// Collection of loaded Regions for the ServiceProvider.
        /// </summary>
        public ObservableCollection<EntityOption> ServiceTemplateOptions { get { return _serviceTemplateOptions; } }

        private readonly ObservableCollection<EntityOption> _regionOptions = new ObservableCollection<EntityOption>();
        /// <summary>
        /// Collection of loaded Regions for the ServiceProvider.
        /// </summary>
        public ObservableCollection<EntityOption> RegionOptions { get { return _regionOptions; } }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DispatcherFilterVM"/> class.
        /// </summary>
        [ImportingConstructor]
        public DispatcherFilterVM()
        {
            SetupDataLoading();
        }

        private void SetupDataLoading()
        {
            //Setup the ServiceTemplateOptions whenever the CurrentServiceTemplatesObservable updates
            Manager.Context.CurrentServiceTemplatesObservable.ObserveOnDispatcher().Subscribe(loadedServiceTemplates =>
            {
                ServiceTemplateOptions.Clear();
                foreach (var serviceTemplate in loadedServiceTemplates)
                    ServiceTemplateOptions.Add(new EntityOption(serviceTemplate));
            });

            //Setup an observable for whenever the Dispatcher is entered/reentered
            var enteredDispatcher = MessageBus.Current.Listen<NavigateToMessage>().Where(m => m.UriToNavigateTo.ToString().Contains("Dispatcher"));

            //Load the regions whenever the Dispatcher is entered
            enteredDispatcher.ObserveOnDispatcher().Subscribe(_ =>
            {
                var regionsQuery = Manager.Data.DomainContext.GetRegionsForServiceProviderQuery(Manager.Context.RoleId);
                Manager.Data.DomainContext.Load(regionsQuery,
                                                lo =>
                                                {
                                                    LoadedRegions = new ObservableCollection<Region>(lo.Entities);

                                                    RegionOptions.Clear();

                                                    foreach (var region in LoadedRegions)
                                                        RegionOptions.Add(new EntityOption(region));
                                                },
                                                null);
            });
        }

        #endregion
    }

    /// <summary>
    /// An option to select or not select a ServiceTemplate.
    /// </summary>
    public class EntityOption : ThreadableNotifiableObject
    {
        #region Public Properties

        private readonly Entity _entity;
        /// <summary>
        /// Gets the selected regions of routes to display.
        /// </summary>
        public Entity Entity { get { return _entity; } }

        private bool _isSelected = true;
        /// <summary>
        /// Whether or not this is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                this.RaisePropertyChanged("IsSelected");
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityOption"/> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public EntityOption(Entity entity)
        {
            _entity = entity;
        }

        #endregion
    }
}