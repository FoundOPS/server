CREATE TABLE [dbo].[Vehicles] (
    [Id]           UNIQUEIDENTIFIER NOT NULL,
    [VehicleId]    NVARCHAR (MAX)   NULL,
    [Mileage]      INT              NULL,
    [LicensePlate] NVARCHAR (MAX)   NULL,
    [VIN]          NVARCHAR (MAX)   NULL,
    [Year]         INT              NULL,
    [Make]         NVARCHAR (MAX)   NULL,
    [Model]        NVARCHAR (MAX)   NULL,
    [Notes]        NVARCHAR (MAX)   NULL,
    [OwnerPartyId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Vehicles] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_VehicleParty] FOREIGN KEY ([OwnerPartyId]) REFERENCES [dbo].[Parties] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
);


GO
CREATE NONCLUSTERED INDEX [IX_FK_VehicleParty]
    ON [dbo].[Vehicles]([OwnerPartyId] ASC);

