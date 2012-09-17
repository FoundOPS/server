CREATE TABLE [dbo].[Vehicles] (
    [Id]                       UNIQUEIDENTIFIER NOT NULL,
    [VehicleId]                NVARCHAR (MAX)   NULL,
    [Mileage]                  INT              NULL,
    [LicensePlate]             NVARCHAR (MAX)   NULL,
    [VIN]                      NVARCHAR (MAX)   NULL,
    [Year]                     INT              NULL,
    [Make]                     NVARCHAR (MAX)   NULL,
    [Model]                    NVARCHAR (MAX)   NULL,
    [Notes]                    NVARCHAR (MAX)   NULL,
    [LastCompassDirection]     INT              NULL,
    [LastLongitude]            FLOAT (53)       NULL,
    [LastLatitude]             FLOAT (53)       NULL,
    [LastTimeStamp]            DATETIME         NULL,
    [LastSpeed]                FLOAT (53)       NULL,
    [LastSource]               NVARCHAR (MAX)   NULL,
    [LastPushToAzureTimeStamp] DATETIME         NULL,
    [LastAccuracy]             INT              NULL,
    [BusinessAccountId]        UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Vehicles] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_BusinessAccountVehicle] FOREIGN KEY ([BusinessAccountId]) REFERENCES [dbo].[Parties_BusinessAccount] ([Id])
);










GO
CREATE NONCLUSTERED INDEX [IX_FK_BusinessAccountVehicle]
    ON [dbo].[Vehicles]([BusinessAccountId] ASC);

