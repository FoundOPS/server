CREATE TABLE [dbo].[Clients] (
    [Id]                       UNIQUEIDENTIFIER NOT NULL,
    [DateAdded]                DATETIME         NOT NULL,
    [Salesperson]              NVARCHAR (MAX)   NULL,
    [VendorId]                 UNIQUEIDENTIFIER NOT NULL,
    [DefaultBillingLocationId] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Clients] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_BusinessAccountClient] FOREIGN KEY ([VendorId]) REFERENCES [dbo].[Parties_BusinessAccount] ([Id]),
    CONSTRAINT [FK_ClientLocation] FOREIGN KEY ([DefaultBillingLocationId]) REFERENCES [dbo].[Locations] ([Id]) ON DELETE SET NULL,
    CONSTRAINT [FK_ClientParty] FOREIGN KEY ([Id]) REFERENCES [dbo].[Parties] ([Id])
);








GO
CREATE NONCLUSTERED INDEX [IX_FK_BusinessAccountClient]
    ON [dbo].[Clients]([VendorId] ASC);


GO


