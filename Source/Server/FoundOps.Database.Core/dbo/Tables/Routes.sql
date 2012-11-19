CREATE TABLE [dbo].[Routes] (
    [Id]                     UNIQUEIDENTIFIER NOT NULL,
    [Name]                   NVARCHAR (MAX)   NULL,
    [Date]                   DATETIME         NOT NULL,
    [StartTime]              DATETIME         NOT NULL,
    [EndTime]                DATETIME         NOT NULL,
    [OwnerBusinessAccountId] UNIQUEIDENTIFIER NOT NULL,
    [RouteType]              NVARCHAR (MAX)   NOT NULL,
    [CreatedDate]            DATETIME         NOT NULL,
    [LastModified]           DATETIME         NULL,
    [LastModifyingUserId]    UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Routes] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_BusinessAccountRoute] FOREIGN KEY ([OwnerBusinessAccountId]) REFERENCES [dbo].[Parties_BusinessAccount] ([Id])
);




GO
CREATE NONCLUSTERED INDEX [IX_FK_BusinessAccountRoute]
    ON [dbo].[Routes]([OwnerBusinessAccountId] ASC);

