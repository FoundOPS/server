CREATE TABLE [dbo].[Parties_Person] (
    [FirstName]     NVARCHAR (MAX)   NULL,
    [LastName]      NVARCHAR (MAX)   NULL,
    [MiddleInitial] NVARCHAR (MAX)   NULL,
    [GenderInt]     SMALLINT         NULL,
    [DateOfBirth]   DATETIME         NULL,
    [Id]            UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Parties_Person] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Person_inherits_Party] FOREIGN KEY ([Id]) REFERENCES [dbo].[Parties] ([Id]) ON DELETE CASCADE
);



