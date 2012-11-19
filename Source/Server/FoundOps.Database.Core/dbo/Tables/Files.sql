CREATE TABLE [dbo].[Files] (
    [Id]                  UNIQUEIDENTIFIER NOT NULL,
    [Data]                TINYINT          NOT NULL,
    [Name]                NVARCHAR (MAX)   NULL,
    [LocationId]          UNIQUEIDENTIFIER NULL,
    [PartyId]             UNIQUEIDENTIFIER NOT NULL,
    [CreatedDate]         DATETIME         NOT NULL,
    [LastModified]        DATETIME         NULL,
    [LastModifyingUserId] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Files] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_FileParty] FOREIGN KEY ([PartyId]) REFERENCES [dbo].[Parties] ([Id]),
    CONSTRAINT [FK_LocationFile] FOREIGN KEY ([LocationId]) REFERENCES [dbo].[Locations] ([Id])
);




GO
CREATE NONCLUSTERED INDEX [IX_FK_FileParty]
    ON [dbo].[Files]([PartyId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FK_LocationFile]
    ON [dbo].[Files]([LocationId] ASC);

