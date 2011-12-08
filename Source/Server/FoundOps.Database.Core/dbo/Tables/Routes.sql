CREATE TABLE [dbo].[Routes] (
    [Id]                     UNIQUEIDENTIFIER NOT NULL,
    [Name]                   NVARCHAR (MAX)   NULL,
    [Date]                   DATETIME         NOT NULL,
    [StartTime]              DATETIME         NOT NULL,
    [EndTime]                DATETIME         NOT NULL,
    [OwnerBusinessAccountId] UNIQUEIDENTIFIER NOT NULL,
    [RouteType]              NVARCHAR (MAX)   NOT NULL,
    CONSTRAINT [PK_Routes] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_BusinessAccountRoute] FOREIGN KEY ([OwnerBusinessAccountId]) REFERENCES [dbo].[Parties_BusinessAccount] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
);




GO
CREATE NONCLUSTERED INDEX [IX_FK_BusinessAccountRoute]
    ON [dbo].[Routes]([OwnerBusinessAccountId] ASC);

