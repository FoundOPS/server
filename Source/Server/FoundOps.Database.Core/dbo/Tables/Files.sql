CREATE TABLE [dbo].[Files] (
    [Id]         UNIQUEIDENTIFIER NOT NULL,
    [Data]       TINYINT          NOT NULL,
    [Name]       NVARCHAR (MAX)   NULL,
    [LocationId] UNIQUEIDENTIFIER NULL,
    [PartyId]    UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Files] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_FileParty] FOREIGN KEY ([PartyId]) REFERENCES [dbo].[Parties] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_LocationFile] FOREIGN KEY ([LocationId]) REFERENCES [dbo].[Locations] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
);


GO
CREATE NONCLUSTERED INDEX [IX_FK_FileParty]
    ON [dbo].[Files]([PartyId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FK_LocationFile]
    ON [dbo].[Files]([LocationId] ASC);

