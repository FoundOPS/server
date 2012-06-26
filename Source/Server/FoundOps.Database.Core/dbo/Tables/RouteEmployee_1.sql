CREATE TABLE [dbo].[RouteEmployee] (
    [Routes_Id]    UNIQUEIDENTIFIER NOT NULL,
    [Employees_Id] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_RouteEmployee] PRIMARY KEY CLUSTERED ([Routes_Id] ASC, [Employees_Id] ASC),
    CONSTRAINT [FK_RouteEmployee_Employee] FOREIGN KEY ([Employees_Id]) REFERENCES [dbo].[Employees] ([Id]),
    CONSTRAINT [FK_RouteEmployee_Route] FOREIGN KEY ([Routes_Id]) REFERENCES [dbo].[Routes] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_FK_RouteEmployee_Employee]
    ON [dbo].[RouteEmployee]([Employees_Id] ASC);

