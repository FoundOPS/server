CREATE TABLE [dbo].[Positions] (
    [Id]            UNIQUEIDENTIFIER NOT NULL,
    [SSN]           NVARCHAR (MAX)   NULL,
    [HireDate]      DATETIME         NULL,
    [Comments]      NVARCHAR (MAX)   NULL,
    [UserAccountId] UNIQUEIDENTIFIER NOT NULL,
    [BusinessId]    UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Positions] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_BusinessPosition] FOREIGN KEY ([BusinessId]) REFERENCES [dbo].[Parties_Business] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_PositionUserAccount] FOREIGN KEY ([UserAccountId]) REFERENCES [dbo].[Parties_Person] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
);


GO
CREATE NONCLUSTERED INDEX [IX_FK_PositionUserAccount]
    ON [dbo].[Positions]([UserAccountId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FK_BusinessPosition]
    ON [dbo].[Positions]([BusinessId] ASC);

