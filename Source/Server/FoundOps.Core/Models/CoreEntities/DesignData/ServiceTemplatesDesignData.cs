using System;
using System.Collections.Generic;
using System.Linq;

namespace FoundOps.Core.Models.CoreEntities.DesignData
{
    public static class ServiceTemplateConstants
    {
        public static readonly Guid ServiceDestinationFieldId = new Guid("{DB0301FD-5D74-4AB1-8DC5-53CD33F94BD9}");
    }

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

        #endregion

        #region Same Day Delivery

        public static List<ServiceTemplate> SameDayDeliveryCompanyServiceTemplates;

        public static readonly ServiceTemplate DirectServiceTemplate;
        public static readonly ServiceTemplate RushServiceTemplate;
        public static readonly ServiceTemplate RegularServiceTemplate;
        public static readonly ServiceTemplate EconomyServiceTemplate;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the <see cref="ServiceTemplatesDesignData"/> class.
        /// </summary>
        static ServiceTemplatesDesignData()
        {
            #region Oil and GreaseCompany

            OilServiceTemplate = new ServiceTemplate
            {
                Id = new Guid("{8ADDFE8A-7376-4AC4-8312-C7152148F1A5}"),
                Name = "WVO Collection",
                ServiceTemplateLevel = ServiceTemplateLevel.FoundOpsDefined,
                OwnerServiceProvider = BusinessAccountsDesignData.FoundOps,
                Invoice = new InvoicesDesignData().DesignInvoice
            };
            OilServiceTemplate.Invoice.Id = OilServiceTemplate.Id;

            var oilFields = new FieldsDesignData().DesignFields.ToArray();
            foreach (var field in oilFields)
                OilServiceTemplate.Fields.Add(field);

            //OilServiceTemplate.Fields.Add(CreateHoseLengthField());
            //OilServiceTemplate.Fields.Add(CreateLockInfoField());
            //OilServiceTemplate.Fields.Add(CreateNotesField());
            //OilServiceTemplate.Fields.Add(CreateServiceDestinationField());

            var smallGreaseFields = new FieldsDesignData().DesignFields.ToArray();

            SmallGreaseTrapServiceTemplate = new ServiceTemplate
            {
                Id = new Guid("{B1B4C515-EBBC-4D16-AC11-94C6D1B7EF8D}"),
                Name = "Small Grease Trap, 20-500 gallons",
                ServiceTemplateLevel = ServiceTemplateLevel.FoundOpsDefined,
                OwnerServiceProvider = BusinessAccountsDesignData.FoundOps,
                Invoice = new InvoicesDesignData().DesignInvoiceTwo
            };
            SmallGreaseTrapServiceTemplate.Invoice.Id = SmallGreaseTrapServiceTemplate.Id;

            foreach (var field in smallGreaseFields)
                SmallGreaseTrapServiceTemplate.Fields.Add(field);

            //SmallGreaseTrapServiceTemplate.Fields.Add(CreateHoseLengthField());
            //SmallGreaseTrapServiceTemplate.Fields.Add(CreateNotesField());

            SmallGreaseTrapServiceTemplate.Fields.Add(CreateServiceDestinationField());

            var interceptorFields = new FieldsDesignData().DesignFields.ToArray();

            InterceptorServiceTemplate = new ServiceTemplate
            {
                Id = new Guid("{699D428A-3A4F-472D-9850-BD54CFB99625}"),
                Name = "Interceptor, 500+ gallons",
                ServiceTemplateLevel = ServiceTemplateLevel.FoundOpsDefined,
                OwnerServiceProvider = BusinessAccountsDesignData.FoundOps,
                Invoice = new InvoicesDesignData().DesignInvoiceThree
            };
            InterceptorServiceTemplate.Invoice.Id = InterceptorServiceTemplate.Id;

            foreach (var field in interceptorFields)
                InterceptorServiceTemplate.Fields.Add(field);

            //InterceptorServiceTemplate.Fields.Add(CreateHoseLengthField());
            //InterceptorServiceTemplate.Fields.Add(CreateNotesField());
            //InterceptorServiceTemplate.Fields.Add(CreateServiceDestinationField());

            var hydroFields = new FieldsDesignData().DesignFields.ToArray();

            HydrojettingServiceTemplate = new ServiceTemplate
            {
                Id = new Guid("{0A60FAC7-31A2-4CEA-9EDC-B63DD8D953C9}"),
                Name = "Hydrojetting",
                ServiceTemplateLevel = ServiceTemplateLevel.FoundOpsDefined,
                OwnerServiceProvider = BusinessAccountsDesignData.FoundOps,
                Invoice = new InvoicesDesignData().DesignInvoice
            };
            HydrojettingServiceTemplate.Invoice.Id = HydrojettingServiceTemplate.Id;

            foreach (var field in hydroFields)
                HydrojettingServiceTemplate.Fields.Add(field);

            //HydrojettingServiceTemplate.Fields.Add(CreateHoseLengthField());
            //HydrojettingServiceTemplate.Fields.Add(CreateNotesField());
            //HydrojettingServiceTemplate.Fields.Add(CreateServiceDestinationField());

            var containerFields = new FieldsDesignData().DesignFields.ToArray();

            ContainerReplacementServiceTemplate = new ServiceTemplate
            {
                Id = new Guid("{D82CA114-2660-4F09-A9B4-DFBFB4C1D10E}"),
                Name = "Container Replacement",
                ServiceTemplateLevel = ServiceTemplateLevel.FoundOpsDefined,
                OwnerServiceProvider = BusinessAccountsDesignData.FoundOps,
                Invoice = new InvoicesDesignData().DesignInvoiceTwo
            };
            ContainerReplacementServiceTemplate.Invoice.Id = ContainerReplacementServiceTemplate.Id;

            foreach (var field in containerFields)
                ContainerReplacementServiceTemplate.Fields.Add(field);

            //ContainerReplacementServiceTemplate.Fields.Add(CreateHoseLengthField());
            //ContainerReplacementServiceTemplate.Fields.Add(CreateLockInfoField());
            //ContainerReplacementServiceTemplate.Fields.Add(CreateNotesField());
            //ContainerReplacementServiceTemplate.Fields.Add(CreateServiceDestinationField());

            #region Environmental Biotech

            var enviroFields = new FieldsDesignData().DesignFields.ToArray();

            EnvironmentalBiotechServiceTemplate = new ServiceTemplate
            {
                Id = new Guid("{B82D1251-95E0-485E-9667-4A7387D46FA1}"),
                Name = "Environmental Biotech",
                ServiceTemplateLevel = ServiceTemplateLevel.FoundOpsDefined,
                OwnerServiceProvider = BusinessAccountsDesignData.FoundOps,
                Invoice = new InvoicesDesignData().DesignInvoiceTwo
            };
            EnvironmentalBiotechServiceTemplate.Invoice.Id = EnvironmentalBiotechServiceTemplate.Id;

            foreach (var field in enviroFields)
                EnvironmentalBiotechServiceTemplate.Fields.Add(field);

            //EnvironmentalBiotechServiceTemplate.Fields.Add(CreateNotesField());
            //EnvironmentalBiotechServiceTemplate.Fields.Add(CreateServiceDestinationField());
            //EnvironmentalBiotechServiceTemplate.Fields.Add(CreateEnvironmentalBiotechServicesCheckListField());

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

            var directFields = new FieldsDesignData().DesignFields.ToArray();
            foreach (var field in directFields)
                DirectServiceTemplate.Fields.Add(field);

            //DirectServiceTemplate.Fields.Add(CreateNotesField());
            //DirectServiceTemplate.Fields.Add(CreateServiceDestinationField());

            RushServiceTemplate = new ServiceTemplate
            {
                Id = new Guid("{88E8A760-0FEC-478F-867D-4AE07EDEDD19}"),
                Name = "Rush",
                ServiceTemplateLevel = ServiceTemplateLevel.FoundOpsDefined,
                OwnerServiceProvider = BusinessAccountsDesignData.FoundOps,
                Invoice = new InvoicesDesignData().DesignInvoice
            };
            RushServiceTemplate.Invoice.Id = RushServiceTemplate.Id;

            var rushFields = new FieldsDesignData().DesignFields.ToArray();
            foreach (var field in rushFields)
                RushServiceTemplate.Fields.Add(field);

            //RushServiceTemplate.Fields.Add(CreateNotesField());
            //RushServiceTemplate.Fields.Add(CreateServiceDestinationField());

            RegularServiceTemplate = new ServiceTemplate
            {
                Id = new Guid("{CE0EC8C5-6B39-41BE-922F-A1E1DDD367C5}"),
                Name = "Regular",
                ServiceTemplateLevel = ServiceTemplateLevel.FoundOpsDefined,
                OwnerServiceProvider = BusinessAccountsDesignData.FoundOps,
                Invoice = new InvoicesDesignData().DesignInvoiceTwo
            };
            RegularServiceTemplate.Invoice.Id = RegularServiceTemplate.Id;

            var regularFields = new FieldsDesignData().DesignFields.ToArray();
            foreach (var field in regularFields)
                RegularServiceTemplate.Fields.Add(field);

            //RegularServiceTemplate.Fields.Add(CreateNotesField());
            //RegularServiceTemplate.Fields.Add(CreateHoseLengthField());
            //RegularServiceTemplate.Fields.Add(CreateLockInfoField());
            //RegularServiceTemplate.Fields.Add(CreateEnvironmentalBiotechServicesCheckListField());
            //RegularServiceTemplate.Fields.Add(CreateServiceDestinationField());

            EconomyServiceTemplate = new ServiceTemplate
            {
                Id = new Guid("{BE09BB53-DE15-4E6C-BF7A-1595EA91DF56}"),
                Name = "Economy",
                ServiceTemplateLevel = ServiceTemplateLevel.FoundOpsDefined,
                OwnerServiceProvider = BusinessAccountsDesignData.FoundOps,
                Invoice = new InvoicesDesignData().DesignInvoiceThree
            };
            EconomyServiceTemplate.Invoice.Id = EconomyServiceTemplate.Id;

            var economyFields = new FieldsDesignData().DesignFields.ToArray();
            foreach (var field in economyFields)
                EconomyServiceTemplate.Fields.Add(field);

            //EconomyServiceTemplate.Fields.Add(CreateNotesField());
            //EconomyServiceTemplate.Fields.Add(CreateServiceDestinationField());

            SameDayDeliveryCompanyServiceTemplates = new List<ServiceTemplate>
                                                         {
                                                             DirectServiceTemplate,
                                                             RushServiceTemplate,
                                                             RegularServiceTemplate,
                                                             EconomyServiceTemplate
                                                         };

            #endregion
        }

        #region Create Field Methods

        private static Field CreateHoseLengthField()
        {
            var hoseLength = new OptionsField
            {
                Id = Guid.NewGuid(),
                Name = "Hose Length",
                Required = false
            };

            hoseLength.Options.Add(new Option { Name = "5 Feet", Index = 0 });
            hoseLength.Options.Add(new Option { Name = "10 Feet", Index = 1 });
            hoseLength.Options.Add(new Option { Name = "15 Feet", Index = 2 });

            return hoseLength;
        }
        private static Field CreateLockInfoField()
        {
            return new TextBoxField
            {
                Id = Guid.NewGuid(),
                Name = "Lock Info",
                IsMultiline = false,
                Required = false
            };
        }
        private static Field CreateNotesField()
        {
            return new TextBoxField
            {
                Id = Guid.NewGuid(),
                Name = "Notes",
                IsMultiline = true,
                Required = false
            };
        }
        private static Field CreateServiceDestinationField()
        {
            return new LocationField
            {
                Id = Guid.NewGuid(),
                Name = "Service Destination",
                Tooltip = "Enter the Service Destination here",
                LocationFieldType = LocationFieldType.Destination,
                Required = true
            };
        }
        private static Field CreateEnvironmentalBiotechServicesCheckListField()
        {
            var environmentalBiotechServicesCheckListField = new OptionsField
            {
                Id = Guid.NewGuid(),
                Name = "Subservices to Provide",
                AllowMultipleSelection = true,
                OptionsType = OptionsType.Checklist,
                Required = true
            };

            environmentalBiotechServicesCheckListField.Options.Add(new Option { Name = "Grease Eradication Bacteria", Index = 0 });
            environmentalBiotechServicesCheckListField.Options.Add(new Option { Name = "Sugar Eradication Bacteria", Index = 1 });
            environmentalBiotechServicesCheckListField.Options.Add(new Option { Name = "Urinal Eradication Bacteria", Index = 2, Tooltip = "Crystal Drip System" });

            return environmentalBiotechServicesCheckListField;
        }

        #endregion

        #endregion
    }
}