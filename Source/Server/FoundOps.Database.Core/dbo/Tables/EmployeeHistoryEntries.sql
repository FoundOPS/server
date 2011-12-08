CREATE TABLE [dbo].[EmployeeHistoryEntries] (
    [Id]         UNIQUEIDENTIFIER NOT NULL,
    [Date]       DATETIME         NULL,
    [Type]       NVARCHAR (MAX)   NULL,
    [Summary]    NVARCHAR (MAX)   NULL,
    [Notes]      NVARCHAR (MAX)   NULL,
    [EmployeeId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_EmployeeHistoryEntries] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EmployeeHistoryEntryEmployee] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employees] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
);


GO
CREATE NONCLUSTERED INDEX [IX_FK_EmployeeHistoryEntryEmployee]
    ON [dbo].[EmployeeHistoryEntries]([EmployeeId] ASC);

