CREATE TABLE [dbo].[RoleBlock] (
    [Roles_Id]  UNIQUEIDENTIFIER NOT NULL,
    [Blocks_Id] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_RoleBlock] PRIMARY KEY CLUSTERED ([Roles_Id] ASC, [Blocks_Id] ASC),
    CONSTRAINT [FK_RoleBlock_Block] FOREIGN KEY ([Blocks_Id]) REFERENCES [dbo].[Blocks] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_RoleBlock_Role] FOREIGN KEY ([Roles_Id]) REFERENCES [dbo].[Roles] ([Id]) ON DELETE CASCADE
);




GO
CREATE NONCLUSTERED INDEX [IX_FK_RoleBlock_Block]
    ON [dbo].[RoleBlock]([Blocks_Id] ASC);

