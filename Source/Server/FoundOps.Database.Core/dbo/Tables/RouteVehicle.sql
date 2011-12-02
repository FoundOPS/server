CREATE TABLE [dbo].[RouteVehicle] (
    [Routes_Id]   UNIQUEIDENTIFIER NOT NULL,
    [Vehicles_Id] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_RouteVehicle] PRIMARY KEY CLUSTERED ([Routes_Id] ASC, [Vehicles_Id] ASC),
    CONSTRAINT [FK_RouteVehicle_Route] FOREIGN KEY ([Routes_Id]) REFERENCES [dbo].[Routes] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_RouteVehicle_Vehicle] FOREIGN KEY ([Vehicles_Id]) REFERENCES [dbo].[Vehicles] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
);


GO
CREATE NONCLUSTERED INDEX [IX_FK_RouteVehicle_Vehicle]
    ON [dbo].[RouteVehicle]([Vehicles_Id] ASC);

