CREATE TABLE [dbo].[Errors] (
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [Date]           DATETIME         NULL,
    [BusinessName]   NVARCHAR (MAX)   NULL,
    [UserEmail]      NVARCHAR (MAX)   NULL,
    [ErrorText]      NVARCHAR (MAX)   NULL,
    [InnerException] NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_Errors] PRIMARY KEY CLUSTERED ([Id] ASC)
);

