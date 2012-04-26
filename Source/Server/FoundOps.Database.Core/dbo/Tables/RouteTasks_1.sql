CREATE TABLE [dbo].[RouteTasks] (
    [Id]                      UNIQUEIDENTIFIER NOT NULL,
    [LocationId]              UNIQUEIDENTIFIER NULL,
    [RouteDestinationId]      UNIQUEIDENTIFIER NULL,
    [ClientId]                UNIQUEIDENTIFIER NULL,
    [ServiceId]               UNIQUEIDENTIFIER NULL,
    [ReadOnly]                BIT              NOT NULL,
    [BusinessAccountId]       UNIQUEIDENTIFIER NOT NULL,
    [EstimatedDuration]       TIME (7)         NOT NULL,
    [Name]                    NVARCHAR (MAX)   NOT NULL,
    [StatusInt]               INT              NOT NULL  ,
    [Date]                    DATETIME         NOT NULL,
    [OrderInRouteDestination] INT              NOT NULL,
    [RecurringServiceId]      UNIQUEIDENTIFIER NULL,
    [DelayedChildId]          UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_RouteTasks] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_BusinessAccountRouteTask] FOREIGN KEY ([BusinessAccountId]) REFERENCES [dbo].[Parties_BusinessAccount] ([Id]),
    CONSTRAINT [FK_RouteTaskClient] FOREIGN KEY ([ClientId]) REFERENCES [dbo].[Clients] ([Id]),
    CONSTRAINT [FK_RouteTaskLocation] FOREIGN KEY ([LocationId]) REFERENCES [dbo].[Locations] ([Id]),
    CONSTRAINT [FK_RouteTaskRecurringService] FOREIGN KEY ([RecurringServiceId]) REFERENCES [dbo].[RecurringServices] ([Id]),
    CONSTRAINT [FK_RouteTaskRouteDestination] FOREIGN KEY ([RouteDestinationId]) REFERENCES [dbo].[RouteDestinations] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_RouteTaskService] FOREIGN KEY ([ServiceId]) REFERENCES [dbo].[Services] ([Id])
);












GO
CREATE NONCLUSTERED INDEX [IX_FK_BusinessAccountRouteTask]
    ON [dbo].[RouteTasks]([BusinessAccountId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FK_RouteTaskService]
    ON [dbo].[RouteTasks]([ServiceId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FK_RouteTaskClient]
    ON [dbo].[RouteTasks]([ClientId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FK_RouteTaskRouteDestination]
    ON [dbo].[RouteTasks]([RouteDestinationId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FK_RouteTaskLocation]
    ON [dbo].[RouteTasks]([LocationId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FK_RouteTaskRecurringService]
    ON [dbo].[RouteTasks]([RecurringServiceId] ASC);

