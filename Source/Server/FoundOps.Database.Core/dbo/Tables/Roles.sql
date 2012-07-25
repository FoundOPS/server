CREATE TABLE [dbo].[Roles] (
    [Id]                     UNIQUEIDENTIFIER NOT NULL,
    [Name]                   NVARCHAR (MAX)   NULL,
    [Description]            NVARCHAR (MAX)   NULL,
    [RoleTypeInt]            SMALLINT         NOT NULL,
    [OwnerBusinessAccountId] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_RoleBusinessAccount] FOREIGN KEY ([OwnerBusinessAccountId]) REFERENCES [dbo].[Parties_BusinessAccount] ([Id])
);






GO
CREATE NONCLUSTERED INDEX [IX_FK_RoleBusinessAccount]
    ON [dbo].[Roles]([OwnerBusinessAccountId] ASC);

