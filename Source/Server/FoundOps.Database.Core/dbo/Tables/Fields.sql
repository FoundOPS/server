CREATE TABLE [dbo].[Fields] (
    [Id]                UNIQUEIDENTIFIER NOT NULL,
    [Name]              NVARCHAR (MAX)   NOT NULL,
    [Group]             NVARCHAR (MAX)   NOT NULL,
    [Required]          BIT              NOT NULL,
    [Tooltip]           NVARCHAR (MAX)   NULL,
    [ParentFieldId]     UNIQUEIDENTIFIER NULL,
    [ServiceTemplateId] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Fields] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_FieldField] FOREIGN KEY ([ParentFieldId]) REFERENCES [dbo].[Fields] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_ServiceTemplateField] FOREIGN KEY ([ServiceTemplateId]) REFERENCES [dbo].[ServiceTemplates] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
);


GO
CREATE NONCLUSTERED INDEX [IX_FK_ServiceTemplateField]
    ON [dbo].[Fields]([ServiceTemplateId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FK_FieldField]
    ON [dbo].[Fields]([ParentFieldId] ASC);

