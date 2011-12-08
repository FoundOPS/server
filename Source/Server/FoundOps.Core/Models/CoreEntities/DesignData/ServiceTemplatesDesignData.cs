using System;
using System.Collections.Generic;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.CoreEntities.DesignData;

namespace FoundOps.Core.Models.CoreEntities.DesignData
{
    public static class ServiceTemplatesDesignData
    {
        #region Oil and Grease

        public static List<ServiceTemplate> OilGreaseCompanyServiceTemplates;

        public static readonly ServiceTemplate OilServiceTemplate;
        public static readonly ServiceTemplate SmallGreaseTrapServiceTemplate;
        public static readonly ServiceTemplate InterceptorServiceTemplate;
        public static readonly ServiceTemplate HydrojettingServiceTemplate;
        public static readonly ServiceTemplate ContainerReplacementServiceTemplate;
        public static readonly ServiceTemplate EnvironmentalBiotechServiceTemplate;

        public static readonly OptionsField HoseLength;
        public static readonly OptionsField EnvironmentalBiotechServicesCheckListField;

        #endregion

        #region Same Day Delivery

        public static List<ServiceTemplate> SameDayDeliveryCompanyServiceTemplates;

        public static readonly ServiceTemplate DirectServiceTemplate;
        public static readonly ServiceTemplate RushServiceTemplate;
        public static readonly ServiceTemplate RegularServiceTemplate;
        public static readonly ServiceTemplate EconomyServiceTemplate;

        #endregion

        #region Common Fields

        public static readonly List<Field> Fields;

        public static readonly TextBoxField LockInfo;
        public static readonly TextBoxField Notes;

        public static readonly LocationField ServiceDestinationField;

        #endregion

        static ServiceTemplatesDesignData()
        {
            #region Fields

            HoseLength = new OptionsField
            {
                Id = new Guid("{C94D7936-AA32-4337-B086-25841F4F9361}"),
                Group = "Service Details",
                Name = "Hose Length",
                Required = false
            };

            HoseLength.Options.Add(new Option { Name = "5 Feet", Index = 0 });
            HoseLength.Options.Add(new Option { Name = "10 Feet", Index = 1 });
            HoseLength.Options.Add(new Option { Name = "15 Feet", Index = 2 });

            LockInfo = new TextBoxField
            {
                Id = new Guid("{178BCE3A-6453-419D-9BDD-D474B72421AA}"),
                Group = "Service Details",
                Name = "Lock Info",
                IsMultiline = false,
                Required = false
            };

            Notes = new TextBoxField
            {
                Id = new Guid("{76AA6A16-D328-4ECD-8601-F6CCDDC30B09}"),
                Group = "Service Details",
                Name = "Notes",
                IsMultiline = true,
                Required = false
            };

            ServiceDestinationField = new LocationField
            {
                Id = new Guid("{DB0301FD-5D74-4AB1-8DC5-53CD33F94BD9}"),
                Group = "Location",
                Name = "Service Destination",
                Tooltip = "Enter the Service Destination here",
                LocationFieldType = LocationFieldType.Destination,
                Required = true
            };

            EnvironmentalBiotechServicesCheckListField = new OptionsField
            {
                Id = new Guid("{70F78F39-EAB8-4D38-AC7E-14BE6C591418}"),
                Group = "Service Details",
                Name = "Subservices to Provide",
                AllowMultipleSelection = true,
                OptionsType = OptionsType.Checklist,
                Required = true
            };

            EnvironmentalBiotechServicesCheckListField.Options.Add(new Option { Name = "Grease Eradication Bacteria", Index = 0 });
            EnvironmentalBiotechServicesCheckListField.Options.Add(new Option { Name = "Sugar Eradication Bacteria", Index = 1 });
            EnvironmentalBiotechServicesCheckListField.Options.Add(new Option { Name = "Urinal Eradication Bacteria", Index = 2, Tooltip = "Crystal Drip System" });

            Fields = new List<Field>
                         {
                             HoseLength,
                             LockInfo,
                             Notes,
                             ServiceDestinationField,
                             EnvironmentalBiotechServicesCheckListField
                         };

            #endregion

            #region Oil and GreaseCompany

            OilServiceTemplate = new ServiceTemplate
            {
                Id = new Guid("{8ADDFE8A-7376-4AC4-8312-C7152148F1A5}"),
                Name = "Oil",
                ServiceTemplateLevel = ServiceTemplateLevel.FoundOpsDefined,
                OwnerServiceProvider = BusinessAccountsDesignData.FoundOps,
                Invoice = new InvoicesDesignData().DesignInvoice
            };
            OilServiceTemplate.Invoice.Id = OilServiceTemplate.Id;

            OilServiceTemplate.Fields.Add(HoseLength.MakeChild());
            OilServiceTemplate.Fields.Add(LockInfo.MakeChild());
            OilServiceTemplate.Fields.Add(Notes.MakeChild());
            OilServiceTemplate.Fields.Add(ServiceDestinationField.MakeChild());

            SmallGreaseTrapServiceTemplate = new ServiceTemplate
            {
                Id = new Guid("{B1B4C515-EBBC-4D16-AC11-94C6D1B7EF8D}"),
                Name = "Small Grease Trap, 20-500 gallons",
                ServiceTemplateLevel = ServiceTemplateLevel.FoundOpsDefined,
                OwnerServiceProvider = BusinessAccountsDesignData.FoundOps,
                Invoice = new InvoicesDesignData().DesignInvoiceTwo
            };
            SmallGreaseTrapServiceTemplate.Invoice.Id = SmallGreaseTrapServiceTemplate.Id;

            SmallGreaseTrapServiceTemplate.Fields.Add(HoseLength.MakeChild());
            SmallGreaseTrapServiceTemplate.Fields.Add(Notes.MakeChild());

            SmallGreaseTrapServiceTemplate.Fields.Add(ServiceDestinationField.MakeChild());

            InterceptorServiceTemplate = new ServiceTemplate
            {
                Id = new Guid("{699D428A-3A4F-472D-9850-BD54CFB99625}"),
                Name = "Interceptor, 500+ gallons",
                ServiceTemplateLevel = ServiceTemplateLevel.FoundOpsDefined,
                OwnerServiceProvider = BusinessAccountsDesignData.FoundOps,
                Invoice = new InvoicesDesignData().DesignInvoiceThree
            };
            InterceptorServiceTemplate.Invoice.Id = InterceptorServiceTemplate.Id;

            InterceptorServiceTemplate.Fields.Add(HoseLength.MakeChild());
            InterceptorServiceTemplate.Fields.Add(Notes.MakeChild());
            InterceptorServiceTemplate.Fields.Add(ServiceDestinationField.MakeChild());

            HydrojettingServiceTemplate = new ServiceTemplate
            {
                Id = new Guid("{0A60FAC7-31A2-4CEA-9EDC-B63DD8D953C9}"),
                Name = "Hydrojetting",
                ServiceTemplateLevel = ServiceTemplateLevel.FoundOpsDefined,
                OwnerServiceProvider = BusinessAccountsDesignData.FoundOps,
                Invoice = new InvoicesDesignData().DesignInvoice
            };
            HydrojettingServiceTemplate.Invoice.Id = HydrojettingServiceTemplate.Id;

            HydrojettingServiceTemplate.Fields.Add(HoseLength.MakeChild());
            HydrojettingServiceTemplate.Fields.Add(Notes.MakeChild());

            HydrojettingServiceTemplate.Fields.Add(ServiceDestinationField.MakeChild());

            ContainerReplacementServiceTemplate = new ServiceTemplate
            {
                Id = new Guid("{D82CA114-2660-4F09-A9B4-DFBFB4C1D10E}"),
                Name = "Container Replacement",
                ServiceTemplateLevel = ServiceTemplateLevel.FoundOpsDefined,
                OwnerServiceProvider = BusinessAccountsDesignData.FoundOps,
                Invoice = new InvoicesDesignData().DesignInvoiceTwo
            };
            ContainerReplacementServiceTemplate.Invoice.Id = ContainerReplacementServiceTemplate.Id;

            ContainerReplacementServiceTemplate.Fields.Add(HoseLength.MakeChild());
            ContainerReplacementServiceTemplate.Fields.Add(LockInfo.MakeChild());
            ContainerReplacementServiceTemplate.Fields.Add(Notes.MakeChild());
            ContainerReplacementServiceTemplate.Fields.Add(ServiceDestinationField.MakeChild());

            #region Environmental Biotech

            EnvironmentalBiotechServiceTemplate = new ServiceTemplate
            {
                Id = new Guid("{B82D1251-95E0-485E-9667-4A7387D46FA1}"),
                Name = "Environmental Biotech",
                ServiceTemplateLevel = ServiceTemplateLevel.FoundOpsDefined,
                OwnerServiceProvider = BusinessAccountsDesignData.FoundOps,
                Invoice = new InvoicesDesignData().DesignInvoiceTwo
            };
            EnvironmentalBiotechServiceTemplate.Invoice.Id = EnvironmentalBiotechServiceTemplate.Id;

            EnvironmentalBiotechServiceTemplate.Fields.Add(Notes);
            EnvironmentalBiotechServiceTemplate.Fields.Add(ServiceDestinationField);
            EnvironmentalBiotechServiceTemplate.Fields.Add(EnvironmentalBiotechServicesCheckListField);

            #endregion

            OilGreaseCompanyServiceTemplates = new List<ServiceTemplate>
            {
                OilServiceTemplate,
                SmallGreaseTrapServiceTemplate,
                InterceptorServiceTemplate,
                HydrojettingServiceTemplate,
                ContainerReplacementServiceTemplate,
                EnvironmentalBiotechServiceTemplate
            };

            #endregion

            #region Same Day Delivery

            DirectServiceTemplate = new ServiceTemplate
            {
                Id = new Guid("{C8E89404-3547-4A75-9E90-0B3BF48B04F1}"),
                Name = "Direct",
                ServiceTemplateLevel = ServiceTemplateLevel.FoundOpsDefined,
                OwnerServiceProvider = BusinessAccountsDesignData.FoundOps,
                Invoice = new InvoicesDesignData().DesignInvoiceThree
            };
            DirectServiceTemplate.Invoice.Id = DirectServiceTemplate.Id;

            DirectServiceTemplate.Fields.Add(Notes.MakeChild());
            DirectServiceTemplate.Fields.Add(ServiceDestinationField.MakeChild());

            RushServiceTemplate = new ServiceTemplate
            {
                Id = new Guid("{88E8A760-0FEC-478F-867D-4AE07EDEDD19}"),
                Name = "Rush",
                ServiceTemplateLevel = ServiceTemplateLevel.FoundOpsDefined,
                OwnerServiceProvider = BusinessAccountsDesignData.FoundOps,
                Invoice = new InvoicesDesignData().DesignInvoice
            };
            RushServiceTemplate.Invoice.Id = RushServiceTemplate.Id;

            RushServiceTemplate.Fields.Add(Notes.MakeChild());
            RushServiceTemplate.Fields.Add(ServiceDestinationField.MakeChild());

            RegularServiceTemplate = new ServiceTemplate
            {
                Id = new Guid("{CE0EC8C5-6B39-41BE-922F-A1E1DDD367C5}"),
                Name = "Regular",
                ServiceTemplateLevel = ServiceTemplateLevel.FoundOpsDefined,
                OwnerServiceProvider = BusinessAccountsDesignData.FoundOps,
                Invoice = new InvoicesDesignData().DesignInvoiceTwo
            };
            RegularServiceTemplate.Invoice.Id = RegularServiceTemplate.Id;

            RegularServiceTemplate.Fields.Add(Notes.MakeChild());
            RegularServiceTemplate.Fields.Add(ServiceDestinationField.MakeChild());

            EconomyServiceTemplate = new ServiceTemplate
            {
                Id = new Guid("{BE09BB53-DE15-4E6C-BF7A-1595EA91DF56}"),
                Name = "Economy",
                ServiceTemplateLevel = ServiceTemplateLevel.FoundOpsDefined,
                OwnerServiceProvider = BusinessAccountsDesignData.FoundOps,
                Invoice = new InvoicesDesignData().DesignInvoiceThree
            };
            EconomyServiceTemplate.Invoice.Id = EconomyServiceTemplate.Id;

            EconomyServiceTemplate.Fields.Add(Notes.MakeChild());
            EconomyServiceTemplate.Fields.Add(ServiceDestinationField.MakeChild());

            SameDayDeliveryCompanyServiceTemplates = new List<ServiceTemplate>
                                                         {
                                                             DirectServiceTemplate,
                                                             RushServiceTemplate,
                                                             RegularServiceTemplate,
                                                             EconomyServiceTemplate
                                                         };

            #endregion
        }
    }
}