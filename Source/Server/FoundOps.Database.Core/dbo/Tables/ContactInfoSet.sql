CREATE TABLE [dbo].[ContactInfoSet] (
    [Id]         UNIQUEIDENTIFIER NOT NULL,
    [Type]       NVARCHAR (MAX)   NOT NULL,
    [Label]      NVARCHAR (MAX)   NULL,
    [Data]       NVARCHAR (MAX)   NULL,
    [PartyId]    UNIQUEIDENTIFIER NULL,
    [LocationId] UNIQUEIDENTIFIER NULL,
    [ContactId]  UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_ContactInfoSet] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ContactInfoParty] FOREIGN KEY ([PartyId]) REFERENCES [dbo].[Parties] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_LocationContactInfo] FOREIGN KEY ([LocationId]) REFERENCES [dbo].[Locations] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
);




GO
CREATE NONCLUSTERED INDEX [IX_FK_ContactInfoParty]
    ON [dbo].[ContactInfoSet]([PartyId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FK_LocationContactInfo]
    ON [dbo].[ContactInfoSet]([LocationId] ASC);

