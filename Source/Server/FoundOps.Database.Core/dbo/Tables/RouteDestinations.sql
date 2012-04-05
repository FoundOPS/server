CREATE TABLE [dbo].[RouteDestinations] (
    [Id]           UNIQUEIDENTIFIER NOT NULL,
    [OrderInRoute] INT              NOT NULL,
    [LocationId]   UNIQUEIDENTIFIER NULL,
    [RouteId]      UNIQUEIDENTIFIER NOT NULL,
    [ClientId]     UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_RouteDestinations] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_RouteDestinationClient] FOREIGN KEY ([ClientId]) REFERENCES [dbo].[Clients] ([Id]),
    CONSTRAINT [FK_RouteDestinationLocation] FOREIGN KEY ([LocationId]) REFERENCES [dbo].[Locations] ([Id]),
    CONSTRAINT [FK_RouteDestinationRoute] FOREIGN KEY ([RouteId]) REFERENCES [dbo].[Routes] ([Id]) ON DELETE CASCADE
);




GO
CREATE NONCLUSTERED INDEX [IX_FK_RouteDestinationClient]
    ON [dbo].[RouteDestinations]([ClientId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FK_RouteDestinationRoute]
    ON [dbo].[RouteDestinations]([RouteId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FK_RouteDestinationLocation]
    ON [dbo].[RouteDestinations]([LocationId] ASC);

