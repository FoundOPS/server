CREATE TABLE [dbo].[VehicleMaintenanceLineItems] (
    [Id]                           UNIQUEIDENTIFIER NOT NULL,
    [Type]                         NVARCHAR (MAX)   NULL,
    [Cost]                         DECIMAL (12, 2)  NULL,
    [Details]                      NVARCHAR (MAX)   NULL,
    [VehicleMaintenanceLogEntryId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_VehicleMaintenanceLineItems] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_VehicleMaintenanceLogEntryLineItem] FOREIGN KEY ([VehicleMaintenanceLogEntryId]) REFERENCES [dbo].[VehicleMaintenanceLog] ([Id]) ON DELETE CASCADE
);




GO
CREATE NONCLUSTERED INDEX [IX_FK_VehicleMaintenanceLogEntryLineItem]
    ON [dbo].[VehicleMaintenanceLineItems]([VehicleMaintenanceLogEntryId] ASC);

