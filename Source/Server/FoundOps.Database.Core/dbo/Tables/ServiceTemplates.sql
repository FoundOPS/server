CREATE TABLE [dbo].[ServiceTemplates] (
    [Id]                     UNIQUEIDENTIFIER NOT NULL,
    [OwnerServiceProviderId] UNIQUEIDENTIFIER NULL,
    [OwnerClientId]          UNIQUEIDENTIFIER NULL,
    [OwnerServiceTemplateId] UNIQUEIDENTIFIER NULL,
    [LevelInt]               SMALLINT         NOT NULL,
    [Name]                   NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_ServiceTemplates] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ClientServiceTemplate] FOREIGN KEY ([OwnerClientId]) REFERENCES [dbo].[Clients] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_ServiceTemplateBusinessAccount] FOREIGN KEY ([OwnerServiceProviderId]) REFERENCES [dbo].[Parties_BusinessAccount] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_ServiceTemplateServiceTemplate] FOREIGN KEY ([OwnerServiceTemplateId]) REFERENCES [dbo].[ServiceTemplates] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
);


GO
CREATE NONCLUSTERED INDEX [IX_FK_ServiceTemplateBusinessAccount]
    ON [dbo].[ServiceTemplates]([OwnerServiceProviderId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FK_ClientServiceTemplate]
    ON [dbo].[ServiceTemplates]([OwnerClientId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FK_ServiceTemplateServiceTemplate]
    ON [dbo].[ServiceTemplates]([OwnerServiceTemplateId] ASC);

