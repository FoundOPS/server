/*
Deployment script for FoundOps.Database.Core
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "FoundOps.Database.Core"
:setvar DefaultDataPath "C:\FoundOps\Agile5\Source-DEV\Server\FoundOps.Database.Core\Sandbox"
:setvar DefaultLogPath "C:\FoundOps\Agile5\Source-DEV\Server\FoundOps.Database.Core\Sandbox"

GO
USE [master];


GO
:on error exit
GO
USE [$(DatabaseName)];


GO
IF fulltextserviceproperty(N'IsFulltextInstalled') = 1
    EXECUTE sp_fulltext_database 'disable';


GO
PRINT N'Creating FK_BusinessAccountRouteTask...';


GO
ALTER TABLE [dbo].[RouteTasks] WITH NOCHECK
    ADD CONSTRAINT [FK_BusinessAccountRouteTask] FOREIGN KEY ([BusinessAccountId]) REFERENCES [dbo].[Parties_BusinessAccount] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_RouteTaskClient...';


GO
ALTER TABLE [dbo].[RouteTasks] WITH NOCHECK
    ADD CONSTRAINT [FK_RouteTaskClient] FOREIGN KEY ([ClientId]) REFERENCES [dbo].[Clients] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_RouteTaskLocation...';


GO
ALTER TABLE [dbo].[RouteTasks] WITH NOCHECK
    ADD CONSTRAINT [FK_RouteTaskLocation] FOREIGN KEY ([LocationId]) REFERENCES [dbo].[Locations] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_RouteTaskRouteDestination...';


GO
ALTER TABLE [dbo].[RouteTasks] WITH NOCHECK
    ADD CONSTRAINT [FK_RouteTaskRouteDestination] FOREIGN KEY ([RouteDestinationId]) REFERENCES [dbo].[RouteDestinations] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_RouteTaskService...';


GO
ALTER TABLE [dbo].[RouteTasks] WITH NOCHECK
    ADD CONSTRAINT [FK_RouteTaskService] FOREIGN KEY ([ServiceId]) REFERENCES [dbo].[Services] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Checking existing data against newly created constraints';


GO
USE [$(DatabaseName)];


GO
ALTER TABLE [dbo].[RouteTasks] WITH CHECK CHECK CONSTRAINT [FK_BusinessAccountRouteTask];

ALTER TABLE [dbo].[RouteTasks] WITH CHECK CHECK CONSTRAINT [FK_RouteTaskClient];

ALTER TABLE [dbo].[RouteTasks] WITH CHECK CHECK CONSTRAINT [FK_RouteTaskLocation];

ALTER TABLE [dbo].[RouteTasks] WITH CHECK CHECK CONSTRAINT [FK_RouteTaskRouteDestination];

ALTER TABLE [dbo].[RouteTasks] WITH CHECK CHECK CONSTRAINT [FK_RouteTaskService];


GO
PRINT N'Update complete.'
GO
