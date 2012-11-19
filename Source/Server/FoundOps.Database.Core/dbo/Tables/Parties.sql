CREATE TABLE [dbo].[Parties] (
    [Id]                  UNIQUEIDENTIFIER NOT NULL,
    [CreatedDate]         DATETIME         NOT NULL,
    [LastModified]        DATETIME         NULL,
    [LastModifyingUserId] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Parties] PRIMARY KEY CLUSTERED ([Id] ASC)
);



