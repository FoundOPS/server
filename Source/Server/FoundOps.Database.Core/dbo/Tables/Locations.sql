CREATE TABLE [dbo].[Locations] (
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [OwnerPartyId]   UNIQUEIDENTIFIER NULL,
    [Name]           NVARCHAR (MAX)   NULL,
    [AddressLineOne] NVARCHAR (MAX)   NULL,
    [Longitude]      DECIMAL (11, 8)  NULL,
    [ZipCode]        NVARCHAR (MAX)   NULL,
    [AddressLineTwo] NVARCHAR (MAX)   NULL,
    [State]          NVARCHAR (MAX)   NULL,
    [Latitude]       DECIMAL (11, 8)  NULL,
    [City]           NVARCHAR (MAX)   NULL,
    [PartyId]        UNIQUEIDENTIFIER NULL,
    [RegionId]       UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Locations] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_LocationParty] FOREIGN KEY ([OwnerPartyId]) REFERENCES [dbo].[Parties] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_LocationParty1] FOREIGN KEY ([PartyId]) REFERENCES [dbo].[Parties] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_RegionLocation] FOREIGN KEY ([RegionId]) REFERENCES [dbo].[Regions] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
);


GO
CREATE NONCLUSTERED INDEX [IX_FK_RegionLocation]
    ON [dbo].[Locations]([RegionId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FK_LocationParty1]
    ON [dbo].[Locations]([PartyId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FK_LocationParty]
    ON [dbo].[Locations]([OwnerPartyId] ASC);

