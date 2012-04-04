CREATE TABLE [dbo].[PartyRole] (
    [MemberParties_Id]  UNIQUEIDENTIFIER NOT NULL,
    [RoleMembership_Id] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_PartyRole] PRIMARY KEY CLUSTERED ([MemberParties_Id] ASC, [RoleMembership_Id] ASC),
    CONSTRAINT [FK_PartyRole_Party] FOREIGN KEY ([MemberParties_Id]) REFERENCES [dbo].[Parties] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PartyRole_Role] FOREIGN KEY ([RoleMembership_Id]) REFERENCES [dbo].[Roles] ([Id]) ON DELETE CASCADE
);




GO
CREATE NONCLUSTERED INDEX [IX_FK_PartyRole_Role]
    ON [dbo].[PartyRole]([RoleMembership_Id] ASC);

