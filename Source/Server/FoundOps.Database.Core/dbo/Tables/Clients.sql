CREATE TABLE [dbo].[Clients] (
    [Id]                UNIQUEIDENTIFIER NOT NULL,
    [DateAdded]         DATETIME         NOT NULL,
    [Salesperson]       NVARCHAR (MAX)   NULL,
    [BusinessAccountId] UNIQUEIDENTIFIER NULL,
    [Name]              NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_Clients] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ClientBusinessAccount] FOREIGN KEY ([BusinessAccountId]) REFERENCES [dbo].[Parties_BusinessAccount] ([Id])
);










GO



GO
CREATE NONCLUSTERED INDEX [IX_FK_ClientBusinessAccount]
    ON [dbo].[Clients]([BusinessAccountId] ASC);

