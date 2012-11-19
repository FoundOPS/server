CREATE TABLE [dbo].[Employees] (
    [Id]                       UNIQUEIDENTIFIER NOT NULL,
    [FirstName]                NVARCHAR (MAX)   NULL,
    [LastName]                 NVARCHAR (MAX)   NULL,
    [MiddleInitial]            NVARCHAR (MAX)   NULL,
    [GenderInt]                SMALLINT         NULL,
    [DateOfBirth]              DATETIME         NULL,
    [AddressLineOne]           NVARCHAR (MAX)   NULL,
    [AddressLineTwo]           NVARCHAR (MAX)   NULL,
    [AdminDistrictTwo]         NVARCHAR (MAX)   NULL,
    [Comments]                 NVARCHAR (MAX)   NULL,
    [AdminDistrictOne]         NVARCHAR (MAX)   NULL,
    [PostalCode]               NVARCHAR (MAX)   NULL,
    [Permissions]              NVARCHAR (MAX)   NULL,
    [HireDate]                 DATETIME         NULL,
    [SSN]                      NVARCHAR (MAX)   NULL,
    [LinkedUserAccountId]      UNIQUEIDENTIFIER NULL,
    [EmployerId]               UNIQUEIDENTIFIER NOT NULL,
    [LastCompassDirection]     INT              NULL,
    [LastLongitude]            FLOAT (53)       NULL,
    [LastLatitude]             FLOAT (53)       NULL,
    [LastTimeStamp]            DATETIME         NULL,
    [LastSpeed]                FLOAT (53)       NULL,
    [LastSource]               NVARCHAR (MAX)   NULL,
    [LastPushToAzureTimeStamp] DATETIME         NULL,
    [LastAccuracy]             INT              NULL,
    [CountryCode]              NVARCHAR (MAX)   NULL,
    [CreatedDate]              DATETIME         NOT NULL,
    [LastModified]             DATETIME         NULL,
    [LastModifyingUserId]      UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Employees] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EmployeeBusinessAccount] FOREIGN KEY ([EmployerId]) REFERENCES [dbo].[Parties_BusinessAccount] ([Id]),
    CONSTRAINT [FK_EmployeeUserAccount] FOREIGN KEY ([LinkedUserAccountId]) REFERENCES [dbo].[Parties_UserAccount] ([Id]) ON DELETE SET NULL
);
















GO
CREATE NONCLUSTERED INDEX [IX_FK_EmployeeBusinessAccount]
    ON [dbo].[Employees]([EmployerId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FK_EmployeeUserAccount]
    ON [dbo].[Employees]([LinkedUserAccountId] ASC);

