CREATE TABLE [dbo].[Parties_UserAccount] (
    [PasswordHash]         NVARCHAR (MAX)   NULL,
    [EmailAddress]         NVARCHAR (MAX)   NOT NULL,
    [LastActivity]         DATETIME         NULL,
    [CreationDate]         DATETIME         NOT NULL,
    [FirstName]            NVARCHAR (MAX)   NULL,
    [LastName]             NVARCHAR (MAX)   NULL,
    [MiddleInitial]        NVARCHAR (MAX)   NULL,
    [GenderInt]            SMALLINT         NULL,
    [DateOfBirth]          DATETIME         NULL,
    [TimeZone]             NVARCHAR (MAX)   NULL,
    [ColumnConfigurations] NVARCHAR (MAX)   NULL,
    [PasswordSalt]         VARBINARY (MAX)  NOT NULL,
    [TempResetToken]       NVARCHAR (MAX)   NULL,
    [TempTokenExpireTime]  DATETIME         NULL,
    [Id]                   UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Parties_UserAccount] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserAccount_inherits_Party] FOREIGN KEY ([Id]) REFERENCES [dbo].[Parties] ([Id]) ON DELETE CASCADE
);











