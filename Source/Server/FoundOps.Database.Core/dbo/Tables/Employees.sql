CREATE TABLE [dbo].[Employees] (
    [Id]                   UNIQUEIDENTIFIER NOT NULL,
    [AddressLineOne]       NVARCHAR (MAX)   NULL,
    [AddressLineTwo]       NVARCHAR (MAX)   NULL,
    [City]                 NVARCHAR (MAX)   NULL,
    [Comments]             NVARCHAR (MAX)   NULL,
    [State]                NVARCHAR (MAX)   NULL,
    [ZipCode]              NVARCHAR (MAX)   NULL,
    [Permissions]          NVARCHAR (MAX)   NULL,
    [HireDate]             DATETIME         NULL,
    [SSN]                  NVARCHAR (MAX)   NULL,
    [LinkedUserAccountId]  UNIQUEIDENTIFIER NULL,
    [EmployerId]           UNIQUEIDENTIFIER NOT NULL,
    [LastCompassDirection] INT              NULL,
    [LastLongitude]        FLOAT (53)       NULL,
    [LastLatitude]         FLOAT (53)       NULL,
    [LastTimeStamp]        DATETIME         NULL,
    [LastSpeed]            FLOAT (53)       NULL,
    [LastSource]           NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_Employees] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EmployeeBusinessAccount] FOREIGN KEY ([EmployerId]) REFERENCES [dbo].[Parties_BusinessAccount] ([Id]),
    CONSTRAINT [FK_EmployeePerson] FOREIGN KEY ([Id]) REFERENCES [dbo].[Parties_Person] ([Id]),
    CONSTRAINT [FK_EmployeeUserAccount] FOREIGN KEY ([LinkedUserAccountId]) REFERENCES [dbo].[Parties_UserAccount] ([Id]) ON DELETE SET NULL
);






GO
CREATE NONCLUSTERED INDEX [IX_FK_EmployeeBusinessAccount]
    ON [dbo].[Employees]([EmployerId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FK_EmployeeUserAccount]
    ON [dbo].[Employees]([LinkedUserAccountId] ASC);

