CREATE TABLE [dbo].[EmployeeHistoryEntries] (
[Id]         UNIQUEIDENTIFIER NOT NULL,
[Date]       DATETIME,
[Type]       NVARCHAR (MAX),
[Summary]    NVARCHAR (MAX),
[Notes]      NVARCHAR (MAX),
[EmployeeId] UNIQUEIDENTIFIER NOT NULL,
CONSTRAINT [PK_EmployeeHistoryEntries] PRIMARY KEY CLUSTERED ([Id] ASC),
CONSTRAINT [FK_EmployeeHistoryEntryEmployee] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employees] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
);


GO
CREATE NONCLUSTERED INDEX [IX_FK_EmployeeHistoryEntryEmployee]
ON [dbo].[EmployeeHistoryEntries]([EmployeeId] ASC);