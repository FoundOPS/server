using System;
using System.Linq;
using FoundOps.Common.Composite.Entities;

// Partial class needs to bee in the same namespace
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public enum ServiceTemplateLevel
    {
        FoundOpsDefined = 0, //We define it
        ServiceProviderDefined = 1, //A service provider defines it. Ex. GotGrease
        ClientGroupDefined = 2, //It is defined by a client group. Ex. GotGrease's Restaraunt's Grossing Over 5 mil
        ClientDefined = 3, //It is defined by a client. Ex. GotGrease's Client (McDonalds) has this as an available service
        RecurringServiceDefined = 4, //It is defined by a recurring service. Ex. Oil Collection at McDonalds every 2 weeks
        ServiceDefined = 5 //It is defined by a service. Oil Collection at McDonalds on 1/1/2012
    }

    public partial class ServiceTemplate : IEntityDefaultCreation, ICompositeRaiseEntityPropertyChanged
    {
        #region Implementation of IEntityDefaultCreation

#if SILVERLIGHT

        partial void OnCreated()
        {
            ((IEntityDefaultCreation) this).OnCreate();
        }
#else
        public ServiceTemplate()
        {
            ((IEntityDefaultCreation)this).OnCreate();
        }

#endif
        partial void OnCreation(); //For Extensions on Silverlight Side

        void IEntityDefaultCreation.OnCreate()
        {
            Id = Guid.NewGuid();
            OnCreation();
        }

        #endregion

        #region Implementation of ICompositeRaiseEntityPropertyChanged

#if SILVERLIGHT
        public void CompositeRaiseEntityPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
#else
        public void CompositeRaiseEntityPropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }
#endif
        #endregion

        public ServiceTemplate MakeChild(ServiceTemplateLevel level)
        {
#if !SILVERLIGHT
            var serviceTemplateChild = new ServiceTemplate
            {
                Id = Guid.NewGuid(),
                Name = this.Name,
                ParentServiceTemplate = this,
                ServiceTemplateLevel = level,
                CreatedDate = DateTime.UtcNow
            };

            if (serviceTemplateChild.ServiceTemplateLevel == ServiceTemplateLevel.ServiceProviderDefined)
                serviceTemplateChild.OwnerServiceProvider = this.OwnerServiceProvider;

            if (serviceTemplateChild.ServiceTemplateLevel == ServiceTemplateLevel.ClientDefined)
                serviceTemplateChild.OwnerClient = this.OwnerClient;

            if (serviceTemplateChild.ServiceTemplateLevel == ServiceTemplateLevel.RecurringServiceDefined)
                serviceTemplateChild.OwnerRecurringService = this.OwnerRecurringService;

            if (serviceTemplateChild.ServiceTemplateLevel == ServiceTemplateLevel.ServiceDefined)
                serviceTemplateChild.OwnerService = this.OwnerService;

            foreach (var field in this.Fields.ToArray())
                serviceTemplateChild.Fields.Add(field.MakeChild());

            //TODO: Uncomment.
            //if (this.Invoice != null)
            //{
            //    serviceTemplateChild.Invoice = this.Invoice.MakeChild();
            //    serviceTemplateChild.Invoice.Id = serviceTemplateChild.Id;
            //}

            return serviceTemplateChild;
#else
            var serviceTemplateChild = MakeChildSilverlight(level);

            //TODO: Uncomment.
            //if (this.Invoice != null)
            //{
            //    serviceTemplateChild.Invoice = this.Invoice.MakeChild();
            //    serviceTemplateChild.Invoice.Id = serviceTemplateChild.Id;
            //}

            return serviceTemplateChild;
#endif
        }

        public ServiceTemplateLevel ServiceTemplateLevel
        {
            get
            {
                return (ServiceTemplateLevel)Convert.ToInt32(this.LevelInt);
            }
            set
            {
                this.LevelInt = Convert.ToInt16(value);
            }
        }

        partial void OnLevelIntChanged()
        {
            this.CompositeRaiseEntityPropertyChanged("ServiceTemplateLevel");
        }
    }
}