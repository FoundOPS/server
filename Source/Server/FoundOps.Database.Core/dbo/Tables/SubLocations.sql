CREATE TABLE [dbo].[SubLocations] (
    [Id]         UNIQUEIDENTIFIER NOT NULL,
    [Name]       NVARCHAR (MAX)   NOT NULL,
    [Longitude]  DECIMAL (11, 8)  NULL,
    [Latitude]   DECIMAL (11, 8)  NULL,
    [Notes]      NVARCHAR (MAX)   NULL,
    [LocationId] UNIQUEIDENTIFIER NULL,
    [Number]     INT              NOT NULL,
    CONSTRAINT [PK_SubLocations] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_LocationSubLocation] FOREIGN KEY ([LocationId]) REFERENCES [dbo].[Locations] ([Id]) ON DELETE CASCADE
);




GO
CREATE NONCLUSTERED INDEX [IX_FK_LocationSubLocation]
    ON [dbo].[SubLocations]([LocationId] ASC);

