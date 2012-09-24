CREATE TABLE [dbo].[Locations] (
    [Id]                       UNIQUEIDENTIFIER NOT NULL,
    [Name]                     NVARCHAR (MAX)   NULL,
    [AddressLineOne]           NVARCHAR (MAX)   NULL,
    [Longitude]                DECIMAL (11, 8)  NULL,
    [ZipCode]                  NVARCHAR (MAX)   NULL,
    [AddressLineTwo]           NVARCHAR (MAX)   NULL,
    [State]                    NVARCHAR (MAX)   NULL,
    [Latitude]                 DECIMAL (11, 8)  NULL,
    [City]                     NVARCHAR (MAX)   NULL,
    [RegionId]                 UNIQUEIDENTIFIER NULL,
    [BusinessAccountIdIfDepot] UNIQUEIDENTIFIER NULL,
    [BusinessAccountId]        UNIQUEIDENTIFIER NULL,
    [ClientId]                 UNIQUEIDENTIFIER NULL,
    [IsDefaultBillingLocation] BIT              NULL,
    CONSTRAINT [PK_Locations] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_BusinessAccountLocation] FOREIGN KEY ([BusinessAccountIdIfDepot]) REFERENCES [dbo].[Parties_BusinessAccount] ([Id]),
    CONSTRAINT [FK_ClientLocation1] FOREIGN KEY ([ClientId]) REFERENCES [dbo].[Clients] ([Id]),
    CONSTRAINT [FK_LocationBusinessAccount] FOREIGN KEY ([BusinessAccountId]) REFERENCES [dbo].[Parties_BusinessAccount] ([Id]),
    CONSTRAINT [FK_RegionLocation] FOREIGN KEY ([RegionId]) REFERENCES [dbo].[Regions] ([Id])
);










GO
CREATE NONCLUSTERED INDEX [IX_FK_RegionLocation]
    ON [dbo].[Locations]([RegionId] ASC);


GO



GO



GO
CREATE NONCLUSTERED INDEX [IX_FK_BusinessAccountLocation]
    ON [dbo].[Locations]([BusinessAccountIdIfDepot] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FK_LocationBusinessAccount]
    ON [dbo].[Locations]([BusinessAccountId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FK_ClientLocation1]
    ON [dbo].[Locations]([ClientId] ASC);

