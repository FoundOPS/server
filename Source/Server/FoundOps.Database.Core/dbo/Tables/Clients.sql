CREATE TABLE [dbo].[Clients] (
    [Id]            UNIQUEIDENTIFIER NOT NULL,
    [DateAdded]     DATETIME         NOT NULL,
    [Salesperson]   NVARCHAR (MAX)   NULL,
    [LinkedPartyId] UNIQUEIDENTIFIER NULL,
    [VendorId]      UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Clients] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_BusinessAccountClient] FOREIGN KEY ([VendorId]) REFERENCES [dbo].[Parties_BusinessAccount] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_ClientParty] FOREIGN KEY ([Id]) REFERENCES [dbo].[Parties] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_ClientParty1] FOREIGN KEY ([LinkedPartyId]) REFERENCES [dbo].[Parties] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
);


GO
CREATE NONCLUSTERED INDEX [IX_FK_ClientParty1]
    ON [dbo].[Clients]([LinkedPartyId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FK_BusinessAccountClient]
    ON [dbo].[Clients]([VendorId] ASC);

