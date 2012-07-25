CREATE TABLE [dbo].[Parties_BusinessAccount] (
    [QuickBooksEnabled]           BIT              NOT NULL,
    [QuickBooksAccessToken]       NVARCHAR (MAX)   NULL,
    [QuickBooksAccessTokenSecret] NVARCHAR (MAX)   NULL,
    [RouteManifestSettings]       NVARCHAR (MAX)   NULL,
    [QuickBooksSessionXml]        NVARCHAR (MAX)   NULL,
    [MaxRoutes]                   INT              NOT NULL,
    [Name]                        NVARCHAR (MAX)   NULL,
    [Id]                          UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Parties_BusinessAccount] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_BusinessAccount_inherits_Party] FOREIGN KEY ([Id]) REFERENCES [dbo].[Parties] ([Id]) ON DELETE CASCADE
);







