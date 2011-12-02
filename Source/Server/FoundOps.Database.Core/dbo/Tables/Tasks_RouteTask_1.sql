CREATE TABLE [dbo].[Tasks_RouteTask] (
    [Date]               DATETIME         NOT NULL,
    [LocationId]         UNIQUEIDENTIFIER NULL,
    [RouteDestinationId] UNIQUEIDENTIFIER NULL,
    [ClientId]           UNIQUEIDENTIFIER NULL,
    [ServiceId]          UNIQUEIDENTIFIER NULL,
    [ReadOnly]           BIT              NOT NULL,
    [BusinessAccountId]  UNIQUEIDENTIFIER NOT NULL,
    [EstimatedDuration]  TIME (7)         NOT NULL,
    [Id]                 UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Tasks_RouteTask] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_BusinessAccountRouteTask] FOREIGN KEY ([BusinessAccountId]) REFERENCES [dbo].[Parties_BusinessAccount] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_RouteTask_inherits_Task] FOREIGN KEY ([Id]) REFERENCES [dbo].[Tasks] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_RouteTaskClient] FOREIGN KEY ([ClientId]) REFERENCES [dbo].[Clients] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_RouteTaskLocation] FOREIGN KEY ([LocationId]) REFERENCES [dbo].[Locations] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_RouteTaskRouteDestination] FOREIGN KEY ([RouteDestinationId]) REFERENCES [dbo].[RouteDestinations] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_RouteTaskService] FOREIGN KEY ([ServiceId]) REFERENCES [dbo].[Services] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
);


GO
CREATE NONCLUSTERED INDEX [IX_FK_RouteTaskService]
    ON [dbo].[Tasks_RouteTask]([ServiceId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FK_RouteTaskRouteDestination]
    ON [dbo].[Tasks_RouteTask]([RouteDestinationId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FK_RouteTaskLocation]
    ON [dbo].[Tasks_RouteTask]([LocationId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FK_RouteTaskClient]
    ON [dbo].[Tasks_RouteTask]([ClientId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FK_BusinessAccountRouteTask]
    ON [dbo].[Tasks_RouteTask]([BusinessAccountId] ASC);

