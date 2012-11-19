CREATE TABLE [dbo].[ServiceTemplates] (
    [Id]                     UNIQUEIDENTIFIER NOT NULL,
    [OwnerServiceProviderId] UNIQUEIDENTIFIER NULL,
    [OwnerClientId]          UNIQUEIDENTIFIER NULL,
    [OwnerServiceTemplateId] UNIQUEIDENTIFIER NULL,
    [LevelInt]               SMALLINT         NOT NULL,
    [Name]                   NVARCHAR (MAX)   NULL,
    [CreatedDate]            DATETIME         NOT NULL,
    [LastModified]           DATETIME         NULL,
    [LastModifyingUserId]    UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_ServiceTemplates] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ClientServiceTemplate] FOREIGN KEY ([OwnerClientId]) REFERENCES [dbo].[Clients] ([Id]),
    CONSTRAINT [FK_ServiceTemplateBusinessAccount] FOREIGN KEY ([OwnerServiceProviderId]) REFERENCES [dbo].[Parties_BusinessAccount] ([Id]),
    CONSTRAINT [FK_ServiceTemplateServiceTemplate] FOREIGN KEY ([OwnerServiceTemplateId]) REFERENCES [dbo].[ServiceTemplates] ([Id])
);




GO
CREATE NONCLUSTERED INDEX [IX_FK_ServiceTemplateServiceTemplate]
    ON [dbo].[ServiceTemplates]([OwnerServiceTemplateId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FK_ClientServiceTemplate]
    ON [dbo].[ServiceTemplates]([OwnerClientId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FK_ServiceTemplateBusinessAccount]
    ON [dbo].[ServiceTemplates]([OwnerServiceProviderId] ASC);

