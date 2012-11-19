CREATE TABLE [dbo].[Regions] (
    [Id]                  UNIQUEIDENTIFIER NOT NULL,
    [Name]                NVARCHAR (MAX)   NOT NULL,
    [BusinessAccountId]   UNIQUEIDENTIFIER NULL,
    [Color]               NVARCHAR (MAX)   NULL,
    [Notes]               NVARCHAR (MAX)   NULL,
    [CreatedDate]         DATETIME         NOT NULL,
    [LastModified]        DATETIME         NULL,
    [LastModifyingUserId] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Regions] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_BusinessAccountRegion] FOREIGN KEY ([BusinessAccountId]) REFERENCES [dbo].[Parties_BusinessAccount] ([Id])
);




GO
CREATE NONCLUSTERED INDEX [IX_FK_BusinessAccountRegion]
    ON [dbo].[Regions]([BusinessAccountId] ASC);

