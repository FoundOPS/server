 

-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 10/29/2012 16:18:01
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

IF OBJECT_ID(N'[dbo].[FK_BusinessAccount_inherits_Party]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Parties_BusinessAccount] DROP CONSTRAINT [FK_BusinessAccount_inherits_Party];
GO
IF OBJECT_ID(N'[dbo].[FK_BusinessAccountInvoice]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Invoices] DROP CONSTRAINT [FK_BusinessAccountInvoice];
GO
IF OBJECT_ID(N'[dbo].[FK_BusinessAccountLocation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Locations] DROP CONSTRAINT [FK_BusinessAccountLocation];
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
IF OBJECT_ID(N'[dbo].[FK_BusinessAccountVehicle]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Vehicles] DROP CONSTRAINT [FK_BusinessAccountVehicle];
GO
IF OBJECT_ID(N'[dbo].[FK_ClientBusinessAccount]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Clients] DROP CONSTRAINT [FK_ClientBusinessAccount];
GO
IF OBJECT_ID(N'[dbo].[FK_ClientContactInfo]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ContactInfoSet] DROP CONSTRAINT [FK_ClientContactInfo];
GO
IF OBJECT_ID(N'[dbo].[FK_ClientInvoice]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Invoices] DROP CONSTRAINT [FK_ClientInvoice];
GO
IF OBJECT_ID(N'[dbo].[FK_ClientLocation1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Locations] DROP CONSTRAINT [FK_ClientLocation1];
GO
IF OBJECT_ID(N'[dbo].[FK_ClientServiceTemplate]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ServiceTemplates] DROP CONSTRAINT [FK_ClientServiceTemplate];
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
IF OBJECT_ID(N'[dbo].[FK_LocationBusinessAccount]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Locations] DROP CONSTRAINT [FK_LocationBusinessAccount];
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
IF OBJECT_ID(N'[dbo].[FK_LocationSubLocation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SubLocations] DROP CONSTRAINT [FK_LocationSubLocation];
GO
IF OBJECT_ID(N'[dbo].[FK_NumericField_inherits_Field]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Fields_NumericField] DROP CONSTRAINT [FK_NumericField_inherits_Field];
GO
IF OBJECT_ID(N'[dbo].[FK_OptionsField_inherits_Field]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Fields_OptionsField] DROP CONSTRAINT [FK_OptionsField_inherits_Field];
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
IF OBJECT_ID(N'[dbo].[FK_RoleBusinessAccount]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Roles] DROP CONSTRAINT [FK_RoleBusinessAccount];
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
IF OBJECT_ID(N'[dbo].[FK_RouteTaskRecurringService]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RouteTasks] DROP CONSTRAINT [FK_RouteTaskRecurringService];
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
IF OBJECT_ID(N'[dbo].[FK_TaskStatusBusinessAccount]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TaskStatuses] DROP CONSTRAINT [FK_TaskStatusBusinessAccount];
GO
IF OBJECT_ID(N'[dbo].[FK_TaskStatusRouteTask]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RouteTasks] DROP CONSTRAINT [FK_TaskStatusRouteTask];
GO
IF OBJECT_ID(N'[dbo].[FK_TextBoxField_inherits_Field]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Fields_TextBoxField] DROP CONSTRAINT [FK_TextBoxField_inherits_Field];
GO
IF OBJECT_ID(N'[dbo].[FK_SignatureField_inherits_Field]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Fields_SignatureField] DROP CONSTRAINT [FK_SignatureField_inherits_Field];
GO
IF OBJECT_ID(N'[dbo].[FK_UserAccount_inherits_Party]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Parties_UserAccount] DROP CONSTRAINT [FK_UserAccount_inherits_Party];
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

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Blocks]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Blocks];
GO
IF OBJECT_ID(N'[dbo].[Clients]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Clients];
GO
IF OBJECT_ID(N'[dbo].[ContactInfoSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ContactInfoSet];
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
IF OBJECT_ID(N'[dbo].[Fields_SignatureField]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Fields_SignatureField];
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
IF OBJECT_ID(N'[dbo].[Parties]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Parties];
GO
IF OBJECT_ID(N'[dbo].[Parties_BusinessAccount]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Parties_BusinessAccount];
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
IF OBJECT_ID(N'[dbo].[TaskStatuses]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TaskStatuses];
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
    [Color] nvarchar(max)  NOT NULL,
    [HideFromNavigation] bit  NOT NULL,
    [IconUrl] nvarchar(max)  NULL,
    [HoverIconUrl] nvarchar(max)  NULL,
    [Url] nvarchar(max)  NULL,
    [IsSilverlight] bit  NULL
);
GO

-- Creating table 'Roles'
CREATE TABLE [dbo].[Roles] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(max)  NULL,
    [Description] nvarchar(max)  NULL,
    [RoleTypeInt] smallint  NOT NULL,
    [OwnerBusinessAccountId] uniqueidentifier  NULL
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
    [PostalCode] nvarchar(max)  NULL,
    [AddressLineTwo] nvarchar(max)  NULL,
    [AdminDistrictOne] nvarchar(max)  NULL,
    [Latitude] decimal(11,8)  NULL,
    [AdminDistrictTwo] nvarchar(max)  NULL,
    [RegionId] uniqueidentifier  NULL,
    [BusinessAccountIdIfDepot] uniqueidentifier  NULL,
    [BusinessAccountId] uniqueidentifier  NULL,
    [ClientId] uniqueidentifier  NULL,
    [IsDefaultBillingLocation] bit  NULL,
    [CountryCode] nvarchar(max)  NULL,
    [DateDeleted] datetime  NULL
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
    [LastPushToAzureTimeStamp] datetime  NULL,
    [LastAccuracy] int  NULL,
    [BusinessAccountId] uniqueidentifier  NOT NULL
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
    [Name] nvarchar(max)  NULL,
    [DateDeleted] datetime  NULL
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
    [ExcludedDatesString] nvarchar(max)  NULL,
    [DateDeleted] datetime  NULL
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
    [FirstName] nvarchar(max)  NULL,
    [LastName] nvarchar(max)  NULL,
    [MiddleInitial] nvarchar(max)  NULL,
    [GenderInt] smallint  NULL,
    [DateOfBirth] datetime  NULL,
    [AddressLineOne] nvarchar(max)  NULL,
    [AddressLineTwo] nvarchar(max)  NULL,
    [AdminDistrictTwo] nvarchar(max)  NULL,
    [Comments] nvarchar(max)  NULL,
    [AdminDistrictOne] nvarchar(max)  NULL,
    [PostalCode] nvarchar(max)  NULL,
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
    [LastAccuracy] int  NULL,
    [CountryCode] nvarchar(max)  NULL
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
    [OriginalDate] datetime  NULL,
    [BusinessAccountId] uniqueidentifier  NOT NULL,
    [EstimatedDuration] time  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [StatusInt] int  NOT NULL,
    [Date] datetime  NOT NULL,
    [OrderInRouteDestination] int  NOT NULL,
    [RecurringServiceId] uniqueidentifier  NULL,
    [DelayedChildId] uniqueidentifier  NULL,
    [TaskStatusId] uniqueidentifier  NULL
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

-- Creating table 'TaskStatuses'
CREATE TABLE [dbo].[TaskStatuses] (
    [Id] uniqueidentifier  NOT NULL,
    [Color] nvarchar(max)  NULL,
    [Name] nvarchar(max)  NULL,
    [DefaultTypeInt] int  NULL,
    [RemoveFromRoute] bit  NOT NULL,
    [BusinessAccountId] uniqueidentifier  NULL
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
    [Name] nvarchar(max)  NULL,
    [Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'Parties_UserAccount'
CREATE TABLE [dbo].[Parties_UserAccount] (
    [PasswordHash] nvarchar(max)  NULL,
    [EmailAddress] nvarchar(max)  NOT NULL,
    [LastActivity] datetime  NULL,
    [CreationDate] datetime  NOT NULL,
    [FirstName] nvarchar(max)  NULL,
    [LastName] nvarchar(max)  NULL,
    [MiddleInitial] nvarchar(max)  NULL,
    [GenderInt] smallint  NULL,
    [DateOfBirth] datetime  NULL,
    [TimeZone] nvarchar(max)  NULL,
    [ColumnConfigurations] nvarchar(max)  NULL,
    [PasswordSalt] varbinary(max)  NOT NULL,
    [TempResetToken] nvarchar(max)  NULL,
    [TempTokenExpireTime] datetime  NULL,
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

-- Creating table 'Fields_SignatureField'
CREATE TABLE [dbo].[Fields_SignatureField] (
    [Value] nvarchar(max)  NULL,
    [Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'Fields_OptionsField'
CREATE TABLE [dbo].[Fields_OptionsField] (
    [AllowMultipleSelection] bit  NOT NULL,
    [TypeInt] smallint  NOT NULL,
    [OptionsString] nvarchar(max)  NULL,
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

-- Creating primary key on [Id] in table 'TaskStatuses'
ALTER TABLE [dbo].[TaskStatuses]
ADD CONSTRAINT [PK_TaskStatuses]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Parties_BusinessAccount'
ALTER TABLE [dbo].[Parties_BusinessAccount]
ADD CONSTRAINT [PK_Parties_BusinessAccount]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Parties_UserAccount'
ALTER TABLE [dbo].[Parties_UserAccount]
ADD CONSTRAINT [PK_Parties_UserAccount]
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

-- Creating primary key on [Id] in table 'Fields_SignatureField'
ALTER TABLE [dbo].[Fields_SignatureField]
ADD CONSTRAINT [PK_Fields_SignatureField]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Fields_OptionsField'
ALTER TABLE [dbo].[Fields_OptionsField]
ADD CONSTRAINT [PK_Fields_OptionsField]
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

-- Creating primary key on [Routes_Id], [Employees_Id] in table 'RouteEmployee'
ALTER TABLE [dbo].[RouteEmployee]
ADD CONSTRAINT [PK_RouteEmployee]
    PRIMARY KEY CLUSTERED ([Routes_Id], [Employees_Id] ASC);
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
    ON DELETE CASCADE ON UPDATE NO ACTION;
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
    ON DELETE SET NULL ON UPDATE NO ACTION;

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
    ON DELETE SET NULL ON UPDATE NO ACTION;

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
    ON DELETE SET NULL ON UPDATE NO ACTION;
GO

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

-- Creating foreign key on [RecurringServiceId] in table 'RouteTasks'
ALTER TABLE [dbo].[RouteTasks]
ADD CONSTRAINT [FK_RouteTaskRecurringService]
    FOREIGN KEY ([RecurringServiceId])
    REFERENCES [dbo].[RecurringServices]
        ([Id])
    ON DELETE SET NULL ON UPDATE NO ACTION;

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
    ON DELETE CASCADE ON UPDATE NO ACTION;
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

-- Creating foreign key on [OwnerBusinessAccountId] in table 'Roles'
ALTER TABLE [dbo].[Roles]
ADD CONSTRAINT [FK_RoleBusinessAccount]
    FOREIGN KEY ([OwnerBusinessAccountId])
    REFERENCES [dbo].[Parties_BusinessAccount]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RoleBusinessAccount'
CREATE INDEX [IX_FK_RoleBusinessAccount]
ON [dbo].[Roles]
    ([OwnerBusinessAccountId]);
GO

-- Creating foreign key on [BusinessAccountId] in table 'TaskStatuses'
ALTER TABLE [dbo].[TaskStatuses]
ADD CONSTRAINT [FK_TaskStatusBusinessAccount]
    FOREIGN KEY ([BusinessAccountId])
    REFERENCES [dbo].[Parties_BusinessAccount]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TaskStatusBusinessAccount'
CREATE INDEX [IX_FK_TaskStatusBusinessAccount]
ON [dbo].[TaskStatuses]
    ([BusinessAccountId]);
GO

-- Creating foreign key on [TaskStatusId] in table 'RouteTasks'
ALTER TABLE [dbo].[RouteTasks]
ADD CONSTRAINT [FK_TaskStatusRouteTask]
    FOREIGN KEY ([TaskStatusId])
    REFERENCES [dbo].[TaskStatuses]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TaskStatusRouteTask'
CREATE INDEX [IX_FK_TaskStatusRouteTask]
ON [dbo].[RouteTasks]
    ([TaskStatusId]);
GO

-- Creating foreign key on [BusinessAccountId] in table 'Vehicles'
ALTER TABLE [dbo].[Vehicles]
ADD CONSTRAINT [FK_BusinessAccountVehicle]
    FOREIGN KEY ([BusinessAccountId])
    REFERENCES [dbo].[Parties_BusinessAccount]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_BusinessAccountVehicle'
CREATE INDEX [IX_FK_BusinessAccountVehicle]
ON [dbo].[Vehicles]
    ([BusinessAccountId]);
GO

-- Creating foreign key on [Id] in table 'Parties_BusinessAccount'
ALTER TABLE [dbo].[Parties_BusinessAccount]
ADD CONSTRAINT [FK_BusinessAccount_inherits_Party]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Parties]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'Parties_UserAccount'
ALTER TABLE [dbo].[Parties_UserAccount]
ADD CONSTRAINT [FK_UserAccount_inherits_Party]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Parties]
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

-- Creating foreign key on [Id] in table 'Fields_SignatureField'
ALTER TABLE [dbo].[Fields_SignatureField]
ADD CONSTRAINT [FK_SignatureField_inherits_Field]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Fields]
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

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
USE Core
GO
IF OBJECT_ID(N'[dbo].[ArchiveClientBasedOnId]', N'FN') IS NOT NULL
DROP PROCEDURE [dbo].ArchiveClientBasedOnId
GO
/****************************************************************************************************************************************************
* FUNCTION ArchiveClientBasedOnId will archive a Client and all entities associated with it
* Follows the following progression to delete: RouteDestinations, RouteTasks, Services, ServiceTemplates, RecurringServices, Locations 
* ClientTitles, Parties_Business and finally the Client itself
** Input Parameters **
* @clientId - The Client Id to be deleted
***************************************************************************************************************************************************/
CREATE PROCEDURE dbo.ArchiveClientBasedOnId
		(@clientId uniqueidentifier)

	AS
	BEGIN
	DECLARE @date DATE
	SET @date = GETUTCDATE()  
	
	UPDATE dbo.Clients
	SET DateDeleted = @date
	WHERE Id = @clientId
	  
-------------------------------------------------------------------------------------------------------------------------
--Delete Locations for Client
-------------------------------------------------------------------------------------------------------------------------
	DECLARE @LocationId uniqueidentifier
	
	DECLARE @LocationIdsForClient TABLE
	(
		LocationId uniqueidentifier
	)

	--Finds all Locations that are associated with the Client
	INSERT INTO @LocationIdsForClient
	SELECT Id FROM Locations
	WHERE	ClientId = @clientId

	DECLARE @RowCount int
	SET @RowCount = (SELECT COUNT(*) FROM @LocationIdsForClient)

	--Iterates through @LocationIdsForClient and calls DeleteLocationBasedOnId on each
	WHILE @RowCount > 0
	BEGIN
			SET @LocationId = (SELECT MIN(LocationId) FROM @LocationIdsForClient)

			EXEC dbo.ArchiveLocationBasedOnId @locationId = @LocationId, @date = @date

			DELETE FROM @LocationIdsForClient
			WHERE LocationId = @LocationId

			SET @RowCount = (SELECT COUNT(*) FROM @LocationIdsForClient)
	END
-------------------------------------------------------------------------------------------------------------------------
	END
	RETURN

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
USE Core
GO

IF OBJECT_ID(N'[dbo].[ArchiveLocationBasedOnId]', N'FN') IS NOT NULL
DROP PROCEDURE [dbo].[ArchiveLocationBasedOnId]
GO
/****************************************************************************************************************************************************
* FUNCTION ArchiveLocationBasedOnId will delete a Location and all entities associated with it
* Follows the following progression to delete: RouteTasks, SubLocations, ContactInfoSet, RouteDestinations and finally the Location itself
** Input Parameters **
* @locationId - The Location Id to be deleted
***************************************************************************************************************************************************/
CREATE PROCEDURE dbo.ArchiveLocationBasedOnId
		(@locationId UNIQUEIDENTIFIER, @date DATE)

	AS
	BEGIN
  
	UPDATE	dbo.Locations
	SET		DateDeleted = @date
	WHERE	Id = @locationId   

	CREATE TABLE #RecurringService (Id UNIQUEIDENTIFIER)

	INSERT INTO #RecurringService
	SELECT Id FROM dbo.RecurringServices
	WHERE	Id IN (
			SELECT	Id
			FROM	dbo.ServiceTemplates
			WHERE	Id IN ( SELECT	ServiceTemplateId
							FROM	dbo.Fields
							WHERE	Id IN ( SELECT	Id
											FROM	dbo.Fields_LocationField
											WHERE	LocationId = @locationId ) ) )

	
	UPDATE	dbo.RecurringServices
	SET		DateDeleted = @date
	WHERE Id IN (SELECT Id FROM #RecurringService)

	UPDATE dbo.Repeats
	SET EndDate = @date
	WHERE Id IN (SELECT Id FROM #RecurringService)

	CREATE TABLE #RouteTasksToDelete
		(
		  Id UNIQUEIDENTIFIER ,
		  RouteDestinationId UNIQUEIDENTIFIER
		)

	INSERT	INTO #RouteTasksToDelete
			SELECT	Id ,
					RouteDestinationId
			FROM	dbo.RouteTasks
			WHERE	LocationId = @locationId
					AND [Date] > @date

	DELETE	FROM RouteTasks
	WHERE	Id IN ( SELECT	Id
					FROM	#RouteTasksToDelete )

	DELETE	FROM RouteDestinations
	WHERE	Id IN ( SELECT	RouteDestinationId
					FROM	#RouteTasksToDelete )
	
	CREATE TABLE #ServicesToDelete ( Id UNIQUEIDENTIFIER )
	
	INSERT	INTO #ServicesToDelete
			SELECT	Id
			FROM	dbo.Services
			WHERE	Id IN (
					SELECT	Id
					FROM	dbo.ServiceTemplates
					WHERE	Id IN (
							SELECT	ServiceTemplateId
							FROM	dbo.Fields
							WHERE	Id IN ( SELECT	Id
											FROM	dbo.Fields_LocationField
											WHERE	LocationId = @locationId ) ) 
					AND ServiceDate >= @Date
					AND RecurringServiceId IS NULL
					AND LevelInt = 5)
	
	DELETE FROM dbo.Services
	WHERE Id IN (SELECT Id FROM #ServicesToDelete)

	DECLARE @RowCount INT
	DECLARE @RowId UNIQUEIDENTIFIER

	SET @RowCount = (SELECT COUNT(*) FROM #ServicesToDelete)

	WHILE @RowCount > 0
	BEGIN
	
		SET @RowId = (SELECT TOP(1) Id FROM #ServicesToDelete ORDER BY Id)

		EXECUTE [dbo].[DeleteServiceTemplateAndChildrenBasedOnServiceTemplateId] @RowId

		DELETE FROM #ServicesToDelete WHERE Id = @RowId
	
		SET @RowCount = (SELECT COUNT(*) FROM #ServicesToDelete)          
	END
	
	END
	RETURN

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
USE Core
GO
/****************************************************************************************************************************************************
* FUNCTION CheckServiceTemplateForChildren will check a ServiceTemplate to see if it has any child ServiceTemplates
** Input Parameters **
* @serviceTemplateId - The Id of the Service Template that you want to check 
** Output Parameters: **
* INT - Will be '0' if there are no children and will be '1' if children exist
***************************************************************************************************************************************************/
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

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
USE Core
GO
IF OBJECT_ID(N'[dbo].[DeleteBasicPartyBasedOnId]', N'FN') IS NOT NULL
DROP PROCEDURE [dbo].DeleteBasicPartyBasedOnId
GO
--This procedure deletes all the basic info held on a Party (Locations, Contacts, ContactInfoSet, Roles, Vehicles and Files)
CREATE PROCEDURE dbo.DeleteBasicPartyBasedOnId
		(@providerId uniqueidentifier)
	AS
	BEGIN

	DELETE FROM Files
	WHERE	PartyId = @providerId

	DELETE FROM dbo.Parties
	WHERE	Id = @providerId
	
	END
	RETURN

GO

USE Core
GO
IF OBJECT_ID(N'[dbo].[DeleteBusinessAccountBasedOnId]', N'FN') IS NOT NULL
DROP PROCEDURE [dbo].DeleteBusinessAccountBasedOnId
GO
/****************************************************************************************************************************************************
* FUNCTION DeleteBusinessAccountBasedOnId will delete a BusinessAccount and all entities associated with it
* Follows the following progression to delete: RouteEmployee, RouteVehicle, Routes, RouteTasks, Services, ServiceTemplates, Clients, Locations
* Regions, Contacts, ContactInfoSet, Roles, Vehicles, Files, Employees and finally it deletes the BusinessAccount itself 
** Input Parameters **
* @providerId - The BusinessAccount Id
***************************************************************************************************************************************************/
CREATE PROCEDURE dbo.DeleteBusinessAccountBasedOnId
		(@providerId uniqueidentifier)

	AS
	BEGIN

	DELETE FROM RouteEmployee
	WHERE Routes_Id IN
	(
		SELECT Id
		FROM Routes
		WHERE OwnerBusinessAccountId = @providerId
	)

	DELETE FROM RouteVehicle
	WHERE Routes_Id IN
	(
		SELECT Id
		FROM Routes
		WHERE OwnerBusinessAccountId = @providerId
	)

	DELETE FROM Vehicles 
	WHERE	BusinessAccountId = @providerId

	DELETE FROM Routes
	WHERE OwnerBusinessAccountId = @providerId

	DELETE FROM RouteTasks
	WHERE BusinessAccountId = @providerId
	
	DELETE FROM dbo.TaskStatuses
	WHERE BusinessAccountId = @providerId

	DELETE FROM Services
	WHERE ServiceProviderId = @providerId

	EXEC dbo.DeleteServiceTemplatesAndChildrenBasedOnContextId @serviceProviderId = @providerId, @ownerClientId = NULL
    
	BEGIN    --Delete Clients for ServiceProvider
		
		DECLARE @ClientId uniqueidentifier

		DECLARE @ClientIdsForServiceProvider TABLE
		(
			ClientId uniqueidentifier
		)

		--Finds all Clients that are associated with the BusinessAccount
		INSERT INTO @ClientIdsForServiceProvider
		SELECT Id FROM Clients
		WHERE	BusinessAccountId = @providerId

		DECLARE @ClientRowCount int
		SET @ClientRowCount = (SELECT COUNT(*) FROM @ClientIdsForServiceProvider)

		--Iterates through @ClientIdsForServiceProvider and calls DeleteClientBasedOnId on each
		WHILE @ClientRowCount > 0
		BEGIN
				SET @ClientId = (SELECT MIN(ClientId) FROM @ClientIdsForServiceProvider)

				EXEC dbo.DeleteClientBasedOnId @clientId = @ClientId

				DELETE FROM @ClientIdsForServiceProvider
				WHERE ClientId = @ClientId

				SET @ClientRowCount = (SELECT COUNT(*) FROM @ClientIdsForServiceProvider)
		END

	END

	BEGIN    --Delete Locations for ServiceProvider
		
		DECLARE @LocationId uniqueidentifier

		DECLARE @LocationIdsForServiceProvider TABLE
		(
			LocationId uniqueidentifier
		)
		
		DECLARE @date DATE
		SET @date = GETUTCDATE()  
		
		--Finds all Locations that are associated with the BusinessAccount
		INSERT INTO @LocationIdsForServiceProvider
		SELECT Id FROM Locations
		WHERE	BusinessAccountId = @providerId OR BusinessAccountIdIfDepot = @providerId

		DECLARE @LocationRowCount int
		SET @LocationRowCount = (SELECT COUNT(*) FROM @LocationIdsForServiceProvider)

		--Iterates through @LocationIdsForServiceProvider and calls DeleteLocationBasedOnId on each
		WHILE @LocationRowCount > 0
		BEGIN
				SET @LocationId = (SELECT MIN(LocationId) FROM @LocationIdsForServiceProvider)

				EXEC dbo.DeleteLocationBasedOnId @locationId = @LocationId

				DELETE FROM @LocationIdsForServiceProvider
				WHERE LocationId = @LocationId

				SET @LocationRowCount = (SELECT COUNT(*) FROM @LocationIdsForServiceProvider)
		END
        
	END
    
-------------------------------------------------------------------------------------------------------------------------

	DELETE FROM Regions
	WHERE BusinessAccountId = @providerId

	DELETE FROM ContactInfoSet
	WHERE		PartyId = @providerId

	DELETE FROM Employees
	WHERE EmployerId = @providerId

-------------------------------------------------------------------------------------------------------------------------
--Delete the BusinessAccount itself
-------------------------------------------------------------------------------------------------------------------------
	DELETE FROM Roles
	WHERE	OwnerBusinessAccountId = @providerId
	
	DELETE FROM Parties_BusinessAccount
	WHERE Id = @providerId

-------------------------------------------------------------------------------------------------------------------------
--Delete all off of Parties
-------------------------------------------------------------------------------------------------------------------------

	EXECUTE [dbo].[DeleteBasicPartyBasedOnId] @providerId

	END
	RETURN

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
USE Core
GO
IF OBJECT_ID(N'[dbo].[DeleteClientBasedOnId]', N'FN') IS NOT NULL
DROP PROCEDURE [dbo].DeleteClientBasedOnId
GO
/****************************************************************************************************************************************************
* FUNCTION DeleteClientBasedOnId will delete a Client and all entities associated with it
* Follows the following progression to delete: RouteDestinations, RouteTasks, Services, ServiceTemplates, RecurringServices, Locations 
* ClientTitles, Parties_Business and finally the Client itself
** Input Parameters **
* @clientId - The Client Id to be deleted
***************************************************************************************************************************************************/
CREATE PROCEDURE dbo.DeleteClientBasedOnId
		(@clientId uniqueidentifier)

	AS
	BEGIN

	DELETE FROM RouteDestinations
	WHERE ClientId = @clientId

	DELETE FROM Services
	WHERE ClientId = @clientId

	EXEC dbo.DeleteServiceTemplatesAndChildrenBasedOnContextId @serviceProviderId = NULL, @ownerClientId = @clientId

	DELETE FROM RecurringServices
	WHERE ClientId = @clientId

-------------------------------------------------------------------------------------------------------------------------
--Delete Locations for Client
-------------------------------------------------------------------------------------------------------------------------
	DECLARE @LocationId uniqueidentifier
	
	DECLARE @LocationIdsForClient TABLE
	(
		LocationId uniqueidentifier
	)

	--Finds all Locations that are associated with the Client
	INSERT INTO @LocationIdsForClient
	SELECT Id FROM Locations
	WHERE	ClientId = @clientId

	DECLARE @RowCount int
	SET @RowCount = (SELECT COUNT(*) FROM @LocationIdsForClient)

	--Iterates through @LocationIdsForClient and calls DeleteLocationBasedOnId on each
	WHILE @RowCount > 0
	BEGIN
			SET @LocationId = (SELECT MIN(LocationId) FROM @LocationIdsForClient)

			EXEC dbo.DeleteLocationBasedOnId @locationId = @LocationId

			DELETE FROM @LocationIdsForClient
			WHERE LocationId = @LocationId

			SET @RowCount = (SELECT COUNT(*) FROM @LocationIdsForClient)
	END
-------------------------------------------------------------------------------------------------------------------------

	DELETE FROM ContactInfoSet
	WHERE ClientId = @clientId

	DELETE FROM Clients
	WHERE Id = @clientId

	END
	RETURN

GO

	USE Core
	GO
	SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
	CREATE PROCEDURE [dbo].[DeleteFieldAndChildrenBasedOnFieldId]
		(@parentFieldId uniqueidentifier)
	AS

	BEGIN

	CREATE TABLE #TempTable
	(
		Id uniqueidentifier
	);
	
	WITH FieldRecurs AS
	(
    --Select all the Fields from a ServiceTemplate whose OwnerServiceProvider = providerId
    SELECT	Fields.Id, Fields.ParentFieldId
    FROM	Fields 
    WHERE	Fields.Id = @parentFieldId
    --Recursively select the children
    UNION	ALL
    SELECT	Fields.Id, Fields.ParentFieldId
    FROM	Fields 
    JOIN	FieldRecurs 
    ON		Fields.ParentFieldId = FieldRecurs.Id
	)

	INSERT INTO #TempTable (Id)
	SELECT	FieldRecurs.Id
	FROM	FieldRecurs

	DELETE 
	FROM		Fields
	--This is a Semi-Join between the FieldRecurs table created above and the Fields Table
	--Semi-Join simply means that it has all the same logic as a normal join, but it doesnt actually join the tables
	--In this case, it finds all the rows on Fields that correspond to a row in FieldRecurs
	WHERE		Id IN
	(
	SELECT		Id
	FROM		#TempTable
	)
	OR
	ParentFieldId IN
	(
	SELECT		Id
	FROM		#TempTable
	)  

	--Drop the table that was created to store the CTE
	DROP TABLE #TempTable
	
	END
	RETURN

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
USE Core
GO

IF OBJECT_ID(N'[dbo].[DeleteLocationBasedOnId]', N'FN') IS NOT NULL
DROP PROCEDURE [dbo].[DeleteLocationBasedOnId]
GO
/****************************************************************************************************************************************************
* FUNCTION DeleteLocationBasedOnId will delete a Location and all entities associated with it
* Follows the following progression to delete: RouteTasks, SubLocations, ContactInfoSet, RouteDestinations and finally the Location itself
** Input Parameters **
* @locationId - The Location Id to be deleted
***************************************************************************************************************************************************/
CREATE PROCEDURE dbo.DeleteLocationBasedOnId
		(@locationId uniqueidentifier)

	AS
	BEGIN

	DELETE FROM RouteTasks
	WHERE LocationId = @locationId

	DELETE FROM SubLocations
	WHERE LocationId = @locationId

	DELETE ContactInfoSet
	WHERE LocationId = @locationId

	DELETE FROM RouteDestinations
	WHERE LocationId = @locationId

	DELETE FROM Locations
	WHERE Id = @locationId	

	END
	RETURN

GO

USE Core
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF OBJECT_ID(N'[dbo].[DeleteRecurringService]', N'FN') IS NOT NULL
DROP PROCEDURE [dbo].[DeleteRecurringService]
GO
/****************************************************************************************************************************************************
* FUNCTION DeleteRecurringService will delete a RecurringService and all entities associated with it
* Follows the following progression to delete: Services, ServiceTemplates, Repeats and finally the RecurringService itself
** Input Parameters **
* @recurringServiceId - The RecurringService Id to be deleted
***************************************************************************************************************************************************/
CREATE PROCEDURE [dbo].[DeleteRecurringService]
		(@recurringServiceId uniqueidentifier)

	AS
	BEGIN

	DECLARE @serviceTemplateId uniqueidentifier

	--Find the ServiceTemplate that correspods to the Recurring Service  
	SET		@serviceTemplateId = (SELECT Id FROM ServiceTemplates WHERE Id = @recurringServiceId)

	--Delete the ServiceTemplate found above
	EXEC	[dbo].[DeleteServiceTemplateAndChildrenBasedOnServiceTemplateId]	@parentTemplateId = @serviceTemplateId

	DELETE
	FROM	Repeats 
	WHERE	Id = @recurringServiceId

	DELETE 
	FROM	RecurringServices 
	WHERE	Id = @recurringServiceId

	END
	RETURN

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
USE Core
GO
IF OBJECT_ID(N'[dbo].[DeleteServiceTemplateAndChildrenBasedOnServiceTemplateId]', N'FN') IS NOT NULL
DROP PROCEDURE [dbo].DeleteServiceTemplateAndChildrenBasedOnServiceTemplateId
GO
/****************************************************************************************************************************************************
* FUNCTION DeleteServiceTemplateAndChildrenBasedOnServiceTemplateId will delete a ServiceTemplate and all child ServiceTemplates associated with it
* Begins by using a CTE(Common Table Expression) to find all chlid ServiceTemplates. It does this by making the CTE self referencing so it becomes recursive
* Since the CTE will only last for one operation (it is not actually stored as an abject) we save it into a teemporary table for later use.
* After we have this table, we do a series of Semi-Joins to find RouteTasks, Services, RecurringServices and ServiceTemplates that have an Id that exists in the table
* Finally we drop the temp table so it can be recreated later.
** Input Parameters **
* @parentTemplateId - The ServiceTemplalate Id to use to find children ServiceTemplates
***************************************************************************************************************************************************/
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
	FROM		RouteTasks
	--This is a Semi-Join between the ServiceTemplateRecurs table created above and the RouteTasks Table
	--Semi-Join simply means that it has all the same logic as a normal join, but it doesnt actually join the tables
	--In this case, it finds all the rows on RouteTasks that correspond to a row in ServiceTemplateRecurs
	WHERE EXISTS
	(
	SELECT		#TempTable.Id
	FROM		#TempTable
	WHERE		#TempTable.Id = RouteTasks.ServiceId
	)

	DELETE 
	FROM		Services
	--This is a Semi-Join between the ServiceTemplateRecurs table created above and the Services Table
	--Semi-Join simply means that it has all the same logic as a normal join, but it doesnt actually join the tables
	--In this case, it finds all the rows on Services that correspond to a row in ServiceTemplateRecurs
	WHERE		EXISTS
	(
	SELECT		#TempTable.Id
	FROM		#TempTable
	WHERE		#TempTable.Id = Services.Id OR #TempTable.Id = Services.RecurringServiceId
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

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
USE Core
GO
IF OBJECT_ID(N'[dbo].[DeleteServiceTemplatesAndChildrenBasedOnContextId]', N'FN') IS NOT NULL
DROP PROCEDURE [dbo].[DeleteServiceTemplatesAndChildrenBasedOnContextId]
GO
/****************************************************************************************************************************************************
* FUNCTION DeleteServiceTemplatesAndChildrenBasedOnContextId will delete all ServiceTemplates on a Client or BusinessAccount and all child ServiceTemplates associated with them
* Begins by using a CTE(Common Table Expression) to find all chlid ServiceTemplates. It does this by making the CTE self referencing so it becomes recursive
* Since the CTE will only last for one operation (it is not actually stored as an abject) we save it into a teemporary table for later use.
* After we have this table, we do a series of Semi-Joins to find RouteTasks, Services, RecurringServices and ServiceTemplates that have an Id that exists in the table
* Finally we drop the temp table so it can be recreated later.
** Input Parameters **
* @serviceProviderId - The BusinessAccount Id to use to find all ServiceTemplates for
* @ownerClientId - The Client Id to use to find all ServiceTemplates for
***************************************************************************************************************************************************/
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
	FROM		RouteTasks
	--This is a Semi-Join between the ServiceTemplateRecurs table created above and the RouteTasks Table
	--Semi-Join simply means that it has all the same logic as a normal join, but it doesnt actually join the tables
	--In this case, it finds all the rows on RouteTasks that correspond to a row in ServiceTemplateRecurs
	WHERE EXISTS
	(
	SELECT		#TempTable.Id
	FROM		#TempTable
	WHERE		#TempTable.Id = RouteTasks.ServiceId
	)

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

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
USE Core
GO
IF OBJECT_ID(N'[dbo].[DeleteUserAccountBasedOnId]', N'FN') IS NOT NULL
DROP PROCEDURE [dbo].[DeleteUserAccountBasedOnId]
GO
/****************************************************************************************************************************************************
* FUNCTION DeleteUserAccountBasedOnId will delete a UserAccount and all entities associated with it
* Follows the following progression to delete: Locations, Contacts, ContactInfoSet, Roles, Vehicles, Files, UserAccountLog and finally the UserAccount itself 
** Input Parameters **
* @providerId - The UserAccount Id to be deleted
***************************************************************************************************************************************************/
CREATE PROCEDURE dbo.DeleteUserAccountBasedOnId
		(@providerId uniqueidentifier)

	AS
	BEGIN

	EXECUTE [dbo].[DeleteBasicPartyBasedOnId] @providerId

	DELETE FROM dbo.UserAccountLog
	WHERE UserAccountId = @providerId

	UPDATE dbo.Employees
	SET LinkedUserAccountId = NULL
	WHERE LinkedUserAccountId = @providerId

	DELETE FROM Parties_UserAccount
	WHERE Id = @providerId

	END
	RETURN

GO

USE [Core]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetDateRangeForServices]
(@serviceProviderIdContext uniqueidentifier,
@clientIdContext uniqueidentifier,
@recurringServiceIdContext uniqueidentifier,
@seedDate date,
@frontBackMinimum int,
@getPrevious bit,
@getNext BIT,
@serviceTypeContext NVARCHAR(MAX))

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
	--In this case, it finds all the RecurringServices that correspond to a Client with a BusinessAccountId = @serviceProviderIdContext
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
				AND Clients.BusinessAccountId = @serviceProviderIdContext 
				AND RecurringServices.Id = t1.Id
				AND RecurringServices.Id = t2.Id
		)
	)
	END
  
	IF @serviceTypeContext IS NOT NULL
	BEGIN  

		DELETE FROM @TempGenServiceTable
		WHERE ServiceName <> @serviceTypeContext
		
    END 

    

	--This table will hold the Recurring Services and Existing Services that occur on @seedDate
	DECLARE @ServicesForToday TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)

------------------------------------------------------------------------------------------------------------------------------------------
--This section will find the first occurrence of all the RecurringServices on or after the SeedDate
------------------------------------------------------------------------------------------------------------------------------------------
	IF @getNext = 1
	BEGIN
		--This table is simply the result of merging @TempGenServiceTable and @TempNextDateTable
		DECLARE @NextGenServices TABLE
		(Id uniqueidentifier,
		 EndDate date,
		 EndAfterTimes int,
		 RepeatEveryTimes int,
		 FrequencyInt int,
		 FrequencyDetailInt int,
		 StartDate date,
		 NextDate date,
		 ServiceName nvarchar(max))
	 
		--Merges @TempGenServiceTable amd @TempNextDateTable created above based on their Id into @NextGenServices
		INSERT INTO @NextGenServices (Id, EndDate, EndAfterTimes, RepeatEveryTimes, FrequencyInt, FrequencyDetailInt, StartDate, ServiceName)
		SELECT	t1.Id, t1.EndDate, t1.EndAfterTimes, t1.RepeatEveryTimes, t1.FrequencyInt, t1.FrequencyDetailInt, t1.StartDate, t1.ServiceName
		FROM	@TempGenServiceTable t1

		UPDATE @NextGenServices
		SET NextDate = (SELECT dbo.GetNextOccurence(@seedDate, StartDate,  EndDate, EndAfterTimes, FrequencyInt, RepeatEveryTimes, FrequencyDetailInt))
		FROM @NextGenServices

		--Puts all the Generated Services that occur today in a separate table
		INSERT INTO @ServicesForToday (RecurringServiceId, OccurDate, ServiceName)
		SELECT t1.Id, t1.NextDate, t1.ServiceName
		FROM @NextGenServices t1
		WHERE t1.NextDate = @seedDate

		UPDATE @NextGenServices
		SET NextDate = (SELECT dbo.GetNextOccurence(DATEADD(day, 1, @seedDate), StartDate,  EndDate, EndAfterTimes, FrequencyInt, RepeatEveryTimes, FrequencyDetailInt))
		FROM @NextGenServices
		WHERE NextDate = @seedDate

		--Remove any rows that do not have a NextOccurrence past or on the OnOrAfterDate
		DELETE FROM @NextGenServices
		WHERE NextDate IS NULL

	END
------------------------------------------------------------------------------------------------------------------------------------------
--This section will find the first occurrence of all the RecurringServices on or before the SeedDate
------------------------------------------------------------------------------------------------------------------------------------------
	IF @getPrevious = 1
	BEGIN
		DECLARE @previousDayToLookForServices date

		--Subtracts one day to @seedDate if you are looking for both Next and Previous
		--If we were to omit this, the GetPreviousOccurrence and GetNextOccurrence functions
		--would both include the original @seedDate
		--Thus throwing off the output of this function
		IF @getNext = 1 AND @getPrevious = 1
			SET @previousDayToLookForServices = DATEADD (day, -1, @seedDate)
		ELSE
			SET @previousDayToLookForServices = @seedDate
							
		--This table is simply the result of merging @TempGenServiceTable and @TempPreviousDateTable
		DECLARE @PreviousGenServices TABLE
		(Id uniqueidentifier,
		 EndDate date,
		 EndAfterTimes int,
		 RepeatEveryTimes int,
		 FrequencyInt int,
		 FrequencyDetailInt int,
		 StartDate date,
		 PreviousDate date,
		 ServiceName nvarchar(max))
	
		--Merges @TempGenServiceTable amd @TempPreviousDateTable created above based on their Id into @NextGenServices
		INSERT INTO @PreviousGenServices (Id, EndDate, EndAfterTimes, RepeatEveryTimes, FrequencyInt, FrequencyDetailInt, StartDate, ServiceName)
		SELECT	t1.Id, t1.EndDate, t1.EndAfterTimes, t1.RepeatEveryTimes, t1.FrequencyInt, t1.FrequencyDetailInt, t1.StartDate, t1.ServiceName
		FROM	@TempGenServiceTable t1

		UPDATE @PreviousGenServices
		SET PreviousDate = (SELECT dbo.GetPreviousOccurrence(@previousDayToLookForServices, StartDate,  EndDate, EndAfterTimes, FrequencyInt, RepeatEveryTimes, FrequencyDetailInt))
		FROM @PreviousGenServices

		IF @getNext = 0
		BEGIN
			--Puts all the Generated Services that occur today in a separate table
			INSERT INTO @ServicesForToday (RecurringServiceId, OccurDate, ServiceName)
			SELECT t1.Id, t1.PreviousDate, t1.ServiceName
			FROM @PreviousGenServices t1
			WHERE t1.PreviousDate = @seedDate

			UPDATE @PreviousGenServices
			SET PreviousDate = (SELECT dbo.GetNextOccurence(DATEADD(day, -1, @seedDate), StartDate,  EndDate, EndAfterTimes, FrequencyInt, RepeatEveryTimes, FrequencyDetailInt))
			FROM @PreviousGenServices
			WHERE PreviousDate = @seedDate

			--Remove any rows that do not have a NextOccurrence past or on the OnOrAfterDate
			DELETE FROM @PreviousGenServices
			WHERE PreviousDate IS NULL
		END
		ELSE
		--Remove any rows that do not have a PreviousOccurrence on or before the OnOrBeforeDate
		DELETE FROM @PreviousGenServices
		WHERE PreviousDate IS NULL
	END
------------------------------------------------------------------------------------------------------------------------------------------
	DECLARE @CombinedGenServices TABLE
		(Id uniqueidentifier,
		 EndDate date,
		 EndAfterTimes int,
		 RepeatEveryTimes int,
		 FrequencyInt int,
		 FrequencyDetailInt int,
		 StartDate date,
		 OccurDate date,
		 ServiceName nvarchar(max))

	INSERT INTO @CombinedGenServices (Id, OccurDate, ServiceName)
	SELECT Id, PreviousDate, ServiceName  FROM @PreviousGenServices
	
	INSERT INTO @CombinedGenServices (Id, OccurDate, ServiceName)
	SELECT Id, NextDate, ServiceName  FROM @NextGenServices

	--Table will hold all Ids and ExcludedDatesStrings
	--Only those RecurringServices that have been scheduled for the date provided and have an ExcludedDatesString will appear
	DECLARE @RecurringServicesWithExcludedDates TABLE
	(
		Id uniqueidentifier,
		ExcludedDatesString nvarchar(max)
	)

	INSERT INTO @RecurringServicesWithExcludedDates
	SELECT DISTINCT t1.Id, t1.ExcludedDatesString FROM RecurringServices t1, @CombinedGenServices t2
	WHERE t1.Id = t2.Id AND t1.ExcludedDatesString IS NOT NULL	

	DECLARE @RecurringServicesWithExcludedDatesSplit TABLE
	(
		Id uniqueidentifier,
		ExcludedDate nvarchar(max)
	)

	DECLARE @RowCount int --Row count for @RecurringServicesWithExcludedDates (We delete from this table as we input into @RecurringServicesWithExcludedDatesSplit)
	DECLARE @RowId  uniqueidentifier --RecurringServiceId of the current row
	DECLARE @RowExcludedDateString nvarchar(max) --ExcludedDatesString for the current row

	SET @RowCount = (SELECT COUNT(*) FROM @RecurringServicesWithExcludedDates)

	WHILE @RowCount > 0
	BEGIN 
		SET @RowId = (SELECT TOP(1) Id FROM @RecurringServicesWithExcludedDates ORDER BY Id) --Find the RowId of the top row sorted by Id

		SET @RowExcludedDateString = (SELECT ExcludedDatesString FROM @RecurringServicesWithExcludedDates WHERE Id = @RowId) --Find the ExcludedDatesString of the top row found above

		--Converts the ExcludedDateString to a Table(See example below for more information)
		/****************************************************************************************************************************************************
		* FUNCTION Split will convert the comma separated string of dates ()
		** Input Parameters **
		* @Id - RecurringServiceId
		* @sInputList - List of delimited ExcludedDates
		* @sDelimiter - -- Delimiter that separates ExcludedDates
		** Output Parameters: **
		*  @List TABLE (Id uniqueidentifier, ExcludedDate VARCHAR(8000)) - Ex. below
		* Id                                     | ExcludedDate
		* -----------------------------------------------------------------------------------------
		* {036BD670-39A5-478F-BFA3-AD312E3F7F47} | 1/1/2012
		* {B30A43AD-655A-449C-BD4E-951F8F988718} | 1/1/2012
		* {03DB9F9B-2FF6-4398-B984-533FB3E19C50} | 1/2/2012
		***************************************************************************************************************************************************/
		INSERT INTO @RecurringServicesWithExcludedDatesSplit
		SELECT * FROM [dbo].[Split] (
								@RowId,
								@RowExcludedDateString,
								','
								)
		
		--Now that we have converted this row, remove it from @RecurringServicesWithExcludedDates
		DELETE FROM @RecurringServicesWithExcludedDates
		WHERE Id = @RowId

		--Reset @RowCount for the loop condition
		SET @RowCount = (SELECT COUNT(*) FROM @RecurringServicesWithExcludedDates)
	END

------------------------------------------------------------------------------------------------------------------------------------------
--Here we will take the Recurring Services that are in @NextGenServices and find all occurrences in date order until
--we get to the number specified by @frontBackMinimum in the future
--We will also do the same thing with @PreviousGenServices except in reverse date order
------------------------------------------------------------------------------------------------------------------------------------------
	DECLARE @minVal date
	DECLARE @lastDateToLookFor date
	DECLARE @firstDateToLookFor date
	DECLARE @ServiceCount int
	DECLARE @NextDay date
	DECLARE @RowCountForRecurringServiceOccurrenceTable int
	DECLARE @RowCountForTempGenServiceTableWithNextOccurrence int
	DECLARE @RowCountForTempGenServiceTableWithPreviousOccurrence int
	DECLARE @remainingNumberOfServicesToFind int

	--Table to temporarily store all the Recurring Services dates in the future, deleted after each iteration of the loop
	DECLARE @TempNextRecurringServiceOccurrenceTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)

	--Table to temporarily store all the Recurring Services dates in the past, deleted after each iteration of the loop
	DECLARE @TempPreviousRecurringServiceOccurrenceTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)

	--Collection of all services that were temporarily stored in @TempNextRecurringServiceOccurrenceTable
	DECLARE @NextRecurringServiceOccurrenceTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)

	--Collection of all services that were temporarily stored in @TempPreviousRecurringServiceOccurrenceTable
	DECLARE @PreviousRecurringServiceOccurrenceTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)

	--Preset to a value above 0 just so it makes it into the loop. Actual value is calculated there
	SET		@RowCountForTempGenServiceTableWithPreviousOccurrence = 1
	SET		@RowCountForRecurringServiceOccurrenceTable = 0

	--Preset to @seedDate
	SET		@NextDay = @seedDate

	IF		@GetPrevious = 1
	BEGIN
	--Will loop until it finds @@frontBackMinimum services in the past
	WHILE	@RowCountForRecurringServiceOccurrenceTable <= @frontBackMinimum
	BEGIN
		SET		@minVal = (SELECT MAX(PreviousDate) FROM @PreviousGenServices)

		SET		@NextDay = DATEADD(day, -1, @minVal)

		SET		@RowCountForTempGenServiceTableWithPreviousOccurrence = (SELECT COUNT(*) FROM @PreviousGenServices)

		--Checks to be sure that there are still rows to look at in @NextGenServices
		IF NOT	@RowCountForTempGenServiceTableWithPreviousOccurrence > 0
		BEGIN
				BREAK
		END
		
		DECLARE @TempPreviousShouldAddTable TABLE
		(
			RecurringServiceId uniqueidentifier,
			OccurDate date,
			ServiceName nvarchar(max)
		)

		--Inserts all Services with the lowest date to @PreviousRecurringServiceOccurrenceTable
		INSERT INTO @TempPreviousRecurringServiceOccurrenceTable
		SELECT DISTINCT	t1.Id, t1.PreviousDate, t1.ServiceName
		FROM	@PreviousGenServices t1
		WHERE	t1.PreviousDate = @minVal

		--Will hold all the services that need to be removed from the final list based on the RecurringService's ExcludedDates
		DECLARE @PreviousServicesToRemove TABLE
		(
				RecurringServiceId uniqueidentifier,
				OccurDate date,
				ServiceName nvarchar(max)
		)

		--Inserts all Services that need to be removed to @PreviousServicesToRemove
		INSERT INTO @PreviousServicesToRemove
		SELECT DISTINCT * FROM @TempPreviousShouldAddTable t3
		WHERE EXISTS
		(
			SELECT DISTINCT t1.Id
			FROM @RecurringServicesWithExcludedDatesSplit t1
			WHERE EXISTS
			(
				SELECT DISTINCT t2.RecurringServiceId
				FROM @TempPreviousShouldAddTable t2
				WHERE (t1.Id = t2.RecurringServiceId AND t1.ExcludedDate = t2.OccurDate)
			) AND t1.ExcludedDate = t3.OccurDate AND t1.Id = t3.RecurringServiceId
		) 

		--Takes all ExcludedDate services out of the list of Next Services to return
		INSERT INTO @TempPreviousRecurringServiceOccurrenceTable
		SELECT DISTINCT * FROM @TempPreviousShouldAddTable
		EXCEPT
		SELECT DISTINCT * FROM @PreviousServicesToRemove

		--Add the temporary list of @TempPreviousRecurringServiceOccurrenceTable to the final list of @PreviousRecurringServiceOccurrenceTable
		INSERT INTO @PreviousRecurringServiceOccurrenceTable
		SELECT DISTINCT * FROM @TempPreviousRecurringServiceOccurrenceTable

		--Updates all Services that were just put into a new table to show their previous occurrence date
		UPDATE	@PreviousGenServices
		SET		PreviousDate = (SELECT dbo.GetPreviousOccurrence(@NextDay, StartDate,  EndDate, EndAfterTimes, FrequencyInt, RepeatEveryTimes, FrequencyDetailInt))
		FROM	@PreviousGenServices
		WHERE	PreviousDate = @minVal

		--If any of those Services updated above do not have a previous occurrence, they will be deleted from the table
		DELETE FROM @PreviousGenServices
		WHERE	PreviousDate IS NULL

		--Sets up row count so that when the loop returns to the top, we will know if there are any rows remaining
		SET		@RowCountForRecurringServiceOccurrenceTable = (SELECT COUNT(*) FROM @PreviousRecurringServiceOccurrenceTable)

		DELETE FROM @PreviousServicesToRemove
		DELETE FROM @TempPreviousShouldAddTable
		DELETE FROM @TempPreviousRecurringServiceOccurrenceTable
	END
	END

	--Preset to a value above 0 just so it makes it into the loop. Actual value is calculated there
	SET		@RowCountForTempGenServiceTableWithNextOccurrence = 1
	SET		@RowCountForRecurringServiceOccurrenceTable = 0

	--Preset to @seedDate
	SET		@NextDay = @seedDate

	IF		@GetNext = 1
	BEGIN
	--Will loop until it fills in remaining number of Services to get to @numberOfOccurrences
	WHILE	@RowCountForRecurringServiceOccurrenceTable <= @frontBackMinimum
	BEGIN

		SET		@minVal = (SELECT MIN(NextDate) FROM @NextGenServices)
		SET		@NextDay = DATEADD(day, 1, @minVal)

		SET		@RowCountForTempGenServiceTableWithNextOccurrence = (SELECT COUNT(*) FROM @NextGenServices)

		--Checks to be sure that there are still rows to look at in @NextGenServices
		IF NOT	@RowCountForTempGenServiceTableWithNextOccurrence > 0
		BEGIN
			BREAK
		END

		DECLARE @TempNextShouldAddTable TABLE
		(
			RecurringServiceId uniqueidentifier,
			OccurDate date,
			ServiceName nvarchar(max)
		)

		--Inserts all  Services with the lowest date to @TempNextShouldAddTable
		INSERT INTO @TempNextShouldAddTable
		SELECT DISTINCT	t1.Id, t1.NextDate, t1.ServiceName
		FROM	@NextGenServices t1
		WHERE	NextDate = @minVal 

		--Will hold all the services that need to be removed from the final list based on the RecurringService's ExcludedDates
		DECLARE @NextServicesToRemove TABLE
		(
				RecurringServiceId uniqueidentifier,
				OccurDate date,
				ServiceName nvarchar(max)
		)

		--Inserts all Services that need to be removed to @NextServicesToRemove
		INSERT INTO @NextServicesToRemove
		SELECT DISTINCT * FROM @TempNextShouldAddTable t3
		WHERE EXISTS
		(
			SELECT DISTINCT t1.Id
			FROM @RecurringServicesWithExcludedDatesSplit t1
			WHERE EXISTS
			(
				SELECT DISTINCT t2.RecurringServiceId
				FROM @TempNextShouldAddTable t2
				WHERE (t1.Id = t2.RecurringServiceId AND t1.ExcludedDate = t2.OccurDate)
			) AND t1.ExcludedDate = t3.OccurDate AND t1.Id = t3.RecurringServiceId
		)

		--Takes all ExcludedDate services out of the list of Next Services to return
		INSERT INTO @TempNextRecurringServiceOccurrenceTable
		SELECT DISTINCT * FROM @TempNextShouldAddTable
		EXCEPT
		SELECT DISTINCT * FROM @NextServicesToRemove

		--Add the temporary list of @TempNextRecurringServiceOccurrenceTable to the final list of @NextRecurringServiceOccurrenceTable
		INSERT INTO @NextRecurringServiceOccurrenceTable
		SELECT DISTINCT * FROM @TempNextRecurringServiceOccurrenceTable

		--Updates all Services that were just put into a new table to show their next occurrence date
		UPDATE	@NextGenServices
		SET		NextDate = (SELECT dbo.GetNextOccurence(@NextDay, StartDate,  EndDate, EndAfterTimes, FrequencyInt, RepeatEveryTimes, FrequencyDetailInt))
		FROM	@NextGenServices
		WHERE	NextDate = @minVal

		--If any of those Services updated above do not have a next occurrence, they will be deleted from the table
		DELETE FROM @NextGenServices
		WHERE	NextDate IS NULL

		--Sets up row count so that when the loop returns to the top, we will know if there are any rows remaining
		SET		@RowCountForRecurringServiceOccurrenceTable = (SELECT COUNT(*) FROM @NextRecurringServiceOccurrenceTable)
		
		DELETE FROM @NextServicesToRemove
		DELETE FROM @TempNextShouldAddTable
		DELETE FROM @TempNextRecurringServiceOccurrenceTable
	END
	END

	SET		@lastDateToLookFor = (SELECT MAX(OccurDate) FROM @NextRecurringServiceOccurrenceTable)
	SET		@firstDateToLookFor = (SELECT MIN(OccurDate) FROM @PreviousRecurringServiceOccurrenceTable)
------------------------------------------------------------------------------------------------------------------------------------------
--Remove all ExcludedDates where the date is @seedDate
------------------------------------------------------------------------------------------------------------------------------------------
	DECLARE @ServicesForTodayExcluded TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)

	INSERT INTO @ServicesForTodayExcluded
	SELECT t1.* FROM @ServicesForToday t1, @RecurringServicesWithExcludedDatesSplit t2
	WHERE t1.RecurringServiceId = t2.Id AND t1.OccurDate = t2.ExcludedDate

	DECLARE @ServicesForTodayWithoutExcluded TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)

	INSERT INTO @ServicesForTodayWithoutExcluded
	SELECT * FROM @ServicesForToday
	EXCEPT
	SELECT * FROM @ServicesForTodayExcluded
------------------------------------------------------------------------------------------------------------------------------------------
--Here we will add Existing Services to tables of their own
--Just as in the RecurringServiceTables, we will have separate tables for Previous, Next, and SeedDate
------------------------------------------------------------------------------------------------------------------------------------------
	--This table will store all Existing Services prior to the OnOrAfterDate
	DECLARE @PreviousExistingServiceTable TABLE
	(ServiceId uniqueidentifier,
	RecurringServiceId uniqueidentifier,
	OccurDate date,
	ServiceName nvarchar(max))

	--This table will store all Existing Services after to the OnOrAfterDate
	DECLARE @NextExistingServiceTable TABLE
	(ServiceId uniqueidentifier,
	RecurringServiceId uniqueidentifier,
	OccurDate date,
	ServiceName nvarchar(max))

	DECLARE @dayBeforeSeedDate date
	DECLARE @dayAfterSeedDate date
	SET @dayBeforeSeedDate = DATEADD(Day, -1, @seedDate)
	SET @dayAfterSeedDate = DATEADD(Day, 1, @seedDate)

	IF @recurringServiceIdContext IS NOT NULL
	BEGIN
		--Fills @PreviousExistingServiceTable with existing Services in the past
		INSERT INTO @PreviousExistingServiceTable (ServiceId, RecurringServiceId, OccurDate, ServiceName)
		SELECT  t1.Id, t1.RecurringServiceId, t1.ServiceDate, t2.Name
		FROM	Services t1, ServiceTemplates t2
		WHERE	t1.RecurringServiceId = @recurringServiceIdContext 
				AND t1.ServiceDate BETWEEN @firstDateToLookFor AND @dayBeforeSeedDate
				AND t1.Id = t2.Id

		--Fills @NextExistingServiceTable with existing Services in the future
		INSERT INTO @NextExistingServiceTable (ServiceId, RecurringServiceId, OccurDate, ServiceName)
		SELECT  t1.Id, t1.RecurringServiceId, t1.ServiceDate, t2.Name
		FROM	Services t1, ServiceTemplates t2
		WHERE	t1.RecurringServiceId = @recurringServiceIdContext 
				AND t1.ServiceDate BETWEEN @dayAfterSeedDate AND @lastDateToLookFor
				AND t1.Id = t2.Id

		--Fills @ServicesForTodayWithoutExcluded will Existing Services on the SeedDate
		INSERT INTO @ServicesForTodayWithoutExcluded (RecurringServiceId, ServiceId, OccurDate, ServiceName)
		SELECT  t1.RecurringServiceId, t1.Id, t1.ServiceDate, t2.Name
		FROM	Services t1, ServiceTemplates t2
		WHERE	t1.RecurringServiceId = @recurringServiceIdContext 
				AND t1.ServiceDate = @seedDate
				AND t1.Id = t2.Id
	END

	ELSE IF @clientIdContext IS NOT NULL
	BEGIN
		--Fills @PreviousExistingServiceTable with existing Services in the past
		INSERT INTO @PreviousExistingServiceTable (ServiceId, RecurringServiceId, OccurDate, ServiceName)
		SELECT  t1.Id, t1.RecurringServiceId, t1.ServiceDate, t2.Name
		FROM	Services t1, ServiceTemplates t2
		WHERE	t1.ClientId = @clientIdContext 
				AND t1.ServiceDate BETWEEN @firstDateToLookFor AND @dayBeforeSeedDate
				AND t1.Id = t2.Id

		--Fills @NextExistingServiceTable with existing Services in the future
		INSERT INTO @NextExistingServiceTable (ServiceId, RecurringServiceId, OccurDate, ServiceName)
		SELECT  t1.Id, t1.RecurringServiceId, t1.ServiceDate, t2.Name
		FROM	Services t1, ServiceTemplates t2
		WHERE	t1.ClientId = @clientIdContext 
				AND t1.ServiceDate BETWEEN @dayAfterSeedDate AND @lastDateToLookFor
				AND t1.Id = t2.Id

		--Fills @ServicesForTodayWithoutExcluded will Existing Services on the SeedDate
		INSERT INTO @ServicesForTodayWithoutExcluded (ServiceId, RecurringServiceId, OccurDate, ServiceName)
		SELECT  t1.Id, t1.RecurringServiceId, t1.ServiceDate, t2.Name
		FROM	Services t1, ServiceTemplates t2
		WHERE	t1.ClientId = @clientIdContext 
				AND t1.ServiceDate = @seedDate
				AND t1.Id = t2.Id
	END

	ELSE IF @serviceProviderIdContext IS NOT NULL
	BEGIN 
		--Fills @PreviousExistingServiceTable with existing Services in the past
		INSERT INTO @PreviousExistingServiceTable (ServiceId, RecurringServiceId, OccurDate, ServiceName)
		SELECT  t1.Id, t1.RecurringServiceId, t1.ServiceDate, t2.Name
		FROM	Services t1, ServiceTemplates t2
		WHERE	t1.ServiceProviderId = @serviceProviderIdContext 
				AND t1.ServiceDate BETWEEN @firstDateToLookFor AND @dayBeforeSeedDate
				AND t1.Id = t2.Id

		--Fills @NextExistingServiceTable with existing Services in the future
		INSERT INTO @NextExistingServiceTable (ServiceId, RecurringServiceId, OccurDate, ServiceName)
		SELECT  t1.Id, t1.RecurringServiceId, t1.ServiceDate, t2.Name
		FROM	Services t1, ServiceTemplates t2
		WHERE	t1.ServiceProviderId = @serviceProviderIdContext 
				AND t1.ServiceDate BETWEEN @dayAfterSeedDate AND @lastDateToLookFor
				AND t1.Id = t2.Id
	
		--Fills @ServicesForTodayWithoutExcluded will Existing Services on the SeedDate
		INSERT INTO @ServicesForTodayWithoutExcluded (ServiceId, RecurringServiceId, OccurDate, ServiceName)
		SELECT  t1.Id, t1.RecurringServiceId, t1.ServiceDate, t2.Name
		FROM	Services t1, ServiceTemplates t2
		WHERE	t1.ServiceProviderId = @serviceProviderIdContext 
				AND t1.ServiceDate = @seedDate
				AND t1.Id = t2.Id
	END
  
	IF @serviceTypeContext IS NOT NULL
	BEGIN  
		DELETE FROM @PreviousExistingServiceTable
		WHERE ServiceName <> @serviceTypeContext
  
		DELETE FROM @NextExistingServiceTable
		WHERE ServiceName <> @serviceTypeContext

		DELETE FROM @ServicesForTodayWithoutExcluded
		WHERE ServiceName <> @serviceTypeContext
    END  
------------------------------------------------------------------------------------------------------------------------------------------
--Here we will combine the Previous RecurringServices table with the Previous ExistingServices table
--It will then look for duplicate Id's and remove the RecurringService from the combined table
--Following this, it will find the first day (becasue this is the previous section) that a service should be on according @frontBackMinimum
--Then we will remove all Services that with an occur date before the date calculated above
------------------------------------------------------------------------------------------------------------------------------------------
	--Will store all the Services before the OnOrAfterDate
	DECLARE @CombinedPreviousServices TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)

	--Here we add all the previous occurrences to the final temporary table
	INSERT INTO @CombinedPreviousServices (RecurringServiceId, OccurDate, ServiceName)
	SELECT RecurringServiceId, OccurDate, ServiceName
	FROM @PreviousRecurringServiceOccurrenceTable

	INSERT INTO @CombinedPreviousServices (RecurringServiceId, ServiceId, OccurDate, ServiceName)
	SELECT RecurringServiceId, ServiceId, OccurDate, ServiceName
	FROM @PreviousExistingServiceTable

	--This table will temporarily store the RecurringServiceId's of all rows that appear more than once in @TempTable
	DECLARE @PreviousDuplicateIdTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)

	INSERT INTO @PreviousDuplicateIdTable
	SELECT t1.RecurringServiceId, t2.ServiceId, t2.OccurDate, t2.ServiceName FROM (SELECT OccurDate, RecurringServiceId, ServiceId FROM @CombinedPreviousServices AS t2 WHERE ServiceId IS NOT NULL GROUP BY OccurDate, RecurringServiceId, ServiceId) t1
	JOIN @CombinedPreviousServices as t2 on t1.RecurringServiceId = t2.RecurringServiceId AND t1.OccurDate = t2.OccurDate AND t2.ServiceId IS NULL

	DECLARE @previousHolderTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)

	--Filtered down from the table above so that only the correct number of Services is returned from the function
	INSERT TOP(@frontBackMinimum) INTO @previousHolderTable
	SELECT * FROM @CombinedPreviousServices
	EXCEPT
	SELECT * FROM @PreviousDuplicateIdTable
	ORDER BY OccurDate ASC

	Declare @previousLastDate date

	SET @previousLastDate = (SELECT MIN(OccurDate) FROM @previousHolderTable)

	DECLARE @finalPreviousServiceTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)	

	INSERT INTO @finalPreviousServiceTable
	SELECT * FROM @CombinedPreviousServices
	EXCEPT
	SELECT * FROM @PreviousDuplicateIdTable

	DELETE FROM @finalPreviousServiceTable
	WHERE OccurDate < @previousLastDate
------------------------------------------------------------------------------------------------------------------------------------------
--Here we will combine the Next RecurringServices table with the Next ExistingServices table
--It will then look for duplicate Id's and remove the RecurringService from the combined table
--Following this, it will find the last day (becasue this is the next section) that a service should be on according @frontBackMinimum
--Then we will remove all Services that with an occur date after the date calculated above
------------------------------------------------------------------------------------------------------------------------------------------
	--Will store all the Services after the OnOrAfterDate
	DECLARE @CombinedNextServices TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)

	--Here we add the next occurrences to the final temporary table
	INSERT INTO @CombinedNextServices (RecurringServiceId, OccurDate, ServiceName)
	SELECT RecurringServiceId, OccurDate, ServiceName
	FROM @NextRecurringServiceOccurrenceTable

	INSERT INTO @CombinedNextServices (RecurringServiceId, ServiceId, OccurDate, ServiceName)
	SELECT RecurringServiceId, ServiceId, OccurDate, ServiceName
	FROM @NextExistingServiceTable

	--This table will temporarily store the RecurringServiceId's of all rows that appear more than once in @TempTable
	DECLARE @NextDuplicateIdTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)

	INSERT INTO @NextDuplicateIdTable
	SELECT t1.RecurringServiceId, t2.ServiceId, t2.OccurDate, t2.ServiceName FROM (SELECT OccurDate, RecurringServiceId, ServiceId FROM @CombinedNextServices AS t2 WHERE ServiceId IS NOT NULL GROUP BY OccurDate, RecurringServiceId, ServiceId) t1
	JOIN @CombinedNextServices as t2 on t1.RecurringServiceId = t2.RecurringServiceId AND t1.OccurDate = t2.OccurDate AND t2.ServiceId IS NULL

	DECLARE @nextHolderTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)	

	--Filtered down from the table above so that only the correct number of Services is returned from the function
	INSERT TOP(@frontBackMinimum) INTO @nextHolderTable
	SELECT * FROM @CombinedNextServices
	EXCEPT
	SELECT * FROM @NextDuplicateIdTable
	ORDER BY OccurDate ASC

	Declare @nextLastDate date

	SET @nextLastDate = (SELECT MAX(OccurDate) FROM @nextHolderTable)
	
	DECLARE @finalNextServiceTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)	

	INSERT INTO @finalNextServiceTable
	SELECT * FROM @CombinedNextServices
	EXCEPT
	SELECT * FROM @NextDuplicateIdTable

	DELETE FROM @finalNextServiceTable
	WHERE OccurDate > @nextLastDate
	
------------------------------------------------------------------------------------------------------------------------------------------
--Finally, we combine the three tables (previous, onSeedDay, next) into one master table
--This master table is then returned from the function
------------------------------------------------------------------------------------------------------------------------------------------
	--This table will temporarily store the RecurringServiceId's of all rows that appear more than once in @TempTable
	DECLARE @SeedDateDuplicateIdTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)

	INSERT INTO @SeedDateDuplicateIdTable
	SELECT t1.RecurringServiceId, t2.ServiceId, t2.OccurDate, t2.ServiceName FROM (SELECT OccurDate, RecurringServiceId, ServiceId FROM @ServicesForTodayWithoutExcluded AS t2 WHERE ServiceId IS NOT NULL GROUP BY OccurDate, RecurringServiceId, ServiceId) t1
	JOIN @ServicesForTodayWithoutExcluded as t2 on t1.RecurringServiceId = t2.RecurringServiceId AND t1.OccurDate = t2.OccurDate AND t2.ServiceId IS NULL

	DECLARE @seedDateFinalTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)	

	--Filtered down from the table above so that only the correct number of Services is returned from the function
	INSERT INTO @seedDateFinalTable
	SELECT * FROM @ServicesForTodayWithoutExcluded
	EXCEPT
	SELECT * FROM @SeedDateDuplicateIdTable
	ORDER BY OccurDate ASC

------------------------------------------------------------------------------------------------------------------------------------------
--Finally, we combine the three tables (previous, onSeedDay, next) into one master table
--This master table is then returned from the function
------------------------------------------------------------------------------------------------------------------------------------------
	DECLARE @ServicesTableToReturn TABLE
(
	RecurringServiceId uniqueidentifier,
	ServiceId uniqueidentifier,
	OccurDate date,
	ServiceName nvarchar(max)
)
	
	INSERT INTO @ServicesTableToReturn
	SELECT * FROM @finalPreviousServiceTable

	INSERT INTO @ServicesTableToReturn
	SELECT * FROM @seedDateFinalTable

	INSERT INTO @ServicesTableToReturn
	SELECT * FROM @finalNextServiceTable

	SELECT MIN(OccurDate) AS 'First Date' FROM @ServicesTableToReturn 
	SELECT MAX(OccurDate) AS 'Last Date' FROM @ServicesTableToReturn 

RETURN
END

GO

USE Core 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetFieldsInJavaScriptFormat]
(
	@businessAccountId UNIQUEIDENTIFIER,
	@serviceType NVARCHAR(MAX)
)
AS
BEGIN
	DECLARE @fields TABLE
	(
		Id UNIQUEIDENTIFIER,
		[Name] NVARCHAR(Max),
		[Type] NVARCHAR(Max)  
	)

	DECLARE @serviceTemplateId UNIQUEIDENTIFIER
	SET @serviceTemplateId = (SELECT Id FROM dbo.ServiceTemplates WHERE OwnerServiceProviderId = @businessAccountId AND Name = @serviceType AND LevelInt = 1)

	INSERT INTO @fields (Id, Name)
	SELECT Id, Name FROM dbo.Fields WHERE ServiceTemplateId = @serviceTemplateId

	UPDATE @fields
	SET [Type] = 'string'
	WHERE [@fields].Id NOT IN (SELECT Id FROM dbo.Fields_DateTimeField) 
	AND [@fields].Id NOT IN (SELECT Id FROM dbo.Fields_NumericField) 

	UPDATE @fields
	SET [Type] = 'number'
	WHERE [@fields].Id IN (SELECT Id FROM dbo.Fields_NumericField) 

	UPDATE @fields
	SET [Type] = 'dateTime'
	WHERE [@fields].Id IN (SELECT Id FROM dbo.Fields_DateTimeField WHERE TypeInt = 0)

	UPDATE @fields
	SET [Type] = 'time'
	WHERE [@fields].Id IN (SELECT Id FROM dbo.Fields_DateTimeField WHERE TypeInt = 1)

	UPDATE @fields
	SET [Type] = 'date'
	WHERE [@fields].Id IN (SELECT Id FROM dbo.Fields_DateTimeField WHERE TypeInt = 2)

	SELECT Name, [Type] FROM @fields

RETURN  
END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Use Core
Go
IF OBJECT_ID(N'[dbo].[GetNextOccurence]', N'FN') IS NOT NULL
DROP FUNCTION [dbo].[GetNextOccurence]
GO
/****************************************************************************************************************************************************
* FUNCTION GetNextOccurence will take in a Service schedule and return the next date that that Service is scheduled to occur that is on or after the OnOrAfterDate
** Input Parameters **
* @onOrAfterDate - The first date the can be accepted as a response
* @startDate - The StartDate of the Repeat
* @endDate - The EndDate of the Repeat
* @endAfterTimes - The number of times the Service is scheduled to occur before it ends
* @frequencyInt - Corresponds to the type of schedule (Once, Daily, Weekly, Monthly or Yearly)
* @repeatEveryTimes - Corresponds to how often the Service is sceduled to repeat (ex. a value of 2 would mean that it repeats every 2 days, weeks, months or years)
* @FrequencyDetailInt - Corresponds to weekly and monthly schedules only. For Weekly it is equal to a list of numbers that correspond to days of the week (ex. 1237 would mean that the service happens on Monday, Tuesday, Wednesday and Sunday)
*						For Monthly it corresponds to when during the month the Service is schedule (ex. LastDayOfMonth, FirstOfDayOfMonth, ThirdOfDayOfMonth, etc.)
** Output Parameters  **
*  DateTime - The date of the next scheduled Service occurrence
***************************************************************************************************************************************************/
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
Use Core
GO
IF OBJECT_ID(N'[dbo].[GetPreviousOccurrence]', N'FN') IS NOT NULL
DROP FUNCTION [dbo].[GetPreviousOccurrence]
GO
/****************************************************************************************************************************************************
* FUNCTION GetNextOccurence will take in a Service schedule and return the date that that Service was scheduled to occur that is on or before the OnOrBeforeDate
** Input Parameters **
* @onOrBeforeDate - The first date the can be accepted as a response
* @startDate - The StartDate of the Repeat
* @endDate - The EndDate of the Repeat
* @endAfterTimes - The number of times the Service is scheduled to occur before it ends
* @frequencyInt - Corresponds to the type of schedule (Once, Daily, Weekly, Monthly or Yearly)
* @repeatEveryTimes - Corresponds to how often the Service is sceduled to repeat (ex. a value of 2 would mean that it repeats every 2 days, weeks, months or years)
* @FrequencyDetailInt - Corresponds to weekly and monthly schedules only. For Weekly it is equal to a list of numbers that correspond to days of the week (ex. 1237 would mean that the service happens on Monday, Tuesday, Wednesday and Sunday)
*						For Monthly it corresponds to when during the month the Service is schedule (ex. LastDayOfMonth, FirstOfDayOfMonth, ThirdOfDayOfMonth, etc.)
** Output Parameters  **
*  DateTime - The date of the most recently scheduled Service occurrence on or before the OnOrBeforeDate
***************************************************************************************************************************************************/
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

USE [Core]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF OBJECT_ID(N'[dbo].[GetResourcesWithLatestPoint]', N'FN') IS NOT NULL
DROP FUNCTION [dbo].[GetResourcesWithLatestPoint]
GO
/****************************************************************************************************************************************************
* FUNCTION GetResourcesWithLatestPoint will take a BusinessAccount Id and find all Employees and Vehicles associated with it that are on Routes for the today.
* It will then find all the required information from those Employees and Vehicles and return them in a table as depicted below.
** Input Parameters **
* @serviceProviderId - The BusinessAccount Id that will be used to find all Employees and Vehicles
* @serviceDateUtc - The date (in UTC) you want to get Resource and Locations for
** Output Parameters: **
* @ServicesTableToReturn - Ex. below
* EmployeeId		| VehicleId | EntityName			| CompassHeading | Latitude	| Longitude	| LastTimeStamp	| Speed	| TrackSource	| RouteId
* ---------------------------------------------------------------------------------------------------------------------------------------------------
* {GUID}			|           | Bob Black <- Employee	| 186			 | 47.456	| -86.166	| DateTime		| 46.32	| iPhone		| {GUID}
* {GUID}			|           | Jane Doe <- Employee	| 45			 | 43.265	| -89.254	| DateTime		| 25.13	| Android		| {GUID}
* {GUID}			| {GUID}	| 372 0925 <- Vehicle	| 321			 | 44.165	| -79.365	| Datetime		| 32.89	| Windows Phone	| {GUID}
*****************************************************************************************************************************************************/

CREATE FUNCTION [dbo].[GetResourcesWithLatestPoint]
(@serviceProviderId uniqueidentifier, @serviceDateUtc date)
RETURNS @EmployeeVehicleTableToReturn TABLE
	(
		EmployeeId uniqueidentifier,
		VehicleId uniqueidentifier,
		EntityName nvarchar(max),
		Heading int,
		Latitude decimal(18,8),
		Longitude decimal(18,8),
		CollectedTimeStamp datetime,
		Speed decimal(18,8),
		Source nvarchar(max),
		RouteId uniqueidentifier,
		Accuracy int
	) 
AS
BEGIN

	INSERT INTO @EmployeeVehicleTableToReturn (EmployeeId, RouteId)
	SELECT Employees_Id, Routes_Id FROM dbo.RouteEmployee WHERE Routes_Id IN (SELECT Id FROM dbo.Routes WHERE Date = @serviceDateUtc AND OwnerBusinessAccountId = @serviceProviderId)

	UPDATE @EmployeeVehicleTableToReturn
	SET EntityName = FirstName + ' ' + LastName,
		Heading = LastCompassDirection,
		Latitude = LastLatitude,
		Longitude = LastLongitude,
		CollectedTimeStamp = LastTimeStamp,
		Speed = LastSpeed,
		[Source] = LastSource,
		Accuracy = LastAccuracy
	FROM dbo.Employees t1 WHERE EmployeeId = t1.Id

	DELETE FROM @EmployeeVehicleTableToReturn
	WHERE CollectedTimeStamp < @serviceDateUtc OR CollectedTimeStamp IS NULL

RETURN 
END



GO

USE [Core]
GO
/****** Object:  StoredProcedure [dbo].[GetServiceHolders]    Script Date: 8/10/2012 12:43:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetServiceHolders]
--ALTER PROCEDURE [dbo].[GetServiceHolders]
	(
	@serviceProviderIdContext UNIQUEIDENTIFIER, 
	@clientIdContext UNIQUEIDENTIFIER, 
	@recurringServiceIdContext UNIQUEIDENTIFIER, 
	@firstDate DATE, 
	@lastDate DATE,
	@serviceTypeContext NVARCHAR(MAX),
	@withFields BIT
	)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

BEGIN	--Here we find all of the Services for the correct Context and store them in @TempGenServiceTable

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
	--In this case, it finds all the RecurringServices that correspond to a Client with a BusinessAccountId = @serviceProviderIdContext
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
				AND Clients.BusinessAccountId = @serviceProviderIdContext 
				AND RecurringServices.Id = t1.Id
				AND RecurringServices.Id = t2.Id
		)
	)
	END
END

BEGIN	--This section will find the first occurrence of all the RecurringServices on or after the SeedDate
	
	DECLARE @NextGenServices TABLE
	(Id uniqueidentifier,
		EndDate date,
		EndAfterTimes int,
		RepeatEveryTimes int,
		FrequencyInt int,
		FrequencyDetailInt int,
		StartDate date,
		NextDate date,
		ServiceName nvarchar(max))
	 
	--Merges @TempGenServiceTable amd @TempNextDateTable created above based on their Id into @NextGenServices
	INSERT INTO @NextGenServices (Id, EndDate, EndAfterTimes, RepeatEveryTimes, FrequencyInt, FrequencyDetailInt, StartDate, ServiceName)
	SELECT	t1.Id, t1.EndDate, t1.EndAfterTimes, t1.RepeatEveryTimes, t1.FrequencyInt, t1.FrequencyDetailInt, t1.StartDate, t1.ServiceName
	FROM	@TempGenServiceTable t1

	--Update the NextService date for all Services in the context to the first date on or after @firstDate
	UPDATE @NextGenServices
	SET NextDate = (SELECT dbo.GetNextOccurence(@firstDate, StartDate,  EndDate, EndAfterTimes, FrequencyInt, RepeatEveryTimes, FrequencyDetailInt))
	FROM @NextGenServices

END

BEGIN	--Find all RecurringServices where the ExcludedDateString is not null

	DECLARE @RecurringServicesWithExcludedDates TABLE
	(
		Id uniqueidentifier,
		ExcludedDatesString nvarchar(max)
	)

	INSERT INTO @RecurringServicesWithExcludedDates
	SELECT DISTINCT t1.Id, t1.ExcludedDatesString FROM RecurringServices t1, @NextGenServices t2
	WHERE t1.Id = t2.Id AND t1.ExcludedDatesString IS NOT NULL	

	DECLARE @RecurringServicesWithExcludedDatesSplit TABLE
	(
		Id uniqueidentifier,
		ExcludedDate nvarchar(max)
	)

	DECLARE @RowCount int --Row count for @RecurringServicesWithExcludedDates (We delete from this table as we input into @RecurringServicesWithExcludedDatesSplit)
	DECLARE @RowId  uniqueidentifier --RecurringServiceId of the current row
	DECLARE @RowExcludedDateString nvarchar(max) --ExcludedDatesString for the current row

	SET @RowCount = (SELECT COUNT(*) FROM @RecurringServicesWithExcludedDates)

	WHILE @RowCount > 0
	BEGIN 
		SET @RowId = (SELECT TOP(1) Id FROM @RecurringServicesWithExcludedDates ORDER BY Id) --Find the RowId of the top row sorted by Id

		SET @RowExcludedDateString = (SELECT ExcludedDatesString FROM @RecurringServicesWithExcludedDates WHERE Id = @RowId) --Find the ExcludedDatesString of the top row found above

		--Converts the ExcludedDateString to a Table(See example below for more information)
		/****************************************************************************************************************************************************
		* FUNCTION Split will convert the comma separated string of dates ()
		** Input Parameters **
		* @Id - RecurringServiceId
		* @sInputList - List of delimited ExcludedDates
		* @sDelimiter - -- Delimiter that separates ExcludedDates
		** Output Parameters: **
		*  @List TABLE (Id uniqueidentifier, ExcludedDate VARCHAR(8000)) - Ex. below
		* Id                                     | ExcludedDate
		* -----------------------------------------------------------------------------------------
		* {036BD670-39A5-478F-BFA3-AD312E3F7F47} | 1/1/2012
		* {B30A43AD-655A-449C-BD4E-951F8F988718} | 1/1/2012
		* {03DB9F9B-2FF6-4398-B984-533FB3E19C50} | 1/2/2012
		***************************************************************************************************************************************************/
		INSERT INTO @RecurringServicesWithExcludedDatesSplit
		SELECT * FROM [dbo].[Split] (
								@RowId,
								@RowExcludedDateString,
								','
								)
		
		--Now that we have converted this row, remove it from @RecurringServicesWithExcludedDates
		DELETE FROM @RecurringServicesWithExcludedDates
		WHERE Id = @RowId

		--Reset @RowCount for the loop condition
		SET @RowCount = (SELECT COUNT(*) FROM @RecurringServicesWithExcludedDates)
	END

END

BEGIN	--Cycle through all of the RecurringServices until we get all from @firstDate through @lastDate
	BEGIN	--Declare variables and tables used in this section
		DECLARE @minVal date
		DECLARE @maxVal date
		DECLARE @NextDay date
		DECLARE @RowCountForTempGenServiceTable int
		DECLARE @RowCountForRecurringServiceOccurrenceTable int

		--Collection of all services that were temporarily stored in @TempNextRecurringServiceOccurrenceTable
		DECLARE @NextRecurringServiceOccurrenceTable TABLE
		(
			RecurringServiceId uniqueidentifier,
			OccurDate date,
			ServiceName nvarchar(max)
		)

		--Table to temporarily store all the Recurring Services dates in the future, deleted after each iteration of the loop
		DECLARE @TempNextRecurringServiceOccurrenceTable TABLE
		(
			RecurringServiceId uniqueidentifier,
			OccurDate date,
			ServiceName nvarchar(max)
		)
	END

	SET		@RowCountForTempGenServiceTable = (SELECT COUNT(*) FROM @NextGenServices)

	WHILE	@RowCountForTempGenServiceTable <> 0
	BEGIN
		SET		@minVal = (SELECT MIN(NextDate) FROM @NextGenServices)
		SET		@NextDay = DATEADD(day, 1, @minVal)
		SET		@maxVal = (SELECT MAX(NextDate) FROM @NextGenServices)

		--Checks to be sure that there are still rows to look at in @NextGenServices
		IF NOT	@RowCountForTempGenServiceTable > 0
		BEGIN
			BREAK
		END

		DECLARE @TempNextShouldAddTable TABLE
		(
			RecurringServiceId uniqueidentifier,
			OccurDate date,
			ServiceName nvarchar(max)
		)

		--Inserts all  Services with the lowest date to @TempNextShouldAddTable
		INSERT INTO @TempNextShouldAddTable
		SELECT DISTINCT	t1.Id, t1.NextDate, t1.ServiceName
		FROM	@NextGenServices t1
		WHERE	NextDate = @minVal 

		--Will hold all the services that need to be removed from the final list based on the RecurringService's ExcludedDates
		DECLARE @NextServicesToRemove TABLE
		(
				RecurringServiceId uniqueidentifier,
				OccurDate date,
				ServiceName nvarchar(max)
		)

		--Inserts all Services that need to be removed to @NextServicesToRemove
		INSERT INTO @NextServicesToRemove
		SELECT DISTINCT * FROM @TempNextShouldAddTable t3
		WHERE EXISTS
		(
			SELECT DISTINCT t1.Id
			FROM @RecurringServicesWithExcludedDatesSplit t1
			WHERE EXISTS
			(
				SELECT DISTINCT t2.RecurringServiceId
				FROM @TempNextShouldAddTable t2
				WHERE (t1.Id = t2.RecurringServiceId AND t1.ExcludedDate = t2.OccurDate)
			) AND t1.ExcludedDate = t3.OccurDate AND t1.Id = t3.RecurringServiceId
		)

		--Takes all ExcludedDate services out of the list of Next Services to return
		INSERT INTO @TempNextRecurringServiceOccurrenceTable
		SELECT DISTINCT * FROM @TempNextShouldAddTable
		EXCEPT
		SELECT DISTINCT * FROM @NextServicesToRemove

		--Add the temporary list of @TempNextRecurringServiceOccurrenceTable to the final list of @NextRecurringServiceOccurrenceTable
		INSERT INTO @NextRecurringServiceOccurrenceTable
		SELECT DISTINCT * FROM @TempNextRecurringServiceOccurrenceTable

		--Updates all Services that were just put into a new table to show their next occurrence date
		UPDATE	@NextGenServices
		SET		NextDate = (SELECT dbo.GetNextOccurence(@NextDay, StartDate,  EndDate, EndAfterTimes, FrequencyInt, RepeatEveryTimes, FrequencyDetailInt))
		FROM	@NextGenServices
		WHERE	NextDate = @minVal

		--If any of those Services updated above do not have a next occurrence, they will be deleted from the table
		DELETE FROM @NextGenServices
		WHERE	NextDate IS NULL OR NextDate > @lastDate
		
		DELETE FROM @NextServicesToRemove
		DELETE FROM @TempNextShouldAddTable
		DELETE FROM @TempNextRecurringServiceOccurrenceTable

		SET		@RowCountForTempGenServiceTable = (SELECT COUNT(*) FROM @NextGenServices)
	END

END

BEGIN	--Put all ExistingServices from @firstDate through @lastDate into a table to be added to the list of RecurringServices

	--This table will store all Existing Services after to the OnOrAfterDate
	DECLARE @NextExistingServiceTable TABLE
	(
		ServiceId uniqueidentifier,
		RecurringServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)
	
	IF @recurringServiceIdContext IS NOT NULL
	BEGIN
		--Fills @NextExistingServiceTable with existing Services in the future
		INSERT INTO @NextExistingServiceTable (ServiceId, RecurringServiceId, OccurDate, ServiceName)
		SELECT  t1.Id, t1.RecurringServiceId, t1.ServiceDate, t2.Name
		FROM	Services t1, ServiceTemplates t2
		WHERE	t1.RecurringServiceId = @recurringServiceIdContext 
				AND t1.ServiceDate BETWEEN @firstDate AND @lastDate
				AND t1.Id = t2.Id
	END

	ELSE IF @clientIdContext IS NOT NULL
	BEGIN
		--Fills @NextExistingServiceTable with existing Services in the future
		INSERT INTO @NextExistingServiceTable (ServiceId, RecurringServiceId, OccurDate, ServiceName)
		SELECT  t1.Id, t1.RecurringServiceId, t1.ServiceDate, t2.Name
		FROM	Services t1, ServiceTemplates t2
		WHERE	t1.ClientId = @clientIdContext 
				AND t1.ServiceDate BETWEEN @firstDate AND @lastDate
				AND t1.Id = t2.Id
	END

	ELSE IF @serviceProviderIdContext IS NOT NULL
	BEGIN 
		--Fills @NextExistingServiceTable with existing Services in the future
		INSERT INTO @NextExistingServiceTable (ServiceId, RecurringServiceId, OccurDate, ServiceName)
		SELECT  t1.Id, t1.RecurringServiceId, t1.ServiceDate, t2.Name
		FROM	Services t1, ServiceTemplates t2
		WHERE	t1.ServiceProviderId = @serviceProviderIdContext 
				AND t1.ServiceDate BETWEEN @firstDate AND @lastDate
				AND t1.Id = t2.Id
	END

END

BEGIN	--Combine the RecurringServices table with the ExistingServices table, remove duplicates

	DECLARE @CombinedNextServices TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max),
		ClientName NVARCHAR(MAX)
	)

	--Here we add the next occurrences to the final temporary table
	INSERT INTO @CombinedNextServices (RecurringServiceId, OccurDate, ServiceName)
	SELECT DISTINCT RecurringServiceId, OccurDate, ServiceName
	FROM @NextRecurringServiceOccurrenceTable

	INSERT INTO @CombinedNextServices (RecurringServiceId, ServiceId, OccurDate, ServiceName)
	SELECT RecurringServiceId, ServiceId, OccurDate, ServiceName
	FROM @NextExistingServiceTable

	DELETE FROM @CombinedNextServices
	WHERE ServiceId IS NULL
	AND RecurringServiceId IN
	(
		SELECT RecurringServiceId
		FROM @NextExistingServiceTable
		WHERE OccurDate = [@CombinedNextServices].OccurDate
	)

	UPDATE @CombinedNextServices
	SET ClientName = (SELECT Name FROM dbo.Clients WHERE Id = (SELECT ClientId FROM dbo.[Services] WHERE Id = [@CombinedNextServices].ServiceId))
	WHERE ServiceId IS NOT NULL
  
	UPDATE @CombinedNextServices
	SET ClientName = (SELECT Name FROM dbo.Clients WHERE Id = (SELECT ClientId FROM dbo.RecurringServices WHERE Id = [@CombinedNextServices].RecurringServiceId))
	WHERE ServiceId IS NULL  
END

IF @withFields = 0
	BEGIN
		SELECT RecurringServiceId ,
				ServiceId ,
				OccurDate ,
				ServiceName FROM @CombinedNextServices
		ORDER BY OccurDate
	END
ELSE IF @serviceTypeContext IS NOT NULL
	BEGIN
	SELECT * from @CombinedNextServices
	WHERE ServiceName =  @serviceTypeContext
	ORDER BY OccurDate
	END
ELSE
	BEGIN
	SELECT * from @CombinedNextServices 
	ORDER BY OccurDate
	END

RETURN
END

GO

USE Core 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetServiceTemplatesAndFields]
(
	@serviceProviderContextId UNIQUEIDENTIFIER,
	@clientContextId UNIQUEIDENTIFIER,
	@serviceTemplateContextId UNIQUEIDENTIFIER,
	@levelInt INT
)
AS
BEGIN
	
	DECLARE @fieldIds TABLE 
	(
		Id UNIQUEIDENTIFIER
	)

	IF @serviceProviderContextId IS NOT NULL
	BEGIN

		IF @levelInt IS NOT NULL
		BEGIN
			SELECT * FROM dbo.ServiceTemplates WHERE OwnerServiceProviderId = @serviceProviderContextId AND LevelInt = @levelInt	

			INSERT INTO @fieldIds
			SELECT Id FROM dbo.Fields 
			WHERE ServiceTemplateId IN (SELECT Id 
										FROM dbo.ServiceTemplates 
										WHERE OwnerServiceProviderId = @serviceProviderContextId
										AND LevelInt = @levelInt)
		END
		ELSE
		BEGIN
			SELECT * FROM dbo.ServiceTemplates WHERE OwnerServiceProviderId = @serviceProviderContextId	
		
			INSERT INTO @fieldIds
			SELECT Id FROM dbo.Fields 
			WHERE ServiceTemplateId IN (SELECT Id 
										FROM dbo.ServiceTemplates 
										WHERE OwnerServiceProviderId = @serviceProviderContextId)	
		END  
	END
  
	IF @clientContextId IS NOT NULL
	BEGIN
  		
		IF @levelInt IS NOT NULL
		BEGIN
			SELECT * FROM dbo.ServiceTemplates WHERE OwnerClientId = @clientContextId AND LevelInt = @levelInt
		
			INSERT INTO @fieldIds
			SELECT Id FROM dbo.Fields 
			WHERE ServiceTemplateId IN (SELECT Id 
										FROM dbo.ServiceTemplates 
										WHERE OwnerClientId = @clientContextId
										AND LevelInt = @levelInt)		
		END
		ELSE
		BEGIN
			SELECT * FROM dbo.ServiceTemplates WHERE OwnerClientId = @clientContextId	
		
			INSERT INTO @fieldIds
			SELECT Id FROM dbo.Fields 
			WHERE ServiceTemplateId IN (SELECT Id 
										FROM dbo.ServiceTemplates 
										WHERE OwnerClientId = @clientContextId)	
		END 
	END
  
	IF @serviceTemplateContextId IS NOT NULL
	BEGIN

		IF @levelInt IS NOT NULL
		BEGIN
			SELECT * FROM dbo.ServiceTemplates WHERE Id = @serviceTemplateContextId AND LevelInt = @levelInt	

			INSERT INTO @fieldIds
			SELECT Id FROM dbo.Fields WHERE ServiceTemplateId IN (	SELECT Id 
																	FROM dbo.ServiceTemplates 
																	WHERE Id = @serviceTemplateContextId
																	AND LevelInt = @levelInt)	
		END
		ELSE
		BEGIN
			SELECT * FROM dbo.ServiceTemplates WHERE Id = @serviceTemplateContextId	
			
			INSERT INTO @fieldIds
			SELECT Id FROM dbo.Fields WHERE ServiceTemplateId IN (	SELECT Id 
																	FROM dbo.ServiceTemplates 
																	WHERE Id = @serviceTemplateContextId)	
		END 

		

	END  

	SELECT * FROM dbo.Fields WHERE Id IN (SELECT Id FROM @fieldIds)

  	SELECT t1.*, t2.* FROM dbo.Fields t1 
	JOIN dbo.Fields_DateTimeField t2 
	ON t1.Id = t2.Id and t1.Id IN (SELECT Id FROM @fieldIds)

	SELECT t1.*, t2.* FROM dbo.Fields t1 
	JOIN dbo.Fields_NumericField t2 
	ON t1.Id = t2.Id and t1.Id IN (SELECT Id FROM @fieldIds)

	SELECT t1.*, t2.* FROM dbo.Fields t1 
	JOIN dbo.Fields_TextBoxField t2 
	ON t1.Id = t2.Id and t1.Id IN (SELECT Id FROM @fieldIds)

	SELECT t1.*, t2.* FROM dbo.Fields t1 
	JOIN dbo.Fields_SignatureField t2 
	ON t1.Id = t2.Id and t1.Id IN (SELECT Id FROM @fieldIds)

	SELECT t1.*, t2.* FROM dbo.Fields t1 
	JOIN dbo.Fields_OptionsField t2 
	ON t1.Id = t2.Id and t1.Id IN (SELECT Id FROM @fieldIds)

	SELECT t1.*, t2.* FROM dbo.Fields t1 
	JOIN dbo.Fields_LocationField t2 
	ON t1.Id = t2.Id and t1.Id IN (SELECT Id FROM @fieldIds)

	SELECT * FROM dbo.Locations WHERE Id IN (SELECT LocationId FROM dbo.Fields_LocationField WHERE Id IN (SELECT Id FROM @fieldIds))

RETURN  
END

GO

USE Core
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
sp_configure 'Show Advanced Options', 1 
GO
RECONFIGURE
GO
sp_configure 'Ad Hoc Distributed Queries', 1 
GO
RECONFIGURE
GO
-- =============================================
-- Author:		Zach Bright
-- Create date: 9/7/2012
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[GetServiceTemplatesWithDateAndDetails]
	(
	  @serviceProviderIdContext UNIQUEIDENTIFIER ,
	  @clientIdContext UNIQUEIDENTIFIER ,
	  @recurringServiceIdContext UNIQUEIDENTIFIER ,
	  @firstDate DATE ,
	  @lastDate DATE ,
	  @serviceTypeContext NVARCHAR(MAX)
	)
AS 
	BEGIN

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
		SET NOCOUNT ON;

		CREATE TABLE #ServiceHolders
			(
			  RecurringServiceId UNIQUEIDENTIFIER ,
			  ServiceId UNIQUEIDENTIFIER ,
			  OccurDate DATE ,
			  ServiceName NVARCHAR(MAX) ,
			  ClientName NVARCHAR(MAX)
			)

		INSERT	INTO #ServiceHolders (RecurringServiceId, ServiceId, OccurDate, ServiceName, ClientName)
				EXEC [dbo].[GetServiceHolders] @serviceProviderIdContext,
					@clientIdContext, @recurringServiceIdContext, @firstDate,
					@lastDate, @serviceTypeContext, 1
		
		CREATE TABLE #ServiceTemplateIds
		(
			Id UNIQUEIDENTIFIER
		)
		CREATE TABLE #FieldIds
		(
			Id UNIQUEIDENTIFIER
		)

		IF @serviceTypeContext IS NOT NULL
		BEGIN
			INSERT INTO #ServiceTemplateIds
			SELECT RecurringServiceId FROM #ServiceHolders WHERE ServiceId IS NULL AND ServiceName = @serviceTypeContext
  
			INSERT INTO #ServiceTemplateIds
			SELECT ServiceId FROM #ServiceHolders WHERE ServiceId IS NOT NULL AND ServiceName = @serviceTypeContext
		END  
		ELSE
		BEGIN
			INSERT INTO #ServiceTemplateIds
			SELECT RecurringServiceId FROM #ServiceHolders WHERE ServiceId IS NULL
  
			INSERT INTO #ServiceTemplateIds
			SELECT ServiceId FROM #ServiceHolders WHERE ServiceId IS NOT NULL
		END    

		
		INSERT INTO #FieldIds
		SELECT Id FROM dbo.Fields 
		WHERE ServiceTemplateId IN (SELECT Id FROM #ServiceTemplateIds)

		SELECT * FROM #ServiceHolders 
		ORDER BY OccurDate, ServiceId, RecurringServiceId
		
		SELECT t2.Id, t2.ServiceTemplateId, t2.Name, t1.Value 
		FROM dbo.Fields_DateTimeField t1
		JOIN dbo.Fields t2
		ON t2.Id = t1.Id AND t2.Id IN (SELECT Id FROM #FieldIds)
		ORDER BY t2.ServiceTemplateId

		SELECT t2.Id, t2.ServiceTemplateId, t2.Name, t1.Value 
		FROM dbo.Fields_NumericField t1
		JOIN dbo.Fields t2
		ON t2.Id = t1.Id AND t2.Id IN (SELECT Id FROM #FieldIds)
		ORDER BY t2.ServiceTemplateId

		SELECT t2.Id, t2.ServiceTemplateId, t2.Name, t1.Value 
		FROM dbo.Fields_TextBoxField t1
		JOIN dbo.Fields t2
		ON t2.Id = t1.Id AND t2.Id IN (SELECT Id FROM #FieldIds)
		ORDER BY t2.ServiceTemplateId

		SELECT t2.Id, t2.ServiceTemplateId, t2.Name, t1.Value 
		FROM dbo.Fields_SignatureField t1
		JOIN dbo.Fields t2
		ON t2.Id = t1.Id AND t2.Id IN (SELECT Id FROM #FieldIds)
		ORDER BY t2.ServiceTemplateId

		SELECT t2.Id, t2.ServiceTemplateId, t2.Name, t1.OptionsString, t1.Value
		FROM dbo.Fields_OptionsField t1
		JOIN dbo.Fields t2
		ON t2.Id = t1.Id AND t2.Id IN (SELECT Id FROM #FieldIds)
		ORDER BY t2.ServiceTemplateId

		SELECT t2.Id, t2.ServiceTemplateId, t2.Name, t3.Name + ', ' + t3.AddressLineOne + ' ' + t3.AddressLineTwo AS 'Value'
		FROM dbo.Fields_LocationField t1
		JOIN dbo.Fields t2
		ON t2.Id = t1.Id
		JOIN dbo.Locations t3
		ON t1.LocationId = t3.Id AND t2.Id IN (SELECT Id FROM #FieldIds)
		ORDER BY t2.ServiceTemplateId

		DROP TABLE #FieldIds
		DROP TABLE #ServiceTemplateIds
		DROP TABLE #ServiceHolders

		RETURN
	END
    

GO

USE [Core]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetUnroutedServicesForDate]    Script Date: 7/5/2012 2:15:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--IF OBJECT_ID(N'[dbo].[GetUnroutedServicesForDate]', N'FN') IS NOT NULL
--DROP FUNCTION [dbo].[GetUnroutedServicesForDate]
--GO
/*****************************************************************************************************************************************************************************************************************************
* FUNCTION GetUnroutedServicesForDate will take the context provided and find all the services that are scheduled for that day
** Input Parameters **
* @serviceProviderIdContext - The BusinessAccount context
** Output Parameters: **
* @ServicesTableToReturn - Ex. below
* RecurringServiceId| ServiceId | OccurDate									| ServiceName | ClientName		| ClientId  | RegionName | LocationName    | LocationId | AddressLine   | Latitude	| Longitude	| StatusInt
* ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
* {GUID}			|           | 1/1/2012 <-- Generated service			| Oil		  | Seltzer Factory | {GUID}	| South		 | Seltzer Factory | {GUID}		| 123 Fake St	| 47.456	| -86.166	| 3	
* {GUID}			|           | 1/1/2012 <-- Existing service				| Direct	  |	GotGrease?		| {GUID}	| North		 | GotGrease?	   | {GUID} 	| 6789 Help Ln	| 43.265	| -89.254	| 2	
* {GUID}			| {GUID}	| 1/2/2012 <-- Existing service w/ RS parent| Regular     | AB Couriers		| {GUID}	| West		 | AB Couriers	   | {GUID}		| 4953 Joe Way	| 44.165	| -79.365	| 4	
****************************************************************************************************************************************************************************************************************************/
CREATE PROCEDURE [dbo].[sp_GetUnroutedServicesForDate]
(@serviceProviderIdContext uniqueidentifier,
@serviceDate date)
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


	 --Delete all RouteTasks in the TaskBoard so they can be regenerated
	--This is necessary 
	DELETE FROM dbo.RouteTasks 
	WHERE RouteDestinationId IS NULL 
	AND BusinessAccountId = @serviceProviderIdContext 
	AND (Date = @serviceDate AND (OriginalDate = @serviceDate OR OriginalDate IS NULL))

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
				AND Clients.BusinessAccountId = @serviceProviderIdContext 
				AND RecurringServices.Id = t1.Id
				AND RecurringServices.Id = t2.Id
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
	 NextDate date,
	 ServiceName nvarchar(max))
	
	--Merges @TempGenServiceTable amd @TempNextDateTable created above based on their Id into @TempGenServiceTableWithNextOccurrence
	INSERT INTO @TempGenServiceTableWithNextOccurrence (Id, EndDate, EndAfterTimes, RepeatEveryTimes, FrequencyInt, FrequencyDetailInt, StartDate, ServiceName)
	SELECT	t1.Id, t1.EndDate, t1.EndAfterTimes, t1.RepeatEveryTimes, t1.FrequencyInt, t1.FrequencyDetailInt, t1.StartDate, t1.ServiceName
	FROM	@TempGenServiceTable t1

	UPDATE @TempGenServiceTableWithNextOccurrence
	SET NextDate = (SELECT dbo.GetNextOccurence(@serviceDate, StartDate,  EndDate, EndAfterTimes, FrequencyInt, RepeatEveryTimes, FrequencyDetailInt))
	FROM @TempGenServiceTableWithNextOccurrence

	--Remove any rows that do not have a NextOccurrence past or on the OnOrAfterDate
	DELETE FROM @TempGenServiceTableWithNextOccurrence
	WHERE NextDate IS NULL OR NextDate <> @serviceDate

	--This table will store all Existing Services after the OnOrAfterDate
	DECLARE @TempNextExistingServiceTable TABLE
	(ServiceId uniqueidentifier,
	RecurringServiceId uniqueidentifier,
	OccurDate date,
	ServiceName nvarchar(max))

	--Fills @ServicesForToday with Existing Services on the SeedDate
	INSERT INTO @TempNextExistingServiceTable (RecurringServiceId, ServiceId, OccurDate, ServiceName)
		SELECT  t1.RecurringServiceId, t1.Id, t1.ServiceDate, t2.Name
		FROM	Services t1, ServiceTemplates t2
		WHERE	t1.ServiceProviderId = @serviceProviderIdContext 
				AND t1.ServiceDate = @serviceDate
				AND t1.Id = t2.Id

	--Will store all the Services after the OnOrAfterDate
	DECLARE @tempHolderTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)

	--Here we add the next occurrences to the final temporary table
	INSERT INTO @tempHolderTable (RecurringServiceId, OccurDate, ServiceName)
	SELECT Id, NextDate, ServiceName
	FROM @TempGenServiceTableWithNextOccurrence

	INSERT INTO @tempHolderTable (RecurringServiceId, ServiceId, OccurDate, ServiceName)
	SELECT RecurringServiceId, ServiceId, OccurDate, ServiceName
	FROM @TempNextExistingServiceTable

	--This table will temporarily store the RecurringServiceId's of all rows that appear more than once in @TempTable
	DECLARE @DuplicateIdTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceDate nvarchar(max)
	)

	INSERT INTO @DuplicateIdTable
	SELECT t1.RecurringServiceId, t2.ServiceId, t2.OccurDate, t2.ServiceName FROM (SELECT OccurDate, RecurringServiceId, ServiceId FROM @tempHolderTable AS t2 WHERE ServiceId IS NOT NULL GROUP BY OccurDate, RecurringServiceId, ServiceId) t1
	JOIN @tempHolderTable as t2 on t1.RecurringServiceId = t2.RecurringServiceId AND t1.OccurDate = t2.OccurDate AND t2.ServiceId IS NULL

	DECLARE @serviceForDayTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)

	--Filtered down from the table above so that only the correct number of Services is returned from the function
	INSERT INTO @serviceForDayTable
	SELECT * FROM @tempHolderTable
	EXCEPT
	SELECT * FROM @DuplicateIdTable

	DECLARE @PreRoutedServices TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)

	--Selects all those RouteTasks that are in a RouteDestination that is in a Route that is on the specified day
	INSERT INTO @PreRoutedServices (RecurringServiceId, ServiceId, OccurDate, ServiceName)
	SELECT t1.RecurringServiceId, t1.ServiceId, t1.OccurDate, t1.ServiceName
	FROM @serviceForDayTable  t1
	WHERE EXISTS
	( 
		SELECT *
		FROM  RouteTasks t2
		WHERE EXISTS
		(
			SELECT *
			FROM RouteDestinations t3
			WHERE EXISTS
			(
				SELECT * FROM Routes t4
				WHERE t3.RouteId = t4.Id AND t2.RouteDestinationId = t3.Id AND t1.RecurringServiceId = RecurringServiceId AND t4.Date = @serviceDate
			)
		)
	)

	INSERT INTO @PreRoutedServices (RecurringServiceId, ServiceId, OccurDate, ServiceName)
	SELECT t1.RecurringServiceId, t1.Id, t1.ServiceDate, t2.Name
	FROM Services t1, ServiceTemplates t2
	WHERE	t1.ServiceProviderId = @serviceProviderIdContext 
				AND t1.ServiceDate = @serviceDate
				AND t1.Id = t2.Id
				AND EXISTS	( 
								SELECT *
								FROM  RouteTasks t2
								WHERE EXISTS
								(
									SELECT *
									FROM RouteDestinations t3
									WHERE EXISTS
									(
										SELECT * FROM Routes t4
										WHERE t3.RouteId = t4.Id AND t2.RouteDestinationId = t3.Id AND t1.Id = ServiceId AND t4.Date = @serviceDate
									)
								)
							)
						
	
	INSERT INTO @PreRoutedServices
	SELECT RecurringServiceId, ServiceId, [Date], Name
	FROM dbo.RouteTasks
	WHERE Date = @serviceDate AND BusinessAccountId = @serviceProviderIdContext AND RouteDestinationId IS NULL

	DECLARE @UnroutedOrUncompletedServices TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max),
		ClientName nvarchar(max),
		ClientId uniqueidentifier,
		LocationId uniqueidentifier,
		RegionName nvarchar(max),
		LocationName nvarchar(max),
		AddressLine nvarchar(max),
		Latitude float,
		Longitude float,
		StatusName nvarchar(max)
	) 

	INSERT INTO @UnroutedOrUncompletedServices (RecurringServiceId, ServiceId, OccurDate, ServiceName)
	SELECT * FROM @serviceForDayTable
	EXCEPT
	SELECT * FROM @PreRoutedServices

	UPDATE @UnroutedOrUncompletedServices
	SET LocationId =	(
							SELECT	TOP 1 LocationId
							FROM	Fields_LocationField t1
							WHERE	EXISTS
							(
								SELECT TOP 1	Id
								FROM	Fields t2
								WHERE	t2.Id = t1.Id 
								AND (t2.ServiceTemplateId = [@UnroutedOrUncompletedServices].RecurringServiceId OR t2.ServiceTemplateId = [@UnroutedOrUncompletedServices].ServiceId)
								AND LocationFieldTypeInt = 0
							)
						)

	UPDATE @UnroutedOrUncompletedServices
	SET RegionName =	(
							SELECT	DISTINCT Name
							FROM	Regions t1
							WHERE	EXISTS
							(
								SELECT	RegionId
								FROM	Locations t2
								WHERE	t2.RegionId = t1.Id 
								AND		LocationId = t2.Id
							)
						)


	UPDATE	@UnroutedOrUncompletedServices
	SET		AddressLine = t1.AddressLineOne, 
			LocationName = t1.Name,
			LocationId = t1.Id,
			Latitude = t1.Latitude, 
			Longitude = t1.Longitude
	FROM	Locations t1
	WHERE	t1.Id = LocationId

	UPDATE	@UnroutedOrUncompletedServices
	SET		ClientName =	(
							SELECT DISTINCT t1.Name
							FROM	dbo.Clients t1
							WHERE	t1.Id IN 
							(
								SELECT  t2.ClientId
								FROM	RecurringServices t2
								WHERE	RecurringServiceId = t2.Id 
								AND		t2.ClientId = t1.Id
							)
							)

	UPDATE	@UnroutedOrUncompletedServices
	SET		ClientName =	(
							SELECT DISTINCT  t1.Name
							FROM	dbo.Clients t1
							WHERE	t1.Id IN 
							(
								SELECT  t2.ClientId
								FROM	Services t2
								WHERE	ServiceId = t2.Id 
								AND		t2.ClientId = t1.Id
							)
							)
	WHERE ClientName IS NULL							

	UPDATE	@UnroutedOrUncompletedServices
	SET		ClientId =		(
							SELECT	DISTINCT ClientId
							FROM	RecurringServices t1
							WHERE	RecurringServiceId = t1.Id
							)

	UPDATE	@UnroutedOrUncompletedServices
	SET		ClientId =		(
							SELECT	DISTINCT ClientId
							FROM	Services t1
							WHERE	ServiceId = t1.Id
							)
	WHERE ClientId IS NULL

	DECLARE @ServicesForDateTable TABLE
		(
			RecurringServiceId uniqueidentifier,
			ServiceId uniqueidentifier,
			OccurDate date,
			ServiceName nvarchar(max),
			ClientName nvarchar(max),
			ClientId uniqueidentifier,
			RegionName nvarchar(max),
			LocationName nvarchar(max),
			LocationId uniqueidentifier,
			AddressLine nvarchar(max),
			Latitude decimal(18,8),
			Longitude decimal(18,8),
			StatusName nvarchar(max)
		) 

	--This will be a complete table of all services that should have been scheduled for the date provided
	--This does not take into account dates that have been excluded
	INSERT @ServicesForDateTable (RecurringServiceId, ServiceId, OccurDate, ServiceName, ClientName, ClientId, RegionName, LocationName, LocationId, AddressLine, Latitude, Longitude, StatusName)
	SELECT RecurringServiceId, ServiceId, OccurDate, ServiceName, ClientName, ClientId, RegionName, LocationName, LocationId, AddressLine, Latitude, Longitude, StatusName FROM @UnroutedOrUncompletedServices

-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--Now that we have all the services that would have been on the date provided, we will take ExcludedDates into account
-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	--Table will hold all Ids and ExcludedDatesStrings
	--Only those RecurringServices that have been scheduled for the date provided and have an ExcludedDatesString will appear
	DECLARE @RecurringServicesWithExcludedDates TABLE
	(
		Id uniqueidentifier,
		ExcludedDatesString nvarchar(max)
	)

	INSERT INTO @RecurringServicesWithExcludedDates
	SELECT t1.Id, t1.ExcludedDatesString FROM RecurringServices t1, @ServicesForDateTable t2
	WHERE t1.Id = t2.RecurringServiceId AND t1.ExcludedDatesString IS NOT NULL

	DECLARE @RecurringServicesWithExcludedDatesSplit TABLE
	(
		Id uniqueidentifier,
		ExcludedDate nvarchar(max)
	)

	DECLARE @RowCount int --Row count for @RecurringServicesWithExcludedDates (We delete from this table as we input into @RecurringServicesWithExcludedDatesSplit)
	DECLARE @RowId  uniqueidentifier --RecurringServiceId of the current row
	DECLARE @RowExcludedDateString nvarchar(max) --ExcludedDatesString for the current row

	SET @RowCount = (SELECT COUNT(*) FROM @RecurringServicesWithExcludedDates)

	WHILE @RowCount > 0
	BEGIN 
		SET @RowId = (SELECT TOP(1) Id FROM @RecurringServicesWithExcludedDates ORDER BY Id) --Find the RowId of the top row sorted by Id
		SET @RowExcludedDateString = (SELECT ExcludedDatesString FROM @RecurringServicesWithExcludedDates WHERE Id = @RowId) --Find the ExcludedDatesString of the top row found above

		--Converts the ExcludedDateString to a Table(See example below for more information)
		/****************************************************************************************************************************************************
		* FUNCTION Split will convert the comma separated string of dates ()
		** Input Parameters **
		* @Id - RecurringServiceId
		* @sInputList - List of delimited ExcludedDates
		* @sDelimiter - -- Delimiter that separates ExcludedDates
		** Output Parameters: **
		*  @List TABLE (Id uniqueidentifier, ExcludedDate VARCHAR(8000)) - Ex. below
		* Id                                     | ExcludedDate
		* -----------------------------------------------------------------------------------------
		* {036BD670-39A5-478F-BFA3-AD312E3F7F47} | 1/1/2012
		* {B30A43AD-655A-449C-BD4E-951F8F988718} | 1/1/2012
		* {03DB9F9B-2FF6-4398-B984-533FB3E19C50} | 1/2/2012
		***************************************************************************************************************************************************/
		INSERT INTO @RecurringServicesWithExcludedDatesSplit
		SELECT * FROM [dbo].[Split] (
								@RowId,
								@RowExcludedDateString,
								','
								)
		
		--Now that we have converted this row, remove it from @RecurringServicesWithExcludedDates
		DELETE FROM @RecurringServicesWithExcludedDates
		WHERE Id = @RowId

		--Reset @RowCount for the loop condition
		SET @RowCount = (SELECT COUNT(*) FROM @RecurringServicesWithExcludedDates)
	END

	--Table that will hold all information about Services that have been excluded
	DECLARE @SevicesThatHaveBeenExcluded TABLE
		(
			RecurringServiceId uniqueidentifier,
			ServiceId uniqueidentifier,
			OccurDate date,
			ServiceName nvarchar(max),
			ClientName nvarchar(max),
			ClientId uniqueidentifier,
			RegionName nvarchar(max),
			LocationName nvarchar(max),
			LocationId uniqueidentifier,
			AddressLine nvarchar(max),
			Latitude decimal(18,8),
			Longitude decimal(18,8),
			StatusName nvarchar(max)
		) 

	--Find all ExcludedServices from @ServicesForDateTable
	INSERT INTO @SevicesThatHaveBeenExcluded
	SELECT t1.* FROM @ServicesForDateTable t1, @RecurringServicesWithExcludedDatesSplit t2
	WHERE t1.RecurringServiceId = t2.Id AND t1.OccurDate = t2.ExcludedDate

	CREATE TABLE #ServicesTableToReturn
(
		Id UNIQUEIDENTIFIER,
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max),
		ClientName nvarchar(max),
		ClientId UNIQUEIDENTIFIER ,
		RegionName nvarchar(max),
		LocationName nvarchar(max),
		LocationId uniqueidentifier,
		AddressLine nvarchar(max),
		Latitude decimal(18,8),
		Longitude decimal(18,8),
		StatusName nvarchar(max)
	) 

	--Add all services that have not been excluded to the output table
	INSERT INTO #ServicesTableToReturn(RecurringServiceId, ServiceId, OccurDate, ServiceName, ClientName, ClientId, RegionName, LocationName, LocationId, AddressLine, Latitude, Longitude, StatusName)
	SELECT * FROM @ServicesForDateTable
	EXCEPT
	SELECT * FROM @SevicesThatHaveBeenExcluded

	UPDATE #ServicesTableToReturn
	SET Id = NEWID()

	CREATE TABLE #RouteTasks
			( Id UNIQUEIDENTIFIER,
			  LocationId UNIQUEIDENTIFIER,
			  RouteDestinationId UNIQUEIDENTIFIER,
			  ClientId UNIQUEIDENTIFIER,
			  ServiceId UNIQUEIDENTIFIER,
			  ReadOnly BIT,
			  BusinessAccountId UNIQUEIDENTIFIER,
			  EstimatedDuration TIME,
			  Name NVARCHAR(MAX),
			  StatusInt INT,
			  [Date] DATETIME,
			  OrderInRouteDestination INT,
			  RecurringServiceId UNIQUEIDENTIFIER,
			  DelayedChildId UNIQUEIDENTIFIER,
			  TaskStatusId UNIQUEIDENTIFIER
			)

	INSERT INTO #RouteTasks(Id, LocationId, ClientId, ServiceId, Name, [Date], RecurringServiceId)
	SELECT Id, LocationId, ClientId, ServiceId, ServiceName, OccurDate, RecurringServiceId
	FROM #ServicesTableToReturn

	UPDATE #RouteTasks
	SET ReadOnly = 0,
		BusinessAccountId = @serviceProviderIdContext,
		EstimatedDuration = '0:0:16.00',
		OrderInRouteDestination = 0,
		TaskStatusId = (SELECT TOP 1 Id FROM dbo.TaskStatuses WHERE BusinessAccountId = @serviceProviderIdContext AND DefaultTypeInt = 1),
		StatusInt = 0

	INSERT INTO dbo.RouteTasks
	SELECT * FROM #RouteTasks

	SELECT * FROM dbo.RouteTasks
	LEFT JOIN dbo.Locations
	ON dbo.RouteTasks.LocationId = dbo.Locations.Id
	LEFT JOIN dbo.Regions
	ON dbo.Locations.RegionId = dbo.Regions.Id
	LEFT JOIN dbo.Clients
	ON dbo.RouteTasks.ClientId = dbo.Clients.Id  
	LEFT JOIN dbo.TaskStatuses
	ON dbo.RouteTasks.TaskStatusId = dbo.TaskStatuses.Id
	WHERE dbo.RouteTasks.BusinessAccountId = @serviceProviderIdContext AND [Date] = @serviceDate AND RouteDestinationId IS NULL 

	DROP TABLE #ServicesTableToReturn
	DROP TABLE #RouteTasks
RETURN 
END

GO

USE Core
GO
/****** Object:  StoredProcedure [dbo].[PropagateNewFields]    Script Date: 6/19/2012 1:04:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[PropagateNewFields]
		(@FieldId uniqueidentifier)
	AS
	BEGIN
		BEGIN -- declaring variables to be used to copy fields
			DECLARE @FieldRowCount int
			DECLARE @fieldName NVARCHAR(max)
			DECLARE @fieldRequired BIT
			DECLARE @fieldToolTip NVARCHAR(MAX)
			DECLARE @fieldServiceTemplateId UNIQUEIDENTIFIER
			DECLARE @serviceTemplateCount INT 
			DECLARE @currentServiceTemplateId UNIQUEIDENTIFIER
			DECLARE @newFieldId UNIQUEIDENTIFIER      
			DECLARE @optionsFieldId UNIQUEIDENTIFIER --Will only be used for copying OptionsFields and for copying the Options
			DECLARE @serviceTemplateId UNIQUEIDENTIFIER
		END

		BEGIN --Declaring tables to use for copying fields 
			DECLARE @CopyOfNewServiceTemplates TABLE
			(
				Id UNIQUEIDENTIFIER
			)    
			  
		END

		BEGIN --Sets all variables to copy basic field data
			SET @fieldName = (SELECT Name FROM Fields WHERE Id = @FieldId)
			SET @fieldRequired = (SELECT [Required] FROM Fields WHERE Id = @FieldId)
			SET @fieldToolTip = (SELECT ToolTip FROM Fields WHERE Id = @FieldId)
			SET @serviceTemplateId = (SELECT ServiceTemplateId FROM dbo.Fields WHERE Id = @FieldId)
		END;
  
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

		INSERT INTO @CopyOfNewServiceTemplates (Id)
		SELECT	TemplateRecurs.Id
		FROM	TemplateRecurs		      
		
		DELETE FROM @CopyOfNewServiceTemplates
		WHERE Id = @serviceTemplateId

		--Track the number of Service Templates left to be created
		SET @serviceTemplateCount = (SELECT COUNT(*) FROM @CopyOfNewServiceTemplates)
        
		WHILE @serviceTemplateCount > 0
		BEGIN     
			SET @currentServiceTemplateId = (SELECT MIN(Id) FROM @CopyOfNewServiceTemplates)				
			
			SET @newFieldId = NEWID()

			INSERT INTO Fields --Add a copy of the old field to the Fields table
				    ( Id ,
				        Name ,
				        [Required] ,
				        ToolTip ,
				        ParentFieldId ,
				        ServiceTemplateId
				    )
			VALUES  ( @newFieldId , -- Id - uniqueidentifier
				        @fieldName , -- Name - nvarchar(max)
				        @fieldRequired , -- Required - bit
				        @fieldToolTip , -- ToolTip - nvarchar(max)
				        @FieldId , -- ParentFieldId - uniqueidentifier
				        @currentServiceTemplateId  -- ServiceTemplateId - uniqueidentifier
				    ) 
				
			BEGIN --Copy field to its appropriate inherited table
				IF @FieldId IN (SELECT Id FROM dbo.Fields_DateTimeField) --Copy the DateTime field, set new Id's
				BEGIN 
					INSERT INTO Fields_DateTimeField
							( Earliest ,
								Latest ,
								TypeInt ,
								VALUE ,
								Id
							)
					VALUES  ( (SELECT Earliest FROM Fields_DateTimeField WHERE Id = @FieldId) , -- Earliest - datetime
								(SELECT Latest FROM Fields_DateTimeField WHERE Id = @FieldId) , -- Latest - datetime
								(SELECT TypeInt FROM Fields_DateTimeField WHERE Id = @FieldId) , -- TypeInt - smallint
								(SELECT Value FROM Fields_DateTimeField WHERE Id = @FieldId) , -- Value - datetime
								@newFieldId  -- Id - uniqueidentifier
							)
				END
    
				IF @FieldId IN (SELECT Id FROM dbo.Fields_LocationField) --Copy Location field, set new Id's
				BEGIN   
			 
					INSERT INTO Fields_LocationField
							( LocationId ,
								LocationFieldTypeInt ,
								Id
							)
					VALUES  ( (SELECT LocationId FROM Fields_LocationField WHERE Id = @FieldId) , --LocationId - uniqueidenetifier
								(SELECT LocationFieldTypeInt FROM Fields_LocationField WHERE Id = @FieldId) , --LocationFieldTypeInt - int
								@newFieldId  -- Id - uniqueidentifier
							)
				END

				IF @FieldId IN (SELECT Id FROM dbo.Fields_NumericField) --Copy Numeric field, set new Id's 
				BEGIN
					INSERT INTO Fields_NumericField
							( Mask ,
								DecimalPlaces ,
								Minimum ,
								Maximum ,
								Value ,
								Id
							)
					VALUES  ( (SELECT Mask FROM Fields_NumericField WHERE Id = @FieldId) , -- Mask - nvarchar(max)
								(SELECT DecimalPlaces FROM Fields_NumericField WHERE Id = @FieldId) , -- DecimalPlaces - int
								(SELECT Minimum FROM Fields_NumericField WHERE Id = @FieldId) , -- Minimum - decimal
								(SELECT Maximum FROM Fields_NumericField WHERE Id = @FieldId) , -- Maximum - decimal
								(SELECT Value FROM Fields_NumericField WHERE Id = @FieldId) , -- Value - decimal
								@newFieldId  -- Id - uniqueidentifier
							)

				END

				IF @FieldId IN (SELECT Id FROM dbo.Fields_TextBoxField) --Copy TextBox field, set new Id's
				BEGIN
					INSERT INTO Fields_TextBoxField
							( IsMultiline, 
								Value, 
								Id )
					VALUES  ( (SELECT IsMultiline FROM dbo.Fields_TextBoxField WHERE Id = @FieldId), -- IsMultiline - bit
								(SELECT Value FROM dbo.Fields_TextBoxField WHERE Id = @FieldId), -- Value - nvarchar(max)
								@newFieldId  -- Id - uniqueidentifier
								)	    
				END

				IF @FieldId IN (SELECT Id FROM dbo.Fields_SignatureField) --Copy Signature field, set new Id's
				BEGIN
					INSERT INTO Fields_SignatureField
							( Value, 
								Id )
					VALUES  ((SELECT Value FROM dbo.Fields_SignatureField WHERE Id = @FieldId), -- Value - nvarchar(max)
								@newFieldId  -- Id - uniqueidentifier
								)	    
				END

				IF @FieldId IN (SELECT Id FROM dbo.Fields_OptionsField) --Copy the Options field, set new Id's 
				BEGIN
					
					INSERT INTO dbo.Fields_OptionsField
							( AllowMultipleSelection ,
							  TypeInt ,
							  Value ,
							  OptionsString ,
							  Id
							)
					VALUES	( (SELECT AllowMultipleSelection FROM dbo.Fields_OptionsField WHERE Id = @FieldId) , -- AllowMultipleSelection - bit
							  (SELECT TypeInt FROM dbo.Fields_OptionsField WHERE Id = @FieldId), -- TypeInt - smallint
							  (SELECT Value FROM dbo.Fields_OptionsField WHERE Id = @FieldId) , -- Value - nvarchar(max)
							  (SELECT OptionsString FROM dbo.Fields_OptionsField WHERE Id = @FieldId) , -- OptionsString - nvarchar(max)
							  @newFieldId  -- Id - uniqueidentifier
							)
				END  		
			END
			      
			DELETE FROM @CopyOfNewServiceTemplates
			WHERE Id = @currentServiceTemplateId

			SET @serviceTemplateCount = @serviceTemplateCount - 1
		END	
	
	END
	RETURN

GO

USE [Core]
GO
/****** Object:  StoredProcedure [dbo].[PropagateNameChange]    Script Date: 6/19/2012 1:04:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PropagateNameChange] 
	(@serviceTemplateId uniqueidentifier)

AS
BEGIN
	DECLARE @newName NVARCHAR(max)
	DECLARE @oldName NVARCHAR(max)
	
	CREATE TABLE #TempTable
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

	INSERT INTO #TempTable (Id)
	SELECT	TemplateRecurs.Id
	FROM	TemplateRecurs

	SET @newName = (SELECT Name 
					FROM dbo.ServiceTemplates 
					WHERE Id = @serviceTemplateId)

	--This will take the first row from the results of a query where the name of the ServiceTemplate is not the new name to be assigned
	--From there it will only select the Name column from that row
	SET @oldName = (SELECT TOP 1 Name 
					FROM dbo.ServiceTemplates 
					WHERE Id IN 
					(
						SELECT Id 
						FROM #TempTable
					) AND Name <> @newName)

	UPDATE dbo.ServiceTemplates
	SET Name = @newName
	WHERE Id IN
	(
		SELECT Id
		FROM #TempTable
	) 

	UPDATE dbo.[Routes] 
	SET RouteType = @newName
	WHERE RouteType = @oldName   

	DROP TABLE  #TempTable

END
RETURN

GO

USE [Core]
GO
/****** Object:  StoredProcedure [dbo].[PropagateNewServiceTemplateToClients]    Script Date: 6/19/2012 1:04:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PropagateNewServiceTemplateToClients]
	(@serviceTemplateId uniqueidentifier)

AS
BEGIN

	DECLARE @serviceProviderId UNIQUEIDENTIFIER
	SET @serviceProviderId = (SELECT OwnerServiceProviderId FROM dbo.ServiceTemplates WHERE Id = @serviceTemplateId)

	BEGIN --Propagate new Service Template to all Clients Avaliable Services
		--find all clients for the business account
		DECLARE @ClientsTable TABLE
		(
			Id UNIQUEIDENTIFIER
		)

		--Table that will hold the new Service Templates created
		DECLARE @newServiceTemplateTable TABLE
		(
			Id UNIQUEIDENTIFIER,
			OwnerServiceProviderId UNIQUEIDENTIFIER,
			OwnerClientId UNIQUEIDENTIFIER,
			OwnerServiceTemplateId UNIQUEIDENTIFIER,
			LevelInt INT,
			Name NVARCHAR(max)
		)

		--Find all clients for the Service Provider
		INSERT INTO @ClientsTable
		SELECT Id FROM dbo.Clients
		WHERE BusinessAccountId = @serviceProviderId

		DECLARE @ClientRowCount int
		SET @ClientRowCount = (SELECT COUNT(*) FROM @ClientsTable)

		--Stores the Name of the Service Templates to be created
		DECLARE @serviceTemplateName NVARCHAR(max)
		--Stores the Id of the parent Service Template
		DECLARE @ownerServiceTemplateId UNIQUEIDENTIFIER

		SET @serviceTemplateName = (SELECT Name FROM dbo.ServiceTemplates WHERE Id = @serviceTemplateId)
		SET @ownerServiceTemplateId = @serviceTemplateId

		DECLARE @currentId UNIQUEIDENTIFIER

		WHILE @ClientRowCount > 0 --Iterate through all clients, while making a new service template and assigning it to the client
		BEGIN 

			SET @currentId = (SELECT MIN(Id) FROM @ClientsTable)

			--Create the new Service Template and add it to the table to be added to Service Template table later
			INSERT INTO @newServiceTemplateTable
					( Id ,
					  OwnerServiceProviderId ,
					  OwnerClientId ,
					  OwnerServiceTemplateId ,
					  LevelInt ,
					  Name
					)
			VALUES  ( NEWID() , -- Id - uniqueidentifier
					  @serviceProviderId , -- OwnerServiceProviderId - uniqueidentifier
					  @currentId , -- OwnerClientId - uniqueidentifier
					  @ownerServiceTemplateId , -- OwnerServiceTemplateId - uniqueidentifier
					  3 , -- LevelInt - int
					  @serviceTemplateName  -- Name - nvarchar(max)
					)

					--Once the client has been used, remove it from the list
					DELETE FROM @ClientsTable
					WHERE Id = @currentId

					SET @ClientRowCount = @ClientRowCount - 1
		END

		--Add all the newly created Service Tempalte to the database
		INSERT INTO dbo.ServiceTemplates
		SELECT * FROM @newServiceTemplateTable
	END 

	-------------------------------------------------------------------------------------------------------------------------------
	--Now that we have all the new Service Templates created, we need to add all the appropriate fields to them
	-------------------------------------------------------------------------------------------------------------------------------

	BEGIN --Propagate all the fields down to the new Service Templates created above
	
		DECLARE @FieldsTable TABLE --Will be used to store Fields that were pulled off of the parent Service Template
		(
			Id UNIQUEIDENTIFIER,
			Name NVARCHAR(MAX),
			[Required] BIT,
			ToolTip NVARCHAR(MAX),
			ParentFieldId UNIQUEIDENTIFIER,
			ServiceTemplateId UNIQUEIDENTIFIER
		) 

		INSERT INTO @FieldsTable
		SELECT * FROM dbo.Fields t1
		WHERE t1.ServiceTemplateId = @ownerServiceTemplateId 

		BEGIN -- declaring variables to be used to copy fields
			DECLARE @FieldRowCount int
			DECLARE @fieldName NVARCHAR(max)
			DECLARE @fieldRequired BIT
			DECLARE @fieldToolTip NVARCHAR(MAX)
			DECLARE @fieldServiceTemplateId UNIQUEIDENTIFIER
			DECLARE @serviceTemplateCount INT 
			DECLARE @currentServiceTemplateId UNIQUEIDENTIFIER
			DECLARE @newFieldId UNIQUEIDENTIFIER      
			DECLARE @optionsFieldId UNIQUEIDENTIFIER --Will only be used for copying OptionsFields and for copying the Options
		END

		BEGIN --Declaring tables to use for copying fields 
			DECLARE @CopyOfNewServiceTemplates TABLE
			(
				Id UNIQUEIDENTIFIER,
				OwnerServiceProviderId UNIQUEIDENTIFIER,
				OwnerClientId UNIQUEIDENTIFIER,
				OwnerServiceTemplateId UNIQUEIDENTIFIER,
				LevelInt INT,
				Name NVARCHAR(max)
			)
			DECLARE @OptionsCopies TABLE
			(
				Id UNIQUEIDENTIFIER,
				NAME NVARCHAR(MAX),
				IsChecked BIT,
				OptionsFieldId UNIQUEIDENTIFIER,
				[Index] INT,
				ToolTip NVARCHAR(MAX)
			)      
		END

		SET @FieldRowCount = (SELECT COUNT(*) FROM @FieldsTable)

		WHILE @FieldRowCount > 0
		BEGIN
			SET @currentId = (SELECT MIN(Id) FROM @FieldsTable)
    
			--Make sure that @CopyOfNewServiceTemplates is empty so it can be repopulated
			DELETE FROM @CopyOfNewServiceTemplates

			--Re-populate
			INSERT INTO @CopyOfNewServiceTemplates
			SELECT * FROM @newServiceTemplateTable

			BEGIN --Sets all variables to copy basic field data
				SET @fieldName = (SELECT Name FROM @FieldsTable WHERE Id = @currentId)
				SET @fieldRequired = (SELECT [Required] FROM @FieldsTable WHERE Id = @currentId)
				SET @fieldToolTip = (SELECT ToolTip FROM @FieldsTable WHERE Id = @currentId)
			END
		
			--Track the number of Service Templates left to be created
			SET @serviceTemplateCount = (SELECT COUNT(*) FROM @CopyOfNewServiceTemplates)
        
			WHILE @serviceTemplateCount > 0
			BEGIN     
				SET @currentServiceTemplateId = (SELECT MIN(Id) FROM @CopyOfNewServiceTemplates)				
			
				SET @newFieldId = NEWID()

				INSERT INTO Fields --Add a copy of the old field to the Fields table
				        ( Id ,
				          Name ,
				          [Required] ,
				          ToolTip ,
				          ParentFieldId ,
				          ServiceTemplateId
				        )
				VALUES  ( @newFieldId , -- Id - uniqueidentifier
				          @fieldName , -- Name - nvarchar(max)
				          @fieldRequired , -- Required - bit
				          @fieldToolTip , -- ToolTip - nvarchar(max)
				          @currentId , -- ParentFieldId - uniqueidentifier
				          @currentServiceTemplateId  -- ServiceTemplateId - uniqueidentifier
				        ) 
				
				BEGIN --Copy field to its appropriate inherited table			
					IF @currentId IN (SELECT Id FROM dbo.Fields_DateTimeField) --Copy the DateTime field, set new Id's
					BEGIN 
						INSERT INTO Fields_DateTimeField
								( Earliest ,
								  Latest ,
								  TypeInt ,
								  VALUE ,
								  Id
								)
						VALUES  ( (SELECT Earliest FROM Fields_DateTimeField WHERE Id = @currentId) , -- Earliest - datetime
								  (SELECT Latest FROM Fields_DateTimeField WHERE Id = @currentId) , -- Latest - datetime
								  (SELECT TypeInt FROM Fields_DateTimeField WHERE Id = @currentId) , -- TypeInt - smallint
								  (SELECT Value FROM Fields_DateTimeField WHERE Id = @currentId) , -- Value - datetime
								 @newFieldId  -- Id - uniqueidentifier
								)
					END
    
					IF @currentId IN (SELECT Id FROM dbo.Fields_LocationField) --Copy Location field, set new Id's
					BEGIN   
			 
						INSERT INTO Fields_LocationField
								( LocationId ,
								  LocationFieldTypeInt ,
								  Id
								)
						VALUES  ( (SELECT LocationId FROM Fields_LocationField WHERE Id = @currentId) , --LocationId - uniqueidenetifier
								  (SELECT LocationFieldTypeInt FROM Fields_LocationField WHERE Id = @currentId) , --LocationFieldTypeInt - int
								  @newFieldId  -- Id - uniqueidentifier
								)
					END

					IF @currentId IN (SELECT Id FROM dbo.Fields_NumericField) --Copy Numeric field, set new Id's 
					BEGIN
						INSERT INTO Fields_NumericField
								( Mask ,
								  DecimalPlaces ,
								  Minimum ,
								  Maximum ,
								  Value ,
								  Id
								)
						VALUES  ( (SELECT Mask FROM Fields_NumericField WHERE Id = @currentId) , -- Mask - nvarchar(max)
								  (SELECT DecimalPlaces FROM Fields_NumericField WHERE Id = @currentId) , -- DecimalPlaces - int
								  (SELECT Minimum FROM Fields_NumericField WHERE Id = @currentId) , -- Minimum - decimal
								  (SELECT Maximum FROM Fields_NumericField WHERE Id = @currentId) , -- Maximum - decimal
								  (SELECT Value FROM Fields_NumericField WHERE Id = @currentId) , -- Value - decimal
								  @newFieldId  -- Id - uniqueidentifier
								)

					END

					IF @currentId IN (SELECT Id FROM dbo.Fields_TextBoxField) --Copy TextBox field, set new Id's
					BEGIN
						INSERT INTO Fields_TextBoxField
								( IsMultiline, 
								  Value, 
								  Id )
						VALUES  ( (SELECT IsMultiline FROM dbo.Fields_TextBoxField WHERE Id = @currentId), -- IsMultiline - bit
								  (SELECT Value FROM dbo.Fields_TextBoxField WHERE Id = @currentId), -- Value - nvarchar(max)
								  @newFieldId  -- Id - uniqueidentifier
								  )	    
					END

					IF @currentId IN (SELECT Id FROM dbo.Fields_SignatureField) --Copy Signature field, set new Id's
					BEGIN
						INSERT INTO Fields_SignatureField
								( Value, 
								  Id )
						VALUES  ( (SELECT Value FROM dbo.Fields_TextBoxField WHERE Id = @currentId), -- Value - nvarchar(max)
								  @newFieldId  -- Id - uniqueidentifier
								  )	    
					END

					IF @currentId IN (SELECT Id FROM dbo.Fields_OptionsField) --Copy the Options field, set new Id's 
					BEGIN
						INSERT INTO dbo.Fields_OptionsField
								( AllowMultipleSelection ,
									TypeInt ,
									Value ,
									OptionsString ,
									Id
								)
						VALUES	( (SELECT AllowMultipleSelection FROM dbo.Fields_OptionsField WHERE Id = @currentId) , -- AllowMultipleSelection - bit
									(SELECT TypeInt FROM dbo.Fields_OptionsField WHERE Id = @currentId), -- TypeInt - smallint
									(SELECT Value FROM dbo.Fields_OptionsField WHERE Id = @currentId) , -- Value - nvarchar(max)
									(SELECT OptionsString FROM dbo.Fields_OptionsField WHERE Id = @currentId) , -- OptionsString - nvarchar(max)
									@newFieldId  -- Id - uniqueidentifier
								)
					END  		
				END
			      
				DELETE FROM @CopyOfNewServiceTemplates
				WHERE Id = @currentServiceTemplateId

				SET @serviceTemplateCount = @serviceTemplateCount - 1
			END	

			DELETE FROM @FieldsTable
			WHERE Id = @currentId

			SET @FieldRowCount = @FieldRowCount - 1
		END
	END

END
RETURN

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
SELECT ServiceTemplates.Id as 'ServiceTemplateId', Parties_BusinessAccount.Id as 'BusinessAccountId'
FROM ServiceTemplates, Parties_BusinessAccount
WHERE ServiceTemplates.OwnerServiceProviderId = Parties_BusinessAccount.Id
UNION ALL
SELECT ServiceTemplates.Id as 'ServiceTemplateId', Clients.BusinessAccountId as 'BusinessAccountId'
FROM ServiceTemplates, Clients
WHERE Clients.Id = ServiceTemplates.OwnerClientId
UNION ALL
SELECT ServiceTemplates.Id as 'ServiceTemplateId', Services.ServiceProviderId as 'BusinessAccountId'
FROM ServiceTemplates, Services
WHERE ServiceTemplates.Id = Services.Id
UNION ALL
SELECT ServiceTemplates.Id as 'ServiceTemplateId', Clients.BusinessAccountId as 'BusinessAccountId'
FROM ServiceTemplates, RecurringServices, Clients
WHERE ServiceTemplates.Id = RecurringServices.Id AND RecurringServices.ClientId = Clients.Id

GO

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF OBJECT_ID(N'[dbo].[Split]', N'FN') IS NOT NULL
DROP FUNCTION [dbo].[Split]
GO
/****************************************************************************************************************************************************
* FUNCTION Split will convert the comma separated string of dates ()
** Input Parameters **
* @Id - RecurringServiceId
* @sInputList - List of delimited ExcludedDates
* @sDelimiter - -- Delimiter that separates ExcludedDates
** Output Parameters: **
*  @List TABLE (Id uniqueidentifier, ExcludedDate VARCHAR(8000)) - Ex. below
* Id                                     | ExcludedDate
* -----------------------------------------------------------------------------------------
* {036BD670-39A5-478F-BFA3-AD312E3F7F47} | 1/1/2012
* {B30A43AD-655A-449C-BD4E-951F8F988718} | 1/1/2012
* {03DB9F9B-2FF6-4398-B984-533FB3E19C50} | 1/2/2012
***************************************************************************************************************************************************/
CREATE FUNCTION dbo.Split(
	@Id			uniqueidentifier --RecurringServiceId
  , @sInputList VARCHAR(8000) -- List of delimited ExcludedDates
  , @sDelimiter VARCHAR(8000) = ',' -- Delimiter that separates ExcludedDates
) RETURNS @List TABLE (Id uniqueidentifier, ExcludedDate VARCHAR(8000))

BEGIN
DECLARE @sItem VARCHAR(8000)
WHILE CHARINDEX(@sDelimiter,@sInputList,0) <> 0
 BEGIN
 SELECT
  @sItem=RTRIM(LTRIM(SUBSTRING(@sInputList,1,CHARINDEX(@sDelimiter,@sInputList,0)-1))),
  @sInputList=RTRIM(LTRIM(SUBSTRING(@sInputList,CHARINDEX(@sDelimiter,@sInputList,0)+LEN(@sDelimiter),LEN(@sInputList))))
 
 IF LEN(@sItem) > 0
 BEGIN
  INSERT INTO @List (ExcludedDate) SELECT @sItem

  UPDATE @List
  SET Id = @Id
 END
 END

IF LEN(@sInputList) > 0
BEGIN
 INSERT INTO @List (ExcludedDate) SELECT @sInputList -- Put the last ExcludedDate in

 UPDATE @List
 SET Id = @Id	
END
RETURN
END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
USE Core
GO
/****************************************************************************************************************************************************
* FUNCTION TestFieldPropagationSuccess will check a ServiceTemplate to see if its children ServiceTemplates have had a field propagated successfully
** Input Parameters **
* @serviceTemplateId - The Id of the Service Template that you want to check 
* @fieldName - The name of the field that was propagated
** Output Parameters: **
* INT - Will be '0' if propagation was not successful and will be '1' if it was
***************************************************************************************************************************************************/
CREATE PROCEDURE [dbo].[TestFieldPropagationSuccess]
(
	@serviceTemplateId UNIQUEIDENTIFIER,
	@fieldName NVARCHAR(MAX)
)
AS
BEGIN
	CREATE TABLE #TemplateIds
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

	INSERT INTO #TemplateIds (Id)
	SELECT	TemplateRecurs.Id
	FROM	TemplateRecurs

	CREATE TABLE #FieldIds
	(
		Id UNIQUEIDENTIFIER
	)

	INSERT INTO #FieldIds
	SELECT Id FROM dbo.Fields WHERE ServiceTemplateId IN (SELECT Id FROM #TemplateIds) AND Name = @fieldName

	IF (SELECT COUNT(*) FROM #FieldIds) <> (SELECT COUNT(*) FROM #TemplateIds)
		SELECT 1
	
	SELECT 0

	DROP TABLE #TemplateIds
	DROP TABLE #FieldIds
END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
USE Core
GO
/****************************************************************************************************************************************************
* FUNCTION TestNewServiceTemplatePropagationSuccess will check if a new Service Template was propagated successfully to all clients
** Input Parameters **
* @serviceTemplateId - The Id of the Service Template that you want to check 
* @oldName - The previous name of the service template that was propagated
** Output Parameters: **
* INT - Will be '0' if propagation was successful and will be '1' if it was not
***************************************************************************************************************************************************/
CREATE PROCEDURE [dbo].[TestNewServiceTemplatePropagationSuccess]
(
	@serviceTemplateId UNIQUEIDENTIFIER
)
AS
BEGIN
	DECLARE @templateName NVARCHAR(MAX)
	SET @templateName = (SELECT Name FROM dbo.ServiceTemplates WHERE Id = @serviceTemplateId)

	DECLARE @businessAccountId UNIQUEIDENTIFIER
	SET @businessAccountId = (SELECT OwnerServiceProviderId FROM dbo.ServiceTemplates WHERE Id = @serviceTemplateId)  

	CREATE TABLE #ClientIds
	(
		Id UNIQUEIDENTIFIER
	)

	--Insert all client Id's for the business account
	INSERT INTO #ClientIds
	SELECT Id FROM dbo.Clients WHERE BusinessAccountId = @businessAccountId

	CREATE TABLE #TemplateIds
	(
		Id uniqueidentifier
	)

	--Insert all service templates that are on the client level and have the correct name
	INSERT INTO #TemplateIds
	SELECT Id FROM dbo.ServiceTemplates WHERE OwnerClientId IN (SELECT Id FROM #ClientIds) AND Name = @templateName

	--If there is a different number of clients than Service Templates, return an error
	IF (SELECT COUNT(*) FROM #ClientIds) <> (SELECT COUNT(*) FROM #TemplateIds)
		SELECT 1

	CREATE TABLE #Fields
	(
		Id UNIQUEIDENTIFIER
	)

	--Inserts all fields from the the service templates in #TemplateIds
	INSERT INTO #Fields
	SELECT Id FROM dbo.Fields WHERE ServiceTemplateId IN (SELECT Id FROM #TemplateIds)

	DECLARE @originalFieldCount INT
	SET @originalFieldCount = (SELECT COUNT(*) FROM dbo.Fields WHERE ServiceTemplateId = @serviceTemplateId)  

	--If there is a different number of fields than was expected, return an error
	IF (SELECT COUNT(*) FROM #TemplateIds) * @originalFieldCount <> (SELECT COUNT(*) FROM #Fields)
		SELECT 1

	SELECT 0

	DROP TABLE #TemplateIds
	DROP TABLE #ClientIds
	DROP TABLE #Fields
END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
USE Core
GO
/****************************************************************************************************************************************************
* FUNCTION TestServiceTemplateNamePropagationSuccess will check a ServiceTemplate to see if its children ServiceTemplates have had a name change propagated successfully
** Input Parameters **
* @serviceTemplateId - The Id of the Service Template that you want to check 
* @oldName - The previous name of the service template that was propagated
** Output Parameters: **
* INT - Will be '0' if propagation was successful and will be '1' if it was not
***************************************************************************************************************************************************/
CREATE PROCEDURE [dbo].[TestServiceTemplateNamePropagationSuccess]
(
	@serviceTemplateId UNIQUEIDENTIFIER,
	@oldName NVARCHAR(MAX)
)
AS
BEGIN
	CREATE TABLE #TemplateIds
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

	INSERT INTO #TemplateIds (Id)
	SELECT	TemplateRecurs.Id
	FROM	TemplateRecurs

	--@failedServiceTemplateId will only be non-null if one of the children service templates did not get re-named properly
	DECLARE @failedServiceTemplateId UNIQUEIDENTIFIER = NULL  
	SET @failedServiceTemplateId = (SELECT TOP 1 Id FROM dbo.ServiceTemplates WHERE Id IN (SELECT Id FROM #TemplateIds) AND Name = @oldName)
	
	IF @failedServiceTemplateId IS NOT NULL  
		SELECT 1
	
	--@failedRoutesId will only be non-null if there is a Route with the old Service Templates name
	DECLARE @failedRoutesId UNIQUEIDENTIFIER = NULL
	SET @failedRoutesId = (SELECT TOP 1 Id FROM dbo.[Routes] WHERE Name = @oldName)
	
	IF @failedRoutesId IS NOT NULL
		SELECT 1  

	SELECT 0

	DROP TABLE #TemplateIds
END	