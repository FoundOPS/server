CREATE TABLE [dbo].[EmployeeHistoryEntries] (
    [Id]                  UNIQUEIDENTIFIER NOT NULL,
    [Date]                DATETIME         NULL,
    [Type]                NVARCHAR (MAX)   NULL,
    [Summary]             NVARCHAR (MAX)   NULL,
    [Notes]               NVARCHAR (MAX)   NULL,
    [EmployeeId]          UNIQUEIDENTIFIER NOT NULL,
    [CreatedDate]         DATETIME         NOT NULL,
    [LastModified]        DATETIME         NULL,
    [LastModifyingUserId] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_EmployeeHistoryEntries] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EmployeeHistoryEntryEmployee] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employees] ([Id]) ON DELETE CASCADE
);






GO
CREATE NONCLUSTERED INDEX [IX_FK_EmployeeHistoryEntryEmployee]
    ON [dbo].[EmployeeHistoryEntries]([EmployeeId] ASC);

