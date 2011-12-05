CREATE TABLE [dbo].[Parties_UserAccount] (
    [PasswordHash] NVARCHAR (MAX)   NULL,
    [EmailAddress] NVARCHAR (MAX)   NOT NULL,
    [LastActivity] DATETIME         NULL,
    [CreationDate] DATETIME         NOT NULL,
    [Id]           UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Parties_UserAccount] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserAccount_inherits_Person] FOREIGN KEY ([Id]) REFERENCES [dbo].[Parties_Person] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
);

