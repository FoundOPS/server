CREATE TABLE [dbo].[ClientTitles] (
    [Id]        UNIQUEIDENTIFIER NOT NULL,
    [Title]     NVARCHAR (MAX)   NOT NULL,
    [ClientId]  UNIQUEIDENTIFIER NULL,
    [ContactId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_ClientTitles] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ClientTitleClient] FOREIGN KEY ([ClientId]) REFERENCES [dbo].[Clients] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ClientTitleContact] FOREIGN KEY ([ContactId]) REFERENCES [dbo].[Contacts] ([Id])
);




GO
CREATE NONCLUSTERED INDEX [IX_FK_ClientTitleContact]
    ON [dbo].[ClientTitles]([ContactId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FK_ClientTitleClient]
    ON [dbo].[ClientTitles]([ClientId] ASC);

