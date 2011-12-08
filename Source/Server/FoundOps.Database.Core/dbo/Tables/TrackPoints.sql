CREATE TABLE [dbo].[TrackPoints] (
    [Id]               BIGINT           NOT NULL,
    [Timestamp]        DATETIME         NULL,
    [CompassDirection] INT              NOT NULL,
    [Latitude]         FLOAT (53)       NOT NULL,
    [Longitude]        FLOAT (53)       NOT NULL,
    [UserAccountId]    UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_TrackPoints] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserAccountTrackPoint] FOREIGN KEY ([UserAccountId]) REFERENCES [dbo].[Parties_UserAccount] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
);


GO
CREATE NONCLUSTERED INDEX [IX_FK_UserAccountTrackPoint]
    ON [dbo].[TrackPoints]([UserAccountId] ASC);

