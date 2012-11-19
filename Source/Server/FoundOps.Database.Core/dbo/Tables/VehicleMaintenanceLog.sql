CREATE TABLE [dbo].[VehicleMaintenanceLog] (
    [Id]                  UNIQUEIDENTIFIER NOT NULL,
    [Date]                DATETIME         NULL,
    [Mileage]             INT              NULL,
    [ServicedBy]          NVARCHAR (MAX)   NULL,
    [Comments]            NVARCHAR (MAX)   NULL,
    [VehicleId]           UNIQUEIDENTIFIER NOT NULL,
    [CreatedDate]         DATETIME         NOT NULL,
    [LastModified]        DATETIME         NULL,
    [LastModifyingUserId] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_VehicleMaintenanceLog] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_VehicleMaintenanceLogEntryVehicle] FOREIGN KEY ([VehicleId]) REFERENCES [dbo].[Vehicles] ([Id]) ON DELETE CASCADE
);






GO
CREATE NONCLUSTERED INDEX [IX_FK_VehicleMaintenanceLogEntryVehicle]
    ON [dbo].[VehicleMaintenanceLog]([VehicleId] ASC);

