CREATE TABLE [dbo].[Services] (
    [Id]                 UNIQUEIDENTIFIER NOT NULL,
    [ClientId]           UNIQUEIDENTIFIER NOT NULL,
    [ServiceProviderId]  UNIQUEIDENTIFIER NOT NULL,
    [RecurringServiceId] UNIQUEIDENTIFIER NULL,
    [ServiceDate]        DATETIME         NOT NULL,
    CONSTRAINT [PK_Services] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_BusinessAccountService] FOREIGN KEY ([ServiceProviderId]) REFERENCES [dbo].[Parties_BusinessAccount] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_RecurringServiceService] FOREIGN KEY ([RecurringServiceId]) REFERENCES [dbo].[RecurringServices] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_ServiceClient] FOREIGN KEY ([ClientId]) REFERENCES [dbo].[Clients] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_ServiceServiceTemplate] FOREIGN KEY ([Id]) REFERENCES [dbo].[ServiceTemplates] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
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

