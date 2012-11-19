CREATE TABLE [dbo].[Services] (
    [Id]                  UNIQUEIDENTIFIER NOT NULL,
    [ClientId]            UNIQUEIDENTIFIER NOT NULL,
    [ServiceProviderId]   UNIQUEIDENTIFIER NOT NULL,
    [RecurringServiceId]  UNIQUEIDENTIFIER NULL,
    [ServiceDate]         DATETIME         NOT NULL,
    [CreatedDate]         DATETIME         NOT NULL,
    [LastModified]        DATETIME         NULL,
    [LastModifyingUserId] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Services] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_BusinessAccountService] FOREIGN KEY ([ServiceProviderId]) REFERENCES [dbo].[Parties_BusinessAccount] ([Id]),
    CONSTRAINT [FK_RecurringServiceService] FOREIGN KEY ([RecurringServiceId]) REFERENCES [dbo].[RecurringServices] ([Id]),
    CONSTRAINT [FK_ServiceClient] FOREIGN KEY ([ClientId]) REFERENCES [dbo].[Clients] ([Id]),
    CONSTRAINT [FK_ServiceServiceTemplate] FOREIGN KEY ([Id]) REFERENCES [dbo].[ServiceTemplates] ([Id])
);




GO
CREATE NONCLUSTERED INDEX [IX_FK_RecurringServiceService]
    ON [dbo].[Services]([RecurringServiceId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FK_BusinessAccountService]
    ON [dbo].[Services]([ServiceProviderId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FK_ServiceClient]
    ON [dbo].[Services]([ClientId] ASC);

