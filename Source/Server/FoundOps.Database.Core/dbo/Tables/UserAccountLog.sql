CREATE TABLE [dbo].[UserAccountLog] (
    [Id]            UNIQUEIDENTIFIER NOT NULL,
    [TypeId]        INT              NULL,
    [TimeStamp]     DATETIME         NOT NULL,
    [UserAccountId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_UserAccountLog] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserAccountUserAccountLogEntry] FOREIGN KEY ([UserAccountId]) REFERENCES [dbo].[Parties_UserAccount] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
);


GO
CREATE NONCLUSTERED INDEX [IX_FK_UserAccountUserAccountLogEntry]
    ON [dbo].[UserAccountLog]([UserAccountId] ASC);

