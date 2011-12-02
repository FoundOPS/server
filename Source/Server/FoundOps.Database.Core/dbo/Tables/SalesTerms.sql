CREATE TABLE [dbo].[SalesTerms] (
    [Id]      UNIQUEIDENTIFIER NOT NULL,
    [DueDays] INT              NULL,
    [Name]    NVARCHAR (MAX)   NOT NULL,
    CONSTRAINT [PK_SalesTerms] PRIMARY KEY CLUSTERED ([Id] ASC)
);

