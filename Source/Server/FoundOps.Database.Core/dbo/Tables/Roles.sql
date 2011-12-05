CREATE TABLE [dbo].[Roles] (
    [Id]           UNIQUEIDENTIFIER NOT NULL,
    [Name]         NVARCHAR (MAX)   NULL,
    [Description]  NVARCHAR (MAX)   NULL,
    [OwnerPartyId] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PartyRole1] FOREIGN KEY ([OwnerPartyId]) REFERENCES [dbo].[Parties] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
);


GO
CREATE NONCLUSTERED INDEX [IX_FK_PartyRole1]
    ON [dbo].[Roles]([OwnerPartyId] ASC);

