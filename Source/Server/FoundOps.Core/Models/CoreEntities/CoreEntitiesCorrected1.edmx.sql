 

-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 03/23/2012 12:44:49
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

IF OBJECT_ID(N'[dbo].[FK_Business_inherits_Party]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Parties_Business] DROP CONSTRAINT [FK_Business_inherits_Party];
GO
IF OBJECT_ID(N'[dbo].[FK_BusinessAccount_inherits_Business]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Parties_BusinessAccount] DROP CONSTRAINT [FK_BusinessAccount_inherits_Business];
GO
IF OBJECT_ID(N'[dbo].[FK_BusinessAccountClient]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Clients] DROP CONSTRAINT [FK_BusinessAccountClient];
GO
IF OBJECT_ID(N'[dbo].[FK_BusinessAccountInvoice]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Invoices] DROP CONSTRAINT [FK_BusinessAccountInvoice];
GO
IF OBJECT_ID(N'[dbo].[FK_BusinessAccountRegion]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Regions] DROP CONSTRAINT [FK_BusinessAccountRegion];
GO
IF OBJECT_ID(N'[dbo].[FK_BusinessAccountRoute]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Routes] DROP CONSTRAINT [FK_BusinessAccountRoute];
GO
IF OBJECT_ID(N'[dbo].[FK_BusinessAccountRouteTask]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RouteTasks] DROP CONSTRAINT [FK_BusinessAccountRouteTask];
GO
IF OBJECT_ID(N'[dbo].[FK_BusinessAccountService]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Services] DROP CONSTRAINT [FK_BusinessAccountService];
GO
IF OBJECT_ID(N'[dbo].[FK_ClientInvoice]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Invoices] DROP CONSTRAINT [FK_ClientInvoice];
GO
IF OBJECT_ID(N'[dbo].[FK_ClientLocation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Clients] DROP CONSTRAINT [FK_ClientLocation];
GO
IF OBJECT_ID(N'[dbo].[FK_ClientParty]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Clients] DROP CONSTRAINT [FK_ClientParty];
GO
IF OBJECT_ID(N'[dbo].[FK_ClientServiceTemplate]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ServiceTemplates] DROP CONSTRAINT [FK_ClientServiceTemplate];
GO
IF OBJECT_ID(N'[dbo].[FK_ClientTitleClient]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ClientTitles] DROP CONSTRAINT [FK_ClientTitleClient];
GO
IF OBJECT_ID(N'[dbo].[FK_ClientTitleContact]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ClientTitles] DROP CONSTRAINT [FK_ClientTitleContact];
GO
IF OBJECT_ID(N'[dbo].[FK_ContactInfoParty]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ContactInfoSet] DROP CONSTRAINT [FK_ContactInfoParty];
GO
IF OBJECT_ID(N'[dbo].[FK_ContactParty]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Contacts] DROP CONSTRAINT [FK_ContactParty];
GO
IF OBJECT_ID(N'[dbo].[FK_ContactPerson]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Contacts] DROP CONSTRAINT [FK_ContactPerson];
GO
IF OBJECT_ID(N'[dbo].[FK_DateTimeField_inherits_Field]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Fields_DateTimeField] DROP CONSTRAINT [FK_DateTimeField_inherits_Field];
GO
IF OBJECT_ID(N'[dbo].[FK_EmployeeBusinessAccount]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Employees] DROP CONSTRAINT [FK_EmployeeBusinessAccount];
GO
IF OBJECT_ID(N'[dbo].[FK_EmployeeHistoryEntryEmployee]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[EmployeeHistoryEntries] DROP CONSTRAINT [FK_EmployeeHistoryEntryEmployee];
GO
IF OBJECT_ID(N'[dbo].[FK_EmployeePerson]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Employees] DROP CONSTRAINT [FK_EmployeePerson];
GO
IF OBJECT_ID(N'[dbo].[FK_EmployeeUserAccount]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Employees] DROP CONSTRAINT [FK_EmployeeUserAccount];
GO
IF OBJECT_ID(N'[dbo].[FK_FieldField]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Fields] DROP CONSTRAINT [FK_FieldField];
GO
IF OBJECT_ID(N'[dbo].[FK_FileParty]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Files] DROP CONSTRAINT [FK_FileParty];
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
IF OBJECT_ID(N'[dbo].[FK_LocationContactInfo]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ContactInfoSet] DROP CONSTRAINT [FK_LocationContactInfo];
GO
IF OBJECT_ID(N'[dbo].[FK_LocationField_inherits_Field]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Fields_LocationField] DROP CONSTRAINT [FK_LocationField_inherits_Field];
GO
IF OBJECT_ID(N'[dbo].[FK_LocationFieldLocation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Fields_LocationField] DROP CONSTRAINT [FK_LocationFieldLocation];
GO
IF OBJECT_ID(N'[dbo].[FK_LocationFile]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Files] DROP CONSTRAINT [FK_LocationFile];
GO
IF OBJECT_ID(N'[dbo].[FK_LocationOption_inherits_Option]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Options_LocationOption] DROP CONSTRAINT [FK_LocationOption_inherits_Option];
GO
IF OBJECT_ID(N'[dbo].[FK_LocationParty]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Locations] DROP CONSTRAINT [FK_LocationParty];
GO
IF OBJECT_ID(N'[dbo].[FK_LocationParty1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Locations] DROP CONSTRAINT [FK_LocationParty1];
GO
IF OBJECT_ID(N'[dbo].[FK_LocationSubLocation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SubLocations] DROP CONSTRAINT [FK_LocationSubLocation];
GO
IF OBJECT_ID(N'[dbo].[FK_NumericField_inherits_Field]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Fields_NumericField] DROP CONSTRAINT [FK_NumericField_inherits_Field];
GO
IF OBJECT_ID(N'[dbo].[FK_OptionsField_inherits_Field]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Fields_OptionsField] DROP CONSTRAINT [FK_OptionsField_inherits_Field];
GO
IF OBJECT_ID(N'[dbo].[FK_OptionsFieldOption]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Options] DROP CONSTRAINT [FK_OptionsFieldOption];
GO
IF OBJECT_ID(N'[dbo].[FK_PartyImage_inherits_File]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Files_PartyImage] DROP CONSTRAINT [FK_PartyImage_inherits_File];
GO
IF OBJECT_ID(N'[dbo].[FK_PartyPartyImage]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Files_PartyImage] DROP CONSTRAINT [FK_PartyPartyImage];
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
IF OBJECT_ID(N'[dbo].[FK_Person_inherits_Party]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Parties_Person] DROP CONSTRAINT [FK_Person_inherits_Party];
GO
IF OBJECT_ID(N'[dbo].[FK_RecurringServiceClient]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RecurringServices] DROP CONSTRAINT [FK_RecurringServiceClient];
GO
IF OBJECT_ID(N'[dbo].[FK_RecurringServiceRepeat]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RecurringServices] DROP CONSTRAINT [FK_RecurringServiceRepeat];
GO
IF OBJECT_ID(N'[dbo].[FK_RecurringServiceService]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Services] DROP CONSTRAINT [FK_RecurringServiceService];
GO
IF OBJECT_ID(N'[dbo].[FK_RecurringServiceServiceTemplate]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RecurringServices] DROP CONSTRAINT [FK_RecurringServiceServiceTemplate];
GO
IF OBJECT_ID(N'[dbo].[FK_RegionLocation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Locations] DROP CONSTRAINT [FK_RegionLocation];
GO
IF OBJECT_ID(N'[dbo].[FK_RoleBlock_Block]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RoleBlock] DROP CONSTRAINT [FK_RoleBlock_Block];
GO
IF OBJECT_ID(N'[dbo].[FK_RoleBlock_Role]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RoleBlock] DROP CONSTRAINT [FK_RoleBlock_Role];
GO
IF OBJECT_ID(N'[dbo].[FK_RouteDestinationClient]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RouteDestinations] DROP CONSTRAINT [FK_RouteDestinationClient];
GO
IF OBJECT_ID(N'[dbo].[FK_RouteDestinationLocation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RouteDestinations] DROP CONSTRAINT [FK_RouteDestinationLocation];
GO
IF OBJECT_ID(N'[dbo].[FK_RouteDestinationRoute]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RouteDestinations] DROP CONSTRAINT [FK_RouteDestinationRoute];
GO
IF OBJECT_ID(N'[dbo].[FK_RouteEmployee_Employee]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RouteEmployee] DROP CONSTRAINT [FK_RouteEmployee_Employee];
GO
IF OBJECT_ID(N'[dbo].[FK_RouteEmployee_Route]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RouteEmployee] DROP CONSTRAINT [FK_RouteEmployee_Route];
GO
IF OBJECT_ID(N'[dbo].[FK_RouteTaskClient]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RouteTasks] DROP CONSTRAINT [FK_RouteTaskClient];
GO
IF OBJECT_ID(N'[dbo].[FK_RouteTaskLocation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RouteTasks] DROP CONSTRAINT [FK_RouteTaskLocation];
GO
IF OBJECT_ID(N'[dbo].[FK_RouteTaskRouteDestination]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RouteTasks] DROP CONSTRAINT [FK_RouteTaskRouteDestination];
GO
IF OBJECT_ID(N'[dbo].[FK_RouteTaskService]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RouteTasks] DROP CONSTRAINT [FK_RouteTaskService];
GO
IF OBJECT_ID(N'[dbo].[FK_RouteVehicle_Route]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RouteVehicle] DROP CONSTRAINT [FK_RouteVehicle_Route];
GO
IF OBJECT_ID(N'[dbo].[FK_RouteVehicle_Vehicle]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RouteVehicle] DROP CONSTRAINT [FK_RouteVehicle_Vehicle];
GO
IF OBJECT_ID(N'[dbo].[FK_ServiceClient]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Services] DROP CONSTRAINT [FK_ServiceClient];
GO
IF OBJECT_ID(N'[dbo].[FK_ServiceServiceTemplate]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Services] DROP CONSTRAINT [FK_ServiceServiceTemplate];
GO
IF OBJECT_ID(N'[dbo].[FK_ServiceTemplateBusinessAccount]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ServiceTemplates] DROP CONSTRAINT [FK_ServiceTemplateBusinessAccount];
GO
IF OBJECT_ID(N'[dbo].[FK_ServiceTemplateField]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Fields] DROP CONSTRAINT [FK_ServiceTemplateField];
GO
IF OBJECT_ID(N'[dbo].[FK_ServiceTemplateServiceTemplate]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ServiceTemplates] DROP CONSTRAINT [FK_ServiceTemplateServiceTemplate];
GO
IF OBJECT_ID(N'[dbo].[FK_TextBoxField_inherits_Field]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Fields_TextBoxField] DROP CONSTRAINT [FK_TextBoxField_inherits_Field];
GO
IF OBJECT_ID(N'[dbo].[FK_UserAccount_inherits_Person]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Parties_UserAccount] DROP CONSTRAINT [FK_UserAccount_inherits_Person];
GO
IF OBJECT_ID(N'[dbo].[FK_UserAccountTrackPoint]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TrackPoints] DROP CONSTRAINT [FK_UserAccountTrackPoint];
GO
IF OBJECT_ID(N'[dbo].[FK_UserAccountUserAccountLogEntry]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserAccountLog] DROP CONSTRAINT [FK_UserAccountUserAccountLogEntry];
GO
IF OBJECT_ID(N'[dbo].[FK_VehicleMaintenanceLogEntryLineItem]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[VehicleMaintenanceLineItems] DROP CONSTRAINT [FK_VehicleMaintenanceLogEntryLineItem];
GO
IF OBJECT_ID(N'[dbo].[FK_VehicleMaintenanceLogEntryVehicle]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[VehicleMaintenanceLog] DROP CONSTRAINT [FK_VehicleMaintenanceLogEntryVehicle];
GO
IF OBJECT_ID(N'[dbo].[FK_VehicleParty]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Vehicles] DROP CONSTRAINT [FK_VehicleParty];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Blocks]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Blocks];
GO
IF OBJECT_ID(N'[dbo].[Clients]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Clients];
GO
IF OBJECT_ID(N'[dbo].[ClientTitles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ClientTitles];
GO
IF OBJECT_ID(N'[dbo].[ContactInfoSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ContactInfoSet];
GO
IF OBJECT_ID(N'[dbo].[Contacts]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Contacts];
GO
IF OBJECT_ID(N'[dbo].[EmployeeHistoryEntries]', 'U') IS NOT NULL
    DROP TABLE [dbo].[EmployeeHistoryEntries];
GO
IF OBJECT_ID(N'[dbo].[Employees]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Employees];
GO
IF OBJECT_ID(N'[dbo].[Errors]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Errors];
GO
IF OBJECT_ID(N'[dbo].[Fields]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Fields];
GO
IF OBJECT_ID(N'[dbo].[Fields_DateTimeField]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Fields_DateTimeField];
GO
IF OBJECT_ID(N'[dbo].[Fields_LocationField]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Fields_LocationField];
GO
IF OBJECT_ID(N'[dbo].[Fields_NumericField]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Fields_NumericField];
GO
IF OBJECT_ID(N'[dbo].[Fields_OptionsField]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Fields_OptionsField];
GO
IF OBJECT_ID(N'[dbo].[Fields_TextBoxField]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Fields_TextBoxField];
GO
IF OBJECT_ID(N'[dbo].[Files]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Files];
GO
IF OBJECT_ID(N'[dbo].[Files_PartyImage]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Files_PartyImage];
GO
IF OBJECT_ID(N'[dbo].[Invoices]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Invoices];
GO
IF OBJECT_ID(N'[dbo].[LineItems]', 'U') IS NOT NULL
    DROP TABLE [dbo].[LineItems];
GO
IF OBJECT_ID(N'[dbo].[Locations]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Locations];
GO
IF OBJECT_ID(N'[dbo].[Options]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Options];
GO
IF OBJECT_ID(N'[dbo].[Options_LocationOption]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Options_LocationOption];
GO
IF OBJECT_ID(N'[dbo].[Parties]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Parties];
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
IF OBJECT_ID(N'[dbo].[PartyRole]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PartyRole];
GO
IF OBJECT_ID(N'[dbo].[RecurringServices]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RecurringServices];
GO
IF OBJECT_ID(N'[dbo].[Regions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Regions];
GO
IF OBJECT_ID(N'[dbo].[Repeats]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Repeats];
GO
IF OBJECT_ID(N'[dbo].[RoleBlock]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RoleBlock];
GO
IF OBJECT_ID(N'[dbo].[Roles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Roles];
GO
IF OBJECT_ID(N'[dbo].[RouteDestinations]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RouteDestinations];
GO
IF OBJECT_ID(N'[dbo].[RouteEmployee]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RouteEmployee];
GO
IF OBJECT_ID(N'[dbo].[Routes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Routes];
GO
IF OBJECT_ID(N'[dbo].[RouteTasks]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RouteTasks];
GO
IF OBJECT_ID(N'[dbo].[RouteVehicle]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RouteVehicle];
GO
IF OBJECT_ID(N'[dbo].[SalesTerms]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SalesTerms];
GO
IF OBJECT_ID(N'[dbo].[Services]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Services];
GO
IF OBJECT_ID(N'[dbo].[ServiceTemplates]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ServiceTemplates];
GO
IF OBJECT_ID(N'[dbo].[SubLocations]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SubLocations];
GO
IF OBJECT_ID(N'[dbo].[TrackPoints]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TrackPoints];
GO
IF OBJECT_ID(N'[dbo].[UserAccountLog]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserAccountLog];
GO
IF OBJECT_ID(N'[dbo].[VehicleMaintenanceLineItems]', 'U') IS NOT NULL
    DROP TABLE [dbo].[VehicleMaintenanceLineItems];
GO
IF OBJECT_ID(N'[dbo].[VehicleMaintenanceLog]', 'U') IS NOT NULL
    DROP TABLE [dbo].[VehicleMaintenanceLog];
GO
IF OBJECT_ID(N'[dbo].[Vehicles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Vehicles];
GO



-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Blocks'
CREATE TABLE [dbo].[Blocks] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [FileName] nvarchar(max)  NULL,
    [NavigateUri] nvarchar(max)  NOT NULL,
    [Icon] varbinary(max)  NULL,
    [Link] nvarchar(max)  NOT NULL,
    [LoginNotRequired] bit  NOT NULL,
    [HideFromNavigation] bit  NOT NULL
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
    [ContactId] uniqueidentifier  NULL
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
    [OwnerPartyId] uniqueidentifier  NULL,
    [Name] nvarchar(max)  NULL,
    [AddressLineOne] nvarchar(max)  NULL,
    [Longitude] decimal(11,8)  NULL,
    [ZipCode] nvarchar(max)  NULL,
    [AddressLineTwo] nvarchar(max)  NULL,
    [State] nvarchar(max)  NULL,
    [Latitude] decimal(11,8)  NULL,
    [City] nvarchar(max)  NULL,
    [PartyId] uniqueidentifier  NULL,
    [RegionId] uniqueidentifier  NULL
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

-- Creating table 'TrackPoints'
CREATE TABLE [dbo].[TrackPoints] (
    [Id] bigint  NOT NULL,
    [Timestamp] datetime  NULL,
    [CompassDirection] int  NOT NULL,
    [Latitude] float  NOT NULL,
    [Longitude] float  NOT NULL,
    [UserAccountId] uniqueidentifier  NOT NULL
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
    [OwnerPartyId] uniqueidentifier  NOT NULL
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
    [VendorId] uniqueidentifier  NOT NULL,
    [DefaultBillingLocationId] uniqueidentifier  NULL
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
    [EmployerId] uniqueidentifier  NOT NULL
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

-- Creating table 'Contacts'
CREATE TABLE [dbo].[Contacts] (
    [Id] uniqueidentifier  NOT NULL,
    [Notes] nvarchar(max)  NULL,
    [OwnerPartyId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'ClientTitles'
CREATE TABLE [dbo].[ClientTitles] (
    [Id] uniqueidentifier  NOT NULL,
    [Title] nvarchar(max)  NOT NULL,
    [ClientId] uniqueidentifier  NULL,
    [ContactId] uniqueidentifier  NOT NULL
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
    [ReadyToInvoice] bit  NULL,
    [Date] datetime  NOT NULL,
    [OrderInRouteDestination] int  NOT NULL
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
    [Technicians_Id] uniqueidentifier  NOT NULL
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

-- Creating primary key on [Id] in table 'TrackPoints'
ALTER TABLE [dbo].[TrackPoints]
ADD CONSTRAINT [PK_TrackPoints]
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

-- Creating primary key on [Id] in table 'Contacts'
ALTER TABLE [dbo].[Contacts]
ADD CONSTRAINT [PK_Contacts]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ClientTitles'
ALTER TABLE [dbo].[ClientTitles]
ADD CONSTRAINT [PK_ClientTitles]
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
    PRIMARY KEY CLUSTERED ([Roles_Id], [Blocks_Id] ASC);
GO

-- Creating primary key on [Routes_Id], [Vehicles_Id] in table 'RouteVehicle'
ALTER TABLE [dbo].[RouteVehicle]
ADD CONSTRAINT [PK_RouteVehicle]
    PRIMARY KEY CLUSTERED ([Routes_Id], [Vehicles_Id] ASC);
GO

-- Creating primary key on [MemberParties_Id], [RoleMembership_Id] in table 'PartyRole'
ALTER TABLE [dbo].[PartyRole]
ADD CONSTRAINT [PK_PartyRole]
    PRIMARY KEY CLUSTERED ([MemberParties_Id], [RoleMembership_Id] ASC);
GO

-- Creating primary key on [Routes_Id], [Technicians_Id] in table 'RouteEmployee'
ALTER TABLE [dbo].[RouteEmployee]
ADD CONSTRAINT [PK_RouteEmployee]
    PRIMARY KEY CLUSTERED ([Routes_Id], [Technicians_Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [PartyId] in table 'ContactInfoSet'
ALTER TABLE [dbo].[ContactInfoSet]
ADD CONSTRAINT [FK_ContactInfoParty]
    FOREIGN KEY ([PartyId])
    REFERENCES [dbo].[Parties]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ContactInfoParty'
CREATE INDEX [IX_FK_ContactInfoParty]
ON [dbo].[ContactInfoSet]
    ([PartyId]);
GO

-- Creating foreign key on [Roles_Id] in table 'RoleBlock'
ALTER TABLE [dbo].[RoleBlock]
ADD CONSTRAINT [FK_RoleBlock_Role]
    FOREIGN KEY ([Roles_Id])
    REFERENCES [dbo].[Roles]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO
GO

-- Creating foreign key on [Blocks_Id] in table 'RoleBlock'
ALTER TABLE [dbo].[RoleBlock]
ADD CONSTRAINT [FK_RoleBlock_Block]
    FOREIGN KEY ([Blocks_Id])
    REFERENCES [dbo].[Blocks]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

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
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_LocationContactInfo'
CREATE INDEX [IX_FK_LocationContactInfo]
ON [dbo].[ContactInfoSet]
    ([LocationId]);
GO

-- Creating foreign key on [OwnerPartyId] in table 'Locations'
ALTER TABLE [dbo].[Locations]
ADD CONSTRAINT [FK_LocationParty]
    FOREIGN KEY ([OwnerPartyId])
    REFERENCES [dbo].[Parties]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_LocationParty'
CREATE INDEX [IX_FK_LocationParty]
ON [dbo].[Locations]
    ([OwnerPartyId]);
GO

-- Creating foreign key on [VehicleId] in table 'VehicleMaintenanceLog'
ALTER TABLE [dbo].[VehicleMaintenanceLog]
ADD CONSTRAINT [FK_VehicleMaintenanceLogEntryVehicle]
    FOREIGN KEY ([VehicleId])
    REFERENCES [dbo].[Vehicles]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

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
    ON DELETE CASCADE ON UPDATE NO ACTION;

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
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

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
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_VehicleMaintenanceLogEntryLineItem'
CREATE INDEX [IX_FK_VehicleMaintenanceLogEntryLineItem]
ON [dbo].[VehicleMaintenanceLineItems]
    ([VehicleMaintenanceLogEntryId]);
GO

-- Creating foreign key on [Id] in table 'Clients'
ALTER TABLE [dbo].[Clients]
ADD CONSTRAINT [FK_ClientParty]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Parties]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
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

-- Creating foreign key on [VendorId] in table 'Clients'
ALTER TABLE [dbo].[Clients]
ADD CONSTRAINT [FK_BusinessAccountClient]
    FOREIGN KEY ([VendorId])
    REFERENCES [dbo].[Parties_BusinessAccount]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_BusinessAccountClient'
CREATE INDEX [IX_FK_BusinessAccountClient]
ON [dbo].[Clients]
    ([VendorId]);
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

-- Creating foreign key on [UserAccountId] in table 'TrackPoints'
ALTER TABLE [dbo].[TrackPoints]
ADD CONSTRAINT [FK_UserAccountTrackPoint]
    FOREIGN KEY ([UserAccountId])
    REFERENCES [dbo].[Parties_UserAccount]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserAccountTrackPoint'
CREATE INDEX [IX_FK_UserAccountTrackPoint]
ON [dbo].[TrackPoints]
    ([UserAccountId]);
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
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO
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
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO
GO

-- Creating foreign key on [RoleMembership_Id] in table 'PartyRole'
ALTER TABLE [dbo].[PartyRole]
ADD CONSTRAINT [FK_PartyRole_Role]
    FOREIGN KEY ([RoleMembership_Id])
    REFERENCES [dbo].[Roles]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

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

-- Creating foreign key on [PartyId] in table 'Locations'
ALTER TABLE [dbo].[Locations]
ADD CONSTRAINT [FK_LocationParty1]
    FOREIGN KEY ([PartyId])
    REFERENCES [dbo].[Parties]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_LocationParty1'
CREATE INDEX [IX_FK_LocationParty1]
ON [dbo].[Locations]
    ([PartyId]);
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
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

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
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_EmployeeHistoryEntryEmployee'
CREATE INDEX [IX_FK_EmployeeHistoryEntryEmployee]
ON [dbo].[EmployeeHistoryEntries]
    ([EmployeeId]);
GO

-- Creating foreign key on [Routes_Id] in table 'RouteEmployee'
ALTER TABLE [dbo].[RouteEmployee]
ADD CONSTRAINT [FK_RouteEmployee_Route]
    FOREIGN KEY ([Routes_Id])
    REFERENCES [dbo].[Routes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Technicians_Id] in table 'RouteEmployee'
ALTER TABLE [dbo].[RouteEmployee]
ADD CONSTRAINT [FK_RouteEmployee_Employee]
    FOREIGN KEY ([Technicians_Id])
    REFERENCES [dbo].[Employees]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RouteEmployee_Employee'
CREATE INDEX [IX_FK_RouteEmployee_Employee]
ON [dbo].[RouteEmployee]
    ([Technicians_Id]);
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
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

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
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

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
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_LocationFieldLocation'
CREATE INDEX [IX_FK_LocationFieldLocation]
ON [dbo].[Fields_LocationField]
    ([LocationId]);
GO

-- Creating foreign key on [Id] in table 'Contacts'
ALTER TABLE [dbo].[Contacts]
ADD CONSTRAINT [FK_ContactPerson]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Parties_Person]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [OwnerPartyId] in table 'Contacts'
ALTER TABLE [dbo].[Contacts]
ADD CONSTRAINT [FK_ContactParty]
    FOREIGN KEY ([OwnerPartyId])
    REFERENCES [dbo].[Parties]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ContactParty'
CREATE INDEX [IX_FK_ContactParty]
ON [dbo].[Contacts]
    ([OwnerPartyId]);
GO

-- Creating foreign key on [ClientId] in table 'ClientTitles'
ALTER TABLE [dbo].[ClientTitles]
ADD CONSTRAINT [FK_ClientTitleClient]
    FOREIGN KEY ([ClientId])
    REFERENCES [dbo].[Clients]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ClientTitleClient'
CREATE INDEX [IX_FK_ClientTitleClient]
ON [dbo].[ClientTitles]
    ([ClientId]);
GO

-- Creating foreign key on [ContactId] in table 'ClientTitles'
ALTER TABLE [dbo].[ClientTitles]
ADD CONSTRAINT [FK_ClientTitleContact]
    FOREIGN KEY ([ContactId])
    REFERENCES [dbo].[Contacts]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ClientTitleContact'
CREATE INDEX [IX_FK_ClientTitleContact]
ON [dbo].[ClientTitles]
    ([ContactId]);
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
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO
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

-- Creating foreign key on [DefaultBillingLocationId] in table 'Clients'
ALTER TABLE [dbo].[Clients]
ADD CONSTRAINT [FK_ClientLocation]
    FOREIGN KEY ([DefaultBillingLocationId])
    REFERENCES [dbo].[Locations]
        ([Id])
    ON DELETE SET NULL ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ClientLocation'
CREATE INDEX [IX_FK_ClientLocation]
ON [dbo].[Clients]
    ([DefaultBillingLocationId]);
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

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[CheckServiceTemplateForChildren]
(
	-- Add the parameters for the function here
	@serviceTemplateId uniqueidentifier
)
RETURNS  INT
AS
BEGIN
	DECLARE @TempTable TABLE
	(
		Id uniqueidentifier
	);

	WITH TemplateRecurs AS
	(
    --Select all the Fields from a ServiceTemplate whose OwnerServiceProvider = providerId
    SELECT	ServiceTemplates.Id, ServiceTemplates.OwnerServiceTemplateId
    FROM	ServiceTemplates 
    WHERE	ServiceTemplates.Id = @serviceTemplateId
    --Recursively select the children
    UNION	ALL
    SELECT	ServiceTemplates.Id, ServiceTemplates.OwnerServiceTemplateId
    FROM	ServiceTemplates 
    JOIN	TemplateRecurs 
    ON		ServiceTemplates.OwnerServiceTemplateId = TemplateRecurs.Id
	)

	INSERT INTO @TempTable (Id)
	SELECT	TemplateRecurs.Id
	FROM	TemplateRecurs

	IF (SELECT COUNT(*) FROM @TempTable) > 1
		RETURN 1

	RETURN 0

END
GO



GO

IF OBJECT_ID(N'[dbo].[DeleteBasicPartyBasedOnId]', N'FN') IS NOT NULL
DROP PROCEDURE [dbo].DeleteBasicPartyBasedOnId
GO
--This procedure deletes a Business Account
CREATE PROCEDURE dbo.DeleteBasicPartyBasedOnId
		(@providerId uniqueidentifier)
	AS
	BEGIN

	DELETE FROM Locations
	WHERE		OwnerPartyId = @providerId
	OR			PartyId = @providerId

	DELETE FROM Contacts
	WHERE		OwnerPartyId = @providerId

	DELETE FROM ContactInfoSet
	WHERE		PartyId = @providerId

	DELETE FROM Roles
	WHERE		OwnerPartyId = @providerId

	DELETE FROM Vehicles 
	WHERE		OwnerPartyId = @providerId

	DELETE FROM Files
	WHERE		PartyId = @providerId


	END
	RETURN

GO

USE Core
GO
IF OBJECT_ID(N'[dbo].[DeleteBusinessAccountBasedOnId]', N'FN') IS NOT NULL
DROP PROCEDURE [dbo].DeleteBusinessAccountBasedOnId
GO
--This procedure deletes a Business Account
CREATE PROCEDURE dbo.DeleteBusinessAccountBasedOnId
		(@providerId uniqueidentifier)

	AS
	BEGIN

	DELETE FROM Services
	WHERE ServiceProviderId = @providerId

	EXEC dbo.DeleteServiceTemplatesAndChildrenBasedOnBusinessAccountId @providerId

	DELETE FROM RouteTasks
	WHERE BusinessAccountId = @providerId

	DELETE FROM Routes
	WHERE OwnerBusinessAccountId = @providerId

	DELETE FROM Clients
	WHERE VendorId = @providerId

	DELETE FROM Locations
	WHERE		PartyId = @providerId
	OR			OwnerPartyId = @providerId

	DELETE FROM Regions
	WHERE BusinessAccountId = @providerId

	DELETE FROM Contacts
	WHERE		OwnerPartyId = @providerId

	DELETE FROM ContactInfoSet
	WHERE		PartyId = @providerId

	DELETE FROM Roles
	WHERE		OwnerPartyId = @providerId

	DELETE FROM Vehicles 
	WHERE		OwnerPartyId = @providerId

	DELETE FROM Files
	WHERE		PartyId = @providerId

	DELETE FROM Employees
	WHERE EmployerId = @providerId
	
	DELETE FROM Parties_BusinessAccount
	WHERE Id = @providerId

	END
	RETURN

GO

IF OBJECT_ID(N'[dbo].[DeleteClientBasedOnId]', N'FN') IS NOT NULL
DROP PROCEDURE [dbo].DeleteClientBasedOnId
GO
--This procedure deletes a Business Account
CREATE PROCEDURE dbo.DeleteClientBasedOnId
		(@clientId uniqueidentifier)

	AS
	BEGIN

	DELETE FROM Services
	WHERE ClientId = @clientId

	EXEC dbo.DeleteServiceTemplatesAndChildrenBasedOnContextId @serviceProviderId = NULL, @ownerClientId = @clientId

	DELETE FROM RouteTasks
	WHERE ClientId = @clientId

	DELETE FROM RouteDestinations
	WHERE ClientId = @clientId

	DELETE FROM RecurringServices
	WHERE ClientId = @clientId

	DELETE FROM ClientTitles
	WHERE ClientId = @clientId

	DELETE FROM Clients
	WHERE Id = @clientId

	END
	RETURN

GO

IF OBJECT_ID(N'[dbo].[DeleteServiceTemplateAndChildrenBasedOnServiceTemplateId]', N'FN') IS NOT NULL
DROP PROCEDURE [dbo].DeleteServiceTemplateAndChildrenBasedOnServiceTemplateId
GO
CREATE PROCEDURE dbo.DeleteServiceTemplateAndChildrenBasedOnServiceTemplateId
		(@parentTemplateId uniqueidentifier)
	AS

	BEGIN

		CREATE TABLE #TempTable
	(
		Id uniqueidentifier
	);

	WITH TemplateRecurs AS
	(
    --Select all the Fields from a ServiceTemplate whose OwnerServiceProvider = providerId
    SELECT	ServiceTemplates.Id, ServiceTemplates.OwnerServiceTemplateId
    FROM	ServiceTemplates 
    WHERE	ServiceTemplates.Id = @parentTemplateId
    --Recursively select the children
    UNION	ALL
    SELECT	ServiceTemplates.Id, ServiceTemplates.OwnerServiceTemplateId
    FROM	ServiceTemplates 
    JOIN	TemplateRecurs 
    ON		ServiceTemplates.OwnerServiceTemplateId = TemplateRecurs.Id
	)

	INSERT INTO #TempTable (Id)
	SELECT	TemplateRecurs.Id
	FROM	TemplateRecurs
	
	DELETE 
	FROM		Services
	--This is a Semi-Join between the ServiceTemplateRecurs table created above and the Services Table
	--Semi-Join simply means that it has all the same logic as a normal join, but it doesnt actually join the tables
	--In this case, it finds all the rows on Services that correspond to a row in ServiceTemplateRecurs
	WHERE		EXISTS
	(
	SELECT		#TempTable.Id
	FROM		#TempTable
	WHERE		#TempTable.Id = Services.Id
	)

	DELETE 
	FROM		RecurringServices
	--This is a Semi-Join between the ServiceTemplateRecurs table created above and the RecurringServices Table
	--Semi-Join simply means that it has all the same logic as a normal join, but it doesnt actually join the tables
	--In this case, it finds all the rows on RecurringServices that correspond to a row in ServiceTemplateRecurs
	WHERE		EXISTS
	(
	SELECT		#TempTable.Id
	FROM		#TempTable
	WHERE		#TempTable.Id = RecurringServices.Id
	)

	DELETE 
	FROM		ServiceTemplates
	--This is a Semi-Join between the ServiceTemplateRecurs table created above and the ServiceTemplates Table
	--Semi-Join simply means that it has all the same logic as a normal join, but it doesnt actually join the tables
	--In this case, it finds all the rows on ServiceTemplates that correspond to a row in ServiceTemplateRecurs
	WHERE		EXISTS
	(
	SELECT		#TempTable.Id
	FROM		#TempTable
	WHERE		#TempTable.Id = ServiceTemplates.Id
	OR			#TempTable.Id = ServiceTemplates.OwnerServiceTemplateId
	)
	
	--Drop the table that was created to store the CTE
	DROP TABLE #TempTable
	

	END
	RETURN

GO

IF OBJECT_ID(N'[dbo].[DeleteServiceTemplatesAndChildrenBasedOnContextId]', N'FN') IS NOT NULL
DROP PROCEDURE [dbo].[DeleteServiceTemplatesAndChildrenBasedOnContextId]
GO
--This procedure deletes all service templates and child service templates
CREATE PROCEDURE dbo.DeleteServiceTemplatesAndChildrenBasedOnContextId
		(@serviceProviderId uniqueidentifier,
		@ownerClientId uniqueidentifier)
	AS
	BEGIN

	CREATE TABLE #TempTable
	(
		Id uniqueidentifier
	);
	
	IF @serviceProviderId IS NOT NULL
	BEGIN
	--This is the CTE for SetviceTemplates and their Children for the specified businessaccount (service provider)
	WITH		ServiceTemplateRecurs as
	(
    SELECT		ServiceTemplates.Id, ServiceTemplates.OwnerServiceTemplateId
    FROM		ServiceTemplates
    WHERE		ServiceTemplates.OwnerServiceProviderId = @serviceProviderId
    UNION		ALL
    SELECT		ServiceTemplates.Id, ServiceTemplates.OwnerServiceTemplateId
    FROM		ServiceTemplates
    JOIN		ServiceTemplateRecurs
    ON			ServiceTemplateRecurs.id = ServiceTemplates.OwnerServiceTemplateId
	)
	
	INSERT INTO #TempTable (Id)
	SELECT	ServiceTemplateRecurs.Id
	FROM	ServiceTemplateRecurs
	END

	ELSE IF @ownerClientId IS NOT NULL
	BEGIN
	WITH		ServiceTemplateRecurs as
	(
    SELECT		ServiceTemplates.Id, ServiceTemplates.OwnerServiceTemplateId
    FROM		ServiceTemplates
    WHERE		ServiceTemplates.OwnerClientId = @ownerClientId
    UNION		ALL
    SELECT		ServiceTemplates.Id, ServiceTemplates.OwnerServiceTemplateId
    FROM		ServiceTemplates
    JOIN		ServiceTemplateRecurs
    ON			ServiceTemplateRecurs.id = ServiceTemplates.OwnerServiceTemplateId
	)
	INSERT INTO #TempTable (Id)
	SELECT	ServiceTemplateRecurs.Id
	FROM	ServiceTemplateRecurs
	END


	DELETE 
	FROM		Services
	--This is a Semi-Join between the ServiceTemplateRecurs table created above and the Services Table
	--Semi-Join simply means that it has all the same logic as a normal join, but it doesnt actually join the tables
	--In this case, it finds all the rows on Services that correspond to a row in ServiceTemplateRecurs
	WHERE		EXISTS
	(
	SELECT		#TempTable.Id
	FROM		#TempTable
	WHERE		#TempTable.Id = Services.Id
	)

	DELETE 
	FROM		RecurringServices
	--This is a Semi-Join between the ServiceTemplateRecurs table created above and the RecurringServices Table
	--Semi-Join simply means that it has all the same logic as a normal join, but it doesnt actually join the tables
	--In this case, it finds all the rows on RecurringServices that correspond to a row in ServiceTemplateRecurs
	WHERE		EXISTS
	(
	SELECT		#TempTable.Id
	FROM		#TempTable
	WHERE		#TempTable.Id = RecurringServices.Id
	)

	DELETE 
	FROM		ServiceTemplates
	--This is a Semi-Join between the ServiceTemplateRecurs table created above and the ServiceTemplates Table
	--Semi-Join simply means that it has all the same logic as a normal join, but it doesnt actually join the tables
	--In this case, it finds all the rows on ServiceTemplates that correspond to a row in ServiceTemplateRecurs
	WHERE		EXISTS
	(
	SELECT		#TempTable.Id
	FROM		#TempTable
	WHERE		#TempTable.Id = ServiceTemplates.Id
	OR			#TempTable.Id = ServiceTemplates.OwnerServiceTemplateId
	)
	
	--Drop the table that was created to store the CTE
	DROP TABLE #TempTable
	

	END
	RETURN

GO

--This procedure deletes a User Account
IF OBJECT_ID(N'[dbo].[DeleteUserAccountBasedOnId]', N'FN') IS NOT NULL
DROP PROCEDURE [dbo].[DeleteUserAccountBasedOnId]
GO
CREATE PROCEDURE dbo.DeleteUserAccountBasedOnId
		(@providerId uniqueidentifier)

	AS
	BEGIN

	DELETE FROM Locations
	WHERE		OwnerPartyId = @providerId
	OR			PartyId = @providerId

	DELETE FROM Contacts
	WHERE		OwnerPartyId = @providerId

	DELETE FROM ContactInfoSet
	WHERE		PartyId = @providerId

	DELETE FROM Roles
	WHERE		OwnerPartyId = @providerId

	DELETE FROM Vehicles 
	WHERE		OwnerPartyId = @providerId

	DELETE FROM Files
	WHERE		PartyId = @providerId

	DELETE FROM Employees
	WHERE LinkedUserAccountId = @providerId

	DELETE FROM TrackPoints
	WHERE UserAccountId = @providerId

	DELETE FROM UserAccountLog
	WHERE UserAccountId = @providerId

	DELETE FROM Parties_UserAccount
	WHERE Id = @providerId

	END
	RETURN

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID(N'[dbo].[GetNextOccurence]', N'FN') IS NOT NULL
DROP FUNCTION [dbo].[GetNextOccurence]
GO
    Create FUNCTION [dbo].[GetNextOccurence]
    (@onOrAfterDate datetime,
    @startDate datetime,
    @endDate datetime,
    @endAfterTimes int,
    @frequencyInt int,
    @repeatEveryTimes int,
    @frequencyDetailInt int
    )       
    returns Datetime       
    as       
    begin  		
		declare @ReturnDate datetime,				
				@LastMeeting datetime, @Remains int				
		
		if @endDate is not null and @onOrAfterDate > @endDate
			return null	
		
		if @frequencyInt = 1 
			begin
				if @onOrAfterDate > @startDate
					return null
				else 
					set @ReturnDate = @startDate
			end
		
		else if @frequencyInt = 2
			begin	
				DECLARE @DateDif int			
				set @DateDif = DATEDIFF ( d , @startDate , @onOrAfterDate )
				if @DateDif <=0
					set @ReturnDate = @startDate
				else if @endAfterTimes is not null and @onOrAfterDate > dateadd(d,(@endAfterTimes -1)*@repeatEveryTimes,@startDate) 
					return null					
				else
					begin
						declare @RemainDays int
						set @RemainDays = @DateDif%@repeatEveryTimes
						if @RemainDays = 0
							set @ReturnDate = @onOrAfterDate
						else
							set @ReturnDate = dateadd(d,@repeatEveryTimes - @RemainDays,@onOrAfterDate)
					end
			end
		
		else if @frequencyInt = 3
			begin
				if @onOrAfterDate <= @startDate
					set @ReturnDate = @startDate
				else
					begin
						declare @DoW char(7),@MeetingInWeek int, @FirstDate int, @LastDate int, @Weeks int,@LastWeekRemains int,
								@TempDate datetime,@DoWonOrAfterDate int, @MeetingDate int,@dwStartDate int
						set @DoW = convert(char(7),@frequencyDetailInt)
						set @MeetingInWeek = len(@DoW)
						set @FirstDate = convert(int,substring(@DoW,1,1))
						-- change start date to begin of the week
						set @dwStartDate = datepart(dw,@startDate)
						set @startDate = dateadd(day,@FirstDate - @dwStartDate,@startDate)				
						
						-- get the date of last meeting if @endAfterTimes is not null
						if @endAfterTimes is not null
							begin		
								set @endAfterTimes = @endAfterTimes + charindex(convert(char(1),@dwStartDate),@DoW)-1				
								set @Weeks = @endAfterTimes / @MeetingInWeek
								set @LastWeekRemains = @endAfterTimes % @MeetingInWeek
								if @LastWeekRemains = 0 
									begin
										set @LastDate = convert(int,substring(@DoW,@MeetingInWeek,1))
										set @TempDate = dateadd(week,(@Weeks - 1)*@repeatEveryTimes,@startDate)
										set @TempDate = dateadd(day,@LastDate - @FirstDate,@TempDate)
									end
								else
									begin
										set @LastDate = convert(int,substring(@DoW,@LastWeekRemains,1))
										set @TempDate = dateadd(week,@Weeks*@repeatEveryTimes,@startDate)
										set @TempDate = dateadd(day,@LastDate - @FirstDate,@TempDate)
									end
								if @onOrAfterDate > @TempDate
									return null						
							end						
						set @Weeks = datediff(week,@startDate,@onOrAfterDate)
						if @Weeks%@repeatEveryTimes>0
							set @ReturnDate = dateadd(week,((@Weeks/@repeatEveryTimes)+1)*@repeatEveryTimes,@startDate)
						else
							begin
								declare @WeekDay table
								(
									DoW int,
									Meeting bit
								)
								insert into @WeekDay values (1,case when charindex('1',@DoW)>0 then 1 else 0 end )
								insert into @WeekDay values (2,case when charindex('2',@DoW)>0 then 1 else 0 end )
								insert into @WeekDay values (3,case when charindex('3',@DoW)>0 then 1 else 0 end )
								insert into @WeekDay values (4,case when charindex('4',@DoW)>0 then 1 else 0 end )
								insert into @WeekDay values (5,case when charindex('5',@DoW)>0 then 1 else 0 end )
								insert into @WeekDay values (6,case when charindex('6',@DoW)>0 then 1 else 0 end )
								insert into @WeekDay values (7,case when charindex('7',@DoW)>0 then 1 else 0 end )
								set @DoWonOrAfterDate = datepart(dw,@onOrAfterDate)
								
								select top 1 @MeetingDate = DoW from @WeekDay where Meeting = 1 and DoW >=@DoWonOrAfterDate
								if @MeetingDate is not null
									set @ReturnDate = dateadd(day, @MeetingDate - @DoWonOrAfterDate, @onOrAfterDate)
								else 
									begin
										set @TempDate = dateadd(day, @FirstDate - @DoWonOrAfterDate, @onOrAfterDate)
										set @ReturnDate = dateadd(week,@repeatEveryTimes,@TempDate)
									end 
							end
					end	 
			end
			
		else if @frequencyInt = 4
			begin				
				if @onOrAfterDate <= @startDate
					set @ReturnDate = @startDate
				else
					begin
						declare @Times int, @MonthDiff int
						set @MonthDiff = DATEDIFF(month, @startDate, @onOrAfterDate)
						set @Times = @MonthDiff/@repeatEveryTimes
						set @Remains= @MonthDiff%@repeatEveryTimes 								
						if @frequencyDetailInt = 10
							begin	
								if @Remains>0
									set @Times = @Times+1											
								set @ReturnDate = dbo.GetLastDayOfMonth(dateadd(month,@Times*@repeatEveryTimes,@startDate))
								if @endAfterTimes is not null and DATEDIFF(month, @startDate, @ReturnDate)/@repeatEveryTimes > @endAfterTimes-1
									return null								
							end
						else if @frequencyDetailInt = 11
							begin
								if @Remains>0
									set @Times = @Times+1
								set @ReturnDate = dbo.GetFirstOfDayOfWeekInMonth(dateadd(month,@Times*@repeatEveryTimes,@startDate),datepart(dw,@startDate))
								if @onOrAfterDate > @ReturnDate
									set @ReturnDate = dbo.GetFirstOfDayOfWeekInMonth(dateadd(month,@repeatEveryTimes,@ReturnDate),datepart(dw,@startDate))
								if @endAfterTimes is not null and DATEDIFF(month, @startDate, @ReturnDate)/@repeatEveryTimes > @endAfterTimes-1
									return null								
							end
						else if @frequencyDetailInt = 12
							begin 
								if @Remains>0
									set @Times = @Times+1
								set @ReturnDate = dbo.GetFirstOfDayOfWeekInMonth(dateadd(month,@Times*@repeatEveryTimes,@startDate),datepart(dw,@startDate))
								set @ReturnDate = dateadd(week,1,@ReturnDate)
								if @onOrAfterDate > @ReturnDate
									begin
										set @ReturnDate = dbo.GetFirstOfDayOfWeekInMonth(dateadd(month,@repeatEveryTimes,@ReturnDate),datepart(dw,@startDate))
										set @ReturnDate = dateadd(week,1,@ReturnDate)
									end
								if @endAfterTimes is not null and (DATEDIFF(month, @startDate, @ReturnDate)/@repeatEveryTimes) > (@endAfterTimes-1)
									return null								
							end
						else if @frequencyDetailInt = 13
							begin 
								if @Remains>0
									set @Times = @Times+1
								set @ReturnDate = dbo.GetFirstOfDayOfWeekInMonth(dateadd(month,@Times*@repeatEveryTimes,@startDate),datepart(dw,@startDate))
								set @ReturnDate = dateadd(week,2,@ReturnDate)
								if @onOrAfterDate > @ReturnDate
									begin
										set @ReturnDate = dbo.GetFirstOfDayOfWeekInMonth(dateadd(month,@repeatEveryTimes,@ReturnDate),datepart(dw,@startDate))
										set @ReturnDate = dateadd(week,2,@ReturnDate)
									end
								if @endAfterTimes is not null and DATEDIFF(month, @startDate, @ReturnDate)/@repeatEveryTimes > @endAfterTimes-1
									return null								
							end
						else if @frequencyDetailInt = 14
							begin
								if @Remains>0
									set @Times = @Times+1
								set @ReturnDate = dbo.GetLastOfDayOfWeekInMonth(dateadd(month,@Times*@repeatEveryTimes,@startDate),datepart(dw,@startDate))
								if @onOrAfterDate > @ReturnDate
									set @ReturnDate = dbo.GetLastOfDayOfWeekInMonth(dateadd(month,@repeatEveryTimes,@ReturnDate),datepart(dw,@startDate))
								if @endAfterTimes is not null and DATEDIFF(month, @startDate, @ReturnDate)/@repeatEveryTimes > @endAfterTimes-1
									return null								
							end
						else if @frequencyDetailInt = 8
							begin
								if day(@startdate) in (29,30,31)
									set @ReturnDate = dbo.GetNextDayInMonth(@startdate,@onOrAfterDate,@repeatEveryTimes,@endAfterTimes)
								else
									begin
										if @Remains>0
											set @Times = @Times+1						
										set @ReturnDate = dateadd(month,@Times*@repeatEveryTimes,@startDate)
										if @onOrAfterDate > @ReturnDate
											set @ReturnDate = dateadd(month,@repeatEveryTimes,@ReturnDate)
										if @endAfterTimes is not null and DATEDIFF(month, @startDate, @ReturnDate)/@repeatEveryTimes > @endAfterTimes-1
											return null	
									end							
							end
					end
			end		
		else if @frequencyInt = 5
			begin
				if @onOrAfterDate <= @startDate
					set @ReturnDate = @startDate
				--else if @endAfterTimes is not null and @onOrAfterDate > dateadd(year,(@endAfterTimes -1)*@repeatEveryTimes,@startDate)
				--	return null
				else 
					begin
						DECLARE @tmpdate datetime,@years int, @months int, @days int,@TimesInt int
						set @tmpdate = @startDate
						SELECT @years = DATEDIFF(yy, @tmpdate, @onOrAfterDate) - CASE WHEN (MONTH(@startDate) > MONTH(@onOrAfterDate)) OR (MONTH(@startDate) = MONTH(@onOrAfterDate) AND DAY(@startDate) > DAY(@onOrAfterDate)) THEN 1 ELSE 0 END
						SELECT @tmpdate = DATEADD(yy, @years, @tmpdate)
						SELECT @months = DATEDIFF(m, @tmpdate, @onOrAfterDate) - CASE WHEN DAY(@startDate) > DAY(@onOrAfterDate) THEN 1 ELSE 0 END
						SELECT @tmpdate = DATEADD(m, @months, @tmpdate)
						SELECT @days = DATEDIFF(d, @tmpdate, @onOrAfterDate)
						if DAY(@startDate) = 29 and MONTH(@startDate) =2 and @repeatEveryTimes%4 > 0 
							begin
								if @repeatEveryTimes % 2 = 0
									set @repeatEveryTimes = @repeatEveryTimes*2
								else
									set @repeatEveryTimes = @repeatEveryTimes*4									
							end
						
						set @Remains = @years%@repeatEveryTimes
						set @TimesInt = @years/@repeatEveryTimes
						if @Remains = 0 and @months = 0 and @days=0
							set @returndate = @onOrAfterDate
						else
							begin								
								set @returndate = DATEADD(year, (@TimesInt+1)*@repeatEveryTimes, @startDate)
							end 
						if @endAfterTimes is not null and DATEDIFF(year, @startDate, @ReturnDate)/@repeatEveryTimes > @endAfterTimes-1 
							return null
						--return @returndate
					end
			end			
		if @endDate is not null and @ReturnDate > @endDate
			return null				
		return @ReturnDate
	end



GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID(N'[dbo].[GetPreviousOccurrence]', N'FN') IS NOT NULL
DROP FUNCTION [dbo].[GetPreviousOccurrence]
GO
    Create FUNCTION [dbo].[GetPreviousOccurrence]
    (@OnOrBeforeDate datetime,
    @startDate datetime,
    @endDate datetime,
    @endAfterTimes int,
    @frequencyInt int,
    @repeatEveryTimes int,
    @frequencyDetailInt int
    )       
    returns Datetime       
    as       
    begin  		
		declare @ReturnDate datetime,				
				@LastMeeting datetime, @Remains int				
		
		--No meeting after end date
		if @endDate is not null and @OnOrBeforeDate > @endDate
			set @OnOrBeforeDate	 = @endDate
		-- No meeting before start date
		if @OnOrBeforeDate < @startDate
			return null
		
		if @frequencyInt = 1
			set @ReturnDate = @startDate
			
		else if @frequencyInt = 2
			begin	
				DECLARE @DateDif int			
				set @DateDif = DATEDIFF ( d , @startDate , @OnOrBeforeDate )
				if @endAfterTimes is not null and @OnOrBeforeDate >= dateadd(d,(@endAfterTimes -1)*@repeatEveryTimes,@startDate) 
					set @ReturnDate = dateadd(d,(@endAfterTimes -1)*@repeatEveryTimes,@startDate)					
				else
					begin
						declare @RemainDays int
						set @RemainDays = @DateDif%@repeatEveryTimes
						if @RemainDays = 0
							set @ReturnDate = @OnOrBeforeDate
						else
							set @ReturnDate = dateadd(d,0 - @RemainDays,@OnOrBeforeDate)
					end
			end
		
		else if @frequencyInt = 3
			begin			
				declare @DoW char(7),@MeetingInWeek int, @FirstDate int, @LastDate int, @Weeks int,@LastWeekRemains int,
						@TempDate datetime,@DoWOnOrBeforeDate int, @MeetingDate int,@dwStartDate int
				set @DoW = convert(char(7),@frequencyDetailInt)
				set @MeetingInWeek = len(@DoW)
				set @FirstDate = convert(int,substring(@DoW,1,1))
				set @LastDate = convert(int,substring(@DoW,@MeetingInWeek,1))
				-- change start date to begin of the week
				set @dwStartDate = datepart(dw,@startDate)
				set @startDate = dateadd(day,@FirstDate - @dwStartDate,@startDate)				
				
				-- get the date of last meeting if @endAfterTimes is not null
				if @endAfterTimes is not null
					begin		
						set @endAfterTimes = @endAfterTimes + charindex(convert(char(1),@dwStartDate),@DoW)-1				
						set @Weeks = @endAfterTimes / @MeetingInWeek
						set @LastWeekRemains = @endAfterTimes % @MeetingInWeek
						if @LastWeekRemains = 0 
							begin								
								set @TempDate = dateadd(week,(@Weeks - 1)*@repeatEveryTimes,@startDate)
								set @TempDate = dateadd(day,@LastDate - @FirstDate,@TempDate)
							end
						else
							begin								
								set @TempDate = dateadd(week,@Weeks*@repeatEveryTimes,@startDate)
								set @TempDate = dateadd(day,convert(int,substring(@DoW,@LastWeekRemains,1)) - @FirstDate,@TempDate)
							end
						if @OnOrBeforeDate >= @TempDate
							return @TempDate					
					end						
				set @Weeks = datediff(week,@startDate,@OnOrBeforeDate)
				if @Weeks%@repeatEveryTimes>0 --in the odd week, get the last meeting in previous week
					begin						
						set @ReturnDate = dateadd(week,((@Weeks/@repeatEveryTimes))*@repeatEveryTimes,@startDate)
						set @ReturnDate = dateadd(day,@LastDate - @FirstDate,@ReturnDate)
					end
				else
					begin
						declare @WeekDay table
						(
							DoW int,
							Meeting bit
						)
						insert into @WeekDay values (1,case when charindex('1',@DoW)>0 then 1 else 0 end )
						insert into @WeekDay values (2,case when charindex('2',@DoW)>0 then 1 else 0 end )
						insert into @WeekDay values (3,case when charindex('3',@DoW)>0 then 1 else 0 end )
						insert into @WeekDay values (4,case when charindex('4',@DoW)>0 then 1 else 0 end )
						insert into @WeekDay values (5,case when charindex('5',@DoW)>0 then 1 else 0 end )
						insert into @WeekDay values (6,case when charindex('6',@DoW)>0 then 1 else 0 end )
						insert into @WeekDay values (7,case when charindex('7',@DoW)>0 then 1 else 0 end )
						set @DoWOnOrBeforeDate = datepart(dw,@OnOrBeforeDate)
						
						select top 1 @MeetingDate = DoW from @WeekDay where Meeting = 1 and DoW <= @DoWOnOrBeforeDate order by DoW DESC
						if @MeetingDate is not null
							set @ReturnDate = dateadd(day, @MeetingDate - @DoWOnOrBeforeDate, @OnOrBeforeDate)
						else 
							begin								
								set @ReturnDate = dateadd(day, @LastDate - @DoWOnOrBeforeDate, @OnOrBeforeDate)	
								set @ReturnDate = dateadd(week, 0-@repeatEveryTimes, @ReturnDate)									
							end 
					end
					 
			end
			
		else if @frequencyInt = 4
			begin
				declare @Times int, @MonthDiff int
				set @MonthDiff = DATEDIFF(month, @startDate, @OnOrBeforeDate)
				set @Times = @MonthDiff/@repeatEveryTimes
				set @Remains= @MonthDiff%@repeatEveryTimes 								
				if @frequencyDetailInt = 10
					begin																			
						set @ReturnDate = dbo.GetLastDayOfMonth(dateadd(month,@Times*@repeatEveryTimes,@startDate))
						if @ReturnDate > @OnOrBeforeDate
							set @ReturnDate = dbo.GetLastDayOfMonth(dateadd(month,0-@repeatEveryTimes,@ReturnDate))
						if @endAfterTimes is not null and DATEDIFF(month, @startDate, @ReturnDate)/@repeatEveryTimes > @endAfterTimes-1
							set @ReturnDate = 	dbo.GetLastDayOfMonth(dateadd(month,(@endAfterTimes-1)*@repeatEveryTimes,@startDate))							
					end
				else if @frequencyDetailInt = 11
					begin
						set @ReturnDate = dbo.GetFirstOfDayOfWeekInMonth(dateadd(month,@Times*@repeatEveryTimes,@startDate),datepart(dw,@startDate))
						if @OnOrBeforeDate < @ReturnDate
							set @ReturnDate = dbo.GetFirstOfDayOfWeekInMonth(dateadd(month,0-@repeatEveryTimes,@ReturnDate),datepart(dw,@startDate))
						if @endAfterTimes is not null and DATEDIFF(month, @startDate, @ReturnDate)/@repeatEveryTimes > @endAfterTimes-1
							set @ReturnDate =	dbo.GetFirstOfDayOfWeekInMonth(dateadd(month,(@endAfterTimes-1)*@repeatEveryTimes,@startDate),datepart(dw,@startDate))							
					end
				else if @frequencyDetailInt = 12
					begin 
						set @ReturnDate = dbo.GetFirstOfDayOfWeekInMonth(dateadd(month,@Times*@repeatEveryTimes,@startDate),datepart(dw,@startDate))
						set @ReturnDate = dateadd(week,1,@ReturnDate)
						if @OnOrBeforeDate < @ReturnDate
							begin
								set @ReturnDate = dbo.GetFirstOfDayOfWeekInMonth(dateadd(month,0-@repeatEveryTimes,@ReturnDate),datepart(dw,@startDate))
								set @ReturnDate = dateadd(week,1,@ReturnDate)
							end
						if @endAfterTimes is not null and (DATEDIFF(month, @startDate, @ReturnDate)/@repeatEveryTimes) > (@endAfterTimes-1)
							begin
								set @ReturnDate =	dbo.GetFirstOfDayOfWeekInMonth(dateadd(month,(@endAfterTimes-1)*@repeatEveryTimes,@startDate),datepart(dw,@startDate))
								set @ReturnDate = dateadd(week,1,@ReturnDate)
							end														
					end
				else if @frequencyDetailInt = 13
					begin 
					set @ReturnDate = dbo.GetFirstOfDayOfWeekInMonth(dateadd(month,@Times*@repeatEveryTimes,@startDate),datepart(dw,@startDate))
						set @ReturnDate = dateadd(week,2,@ReturnDate)
						if @OnOrBeforeDate < @ReturnDate
							begin
								set @ReturnDate = dbo.GetFirstOfDayOfWeekInMonth(dateadd(month,0-@repeatEveryTimes,@ReturnDate),datepart(dw,@startDate))
								set @ReturnDate = dateadd(week,2,@ReturnDate)
							end
						if @endAfterTimes is not null and (DATEDIFF(month, @startDate, @ReturnDate)/@repeatEveryTimes) > (@endAfterTimes-1)
							begin
								set @ReturnDate =	dbo.GetFirstOfDayOfWeekInMonth(dateadd(month,(@endAfterTimes-1)*@repeatEveryTimes,@startDate),datepart(dw,@startDate))
								set @ReturnDate = dateadd(week,2,@ReturnDate)
							end							
					end
				else if @frequencyDetailInt = 14
					begin
						set @ReturnDate = dbo.GetLastOfDayOfWeekInMonth(dateadd(month,@Times*@repeatEveryTimes,@startDate),datepart(dw,@startDate))
						if @OnOrBeforeDate < @ReturnDate
							set @ReturnDate = dbo.GetLastOfDayOfWeekInMonth(dateadd(month,0-@repeatEveryTimes,@ReturnDate),datepart(dw,@startDate))
						if @endAfterTimes is not null and DATEDIFF(month, @startDate, @ReturnDate)/@repeatEveryTimes > @endAfterTimes-1
							set @ReturnDate = dbo.GetLastOfDayOfWeekInMonth(dateadd(month,(@endAfterTimes-1)*@repeatEveryTimes,@startDate),datepart(dw,@startDate))								
					end
				else if @frequencyDetailInt = 8
					begin
						if day(@startdate) in (29,30,31)
							set @ReturnDate = dbo.GetPreviousDayInMonth(@startdate,@OnOrBeforeDate,@repeatEveryTimes,@endAfterTimes)
						else
							begin						
								set @ReturnDate = dateadd(month,@Times*@repeatEveryTimes,@startDate)
								if @OnOrBeforeDate < @ReturnDate
									set @ReturnDate = dateadd(month,0-@repeatEveryTimes,@ReturnDate)
								if @endAfterTimes is not null and DATEDIFF(month, @startDate, @ReturnDate)/@repeatEveryTimes > @endAfterTimes-1
									set @ReturnDate = dateadd(month,(@endAfterTimes-1)*@repeatEveryTimes,@startDate)
							end							
					end
					
			end		
		else if @frequencyInt = 5
			begin
				DECLARE @tmpdate datetime,@years int, @months int, @days int,@TimesInt int
				set @tmpdate = @startDate
				SELECT @years = DATEDIFF(yy, @tmpdate, @OnOrBeforeDate) - CASE WHEN (MONTH(@startDate) > MONTH(@OnOrBeforeDate)) OR (MONTH(@startDate) = MONTH(@OnOrBeforeDate) AND DAY(@startDate) > DAY(@OnOrBeforeDate)) THEN 1 ELSE 0 END
				SELECT @tmpdate = DATEADD(yy, @years, @tmpdate)
				SELECT @months = DATEDIFF(m, @tmpdate, @OnOrBeforeDate) - CASE WHEN DAY(@startDate) > DAY(@OnOrBeforeDate) THEN 1 ELSE 0 END
				SELECT @tmpdate = DATEADD(m, @months, @tmpdate)
				SELECT @days = DATEDIFF(d, @tmpdate, @OnOrBeforeDate)
				if DAY(@startDate) = 29 and MONTH(@startDate) =2 and @repeatEveryTimes%4 > 0 
					begin
						if @repeatEveryTimes % 2 = 0
							set @repeatEveryTimes = @repeatEveryTimes*2
						else
							set @repeatEveryTimes = @repeatEveryTimes*4									
					end
				
				set @Remains = @years%@repeatEveryTimes
				set @TimesInt = @years/@repeatEveryTimes
				if @Remains = 0 and @months = 0 and @days=0
					set @returndate = @OnOrBeforeDate
				else
					begin								
						set @returndate = DATEADD(year, (@TimesInt)*@repeatEveryTimes, @startDate)
					end 
				if @endAfterTimes is not null and DATEDIFF(year, @startDate, @ReturnDate)/@repeatEveryTimes > @endAfterTimes-1 
					set @returndate = DATEADD(year, (@endAfterTimes-1)*@repeatEveryTimes, @startDate)
				--return @returndate
					
			end	
			
		return @ReturnDate
	end

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF OBJECT_ID(N'[dbo].[GetServicesForToday]', N'FN') IS NOT NULL
DROP FUNCTION [dbo].[GetServicesForToday]
GO
/****************************************************************************************************************************************************
* FUNCTION GetServicesForToday will take the context provided and find all the services that are scheduled for that day
** Input Parameters **
* @serviceProviderIdContext - The BusinessAccount context
* @firstDateToLookForServices - The reference date to look for services
** Output Parameters: **
* @ServicesTableToReturn - Ex. below
* RecurringServiceId                     | ServiceId                              | OccurDate
* -----------------------------------------------------------------------------------------
* {036BD670-39A5-478F-BFA3-AD312E3F7F47} |                                        | 1/1/2012 <-- Generated service
* {B30A43AD-655A-449C-BD4E-951F8F988718} |                                        | 1/1/2012 <-- Existing service
* {03DB9F9B-2FF6-4398-B984-533FB3E19C50} | {FC222C74-EFEA-4B45-93FB-B042E6D6DB0D} | 1/2/2012 <-- Existing service with a RecurringService parent **
***************************************************************************************************************************************************/
CREATE FUNCTION [dbo].[GetServicesForToday]
(@serviceProviderIdContext uniqueidentifier,
@serviceDate date)
RETURNS @ServicesTableToReturn TABLE
(
	RecurringServiceId uniqueidentifier,
	ServiceId uniqueidentifier,
	OccurDate date
) 
AS
BEGIN

	--Stores the Recurring Services that are associated with the lowest context provided
	DECLARE @TempGenServiceTable TABLE
	(Id uniqueidentifier,
     EndDate date,
     EndAfterTimes int,
     RepeatEveryTimes int,
     FrequencyInt int,
     FrequencyDetailInt int,
     StartDate date,
	 NextDate date)


	INSERT INTO @TempGenServiceTable (Id, EndDate, EndAfterTimes, RepeatEveryTimes, FrequencyInt, FrequencyDetailInt, StartDate)
	--This is a Semi-Join between the Clients table created above and the RecurringServices Table
	--Semi-Join simply means that it has all the same logic as a normal join, but it doesnt actually join the tables
	--In this case, it finds all the RecurringServices that correspond to a Client with a vendorId = @serviceProviderIdContext
	SELECT	* 
	FROM		Repeats
	WHERE		EXISTS
	(
		SELECT	*
		FROM	Clients
		WHERE	EXISTS
		(
		SELECT	*
		FROM	RecurringServices
		WHERE	RecurringServices.ClientId = Clients.Id 
				AND Clients.VendorId = @serviceProviderIdContext 
				AND RecurringServices.Id = Repeats.Id
		)
	)

	--This table is simply the result of merging @TempGenServiceTable and @TempNextDateTable
	DECLARE @TempGenServiceTableWithNextOccurrence TABLE
	(Id uniqueidentifier,
     EndDate date,
     EndAfterTimes int,
     RepeatEveryTimes int,
     FrequencyInt int,
     FrequencyDetailInt int,
     StartDate date,
	 NextDate date)
	
	--Merges @TempGenServiceTable amd @TempNextDateTable created above based on their Id into @TempGenServiceTableWithNextOccurrence
	INSERT INTO @TempGenServiceTableWithNextOccurrence (Id, EndDate, EndAfterTimes, RepeatEveryTimes, FrequencyInt, FrequencyDetailInt, StartDate)
	SELECT	t1.Id, t1.EndDate, t1.EndAfterTimes, t1.RepeatEveryTimes, t1.FrequencyInt, t1.FrequencyDetailInt, t1.StartDate
	FROM	@TempGenServiceTable t1

	UPDATE @TempGenServiceTableWithNextOccurrence
	SET NextDate = (SELECT dbo.GetNextOccurence(@serviceDate, StartDate,  EndDate, EndAfterTimes, FrequencyInt, RepeatEveryTimes, FrequencyDetailInt))
	FROM @TempGenServiceTableWithNextOccurrence

	--Remove any rows that do not have a NextOccurrence past or on the OnOrAfterDate
	DELETE FROM @TempGenServiceTableWithNextOccurrence
	WHERE NextDate IS NULL OR NextDate > @serviceDate

	--This table will store all Existing Services after to the OnOrAfterDate
	DECLARE @TempNextExistingServiceTable TABLE
	(ServiceId uniqueidentifier,
	RecurringServiceId uniqueidentifier,
	OccurDate date)

	INSERT INTO @TempNextExistingServiceTable (ServiceId, RecurringServiceId, OccurDate)
	SELECT Id, RecurringServiceId, ServiceDate
	FROM Services
	WHERE ServiceProviderId = @serviceProviderIdContext AND ServiceDate = @serviceDate
	ORDER BY ServiceDate

	--Will store all the Services after the OnOrAfterDate
	DECLARE @ServicesForDayTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date
	)

	--Here we add the next occurrences to the final temporary table
	INSERT INTO @ServicesForDayTable (RecurringServiceId, OccurDate)
	SELECT Id, NextDate
	FROM @TempGenServiceTableWithNextOccurrence

	INSERT INTO @ServicesForDayTable (ServiceId, OccurDate)
	SELECT ServiceId, OccurDate
	FROM @TempNextExistingServiceTable

	--This table will temporarily store the RecurringServiceId's of all rows that appear more than once in @TempTable
	DECLARE @DuplicateIdTable TABLE
	(RecurringServiceId uniqueidentifier)

	--Add the duplicate Id's to the table created above
	INSERT INTO @DuplicateIdTable
	SELECT RecurringServiceId FROM @ServicesForDayTable t1
	WHERE 1 < 
	(
		SELECT COUNT (*) 
		FROM @ServicesForDayTable t2
		WHERE t1.RecurringServiceId = t2.RecurringServiceId
	)

	--Delete matching Services between @IdTable and @TempTable. This will delete the Generated Service and will keep the Existing Service
	DELETE FROM @ServicesForDayTable
	WHERE EXISTS
	(
		SELECT * FROM @DuplicateIdTable t1
		WHERE	RecurringServiceId = t1.RecurringServiceId
		AND		ServiceId IS NOT NULL
	)

	INSERT INTO @ServicesTableToReturn
	SELECT DISTINCT *
	FROM @ServicesForDayTable
	ORDER BY OccurDate ASC

RETURN 
END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
USE Core;
GO
IF OBJECT_ID(N'[dbo].[GetServiceHolders]', N'FN') IS NOT NULL
DROP FUNCTION [dbo].[GetServiceHolders]
GO
   /****************************************************************************************************************************************************
	* FUNCTION will take the context provided and find all the service dates
	* for RecurringServices and existing Services with that context. The function will return past, future or both past and future Services.
	* For RecurringServices, if there are existing services for dates it will return those. Otherwise it will generate a date for the instances.
	* You can distinguish generated services because they will not have a ServiceId *
	** Input Parameters **
	* @serviceProviderIdContext - The BusinessAccount context or NULL 
	* @clientIdContext - The Client context or NULL 
	* @recurringServiceIdContext - The RecurringService context or NULL
	* @firstDateToLookForServices - The reference date to look for services before or after. Also known as the onOrBeforeDate and the onOrAfterDate
	* @numberOfOccurrences - The number of occurrences to return
	* @getPrevious - If set to 1, this will return previous services
	* @getNext - If set to 1, this will return future services
	** Output Parameters: **
	* @ServicesTableToReturn - Ex. below
	* RecurringServiceId                     | ServiceId                              | OccurDate
	* -----------------------------------------------------------------------------------------
	* {036BD670-39A5-478F-BFA3-AD312E3F7F47} |                                        | 1/1/2012 <-- Generated service
	* {B30A43AD-655A-449C-BD4E-951F8F988718} |                                        | 1/1/2012 <-- Existing service
	* {03DB9F9B-2FF6-4398-B984-533FB3E19C50} | {FC222C74-EFEA-4B45-93FB-B042E6D6DB0D} | 1/2/2012 <-- Existing service with a RecurringService parent **
	***************************************************************************************************************************************************/
    CREATE FUNCTION [dbo].[GetServiceHolders]
    (@serviceProviderIdContext uniqueidentifier,
    @clientIdContext uniqueidentifier,
    @recurringServiceIdContext uniqueidentifier,
    @firstDateToLookForServices date,
    @numberOfOccurrences int,
	@getPrevious bit,
	@getNext bit)
	--TODO RENAME TempTable to ...
	--TempTable is where we will put all the Services and their corresponding dates
	RETURNS @ServicesTableToReturn TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date
	)

    AS       
    BEGIN  

	--Stores the Recurring Services that are associated with the lowest context provided
	DECLARE @TempGenServiceTable TABLE
	(Id uniqueidentifier,
     EndDate date,
     EndAfterTimes int,
     RepeatEveryTimes int,
     FrequencyInt int,
     FrequencyDetailInt int,
     StartDate date,
	 NextDate date,
	 ServiceName nvarchar(max))

	 --Insert all (the single) RecurringService associated with the @recurringServiceIdContext to @TempGenServiceTable
	IF @recurringServiceIdContext IS NOT NULL
	BEGIN		
	INSERT INTO @TempGenServiceTable (Id, EndDate, EndAfterTimes, RepeatEveryTimes, FrequencyInt, FrequencyDetailInt, StartDate, ServiceName)
	SELECT	t1.Id, t1.EndDate, t1.EndAfterTimes, t1.RepeatEveryTimes, t1.FrequencyInt, t1.FrequencyDetailInt, t1.StartDate, t2.Name
	FROM	Repeats t1, ServiceTemplates t2
	WHERE	@recurringServiceIdContext = t1.Id
			AND @recurringServiceIdContext = t2.Id
	END
	--Insert all (the single) RecurringServices associated with the @clientIdContext to @TempGenServiceTable
	ELSE IF @clientIdContext IS NOT NULL
	BEGIN
	INSERT INTO @TempGenServiceTable (Id, EndDate, EndAfterTimes, RepeatEveryTimes, FrequencyInt, FrequencyDetailInt, StartDate, ServiceName)
	--This is a Semi-Join between the Repeats table created above and the RecurringServices Table
	--Semi-Join simply means that it has all the same logic as a normal join, but it doesnt actually join the tables
	--In this case, it finds all the Repeats that correspond to a RecurringService with a ClientId = @clientIdContext
	SELECT t1.Id, t1.EndDate, t1.EndAfterTimes, t1.RepeatEveryTimes, t1.FrequencyInt, t1.FrequencyDetailInt, t1.StartDate, t2.Name
	FROM		Repeats t1, ServiceTemplates t2
	WHERE		EXISTS
	(
		SELECT	* 
		FROM	RecurringServices
		WHERE	ClientId = @clientIdContext
				AND RecurringServices.Id = t1.Id
				AND RecurringServices.Id = t2.Id
	)
	END
	
	--Insert all (the single) RecurringServices associated with the @serviceProviderIdContext to @TempGenServiceTable
	ELSE
	BEGIN
	INSERT INTO @TempGenServiceTable (Id, EndDate, EndAfterTimes, RepeatEveryTimes, FrequencyInt, FrequencyDetailInt, StartDate, ServiceName)
	--This is a Semi-Join between the Clients table created above and the RecurringServices Table
	--Semi-Join simply means that it has all the same logic as a normal join, but it doesnt actually join the tables
	--In this case, it finds all the RecurringServices that correspond to a Client with a vendorId = @serviceProviderIdContext
	SELECT	t1.Id, t1.EndDate, t1.EndAfterTimes, t1.RepeatEveryTimes, t1.FrequencyInt, t1.FrequencyDetailInt, t1.StartDate, t2.Name
	FROM		Repeats t1, ServiceTemplates t2
	WHERE		EXISTS
	(
		SELECT	*
		FROM	Clients
		WHERE	EXISTS
		(
		SELECT	*
		FROM	RecurringServices
		WHERE	RecurringServices.ClientId = Clients.Id 
				AND Clients.VendorId = @serviceProviderIdContext 
				AND RecurringServices.Id = t1.Id
				AND RecurringServices.Id = t2.Id
		)
	)
	END


	IF @getNext = 1
	BEGIN
		--This table is simply the result of merging @TempGenServiceTable and @TempNextDateTable
		DECLARE @TempGenServiceTableWithNextOccurrence TABLE
		(Id uniqueidentifier,
		 EndDate date,
		 EndAfterTimes int,
		 RepeatEveryTimes int,
		 FrequencyInt int,
		 FrequencyDetailInt int,
		 StartDate date,
		 NextDate date,
		 ServiceName nvarchar(max))
	 
		--Merges @TempGenServiceTable amd @TempNextDateTable created above based on their Id into @TempGenServiceTableWithNextOccurrence
		INSERT INTO @TempGenServiceTableWithNextOccurrence (Id, EndDate, EndAfterTimes, RepeatEveryTimes, FrequencyInt, FrequencyDetailInt, StartDate, ServiceName)
		SELECT	t1.Id, t1.EndDate, t1.EndAfterTimes, t1.RepeatEveryTimes, t1.FrequencyInt, t1.FrequencyDetailInt, t1.StartDate, t1.ServiceName
		FROM	@TempGenServiceTable t1

		UPDATE @TempGenServiceTableWithNextOccurrence
		SET NextDate = (SELECT dbo.GetNextOccurence(@firstDateToLookForServices, StartDate,  EndDate, EndAfterTimes, FrequencyInt, RepeatEveryTimes, FrequencyDetailInt))
		FROM @TempGenServiceTableWithNextOccurrence

		--Remove any rows that do not have a NextOccurrence past or on the OnOrAfterDate
		DELETE FROM @TempGenServiceTableWithNextOccurrence
		WHERE NextDate IS NULL
	END

	IF @getPrevious = 1
	BEGIN
		--Subtracts one day to @firstDateToLookForServices if you are looking for both Next and Previous
		--If we were to omit this, the GetPreviousOccurrence and GetNextOccurrence functions
		--would both include the original @firstDateToLookForServices
		--Thus throwing off the output of this function
		IF @getNext = 1 AND @getPrevious = 1
			SET @firstDateToLookForServices = DATEADD (day, -1, @firstDateToLookForServices)
		
		--This table is simply the result of merging @TempGenServiceTable and @TempPreviousDateTable
		DECLARE @TempGenServiceTableWithPreviousOccurrence TABLE
		(Id uniqueidentifier,
		 EndDate date,
		 EndAfterTimes int,
		 RepeatEveryTimes int,
		 FrequencyInt int,
		 FrequencyDetailInt int,
		 StartDate date,
		 PreviousDate date,
		 ServiceName nvarchar(max))
	
		--Merges @TempGenServiceTable amd @TempPreviousDateTable created above based on their Id into @TempGenServiceTableWithNextOccurrence
		INSERT INTO @TempGenServiceTableWithPreviousOccurrence (Id, EndDate, EndAfterTimes, RepeatEveryTimes, FrequencyInt, FrequencyDetailInt, StartDate, ServiceName)
		SELECT	t1.Id, t1.EndDate, t1.EndAfterTimes, t1.RepeatEveryTimes, t1.FrequencyInt, t1.FrequencyDetailInt, t1.StartDate, t1.ServiceName
		FROM	@TempGenServiceTable t1

		UPDATE @TempGenServiceTableWithPreviousOccurrence
		SET PreviousDate = (SELECT dbo.GetPreviousOccurrence(@firstDateToLookForServices, StartDate,  EndDate, EndAfterTimes, FrequencyInt, RepeatEveryTimes, FrequencyDetailInt))
		FROM @TempGenServiceTableWithPreviousOccurrence

		--Remove any rows that do not have a PreviousOccurrence on or before the OnOrBeforeDate
		DELETE FROM @TempGenServiceTableWithPreviousOccurrence
		WHERE PreviousDate IS NULL
	END

	------------------------------------------------------------------
	--Here we will take the Recurring Services that are in @TempGenServiceTableWithNextOccurrence and find all occurrences in date order until
	--we get to the number specified by @numberOfOccurrences
	--We will also do the same thing with @TempGenServiceTableWithPreviousOccurrence except in reverse date order
	
	DECLARE @minVal date
	DECLARE @lastDateToLookFor date
	DECLARE @firstDateToLookFor date
	DECLARE @ServiceCount int
	DECLARE @NextDay date
	DECLARE @RowCountForRecurringServiceOccurrenceTable int
	DECLARE @RowCountForTempGenServiceTableWithNextOccurrence int
	DECLARE @RowCountForTempGenServiceTableWithPreviousOccurrence int
	DECLARE @splitNumberToGet int
	DECLARE @remainingNumberOfServicesToFind int

	--Table to temporarily store all the Recurring Services dates in the future
	DECLARE @NextRecurringServiceOccurrenceTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)

	--Table to temporarily store all the Recurring Services dates in the past
	DECLARE @PreviousRecurringServiceOccurrenceTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)

	--Set up @splitNumberToGet so that it is half the size of what the function is looking to output
	IF @GetNext = 1 AND @GetPrevious = 1
	BEGIN
		SET @splitNumberToGet = @numberOfOccurrences / 2
	END
	ELSE
	BEGIN
		SET @splitNumberToGet = @numberOfOccurrences
	END

	SET		@RowCountForRecurringServiceOccurrenceTable = 0
	--Preset to a value above 0 just so it makes it into the loop. Actual value is calculated there
	SET		@RowCountForTempGenServiceTableWithPreviousOccurrence = 1
	

	IF		@GetPrevious = 1
	BEGIN
	--Will loop until it finds @splitNumberToGet services in the future
	WHILE	@RowCountForRecurringServiceOccurrenceTable <= @splitNumberToGet
	BEGIN
		SET		@minVal = (SELECT MAX(PreviousDate) FROM @TempGenServiceTableWithPreviousOccurrence)
		SET		@NextDay = DATEADD(day, -1, @minVal)
		
		SET		@RowCountForTempGenServiceTableWithPreviousOccurrence = (SELECT COUNT(*) FROM @TempGenServiceTableWithPreviousOccurrence)

		--Checks to be sure that there are still rows to look at in @TempGenServiceTableWithNextOccurrence
		IF NOT	@RowCountForTempGenServiceTableWithPreviousOccurrence > 0
		BEGIN
				BREAK
		END
		
		--Inserts all Services with the lowest date to @PreviousRecurringServiceOccurrenceTable
		INSERT INTO @PreviousRecurringServiceOccurrenceTable
		SELECT	Id, PreviousDate, ServiceName
		FROM	@TempGenServiceTableWithPreviousOccurrence
		WHERE	PreviousDate = @minVal

		--Updates all Services that were just put into a new table to show their previous occurrence date
		UPDATE	@TempGenServiceTableWithPreviousOccurrence
		SET		PreviousDate = (SELECT dbo.GetPreviousOccurrence(@NextDay, StartDate,  EndDate, EndAfterTimes, FrequencyInt, RepeatEveryTimes, FrequencyDetailInt))
		FROM	@TempGenServiceTableWithPreviousOccurrence
		WHERE	PreviousDate = @minVal

		--If any of those Services updated above do not have a previous occurrence, they will be deleted from the table
		DELETE FROM @TempGenServiceTableWithPreviousOccurrence
		WHERE	PreviousDate IS NULL

		--Sets up row count so that when the loop returns to the top, we will know if there are any rows remaining
		SET		@RowCountForRecurringServiceOccurrenceTable = (SELECT COUNT(*) FROM @PreviousRecurringServiceOccurrenceTable)
	END
	END
	
	--Set to figure out the minimum number of services needed from the GetNextOccurrence Function
	SET		@remainingNumberOfServicesToFind = @numberOfOccurrences - @RowCountForRecurringServiceOccurrenceTable
	
	SET		@RowCountForRecurringServiceOccurrenceTable = 0
	--Preset to a value above 0 just so it makes it into the loop. Actual value is calculated there
	SET		@RowCountForTempGenServiceTableWithNextOccurrence = 1
	IF		@GetNext = 1
	BEGIN
	--Will loop until it fills in remaining number of Services to get to @numberOfOccurrences
	WHILE	@RowCountForRecurringServiceOccurrenceTable <= @remainingNumberOfServicesToFind
	BEGIN
		SET		@minVal = (SELECT MIN(NextDate) FROM @TempGenServiceTableWithNextOccurrence)
		SET		@NextDay = DATEADD(day, 1, @minVal)

		SET		@RowCountForTempGenServiceTableWithNextOccurrence = (SELECT COUNT(*) FROM @TempGenServiceTableWithNextOccurrence)

		--Checks to be sure that there are still rows to look at in @TempGenServiceTableWithNextOccurrence
		IF NOT	@RowCountForTempGenServiceTableWithNextOccurrence > 0
		BEGIN
				BREAK
		END

		--Inserts all Services with the lowest date to @NextRecurringServiceOccurrenceTable
		INSERT INTO @NextRecurringServiceOccurrenceTable
		SELECT	Id, NextDate, ServiceName
		FROM	@TempGenServiceTableWithNextOccurrence
		WHERE	NextDate = @minVal

		--Updates all Services that were just put into a new table to show their next occurrence date
		UPDATE	@TempGenServiceTableWithNextOccurrence
		SET		NextDate = (SELECT dbo.GetNextOccurence(@NextDay, StartDate,  EndDate, EndAfterTimes, FrequencyInt, RepeatEveryTimes, FrequencyDetailInt))
		FROM	@TempGenServiceTableWithNextOccurrence
		WHERE	NextDate = @minVal

		--If any of those Services updated above do not have a next occurrence, they will be deleted from the table
		DELETE FROM @TempGenServiceTableWithNextOccurrence
		WHERE	NextDate IS NULL

		--Sets up row count so that when the loop returns to the top, we will know if there are any rows remaining
		SET		@RowCountForRecurringServiceOccurrenceTable = (SELECT COUNT(*) FROM @NextRecurringServiceOccurrenceTable)

	END
	END
	
	SET		@lastDateToLookFor = (SELECT MAX(OccurDate) FROM @NextRecurringServiceOccurrenceTable)
	SET		@firstDateToLookFor = (SELECT MIN(OccurDate) FROM @PreviousRecurringServiceOccurrenceTable)
	-----------------------------------------------------------------------
	--Here we will add Existing Services to the table of their own

	--This table will store all Existing Services prior to the OnOrAfterDate
	DECLARE @TempPreviousExistingServiceTable TABLE
	(ServiceId uniqueidentifier,
	RecurringServiceId uniqueidentifier,
	OccurDate date,
	ServiceName nvarchar(max))

	--This table will store all Existing Services after to the OnOrAfterDate
	DECLARE @TempNextExistingServiceTable TABLE
	(ServiceId uniqueidentifier,
	RecurringServiceId uniqueidentifier,
	OccurDate date,
	ServiceName nvarchar(max))

	IF @recurringServiceIdContext IS NOT NULL
	BEGIN
	--Inserts all Existing Services with a service date after the OnOrAfterDate or before the OnOrBeforeDate to a temp table
	INSERT INTO @TempPreviousExistingServiceTable (ServiceId, RecurringServiceId, OccurDate, ServiceName)
	SELECT TOP(@numberOfOccurrences) t1.Id, t1.RecurringServiceId, t1.ServiceDate, t2.Name
	FROM	Services t1, ServiceTemplates t2
	WHERE	t1.RecurringServiceId = @recurringServiceIdContext 
			AND t1.ServiceDate BETWEEN @firstDateToLookFor AND @firstDateToLookForServices
			AND t1.Id = t2.Id
	ORDER BY ServiceDate

	INSERT INTO @TempNextExistingServiceTable (ServiceId, RecurringServiceId, OccurDate, ServiceName)
	SELECT TOP(@numberOfOccurrences) t1.Id, t1.RecurringServiceId, t1.ServiceDate, t2.Name
	FROM	Services t1, ServiceTemplates t2
	WHERE	t1.RecurringServiceId = @recurringServiceIdContext 
			AND t1.ServiceDate BETWEEN @firstDateToLookForServices AND @lastDateToLookFor
			AND t1.Id = t2.Id
	ORDER BY ServiceDate
	END

	ELSE IF @clientIdContext IS NOT NULL
	BEGIN
	--Inserts all Existing Services with a service date after the OnOrAfterDate or before the OnOrBeforeDate to a temp table
	INSERT INTO @TempPreviousExistingServiceTable (ServiceId, RecurringServiceId, OccurDate, ServiceName)
	SELECT TOP(@numberOfOccurrences) t1.Id, t1.RecurringServiceId, t1.ServiceDate, t2.Name
	FROM	Services t1, ServiceTemplates t2
	WHERE	t1.ClientId = @clientIdContext 
			AND t1.ServiceDate BETWEEN @firstDateToLookFor AND @firstDateToLookForServices
			AND t1.Id = t2.Id
	ORDER BY ServiceDate

	INSERT INTO @TempNextExistingServiceTable (ServiceId, RecurringServiceId, OccurDate, ServiceName)
	SELECT TOP(@numberOfOccurrences) t1.Id, t1.RecurringServiceId, t1.ServiceDate, t2.Name
	FROM	Services t1, ServiceTemplates t2
	WHERE	t1.ClientId = @clientIdContext 
			AND t1.ServiceDate BETWEEN @firstDateToLookForServices AND @lastDateToLookFor
			AND t1.Id = t2.Id
	ORDER BY ServiceDate
	END

	ELSE IF @serviceProviderIdContext IS NOT NULL
	BEGIN 
	--Inserts all Existing Services with a service date after the OnOrAfterDate or before the OnOrBeforeDate to a temp table
	INSERT INTO @TempPreviousExistingServiceTable (ServiceId, RecurringServiceId, OccurDate, ServiceName)
	SELECT TOP(@numberOfOccurrences) t1.Id, t1.RecurringServiceId, t1.ServiceDate, t2.Name
	FROM	Services t1, ServiceTemplates t2
	WHERE	t1.ServiceProviderId = @serviceProviderIdContext 
			AND t1.ServiceDate BETWEEN @firstDateToLookFor AND @firstDateToLookForServices
			AND t1.Id = t2.Id
	ORDER BY ServiceDate

	INSERT INTO @TempNextExistingServiceTable (ServiceId, RecurringServiceId, OccurDate, ServiceName)
	SELECT TOP(@numberOfOccurrences) t1.Id, t1.RecurringServiceId, t1.ServiceDate, t2.Name
	FROM	Services t1, ServiceTemplates t2
	WHERE	t1.ServiceProviderId = @serviceProviderIdContext 
			AND t1.ServiceDate BETWEEN @firstDateToLookForServices AND @lastDateToLookFor
			AND t1.Id = t2.Id
	ORDER BY ServiceDate
	END
------------------------------------------------------------------------------------------
	--Will store all the Services before the OnOrAfterDate
	DECLARE @PreviousIntermediateTempTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)

	--Will store all the Services after the OnOrAfterDate
	DECLARE @NextIntermediateTempTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)

	--Here we add all the previous occurrences to the final temporary table
	INSERT INTO @PreviousIntermediateTempTable (RecurringServiceId, OccurDate, ServiceName)
	SELECT RecurringServiceId, OccurDate, ServiceName
	FROM @PreviousRecurringServiceOccurrenceTable

	INSERT INTO @PreviousIntermediateTempTable (RecurringServiceId, ServiceId, OccurDate, ServiceName)
	SELECT RecurringServiceId, ServiceId, OccurDate, ServiceName
	FROM @TempPreviousExistingServiceTable

	--Here we add the next occurrences to the final temporary table
	INSERT INTO @NextIntermediateTempTable (RecurringServiceId, OccurDate, ServiceName)
	SELECT RecurringServiceId, OccurDate, ServiceName
	FROM @NextRecurringServiceOccurrenceTable

	INSERT INTO @NextIntermediateTempTable (RecurringServiceId, ServiceId, OccurDate, ServiceName)
	SELECT RecurringServiceId, ServiceId, OccurDate, ServiceName
	FROM @TempNextExistingServiceTable

	--This is just a copy of the output table that allows for more than the specified number of Services to be added
	DECLARE @TempTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)	
	--Here we take the two final temporary tables and combine them into a table that resembles the final table
	INSERT INTO @TempTable
	SELECT * 
	FROM @PreviousIntermediateTempTable
	ORDER BY OccurDate

	INSERT INTO @TempTable
	SELECT * 
	FROM @NextIntermediateTempTable
	ORDER BY OccurDate

	--This table will temporarily store the RecurringServiceId's of all rows that appear more than once in @TempTable
	DECLARE @DuplicateIdTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)

	INSERT INTO @DuplicateIdTable
	SELECT t1.RecurringServiceId, t2.ServiceId, t2.OccurDate, t2.ServiceName FROM (SELECT OccurDate, RecurringServiceId, ServiceId FROM @TempTable AS t2 WHERE ServiceId IS NOT NULL GROUP BY OccurDate, RecurringServiceId, ServiceId) t1
	JOIN @TempTable as t2 on t1.RecurringServiceId = t2.RecurringServiceId AND t1.OccurDate = t2.OccurDate AND t2.ServiceId IS NULL

	DECLARE @holderTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)

	--Filtered down from the table above so that only the correct number of Services is returned from the function
	SET ROWCOUNT @numberOfOccurrences
	INSERT INTO @holderTable
	SELECT * FROM @TempTable
	EXCEPT
	SELECT * FROM @DuplicateIdTable
	ORDER BY OccurDate ASC
	SET ROWCOUNT 0

	Declare @lastDate date

	SET @lastDate = (SELECT MAX(OccurDate) FROM @holderTable)

	INSERT INTO @ServicesTableToReturn
	SELECT * FROM @TempTable
	EXCEPT
	SELECT * FROM @DuplicateIdTable

	DELETE FROM @ServicesTableToReturn
	WHERE OccurDate > @lastDate

	RETURN
	END

GO

CREATE VIEW [dbo].[PartiesWithName]
AS
SELECT        dbo.Parties.Id, ISNULL(dbo.Parties_Person.LastName, '') + ' ' +  ISNULL(dbo.Parties_Person.FirstName, '') +' ' +  ISNULL(dbo.Parties_Person.MiddleInitial, '')  AS 'ChildName'
FROM            dbo.Parties INNER JOIN
                         dbo.Parties_Person ON dbo.Parties.Id = dbo.Parties_Person.Id
UNION
SELECT        dbo.Parties.Id, dbo.Parties_Business.Name AS 'ChildName'
FROM            dbo.Parties INNER JOIN
                         dbo.Parties_Business ON dbo.Parties.Id = dbo.Parties_Business.Id

GO

GO

Create FUNCTION [dbo].[GetFirstDayOfMonth]
    (@Date datetime)       
    returns Datetime       
    as       
    begin  
		return dateadd(day,1-day(@Date),@date)   
    end
GO
Create FUNCTION [dbo].[GetLastDayOfMonth]
    (@Date datetime)       
    returns Datetime       
    as       
    begin  
		return dateadd(day,-1,dateadd(month,1,dbo.GetFirstDayOfMonth(@date)))   
    end
Go
Create FUNCTION [dbo].[GetFirstOfDayOfWeekInMonth]
    (@Date datetime,@DoW int)       
    returns Datetime       
    as       
    begin  
		declare @FirstDate datetime, @DateDif int
		set @FirstDate = dbo.GetFirstDayOfMonth(@date)
		set @DateDif = @DoW - datepart(dw,@FirstDate)
		if @DateDif < 0
			set @DateDif = @DateDif + 7
		return dateadd(d,@DateDif,@FirstDate)
    end
Go
Create FUNCTION [dbo].[GetLastOfDayOfWeekInMonth]
    (@Date datetime,@DoW int)       
    returns Datetime       
    as       
    begin  
		declare @LastDate datetime, @DateDif int
		set @LastDate = dbo.GetLastDayOfMonth(@Date)
		set @DateDif = @DoW - datepart(dw,@LastDate)
		if @DateDif > 0
			set @DateDif = @DateDif - 7
		return dateadd(d,@DateDif,@LastDate)		
    end
go
-- only use for check 29th, 30th, 31st with 8 = OnDayInMonth
Create FUNCTION [dbo].[GetNextDayInMonth]
    (@StarDate datetime,@EndDate datetime, @repeatEveryTimes int, @endAfterTime int)       
    returns Datetime       
    as       
    begin  
		declare @Times int,@TempDate datetime, @TimeOccur datetime
		set @Times = 1
		set @TimeOccur = 1
		set @TempDate = @StarDate
		
		while not (day(@StarDate) = day(@TempDate) and @TempDate > @EndDate)
			begin
				set @TempDate = dateadd(month,@repeatEveryTimes*@Times,@StarDate)
				if day(@StarDate) = day(@TempDate)
					set @TimeOccur = @TimeOccur + 1
				set @Times = @Times + 1
			end
		if @endAfterTime is not null and @TimeOccur > @endAfterTime
			return null
		return @TempDate
    end

GO
IF OBJECT_ID(N'[dbo].[GetPreviousDayInMonth]', N'FN') IS NOT NULL
DROP FUNCTION [dbo].[GetPreviousDayInMonth]
GO
Create FUNCTION [dbo].[GetPreviousDayInMonth]
    (@StarDate datetime,@EndDate datetime, @repeatEveryTimes int, @endAfterTime int)       
    returns Datetime       
    as       
    begin  
		declare @Times int,@TempDate datetime, @TimeOccur int, @ReturnDate datetime
		set @Times = 0
		set @TimeOccur = 0
		set @TempDate = @StarDate
		
		while @TempDate < @EndDate and (@endAfterTime is null or @TimeOccur < @endAfterTime)
			begin
				set @TempDate = dateadd(month,@repeatEveryTimes*@Times,@StarDate)
				if day(@StarDate) = day(@TempDate) and @TempDate <= @EndDate
					begin
						set @TimeOccur = @TimeOccur + 1
						set @ReturnDate = @TempDate
					end
				set @Times = @Times + 1
			end		
		return @ReturnDate
    end
    
 GO


GO

CREATE VIEW [dbo].[ServiceTemplatesWithVendorId]
as
SELECT ServiceTemplates.Id as 'ServiceTemplateId', Parties_BusinessAccount.Id as 'VendorId'
FROM ServiceTemplates, Parties_BusinessAccount
WHERE ServiceTemplates.OwnerServiceProviderId = Parties_BusinessAccount.Id
UNION ALL
SELECT ServiceTemplates.Id as 'ServiceTemplateId', Clients.VendorId as 'VendorId'
FROM ServiceTemplates, Clients
WHERE Clients.Id = ServiceTemplates.OwnerClientId
UNION ALL
SELECT ServiceTemplates.Id as 'ServiceTemplateId', Services.ServiceProviderId as 'VendorId'
FROM ServiceTemplates, Services
WHERE ServiceTemplates.Id = Services.Id
UNION ALL
SELECT ServiceTemplates.Id as 'ServiceTemplateId', Clients.VendorId as 'VendorId'
FROM ServiceTemplates, RecurringServices, Clients
WHERE ServiceTemplates.Id = RecurringServices.Id AND RecurringServices.ClientId = Clients.Id

GO	