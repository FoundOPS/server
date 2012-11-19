CREATE TABLE [dbo].[ContactInfoSet] (
    [Id]                  UNIQUEIDENTIFIER NOT NULL,
    [Type]                NVARCHAR (MAX)   NOT NULL,
    [Label]               NVARCHAR (MAX)   NULL,
    [Data]                NVARCHAR (MAX)   NULL,
    [PartyId]             UNIQUEIDENTIFIER NULL,
    [LocationId]          UNIQUEIDENTIFIER NULL,
    [ClientId]            UNIQUEIDENTIFIER NULL,
    [CreatedDate]         DATETIME         NOT NULL,
    [LastModified]        DATETIME         NULL,
    [LastModifyingUserId] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_ContactInfoSet] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ClientContactInfo] FOREIGN KEY ([ClientId]) REFERENCES [dbo].[Clients] ([Id]),
    CONSTRAINT [FK_LocationContactInfo] FOREIGN KEY ([LocationId]) REFERENCES [dbo].[Locations] ([Id]) ON DELETE CASCADE
);








GO
CREATE NONCLUSTERED INDEX [IX_FK_LocationContactInfo]
    ON [dbo].[ContactInfoSet]([LocationId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FK_ClientContactInfo]
    ON [dbo].[ContactInfoSet]([ClientId] ASC);

