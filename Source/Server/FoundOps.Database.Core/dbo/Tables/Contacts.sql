CREATE TABLE [dbo].[Contacts] (
    [Id]           UNIQUEIDENTIFIER NOT NULL,
    [Notes]        NVARCHAR (MAX)   NULL,
    [OwnerPartyId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Contacts] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ContactParty] FOREIGN KEY ([OwnerPartyId]) REFERENCES [dbo].[Parties] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_ContactPerson] FOREIGN KEY ([Id]) REFERENCES [dbo].[Parties_Person] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
);


GO
CREATE NONCLUSTERED INDEX [IX_FK_ContactParty]
    ON [dbo].[Contacts]([OwnerPartyId] ASC);

