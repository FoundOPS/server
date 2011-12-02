CREATE TABLE [dbo].[Parties_BusinessAccount] (
    [QuickBooksEnabled]           BIT              NOT NULL,
    [QuickBooksAccessToken]       NVARCHAR (MAX)   NULL,
    [QuickBooksAccessTokenSecret] NVARCHAR (MAX)   NULL,
    [RouteManifestSettings]       NVARCHAR (MAX)   NULL,
    [QuickBooksSessionXml]        NVARCHAR (MAX)   NULL,
    [Id]                          UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Parties_BusinessAccount] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_BusinessAccount_inherits_Business] FOREIGN KEY ([Id]) REFERENCES [dbo].[Parties_Business] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
);




