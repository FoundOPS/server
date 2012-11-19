CREATE TABLE [dbo].[TaskStatuses] (
    [Id]                  UNIQUEIDENTIFIER NOT NULL,
    [Color]               NVARCHAR (MAX)   NULL,
    [Name]                NVARCHAR (MAX)   NULL,
    [DefaultTypeInt]      INT              NULL,
    [RemoveFromRoute]     BIT              NOT NULL,
    [BusinessAccountId]   UNIQUEIDENTIFIER NULL,
    [CreatedDate]         DATETIME         NOT NULL,
    [LastModified]        DATETIME         NULL,
    [LastModifyingUserId] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_TaskStatuses] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TaskStatusBusinessAccount] FOREIGN KEY ([BusinessAccountId]) REFERENCES [dbo].[Parties_BusinessAccount] ([Id])
);






GO
CREATE NONCLUSTERED INDEX [IX_FK_TaskStatusBusinessAccount]
    ON [dbo].[TaskStatuses]([BusinessAccountId] ASC);

