
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 07/13/2012 16:15:45
-- Generated from EDMX file: C:\FoundOps\GitHub\Source\Server\FoundOps.Core\Models\CoreEntities\CoreEntities.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [Core];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_RoleBlock_Role]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RoleBlock] DROP CONSTRAINT [FK_RoleBlock_Role];
GO
IF OBJECT_ID(N'[dbo].[FK_RoleBlock_Block]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RoleBlock] DROP CONSTRAINT [FK_RoleBlock_Block];
GO
IF OBJECT_ID(N'[dbo].[FK_LocationContactInfo]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ContactInfoSet] DROP CONSTRAINT [FK_LocationContactInfo];
GO
IF OBJECT_ID(N'[dbo].[FK_VehicleMaintenanceLogEntryVehicle]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[VehicleMaintenanceLog] DROP CONSTRAINT [FK_VehicleMaintenanceLogEntryVehicle];
GO
IF OBJECT_ID(N'[dbo].[FK_RouteVehicle_Route]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RouteVehicle] DROP CONSTRAINT [FK_RouteVehicle_Route];
GO
IF OBJECT_ID(N'[dbo].[FK_RouteVehicle_Vehicle]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RouteVehicle] DROP CONSTRAINT [FK_RouteVehicle_Vehicle];
GO
IF OBJECT_ID(N'[dbo].[FK_RouteTaskLocation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RouteTasks] DROP CONSTRAINT [FK_RouteTaskLocation];
GO
IF OBJECT_ID(N'[dbo].[FK_RouteDestinationLocation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RouteDestinations] DROP CONSTRAINT [FK_RouteDestinationLocation];
GO
IF OBJECT_ID(N'[dbo].[FK_RouteTaskRouteDestination]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RouteTasks] DROP CONSTRAINT [FK_RouteTaskRouteDestination];
GO
IF OBJECT_ID(N'[dbo].[FK_RouteDestinationRoute]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RouteDestinations] DROP CONSTRAINT [FK_RouteDestinationRoute];
GO
IF OBJECT_ID(N'[dbo].[FK_VehicleMaintenanceLogEntryLineItem]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[VehicleMaintenanceLineItems] DROP CONSTRAINT [FK_VehicleMaintenanceLogEntryLineItem];
GO
IF OBJECT_ID(N'[dbo].[FK_RouteTaskClient]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RouteTasks] DROP CONSTRAINT [FK_RouteTaskClient];
GO
IF OBJECT_ID(N'[dbo].[FK_ServiceClient]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Services] DROP CONSTRAINT [FK_ServiceClient];
GO
IF OBJECT_ID(N'[dbo].[FK_RouteDestinationClient]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RouteDestinations] DROP CONSTRAINT [FK_RouteDestinationClient];
GO
IF OBJECT_ID(N'[dbo].[FK_RouteTaskService]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RouteTasks] DROP CONSTRAINT [FK_RouteTaskService];
GO
IF OBJECT_ID(N'[dbo].[FK_BusinessAccountService]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Services] DROP CONSTRAINT [FK_BusinessAccountService];
GO
IF OBJECT_ID(N'[dbo].[FK_BusinessAccountRoute]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Routes] DROP CONSTRAINT [FK_BusinessAccountRoute];
GO
IF OBJECT_ID(N'[dbo].[FK_BusinessAccountRouteTask]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RouteTasks] DROP CONSTRAINT [FK_BusinessAccountRouteTask];
GO
IF OBJECT_ID(N'[dbo].[FK_UserAccountUserAccountLogEntry]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserAccountLog] DROP CONSTRAINT [FK_UserAccountUserAccountLogEntry];
GO
IF OBJECT_ID(N'[dbo].[FK_ServiceTemplateBusinessAccount]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ServiceTemplates] DROP CONSTRAINT [FK_ServiceTemplateBusinessAccount];
GO
IF OBJECT_ID(N'[dbo].[FK_RecurringServiceServiceTemplate]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RecurringServices] DROP CONSTRAINT [FK_RecurringServiceServiceTemplate];
GO
IF OBJECT_ID(N'[dbo].[FK_RecurringServiceService]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Services] DROP CONSTRAINT [FK_RecurringServiceService];
GO
IF OBJECT_ID(N'[dbo].[FK_RecurringServiceClient]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RecurringServices] DROP CONSTRAINT [FK_RecurringServiceClient];
GO
IF OBJECT_ID(N'[dbo].[FK_ClientServiceTemplate]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ServiceTemplates] DROP CONSTRAINT [FK_ClientServiceTemplate];
GO
IF OBJECT_ID(N'[dbo].[FK_VehicleParty]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Vehicles] DROP CONSTRAINT [FK_VehicleParty];
GO
IF OBJECT_ID(N'[dbo].[FK_ServiceServiceTemplate]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Services] DROP CONSTRAINT [FK_ServiceServiceTemplate];
GO
IF OBJECT_ID(N'[dbo].[FK_PartyRole_Party]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PartyRole] DROP CONSTRAINT [FK_PartyRole_Party];
GO
IF OBJECT_ID(N'[dbo].[FK_PartyRole_Role]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PartyRole] DROP CONSTRAINT [FK_PartyRole_Role];
GO
IF OBJECT_ID(N'[dbo].[FK_PartyRole1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Roles] DROP CONSTRAINT [FK_PartyRole1];
GO
IF OBJECT_ID(N'[dbo].[FK_RegionLocation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Locations] DROP CONSTRAINT [FK_RegionLocation];
GO
IF OBJECT_ID(N'[dbo].[FK_BusinessAccountRegion]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Regions] DROP CONSTRAINT [FK_BusinessAccountRegion];
GO
IF OBJECT_ID(N'[dbo].[FK_LocationFile]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Files] DROP CONSTRAINT [FK_LocationFile];
GO
IF OBJECT_ID(N'[dbo].[FK_LocationSubLocation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SubLocations] DROP CONSTRAINT [FK_LocationSubLocation];
GO
IF OBJECT_ID(N'[dbo].[FK_EmployeeUserAccount]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Employees] DROP CONSTRAINT [FK_EmployeeUserAccount];
GO
IF OBJECT_ID(N'[dbo].[FK_EmployeeBusinessAccount]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Employees] DROP CONSTRAINT [FK_EmployeeBusinessAccount];
GO
IF OBJECT_ID(N'[dbo].[FK_EmployeePerson]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Employees] DROP CONSTRAINT [FK_EmployeePerson];
GO
IF OBJECT_ID(N'[dbo].[FK_RecurringServiceRepeat]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RecurringServices] DROP CONSTRAINT [FK_RecurringServiceRepeat];
GO
IF OBJECT_ID(N'[dbo].[FK_EmployeeHistoryEntryEmployee]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[EmployeeHistoryEntries] DROP CONSTRAINT [FK_EmployeeHistoryEntryEmployee];
GO
IF OBJECT_ID(N'[dbo].[FK_ServiceTemplateServiceTemplate]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ServiceTemplates] DROP CONSTRAINT [FK_ServiceTemplateServiceTemplate];
GO
IF OBJECT_ID(N'[dbo].[FK_FieldField]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Fields] DROP CONSTRAINT [FK_FieldField];
GO
IF OBJECT_ID(N'[dbo].[FK_OptionsFieldOption]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Options] DROP CONSTRAINT [FK_OptionsFieldOption];
GO
IF OBJECT_ID(N'[dbo].[FK_ServiceTemplateField]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Fields] DROP CONSTRAINT [FK_ServiceTemplateField];
GO
IF OBJECT_ID(N'[dbo].[FK_LocationFieldLocation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Fields_LocationField] DROP CONSTRAINT [FK_LocationFieldLocation];
GO
IF OBJECT_ID(N'[dbo].[FK_InvoiceLocation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Invoices] DROP CONSTRAINT [FK_InvoiceLocation];
GO
IF OBJECT_ID(N'[dbo].[FK_InvoiceSalesTerm]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Invoices] DROP CONSTRAINT [FK_InvoiceSalesTerm];
GO
IF OBJECT_ID(N'[dbo].[FK_InvoiceServiceTemplate]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Invoices] DROP CONSTRAINT [FK_InvoiceServiceTemplate];
GO
IF OBJECT_ID(N'[dbo].[FK_LineItemInvoice]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[LineItems] DROP CONSTRAINT [FK_LineItemInvoice];
GO
IF OBJECT_ID(N'[dbo].[FK_FileParty]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Files] DROP CONSTRAINT [FK_FileParty];
GO
IF OBJECT_ID(N'[dbo].[FK_PartyPartyImage]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Files_PartyImage] DROP CONSTRAINT [FK_PartyPartyImage];
GO
IF OBJECT_ID(N'[dbo].[FK_BusinessAccountInvoice]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Invoices] DROP CONSTRAINT [FK_BusinessAccountInvoice];
GO
IF OBJECT_ID(N'[dbo].[FK_ClientInvoice]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Invoices] DROP CONSTRAINT [FK_ClientInvoice];
GO
IF OBJECT_ID(N'[dbo].[FK_RouteTaskRecurringService]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RouteTasks] DROP CONSTRAINT [FK_RouteTaskRecurringService];
GO
IF OBJECT_ID(N'[dbo].[FK_BusinessAccountLocation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Locations] DROP CONSTRAINT [FK_BusinessAccountLocation];
GO
IF OBJECT_ID(N'[dbo].[FK_ClientBusinessAccount]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Clients] DROP CONSTRAINT [FK_ClientBusinessAccount];
GO
IF OBJECT_ID(N'[dbo].[FK_LocationBusinessAccount]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Locations] DROP CONSTRAINT [FK_LocationBusinessAccount];
GO
IF OBJECT_ID(N'[dbo].[FK_ClientLocation1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Locations] DROP CONSTRAINT [FK_ClientLocation1];
GO
IF OBJECT_ID(N'[dbo].[FK_ClientContactInfo]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ContactInfoSet] DROP CONSTRAINT [FK_ClientContactInfo];
GO
IF OBJECT_ID(N'[dbo].[FK_RouteEmployee_Route]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RouteEmployee] DROP CONSTRAINT [FK_RouteEmployee_Route];
GO
IF OBJECT_ID(N'[dbo].[FK_RouteEmployee_Employee]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RouteEmployee] DROP CONSTRAINT [FK_RouteEmployee_Employee];
GO
IF OBJECT_ID(N'[dbo].[FK_Business_inherits_Party]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Parties_Business] DROP CONSTRAINT [FK_Business_inherits_Party];
GO
IF OBJECT_ID(N'[dbo].[FK_BusinessAccount_inherits_Business]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Parties_BusinessAccount] DROP CONSTRAINT [FK_BusinessAccount_inherits_Business];
GO
IF OBJECT_ID(N'[dbo].[FK_Person_inherits_Party]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Parties_Person] DROP CONSTRAINT [FK_Person_inherits_Party];
GO
IF OBJECT_ID(N'[dbo].[FK_UserAccount_inherits_Person]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Parties_UserAccount] DROP CONSTRAINT [FK_UserAccount_inherits_Person];
GO
IF OBJECT_ID(N'[dbo].[FK_OptionsField_inherits_Field]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Fields_OptionsField] DROP CONSTRAINT [FK_OptionsField_inherits_Field];
GO
IF OBJECT_ID(N'[dbo].[FK_LocationField_inherits_Field]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Fields_LocationField] DROP CONSTRAINT [FK_LocationField_inherits_Field];
GO
IF OBJECT_ID(N'[dbo].[FK_PartyImage_inherits_File]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Files_PartyImage] DROP CONSTRAINT [FK_PartyImage_inherits_File];
GO
IF OBJECT_ID(N'[dbo].[FK_TextBoxField_inherits_Field]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Fields_TextBoxField] DROP CONSTRAINT [FK_TextBoxField_inherits_Field];
GO
IF OBJECT_ID(N'[dbo].[FK_NumericField_inherits_Field]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Fields_NumericField] DROP CONSTRAINT [FK_NumericField_inherits_Field];
GO
IF OBJECT_ID(N'[dbo].[FK_DateTimeField_inherits_Field]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Fields_DateTimeField] DROP CONSTRAINT [FK_DateTimeField_inherits_Field];
GO
IF OBJECT_ID(N'[dbo].[FK_LocationOption_inherits_Option]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Options_LocationOption] DROP CONSTRAINT [FK_LocationOption_inherits_Option];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Blocks]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Blocks];
GO
IF OBJECT_ID(N'[dbo].[Roles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Roles];
GO
IF OBJECT_ID(N'[dbo].[Parties]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Parties];
GO
IF OBJECT_ID(N'[dbo].[ContactInfoSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ContactInfoSet];
GO
IF OBJECT_ID(N'[dbo].[Services]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Services];
GO
IF OBJECT_ID(N'[dbo].[Fields]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Fields];
GO
IF OBJECT_ID(N'[dbo].[Locations]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Locations];
GO
IF OBJECT_ID(N'[dbo].[RouteDestinations]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RouteDestinations];
GO
IF OBJECT_ID(N'[dbo].[Routes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Routes];
GO
IF OBJECT_ID(N'[dbo].[UserAccountLog]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserAccountLog];
GO
IF OBJECT_ID(N'[dbo].[Vehicles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Vehicles];
GO
IF OBJECT_ID(N'[dbo].[VehicleMaintenanceLog]', 'U') IS NOT NULL
    DROP TABLE [dbo].[VehicleMaintenanceLog];
GO
IF OBJECT_ID(N'[dbo].[VehicleMaintenanceLineItems]', 'U') IS NOT NULL
    DROP TABLE [dbo].[VehicleMaintenanceLineItems];
GO
IF OBJECT_ID(N'[dbo].[Clients]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Clients];
GO
IF OBJECT_ID(N'[dbo].[ServiceTemplates]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ServiceTemplates];
GO
IF OBJECT_ID(N'[dbo].[Repeats]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Repeats];
GO
IF OBJECT_ID(N'[dbo].[RecurringServices]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RecurringServices];
GO
IF OBJECT_ID(N'[dbo].[Regions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Regions];
GO
IF OBJECT_ID(N'[dbo].[Files]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Files];
GO
IF OBJECT_ID(N'[dbo].[SubLocations]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SubLocations];
GO
IF OBJECT_ID(N'[dbo].[Employees]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Employees];
GO
IF OBJECT_ID(N'[dbo].[EmployeeHistoryEntries]', 'U') IS NOT NULL
    DROP TABLE [dbo].[EmployeeHistoryEntries];
GO
IF OBJECT_ID(N'[dbo].[Options]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Options];
GO
IF OBJECT_ID(N'[dbo].[Invoices]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Invoices];
GO
IF OBJECT_ID(N'[dbo].[SalesTerms]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SalesTerms];
GO
IF OBJECT_ID(N'[dbo].[LineItems]', 'U') IS NOT NULL
    DROP TABLE [dbo].[LineItems];
GO
IF OBJECT_ID(N'[dbo].[RouteTasks]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RouteTasks];
GO
IF OBJECT_ID(N'[dbo].[Errors]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Errors];
GO
IF OBJECT_ID(N'[dbo].[ServiceTemplateWithVendorIds]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ServiceTemplateWithVendorIds];
GO
IF OBJECT_ID(N'[dbo].[Parties_Business]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Parties_Business];
GO
IF OBJECT_ID(N'[dbo].[Parties_BusinessAccount]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Parties_BusinessAccount];
GO
IF OBJECT_ID(N'[dbo].[Parties_Person]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Parties_Person];
GO
IF OBJECT_ID(N'[dbo].[Parties_UserAccount]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Parties_UserAccount];
GO
IF OBJECT_ID(N'[dbo].[Fields_OptionsField]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Fields_OptionsField];
GO
IF OBJECT_ID(N'[dbo].[Fields_LocationField]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Fields_LocationField];
GO
IF OBJECT_ID(N'[dbo].[Files_PartyImage]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Files_PartyImage];
GO
IF OBJECT_ID(N'[dbo].[Fields_TextBoxField]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Fields_TextBoxField];
GO
IF OBJECT_ID(N'[dbo].[Fields_NumericField]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Fields_NumericField];
GO
IF OBJECT_ID(N'[dbo].[Fields_DateTimeField]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Fields_DateTimeField];
GO
IF OBJECT_ID(N'[dbo].[Options_LocationOption]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Options_LocationOption];
GO
IF OBJECT_ID(N'[dbo].[RoleBlock]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RoleBlock];
GO
IF OBJECT_ID(N'[dbo].[RouteVehicle]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RouteVehicle];
GO
IF OBJECT_ID(N'[dbo].[PartyRole]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PartyRole];
GO
IF OBJECT_ID(N'[dbo].[RouteEmployee]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RouteEmployee];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Blocks'
CREATE TABLE [dbo].[Blocks] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [HideFromNavigation] bit  NOT NULL,
    [IconUrl] nvarchar(max)  NULL,
    [HoverIconUrl] nvarchar(max)  NULL,
    [Url] nvarchar(max)  NULL,
    [IsSilverlight] bit  NOT NULL
);
GO

-- Creating table 'Roles'
CREATE TABLE [dbo].[Roles] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(max)  NULL,
    [Description] nvarchar(max)  NULL,
    [OwnerPartyId] uniqueidentifier  NULL,
    [RoleTypeInt] smallint  NOT NULL
);
GO

-- Creating table 'Parties'
CREATE TABLE [dbo].[Parties] (
    [Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'ContactInfoSet'
CREATE TABLE [dbo].[ContactInfoSet] (
    [Id] uniqueidentifier  NOT NULL,
    [Type] nvarchar(max)  NOT NULL,
    [Label] nvarchar(max)  NULL,
    [Data] nvarchar(max)  NULL,
    [PartyId] uniqueidentifier  NULL,
    [LocationId] uniqueidentifier  NULL,
    [ClientId] uniqueidentifier  NULL
);
GO

-- Creating table 'Services'
CREATE TABLE [dbo].[Services] (
    [Id] uniqueidentifier  NOT NULL,
    [ClientId] uniqueidentifier  NOT NULL,
    [ServiceProviderId] uniqueidentifier  NOT NULL,
    [RecurringServiceId] uniqueidentifier  NULL,
    [ServiceDate] datetime  NOT NULL
);
GO

-- Creating table 'Fields'
CREATE TABLE [dbo].[Fields] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Group] nvarchar(max)  NOT NULL,
    [Required] bit  NOT NULL,
    [Tooltip] nvarchar(max)  NULL,
    [ParentFieldId] uniqueidentifier  NULL,
    [ServiceTemplateId] uniqueidentifier  NULL
);
GO

-- Creating table 'Locations'
CREATE TABLE [dbo].[Locations] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(max)  NULL,
    [AddressLineOne] nvarchar(max)  NULL,
    [Longitude] decimal(11,8)  NULL,
    [ZipCode] nvarchar(max)  NULL,
    [AddressLineTwo] nvarchar(max)  NULL,
    [State] nvarchar(max)  NULL,
    [Latitude] decimal(11,8)  NULL,
    [City] nvarchar(max)  NULL,
    [RegionId] uniqueidentifier  NULL,
    [BusinessAccountIdIfDepot] uniqueidentifier  NULL,
    [BusinessAccountId] uniqueidentifier  NULL,
    [ClientId] uniqueidentifier  NULL,
    [IsDefaultBillingLocation] bit  NULL
);
GO

-- Creating table 'RouteDestinations'
CREATE TABLE [dbo].[RouteDestinations] (
    [Id] uniqueidentifier  NOT NULL,
    [OrderInRoute] int  NOT NULL,
    [LocationId] uniqueidentifier  NULL,
    [RouteId] uniqueidentifier  NOT NULL,
    [ClientId] uniqueidentifier  NULL
);
GO

-- Creating table 'Routes'
CREATE TABLE [dbo].[Routes] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(max)  NULL,
    [Date] datetime  NOT NULL,
    [StartTime] datetime  NOT NULL,
    [EndTime] datetime  NOT NULL,
    [OwnerBusinessAccountId] uniqueidentifier  NOT NULL,
    [RouteType] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'UserAccountLog'
CREATE TABLE [dbo].[UserAccountLog] (
    [Id] uniqueidentifier  NOT NULL,
    [TypeId] int  NULL,
    [TimeStamp] datetime  NOT NULL,
    [UserAccountId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'Vehicles'
CREATE TABLE [dbo].[Vehicles] (
    [Id] uniqueidentifier  NOT NULL,
    [VehicleId] nvarchar(max)  NULL,
    [Mileage] int  NULL,
    [LicensePlate] nvarchar(max)  NULL,
    [VIN] nvarchar(max)  NULL,
    [Year] int  NULL,
    [Make] nvarchar(max)  NULL,
    [Model] nvarchar(max)  NULL,
    [Notes] nvarchar(max)  NULL,
    [LastCompassDirection] int  NULL,
    [LastLongitude] float  NULL,
    [LastLatitude] float  NULL,
    [LastTimeStamp] datetime  NULL,
    [LastSpeed] float  NULL,
    [LastSource] nvarchar(max)  NULL,
    [OwnerPartyId] uniqueidentifier  NOT NULL,
    [LastPushToAzureTimeStamp] datetime  NULL,
    [LastAccuracy] int  NULL
);
GO

-- Creating table 'VehicleMaintenanceLog'
CREATE TABLE [dbo].[VehicleMaintenanceLog] (
    [Id] uniqueidentifier  NOT NULL,
    [Date] datetime  NULL,
    [Mileage] int  NULL,
    [ServicedBy] nvarchar(max)  NULL,
    [Comments] nvarchar(max)  NULL,
    [VehicleId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'VehicleMaintenanceLineItems'
CREATE TABLE [dbo].[VehicleMaintenanceLineItems] (
    [Id] uniqueidentifier  NOT NULL,
    [Type] nvarchar(max)  NULL,
    [Cost] decimal(12,2)  NULL,
    [Details] nvarchar(max)  NULL,
    [VehicleMaintenanceLogEntryId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'Clients'
CREATE TABLE [dbo].[Clients] (
    [Id] uniqueidentifier  NOT NULL,
    [DateAdded] datetime  NOT NULL,
    [Salesperson] nvarchar(max)  NULL,
    [BusinessAccountId] uniqueidentifier  NULL,
    [Name] nvarchar(max)  NULL
);
GO

-- Creating table 'ServiceTemplates'
CREATE TABLE [dbo].[ServiceTemplates] (
    [Id] uniqueidentifier  NOT NULL,
    [OwnerServiceProviderId] uniqueidentifier  NULL,
    [OwnerClientId] uniqueidentifier  NULL,
    [OwnerServiceTemplateId] uniqueidentifier  NULL,
    [LevelInt] smallint  NOT NULL,
    [Name] nvarchar(max)  NULL
);
GO

-- Creating table 'Repeats'
CREATE TABLE [dbo].[Repeats] (
    [Id] uniqueidentifier  NOT NULL,
    [EndDate] datetime  NULL,
    [EndAfterTimes] int  NULL,
    [RepeatEveryTimes] int  NOT NULL,
    [FrequencyInt] int  NOT NULL,
    [FrequencyDetailInt] int  NULL,
    [StartDate] datetime  NOT NULL
);
GO

-- Creating table 'RecurringServices'
CREATE TABLE [dbo].[RecurringServices] (
    [Id] uniqueidentifier  NOT NULL,
    [ClientId] uniqueidentifier  NOT NULL,
    [ExcludedDatesString] nvarchar(max)  NULL
);
GO

-- Creating table 'Regions'
CREATE TABLE [dbo].[Regions] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [BusinessAccountId] uniqueidentifier  NULL,
    [Color] nvarchar(max)  NULL,
    [Notes] nvarchar(max)  NULL
);
GO

-- Creating table 'Files'
CREATE TABLE [dbo].[Files] (
    [Id] uniqueidentifier  NOT NULL,
    [Data] tinyint  NOT NULL,
    [Name] nvarchar(max)  NULL,
    [LocationId] uniqueidentifier  NULL,
    [PartyId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'SubLocations'
CREATE TABLE [dbo].[SubLocations] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Longitude] decimal(11,8)  NULL,
    [Latitude] decimal(11,8)  NULL,
    [Notes] nvarchar(max)  NULL,
    [LocationId] uniqueidentifier  NULL,
    [Number] int  NOT NULL
);
GO

-- Creating table 'Employees'
CREATE TABLE [dbo].[Employees] (
    [Id] uniqueidentifier  NOT NULL,
    [AddressLineOne] nvarchar(max)  NULL,
    [AddressLineTwo] nvarchar(max)  NULL,
    [City] nvarchar(max)  NULL,
    [Comments] nvarchar(max)  NULL,
    [State] nvarchar(max)  NULL,
    [ZipCode] nvarchar(max)  NULL,
    [Permissions] nvarchar(max)  NULL,
    [HireDate] datetime  NULL,
    [SSN] nvarchar(max)  NULL,
    [LinkedUserAccountId] uniqueidentifier  NULL,
    [EmployerId] uniqueidentifier  NOT NULL,
    [LastCompassDirection] int  NULL,
    [LastLongitude] float  NULL,
    [LastLatitude] float  NULL,
    [LastTimeStamp] datetime  NULL,
    [LastSpeed] float  NULL,
    [LastSource] nvarchar(max)  NULL,
    [LastPushToAzureTimeStamp] datetime  NULL,
    [LastAccuracy] int  NULL
);
GO

-- Creating table 'EmployeeHistoryEntries'
CREATE TABLE [dbo].[EmployeeHistoryEntries] (
    [Id] uniqueidentifier  NOT NULL,
    [Date] datetime  NULL,
    [Type] nvarchar(max)  NULL,
    [Summary] nvarchar(max)  NULL,
    [Notes] nvarchar(max)  NULL,
    [EmployeeId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'Options'
CREATE TABLE [dbo].[Options] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [IsChecked] bit  NOT NULL,
    [OptionsFieldId] uniqueidentifier  NOT NULL,
    [Index] int  NOT NULL,
    [Tooltip] nvarchar(max)  NULL
);
GO

-- Creating table 'Invoices'
CREATE TABLE [dbo].[Invoices] (
    [Id] uniqueidentifier  NOT NULL,
    [LocationId] uniqueidentifier  NULL,
    [FixedScheduleOptionInt] int  NULL,
    [RelativeScheduleDays] int  NULL,
    [ScheduleModeInt] int  NOT NULL,
    [SalesTermId] uniqueidentifier  NULL,
    [Memo] nvarchar(max)  NULL,
    [DueDate] datetime  NULL,
    [SyncToken] nvarchar(max)  NULL,
    [CustomerId] nvarchar(max)  NULL,
    [CreateTime] nvarchar(max)  NULL,
    [LastUpdatedTime] nvarchar(max)  NULL,
    [BusinessAccountId] uniqueidentifier  NULL,
    [ClientId] uniqueidentifier  NULL
);
GO

-- Creating table 'SalesTerms'
CREATE TABLE [dbo].[SalesTerms] (
    [Id] uniqueidentifier  NOT NULL,
    [DueDays] int  NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'LineItems'
CREATE TABLE [dbo].[LineItems] (
    [Id] uniqueidentifier  NOT NULL,
    [InvoiceId] uniqueidentifier  NOT NULL,
    [Description] nvarchar(max)  NULL,
    [Amount] nvarchar(max)  NULL
);
GO

-- Creating table 'RouteTasks'
CREATE TABLE [dbo].[RouteTasks] (
    [Id] uniqueidentifier  NOT NULL,
    [LocationId] uniqueidentifier  NULL,
    [RouteDestinationId] uniqueidentifier  NULL,
    [ClientId] uniqueidentifier  NULL,
    [ServiceId] uniqueidentifier  NULL,
    [ReadOnly] bit  NOT NULL,
    [BusinessAccountId] uniqueidentifier  NOT NULL,
    [EstimatedDuration] time  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [StatusInt] int  NOT NULL,
    [Date] datetime  NOT NULL,
    [OrderInRouteDestination] int  NOT NULL,
    [RecurringServiceId] uniqueidentifier  NULL,
    [DelayedChildId] uniqueidentifier  NULL
);
GO

-- Creating table 'Errors'
CREATE TABLE [dbo].[Errors] (
    [Id] uniqueidentifier  NOT NULL,
    [Date] datetime  NULL,
    [BusinessName] nvarchar(max)  NULL,
    [UserEmail] nvarchar(max)  NULL,
    [ErrorText] nvarchar(max)  NULL,
    [InnerException] nvarchar(max)  NULL
);
GO

-- Creating table 'ServiceTemplateWithVendorIds'
CREATE TABLE [dbo].[ServiceTemplateWithVendorIds] (
    [ServiceTemplateId] uniqueidentifier  NOT NULL,
    [BusinessAccountId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'Parties_Business'
CREATE TABLE [dbo].[Parties_Business] (
    [Name] nvarchar(max)  NULL,
    [Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'Parties_BusinessAccount'
CREATE TABLE [dbo].[Parties_BusinessAccount] (
    [QuickBooksEnabled] bit  NOT NULL,
    [QuickBooksAccessToken] nvarchar(max)  NULL,
    [QuickBooksAccessTokenSecret] nvarchar(max)  NULL,
    [RouteManifestSettings] nvarchar(max)  NULL,
    [QuickBooksSessionXml] nvarchar(max)  NULL,
    [MaxRoutes] int  NOT NULL,
    [Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'Parties_Person'
CREATE TABLE [dbo].[Parties_Person] (
    [FirstName] nvarchar(max)  NULL,
    [LastName] nvarchar(max)  NULL,
    [MiddleInitial] nvarchar(max)  NULL,
    [GenderInt] smallint  NULL,
    [DateOfBirth] datetime  NULL,
    [Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'Parties_UserAccount'
CREATE TABLE [dbo].[Parties_UserAccount] (
    [PasswordHash] nvarchar(max)  NULL,
    [EmailAddress] nvarchar(max)  NOT NULL,
    [LastActivity] datetime  NULL,
    [CreationDate] datetime  NOT NULL,
    [Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'Fields_OptionsField'
CREATE TABLE [dbo].[Fields_OptionsField] (
    [AllowMultipleSelection] bit  NOT NULL,
    [TypeInt] smallint  NOT NULL,
    [Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'Fields_LocationField'
CREATE TABLE [dbo].[Fields_LocationField] (
    [LocationId] uniqueidentifier  NULL,
    [LocationFieldTypeInt] smallint  NOT NULL,
    [Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'Files_PartyImage'
CREATE TABLE [dbo].[Files_PartyImage] (
    [Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'Fields_TextBoxField'
CREATE TABLE [dbo].[Fields_TextBoxField] (
    [IsMultiline] bit  NOT NULL,
    [Value] nvarchar(max)  NULL,
    [Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'Fields_NumericField'
CREATE TABLE [dbo].[Fields_NumericField] (
    [Mask] nvarchar(max)  NOT NULL,
    [DecimalPlaces] int  NOT NULL,
    [Minimum] decimal(16,6)  NOT NULL,
    [Maximum] decimal(16,6)  NOT NULL,
    [Value] decimal(16,6)  NULL,
    [Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'Fields_DateTimeField'
CREATE TABLE [dbo].[Fields_DateTimeField] (
    [Earliest] datetime  NOT NULL,
    [Latest] datetime  NOT NULL,
    [TypeInt] smallint  NOT NULL,
    [Value] datetime  NULL,
    [Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'Options_LocationOption'
CREATE TABLE [dbo].[Options_LocationOption] (
    [Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'RoleBlock'
CREATE TABLE [dbo].[RoleBlock] (
    [Roles_Id] uniqueidentifier  NOT NULL,
    [Blocks_Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'RouteVehicle'
CREATE TABLE [dbo].[RouteVehicle] (
    [Routes_Id] uniqueidentifier  NOT NULL,
    [Vehicles_Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'PartyRole'
CREATE TABLE [dbo].[PartyRole] (
    [MemberParties_Id] uniqueidentifier  NOT NULL,
    [RoleMembership_Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'RouteEmployee'
CREATE TABLE [dbo].[RouteEmployee] (
    [Routes_Id] uniqueidentifier  NOT NULL,
    [Employees_Id] uniqueidentifier  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Blocks'
ALTER TABLE [dbo].[Blocks]
ADD CONSTRAINT [PK_Blocks]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Roles'
ALTER TABLE [dbo].[Roles]
ADD CONSTRAINT [PK_Roles]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Parties'
ALTER TABLE [dbo].[Parties]
ADD CONSTRAINT [PK_Parties]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ContactInfoSet'
ALTER TABLE [dbo].[ContactInfoSet]
ADD CONSTRAINT [PK_ContactInfoSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Services'
ALTER TABLE [dbo].[Services]
ADD CONSTRAINT [PK_Services]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Fields'
ALTER TABLE [dbo].[Fields]
ADD CONSTRAINT [PK_Fields]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Locations'
ALTER TABLE [dbo].[Locations]
ADD CONSTRAINT [PK_Locations]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'RouteDestinations'
ALTER TABLE [dbo].[RouteDestinations]
ADD CONSTRAINT [PK_RouteDestinations]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Routes'
ALTER TABLE [dbo].[Routes]
ADD CONSTRAINT [PK_Routes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserAccountLog'
ALTER TABLE [dbo].[UserAccountLog]
ADD CONSTRAINT [PK_UserAccountLog]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Vehicles'
ALTER TABLE [dbo].[Vehicles]
ADD CONSTRAINT [PK_Vehicles]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'VehicleMaintenanceLog'
ALTER TABLE [dbo].[VehicleMaintenanceLog]
ADD CONSTRAINT [PK_VehicleMaintenanceLog]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'VehicleMaintenanceLineItems'
ALTER TABLE [dbo].[VehicleMaintenanceLineItems]
ADD CONSTRAINT [PK_VehicleMaintenanceLineItems]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Clients'
ALTER TABLE [dbo].[Clients]
ADD CONSTRAINT [PK_Clients]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ServiceTemplates'
ALTER TABLE [dbo].[ServiceTemplates]
ADD CONSTRAINT [PK_ServiceTemplates]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Repeats'
ALTER TABLE [dbo].[Repeats]
ADD CONSTRAINT [PK_Repeats]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'RecurringServices'
ALTER TABLE [dbo].[RecurringServices]
ADD CONSTRAINT [PK_RecurringServices]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Regions'
ALTER TABLE [dbo].[Regions]
ADD CONSTRAINT [PK_Regions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Files'
ALTER TABLE [dbo].[Files]
ADD CONSTRAINT [PK_Files]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SubLocations'
ALTER TABLE [dbo].[SubLocations]
ADD CONSTRAINT [PK_SubLocations]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Employees'
ALTER TABLE [dbo].[Employees]
ADD CONSTRAINT [PK_Employees]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'EmployeeHistoryEntries'
ALTER TABLE [dbo].[EmployeeHistoryEntries]
ADD CONSTRAINT [PK_EmployeeHistoryEntries]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Options'
ALTER TABLE [dbo].[Options]
ADD CONSTRAINT [PK_Options]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Invoices'
ALTER TABLE [dbo].[Invoices]
ADD CONSTRAINT [PK_Invoices]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SalesTerms'
ALTER TABLE [dbo].[SalesTerms]
ADD CONSTRAINT [PK_SalesTerms]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'LineItems'
ALTER TABLE [dbo].[LineItems]
ADD CONSTRAINT [PK_LineItems]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'RouteTasks'
ALTER TABLE [dbo].[RouteTasks]
ADD CONSTRAINT [PK_RouteTasks]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Errors'
ALTER TABLE [dbo].[Errors]
ADD CONSTRAINT [PK_Errors]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [ServiceTemplateId], [BusinessAccountId] in table 'ServiceTemplateWithVendorIds'
ALTER TABLE [dbo].[ServiceTemplateWithVendorIds]
ADD CONSTRAINT [PK_ServiceTemplateWithVendorIds]
    PRIMARY KEY CLUSTERED ([ServiceTemplateId], [BusinessAccountId] ASC);
GO

-- Creating primary key on [Id] in table 'Parties_Business'
ALTER TABLE [dbo].[Parties_Business]
ADD CONSTRAINT [PK_Parties_Business]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Parties_BusinessAccount'
ALTER TABLE [dbo].[Parties_BusinessAccount]
ADD CONSTRAINT [PK_Parties_BusinessAccount]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Parties_Person'
ALTER TABLE [dbo].[Parties_Person]
ADD CONSTRAINT [PK_Parties_Person]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Parties_UserAccount'
ALTER TABLE [dbo].[Parties_UserAccount]
ADD CONSTRAINT [PK_Parties_UserAccount]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Fields_OptionsField'
ALTER TABLE [dbo].[Fields_OptionsField]
ADD CONSTRAINT [PK_Fields_OptionsField]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Fields_LocationField'
ALTER TABLE [dbo].[Fields_LocationField]
ADD CONSTRAINT [PK_Fields_LocationField]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Files_PartyImage'
ALTER TABLE [dbo].[Files_PartyImage]
ADD CONSTRAINT [PK_Files_PartyImage]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Fields_TextBoxField'
ALTER TABLE [dbo].[Fields_TextBoxField]
ADD CONSTRAINT [PK_Fields_TextBoxField]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Fields_NumericField'
ALTER TABLE [dbo].[Fields_NumericField]
ADD CONSTRAINT [PK_Fields_NumericField]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Fields_DateTimeField'
ALTER TABLE [dbo].[Fields_DateTimeField]
ADD CONSTRAINT [PK_Fields_DateTimeField]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Options_LocationOption'
ALTER TABLE [dbo].[Options_LocationOption]
ADD CONSTRAINT [PK_Options_LocationOption]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Roles_Id], [Blocks_Id] in table 'RoleBlock'
ALTER TABLE [dbo].[RoleBlock]
ADD CONSTRAINT [PK_RoleBlock]
    PRIMARY KEY NONCLUSTERED ([Roles_Id], [Blocks_Id] ASC);
GO

-- Creating primary key on [Routes_Id], [Vehicles_Id] in table 'RouteVehicle'
ALTER TABLE [dbo].[RouteVehicle]
ADD CONSTRAINT [PK_RouteVehicle]
    PRIMARY KEY NONCLUSTERED ([Routes_Id], [Vehicles_Id] ASC);
GO

-- Creating primary key on [MemberParties_Id], [RoleMembership_Id] in table 'PartyRole'
ALTER TABLE [dbo].[PartyRole]
ADD CONSTRAINT [PK_PartyRole]
    PRIMARY KEY NONCLUSTERED ([MemberParties_Id], [RoleMembership_Id] ASC);
GO

-- Creating primary key on [Routes_Id], [Employees_Id] in table 'RouteEmployee'
ALTER TABLE [dbo].[RouteEmployee]
ADD CONSTRAINT [PK_RouteEmployee]
    PRIMARY KEY NONCLUSTERED ([Routes_Id], [Employees_Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [Roles_Id] in table 'RoleBlock'
ALTER TABLE [dbo].[RoleBlock]
ADD CONSTRAINT [FK_RoleBlock_Role]
    FOREIGN KEY ([Roles_Id])
    REFERENCES [dbo].[Roles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Blocks_Id] in table 'RoleBlock'
ALTER TABLE [dbo].[RoleBlock]
ADD CONSTRAINT [FK_RoleBlock_Block]
    FOREIGN KEY ([Blocks_Id])
    REFERENCES [dbo].[Blocks]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RoleBlock_Block'
CREATE INDEX [IX_FK_RoleBlock_Block]
ON [dbo].[RoleBlock]
    ([Blocks_Id]);
GO

-- Creating foreign key on [LocationId] in table 'ContactInfoSet'
ALTER TABLE [dbo].[ContactInfoSet]
ADD CONSTRAINT [FK_LocationContactInfo]
    FOREIGN KEY ([LocationId])
    REFERENCES [dbo].[Locations]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_LocationContactInfo'
CREATE INDEX [IX_FK_LocationContactInfo]
ON [dbo].[ContactInfoSet]
    ([LocationId]);
GO

-- Creating foreign key on [VehicleId] in table 'VehicleMaintenanceLog'
ALTER TABLE [dbo].[VehicleMaintenanceLog]
ADD CONSTRAINT [FK_VehicleMaintenanceLogEntryVehicle]
    FOREIGN KEY ([VehicleId])
    REFERENCES [dbo].[Vehicles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_VehicleMaintenanceLogEntryVehicle'
CREATE INDEX [IX_FK_VehicleMaintenanceLogEntryVehicle]
ON [dbo].[VehicleMaintenanceLog]
    ([VehicleId]);
GO

-- Creating foreign key on [Routes_Id] in table 'RouteVehicle'
ALTER TABLE [dbo].[RouteVehicle]
ADD CONSTRAINT [FK_RouteVehicle_Route]
    FOREIGN KEY ([Routes_Id])
    REFERENCES [dbo].[Routes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Vehicles_Id] in table 'RouteVehicle'
ALTER TABLE [dbo].[RouteVehicle]
ADD CONSTRAINT [FK_RouteVehicle_Vehicle]
    FOREIGN KEY ([Vehicles_Id])
    REFERENCES [dbo].[Vehicles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RouteVehicle_Vehicle'
CREATE INDEX [IX_FK_RouteVehicle_Vehicle]
ON [dbo].[RouteVehicle]
    ([Vehicles_Id]);
GO

-- Creating foreign key on [LocationId] in table 'RouteTasks'
ALTER TABLE [dbo].[RouteTasks]
ADD CONSTRAINT [FK_RouteTaskLocation]
    FOREIGN KEY ([LocationId])
    REFERENCES [dbo].[Locations]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RouteTaskLocation'
CREATE INDEX [IX_FK_RouteTaskLocation]
ON [dbo].[RouteTasks]
    ([LocationId]);
GO

-- Creating foreign key on [LocationId] in table 'RouteDestinations'
ALTER TABLE [dbo].[RouteDestinations]
ADD CONSTRAINT [FK_RouteDestinationLocation]
    FOREIGN KEY ([LocationId])
    REFERENCES [dbo].[Locations]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RouteDestinationLocation'
CREATE INDEX [IX_FK_RouteDestinationLocation]
ON [dbo].[RouteDestinations]
    ([LocationId]);
GO

-- Creating foreign key on [RouteDestinationId] in table 'RouteTasks'
ALTER TABLE [dbo].[RouteTasks]
ADD CONSTRAINT [FK_RouteTaskRouteDestination]
    FOREIGN KEY ([RouteDestinationId])
    REFERENCES [dbo].[RouteDestinations]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RouteTaskRouteDestination'
CREATE INDEX [IX_FK_RouteTaskRouteDestination]
ON [dbo].[RouteTasks]
    ([RouteDestinationId]);
GO

-- Creating foreign key on [RouteId] in table 'RouteDestinations'
ALTER TABLE [dbo].[RouteDestinations]
ADD CONSTRAINT [FK_RouteDestinationRoute]
    FOREIGN KEY ([RouteId])
    REFERENCES [dbo].[Routes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RouteDestinationRoute'
CREATE INDEX [IX_FK_RouteDestinationRoute]
ON [dbo].[RouteDestinations]
    ([RouteId]);
GO

-- Creating foreign key on [VehicleMaintenanceLogEntryId] in table 'VehicleMaintenanceLineItems'
ALTER TABLE [dbo].[VehicleMaintenanceLineItems]
ADD CONSTRAINT [FK_VehicleMaintenanceLogEntryLineItem]
    FOREIGN KEY ([VehicleMaintenanceLogEntryId])
    REFERENCES [dbo].[VehicleMaintenanceLog]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_VehicleMaintenanceLogEntryLineItem'
CREATE INDEX [IX_FK_VehicleMaintenanceLogEntryLineItem]
ON [dbo].[VehicleMaintenanceLineItems]
    ([VehicleMaintenanceLogEntryId]);
GO

-- Creating foreign key on [ClientId] in table 'RouteTasks'
ALTER TABLE [dbo].[RouteTasks]
ADD CONSTRAINT [FK_RouteTaskClient]
    FOREIGN KEY ([ClientId])
    REFERENCES [dbo].[Clients]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RouteTaskClient'
CREATE INDEX [IX_FK_RouteTaskClient]
ON [dbo].[RouteTasks]
    ([ClientId]);
GO

-- Creating foreign key on [ClientId] in table 'Services'
ALTER TABLE [dbo].[Services]
ADD CONSTRAINT [FK_ServiceClient]
    FOREIGN KEY ([ClientId])
    REFERENCES [dbo].[Clients]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ServiceClient'
CREATE INDEX [IX_FK_ServiceClient]
ON [dbo].[Services]
    ([ClientId]);
GO

-- Creating foreign key on [ClientId] in table 'RouteDestinations'
ALTER TABLE [dbo].[RouteDestinations]
ADD CONSTRAINT [FK_RouteDestinationClient]
    FOREIGN KEY ([ClientId])
    REFERENCES [dbo].[Clients]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RouteDestinationClient'
CREATE INDEX [IX_FK_RouteDestinationClient]
ON [dbo].[RouteDestinations]
    ([ClientId]);
GO

-- Creating foreign key on [ServiceId] in table 'RouteTasks'
ALTER TABLE [dbo].[RouteTasks]
ADD CONSTRAINT [FK_RouteTaskService]
    FOREIGN KEY ([ServiceId])
    REFERENCES [dbo].[Services]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RouteTaskService'
CREATE INDEX [IX_FK_RouteTaskService]
ON [dbo].[RouteTasks]
    ([ServiceId]);
GO

-- Creating foreign key on [ServiceProviderId] in table 'Services'
ALTER TABLE [dbo].[Services]
ADD CONSTRAINT [FK_BusinessAccountService]
    FOREIGN KEY ([ServiceProviderId])
    REFERENCES [dbo].[Parties_BusinessAccount]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_BusinessAccountService'
CREATE INDEX [IX_FK_BusinessAccountService]
ON [dbo].[Services]
    ([ServiceProviderId]);
GO

-- Creating foreign key on [OwnerBusinessAccountId] in table 'Routes'
ALTER TABLE [dbo].[Routes]
ADD CONSTRAINT [FK_BusinessAccountRoute]
    FOREIGN KEY ([OwnerBusinessAccountId])
    REFERENCES [dbo].[Parties_BusinessAccount]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_BusinessAccountRoute'
CREATE INDEX [IX_FK_BusinessAccountRoute]
ON [dbo].[Routes]
    ([OwnerBusinessAccountId]);
GO

-- Creating foreign key on [BusinessAccountId] in table 'RouteTasks'
ALTER TABLE [dbo].[RouteTasks]
ADD CONSTRAINT [FK_BusinessAccountRouteTask]
    FOREIGN KEY ([BusinessAccountId])
    REFERENCES [dbo].[Parties_BusinessAccount]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_BusinessAccountRouteTask'
CREATE INDEX [IX_FK_BusinessAccountRouteTask]
ON [dbo].[RouteTasks]
    ([BusinessAccountId]);
GO

-- Creating foreign key on [UserAccountId] in table 'UserAccountLog'
ALTER TABLE [dbo].[UserAccountLog]
ADD CONSTRAINT [FK_UserAccountUserAccountLogEntry]
    FOREIGN KEY ([UserAccountId])
    REFERENCES [dbo].[Parties_UserAccount]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserAccountUserAccountLogEntry'
CREATE INDEX [IX_FK_UserAccountUserAccountLogEntry]
ON [dbo].[UserAccountLog]
    ([UserAccountId]);
GO

-- Creating foreign key on [OwnerServiceProviderId] in table 'ServiceTemplates'
ALTER TABLE [dbo].[ServiceTemplates]
ADD CONSTRAINT [FK_ServiceTemplateBusinessAccount]
    FOREIGN KEY ([OwnerServiceProviderId])
    REFERENCES [dbo].[Parties_BusinessAccount]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ServiceTemplateBusinessAccount'
CREATE INDEX [IX_FK_ServiceTemplateBusinessAccount]
ON [dbo].[ServiceTemplates]
    ([OwnerServiceProviderId]);
GO

-- Creating foreign key on [Id] in table 'RecurringServices'
ALTER TABLE [dbo].[RecurringServices]
ADD CONSTRAINT [FK_RecurringServiceServiceTemplate]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[ServiceTemplates]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [RecurringServiceId] in table 'Services'
ALTER TABLE [dbo].[Services]
ADD CONSTRAINT [FK_RecurringServiceService]
    FOREIGN KEY ([RecurringServiceId])
    REFERENCES [dbo].[RecurringServices]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RecurringServiceService'
CREATE INDEX [IX_FK_RecurringServiceService]
ON [dbo].[Services]
    ([RecurringServiceId]);
GO

-- Creating foreign key on [ClientId] in table 'RecurringServices'
ALTER TABLE [dbo].[RecurringServices]
ADD CONSTRAINT [FK_RecurringServiceClient]
    FOREIGN KEY ([ClientId])
    REFERENCES [dbo].[Clients]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RecurringServiceClient'
CREATE INDEX [IX_FK_RecurringServiceClient]
ON [dbo].[RecurringServices]
    ([ClientId]);
GO

-- Creating foreign key on [OwnerClientId] in table 'ServiceTemplates'
ALTER TABLE [dbo].[ServiceTemplates]
ADD CONSTRAINT [FK_ClientServiceTemplate]
    FOREIGN KEY ([OwnerClientId])
    REFERENCES [dbo].[Clients]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ClientServiceTemplate'
CREATE INDEX [IX_FK_ClientServiceTemplate]
ON [dbo].[ServiceTemplates]
    ([OwnerClientId]);
GO

-- Creating foreign key on [OwnerPartyId] in table 'Vehicles'
ALTER TABLE [dbo].[Vehicles]
ADD CONSTRAINT [FK_VehicleParty]
    FOREIGN KEY ([OwnerPartyId])
    REFERENCES [dbo].[Parties]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_VehicleParty'
CREATE INDEX [IX_FK_VehicleParty]
ON [dbo].[Vehicles]
    ([OwnerPartyId]);
GO

-- Creating foreign key on [Id] in table 'Services'
ALTER TABLE [dbo].[Services]
ADD CONSTRAINT [FK_ServiceServiceTemplate]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[ServiceTemplates]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [MemberParties_Id] in table 'PartyRole'
ALTER TABLE [dbo].[PartyRole]
ADD CONSTRAINT [FK_PartyRole_Party]
    FOREIGN KEY ([MemberParties_Id])
    REFERENCES [dbo].[Parties]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [RoleMembership_Id] in table 'PartyRole'
ALTER TABLE [dbo].[PartyRole]
ADD CONSTRAINT [FK_PartyRole_Role]
    FOREIGN KEY ([RoleMembership_Id])
    REFERENCES [dbo].[Roles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_PartyRole_Role'
CREATE INDEX [IX_FK_PartyRole_Role]
ON [dbo].[PartyRole]
    ([RoleMembership_Id]);
GO

-- Creating foreign key on [OwnerPartyId] in table 'Roles'
ALTER TABLE [dbo].[Roles]
ADD CONSTRAINT [FK_PartyRole1]
    FOREIGN KEY ([OwnerPartyId])
    REFERENCES [dbo].[Parties]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_PartyRole1'
CREATE INDEX [IX_FK_PartyRole1]
ON [dbo].[Roles]
    ([OwnerPartyId]);
GO

-- Creating foreign key on [RegionId] in table 'Locations'
ALTER TABLE [dbo].[Locations]
ADD CONSTRAINT [FK_RegionLocation]
    FOREIGN KEY ([RegionId])
    REFERENCES [dbo].[Regions]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RegionLocation'
CREATE INDEX [IX_FK_RegionLocation]
ON [dbo].[Locations]
    ([RegionId]);
GO

-- Creating foreign key on [BusinessAccountId] in table 'Regions'
ALTER TABLE [dbo].[Regions]
ADD CONSTRAINT [FK_BusinessAccountRegion]
    FOREIGN KEY ([BusinessAccountId])
    REFERENCES [dbo].[Parties_BusinessAccount]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_BusinessAccountRegion'
CREATE INDEX [IX_FK_BusinessAccountRegion]
ON [dbo].[Regions]
    ([BusinessAccountId]);
GO

-- Creating foreign key on [LocationId] in table 'Files'
ALTER TABLE [dbo].[Files]
ADD CONSTRAINT [FK_LocationFile]
    FOREIGN KEY ([LocationId])
    REFERENCES [dbo].[Locations]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_LocationFile'
CREATE INDEX [IX_FK_LocationFile]
ON [dbo].[Files]
    ([LocationId]);
GO

-- Creating foreign key on [LocationId] in table 'SubLocations'
ALTER TABLE [dbo].[SubLocations]
ADD CONSTRAINT [FK_LocationSubLocation]
    FOREIGN KEY ([LocationId])
    REFERENCES [dbo].[Locations]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_LocationSubLocation'
CREATE INDEX [IX_FK_LocationSubLocation]
ON [dbo].[SubLocations]
    ([LocationId]);
GO

-- Creating foreign key on [LinkedUserAccountId] in table 'Employees'
ALTER TABLE [dbo].[Employees]
ADD CONSTRAINT [FK_EmployeeUserAccount]
    FOREIGN KEY ([LinkedUserAccountId])
    REFERENCES [dbo].[Parties_UserAccount]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_EmployeeUserAccount'
CREATE INDEX [IX_FK_EmployeeUserAccount]
ON [dbo].[Employees]
    ([LinkedUserAccountId]);
GO

-- Creating foreign key on [EmployerId] in table 'Employees'
ALTER TABLE [dbo].[Employees]
ADD CONSTRAINT [FK_EmployeeBusinessAccount]
    FOREIGN KEY ([EmployerId])
    REFERENCES [dbo].[Parties_BusinessAccount]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_EmployeeBusinessAccount'
CREATE INDEX [IX_FK_EmployeeBusinessAccount]
ON [dbo].[Employees]
    ([EmployerId]);
GO

-- Creating foreign key on [Id] in table 'Employees'
ALTER TABLE [dbo].[Employees]
ADD CONSTRAINT [FK_EmployeePerson]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Parties_Person]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'RecurringServices'
ALTER TABLE [dbo].[RecurringServices]
ADD CONSTRAINT [FK_RecurringServiceRepeat]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Repeats]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [EmployeeId] in table 'EmployeeHistoryEntries'
ALTER TABLE [dbo].[EmployeeHistoryEntries]
ADD CONSTRAINT [FK_EmployeeHistoryEntryEmployee]
    FOREIGN KEY ([EmployeeId])
    REFERENCES [dbo].[Employees]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_EmployeeHistoryEntryEmployee'
CREATE INDEX [IX_FK_EmployeeHistoryEntryEmployee]
ON [dbo].[EmployeeHistoryEntries]
    ([EmployeeId]);
GO

-- Creating foreign key on [OwnerServiceTemplateId] in table 'ServiceTemplates'
ALTER TABLE [dbo].[ServiceTemplates]
ADD CONSTRAINT [FK_ServiceTemplateServiceTemplate]
    FOREIGN KEY ([OwnerServiceTemplateId])
    REFERENCES [dbo].[ServiceTemplates]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ServiceTemplateServiceTemplate'
CREATE INDEX [IX_FK_ServiceTemplateServiceTemplate]
ON [dbo].[ServiceTemplates]
    ([OwnerServiceTemplateId]);
GO

-- Creating foreign key on [ParentFieldId] in table 'Fields'
ALTER TABLE [dbo].[Fields]
ADD CONSTRAINT [FK_FieldField]
    FOREIGN KEY ([ParentFieldId])
    REFERENCES [dbo].[Fields]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_FieldField'
CREATE INDEX [IX_FK_FieldField]
ON [dbo].[Fields]
    ([ParentFieldId]);
GO

-- Creating foreign key on [OptionsFieldId] in table 'Options'
ALTER TABLE [dbo].[Options]
ADD CONSTRAINT [FK_OptionsFieldOption]
    FOREIGN KEY ([OptionsFieldId])
    REFERENCES [dbo].[Fields_OptionsField]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_OptionsFieldOption'
CREATE INDEX [IX_FK_OptionsFieldOption]
ON [dbo].[Options]
    ([OptionsFieldId]);
GO

-- Creating foreign key on [ServiceTemplateId] in table 'Fields'
ALTER TABLE [dbo].[Fields]
ADD CONSTRAINT [FK_ServiceTemplateField]
    FOREIGN KEY ([ServiceTemplateId])
    REFERENCES [dbo].[ServiceTemplates]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ServiceTemplateField'
CREATE INDEX [IX_FK_ServiceTemplateField]
ON [dbo].[Fields]
    ([ServiceTemplateId]);
GO

-- Creating foreign key on [LocationId] in table 'Fields_LocationField'
ALTER TABLE [dbo].[Fields_LocationField]
ADD CONSTRAINT [FK_LocationFieldLocation]
    FOREIGN KEY ([LocationId])
    REFERENCES [dbo].[Locations]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_LocationFieldLocation'
CREATE INDEX [IX_FK_LocationFieldLocation]
ON [dbo].[Fields_LocationField]
    ([LocationId]);
GO

-- Creating foreign key on [LocationId] in table 'Invoices'
ALTER TABLE [dbo].[Invoices]
ADD CONSTRAINT [FK_InvoiceLocation]
    FOREIGN KEY ([LocationId])
    REFERENCES [dbo].[Locations]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_InvoiceLocation'
CREATE INDEX [IX_FK_InvoiceLocation]
ON [dbo].[Invoices]
    ([LocationId]);
GO

-- Creating foreign key on [SalesTermId] in table 'Invoices'
ALTER TABLE [dbo].[Invoices]
ADD CONSTRAINT [FK_InvoiceSalesTerm]
    FOREIGN KEY ([SalesTermId])
    REFERENCES [dbo].[SalesTerms]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_InvoiceSalesTerm'
CREATE INDEX [IX_FK_InvoiceSalesTerm]
ON [dbo].[Invoices]
    ([SalesTermId]);
GO

-- Creating foreign key on [Id] in table 'Invoices'
ALTER TABLE [dbo].[Invoices]
ADD CONSTRAINT [FK_InvoiceServiceTemplate]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[ServiceTemplates]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [InvoiceId] in table 'LineItems'
ALTER TABLE [dbo].[LineItems]
ADD CONSTRAINT [FK_LineItemInvoice]
    FOREIGN KEY ([InvoiceId])
    REFERENCES [dbo].[Invoices]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_LineItemInvoice'
CREATE INDEX [IX_FK_LineItemInvoice]
ON [dbo].[LineItems]
    ([InvoiceId]);
GO

-- Creating foreign key on [PartyId] in table 'Files'
ALTER TABLE [dbo].[Files]
ADD CONSTRAINT [FK_FileParty]
    FOREIGN KEY ([PartyId])
    REFERENCES [dbo].[Parties]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_FileParty'
CREATE INDEX [IX_FK_FileParty]
ON [dbo].[Files]
    ([PartyId]);
GO

-- Creating foreign key on [Id] in table 'Files_PartyImage'
ALTER TABLE [dbo].[Files_PartyImage]
ADD CONSTRAINT [FK_PartyPartyImage]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Parties]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [BusinessAccountId] in table 'Invoices'
ALTER TABLE [dbo].[Invoices]
ADD CONSTRAINT [FK_BusinessAccountInvoice]
    FOREIGN KEY ([BusinessAccountId])
    REFERENCES [dbo].[Parties_BusinessAccount]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_BusinessAccountInvoice'
CREATE INDEX [IX_FK_BusinessAccountInvoice]
ON [dbo].[Invoices]
    ([BusinessAccountId]);
GO

-- Creating foreign key on [ClientId] in table 'Invoices'
ALTER TABLE [dbo].[Invoices]
ADD CONSTRAINT [FK_ClientInvoice]
    FOREIGN KEY ([ClientId])
    REFERENCES [dbo].[Clients]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ClientInvoice'
CREATE INDEX [IX_FK_ClientInvoice]
ON [dbo].[Invoices]
    ([ClientId]);
GO

-- Creating foreign key on [RecurringServiceId] in table 'RouteTasks'
ALTER TABLE [dbo].[RouteTasks]
ADD CONSTRAINT [FK_RouteTaskRecurringService]
    FOREIGN KEY ([RecurringServiceId])
    REFERENCES [dbo].[RecurringServices]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RouteTaskRecurringService'
CREATE INDEX [IX_FK_RouteTaskRecurringService]
ON [dbo].[RouteTasks]
    ([RecurringServiceId]);
GO

-- Creating foreign key on [BusinessAccountIdIfDepot] in table 'Locations'
ALTER TABLE [dbo].[Locations]
ADD CONSTRAINT [FK_BusinessAccountLocation]
    FOREIGN KEY ([BusinessAccountIdIfDepot])
    REFERENCES [dbo].[Parties_BusinessAccount]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_BusinessAccountLocation'
CREATE INDEX [IX_FK_BusinessAccountLocation]
ON [dbo].[Locations]
    ([BusinessAccountIdIfDepot]);
GO

-- Creating foreign key on [BusinessAccountId] in table 'Clients'
ALTER TABLE [dbo].[Clients]
ADD CONSTRAINT [FK_ClientBusinessAccount]
    FOREIGN KEY ([BusinessAccountId])
    REFERENCES [dbo].[Parties_BusinessAccount]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ClientBusinessAccount'
CREATE INDEX [IX_FK_ClientBusinessAccount]
ON [dbo].[Clients]
    ([BusinessAccountId]);
GO

-- Creating foreign key on [BusinessAccountId] in table 'Locations'
ALTER TABLE [dbo].[Locations]
ADD CONSTRAINT [FK_LocationBusinessAccount]
    FOREIGN KEY ([BusinessAccountId])
    REFERENCES [dbo].[Parties_BusinessAccount]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_LocationBusinessAccount'
CREATE INDEX [IX_FK_LocationBusinessAccount]
ON [dbo].[Locations]
    ([BusinessAccountId]);
GO

-- Creating foreign key on [ClientId] in table 'Locations'
ALTER TABLE [dbo].[Locations]
ADD CONSTRAINT [FK_ClientLocation1]
    FOREIGN KEY ([ClientId])
    REFERENCES [dbo].[Clients]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ClientLocation1'
CREATE INDEX [IX_FK_ClientLocation1]
ON [dbo].[Locations]
    ([ClientId]);
GO

-- Creating foreign key on [ClientId] in table 'ContactInfoSet'
ALTER TABLE [dbo].[ContactInfoSet]
ADD CONSTRAINT [FK_ClientContactInfo]
    FOREIGN KEY ([ClientId])
    REFERENCES [dbo].[Clients]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ClientContactInfo'
CREATE INDEX [IX_FK_ClientContactInfo]
ON [dbo].[ContactInfoSet]
    ([ClientId]);
GO

-- Creating foreign key on [Routes_Id] in table 'RouteEmployee'
ALTER TABLE [dbo].[RouteEmployee]
ADD CONSTRAINT [FK_RouteEmployee_Route]
    FOREIGN KEY ([Routes_Id])
    REFERENCES [dbo].[Routes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Employees_Id] in table 'RouteEmployee'
ALTER TABLE [dbo].[RouteEmployee]
ADD CONSTRAINT [FK_RouteEmployee_Employee]
    FOREIGN KEY ([Employees_Id])
    REFERENCES [dbo].[Employees]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RouteEmployee_Employee'
CREATE INDEX [IX_FK_RouteEmployee_Employee]
ON [dbo].[RouteEmployee]
    ([Employees_Id]);
GO

-- Creating foreign key on [Id] in table 'Parties_Business'
ALTER TABLE [dbo].[Parties_Business]
ADD CONSTRAINT [FK_Business_inherits_Party]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Parties]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'Parties_BusinessAccount'
ALTER TABLE [dbo].[Parties_BusinessAccount]
ADD CONSTRAINT [FK_BusinessAccount_inherits_Business]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Parties_Business]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'Parties_Person'
ALTER TABLE [dbo].[Parties_Person]
ADD CONSTRAINT [FK_Person_inherits_Party]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Parties]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'Parties_UserAccount'
ALTER TABLE [dbo].[Parties_UserAccount]
ADD CONSTRAINT [FK_UserAccount_inherits_Person]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Parties_Person]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'Fields_OptionsField'
ALTER TABLE [dbo].[Fields_OptionsField]
ADD CONSTRAINT [FK_OptionsField_inherits_Field]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Fields]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'Fields_LocationField'
ALTER TABLE [dbo].[Fields_LocationField]
ADD CONSTRAINT [FK_LocationField_inherits_Field]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Fields]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'Files_PartyImage'
ALTER TABLE [dbo].[Files_PartyImage]
ADD CONSTRAINT [FK_PartyImage_inherits_File]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Files]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'Fields_TextBoxField'
ALTER TABLE [dbo].[Fields_TextBoxField]
ADD CONSTRAINT [FK_TextBoxField_inherits_Field]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Fields]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'Fields_NumericField'
ALTER TABLE [dbo].[Fields_NumericField]
ADD CONSTRAINT [FK_NumericField_inherits_Field]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Fields]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'Fields_DateTimeField'
ALTER TABLE [dbo].[Fields_DateTimeField]
ADD CONSTRAINT [FK_DateTimeField_inherits_Field]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Fields]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'Options_LocationOption'
ALTER TABLE [dbo].[Options_LocationOption]
ADD CONSTRAINT [FK_LocationOption_inherits_Option]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Options]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------