CREATE TABLE [dbo].[Parties_Business] (
    [Name] NVARCHAR (MAX)   NULL,
    [Id]   UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Parties_Business] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Business_inherits_Party] FOREIGN KEY ([Id]) REFERENCES [dbo].[Parties] ([Id]) ON DELETE CASCADE
);



