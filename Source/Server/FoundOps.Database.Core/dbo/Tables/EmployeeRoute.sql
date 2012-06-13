CREATE TABLE [dbo].[EmployeeRoute] (
    [Employees_Id] UNIQUEIDENTIFIER NOT NULL,
    [Routes_Id]    UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_EmployeeRoute] PRIMARY KEY CLUSTERED ([Employees_Id] ASC, [Routes_Id] ASC),
    CONSTRAINT [FK_EmployeeRoute_Employee] FOREIGN KEY ([Employees_Id]) REFERENCES [dbo].[Employees] ([Id]),
    CONSTRAINT [FK_EmployeeRoute_Route] FOREIGN KEY ([Routes_Id]) REFERENCES [dbo].[Routes] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_FK_EmployeeRoute_Route]
    ON [dbo].[EmployeeRoute]([Routes_Id] ASC);

