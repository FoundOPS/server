CREATE TABLE [dbo].[RouteEmployee] (
    [Routes_Id]      UNIQUEIDENTIFIER NOT NULL,
    [Technicians_Id] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_RouteEmployee] PRIMARY KEY CLUSTERED ([Routes_Id] ASC, [Technicians_Id] ASC),
    CONSTRAINT [FK_RouteEmployee_Employee] FOREIGN KEY ([Technicians_Id]) REFERENCES [dbo].[Employees] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_RouteEmployee_Route] FOREIGN KEY ([Routes_Id]) REFERENCES [dbo].[Routes] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
);


GO
CREATE NONCLUSTERED INDEX [IX_FK_RouteEmployee_Employee]
    ON [dbo].[RouteEmployee]([Technicians_Id] ASC);

