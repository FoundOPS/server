CREATE TABLE [dbo].[RecurringServices] (
    [Id]                  UNIQUEIDENTIFIER NOT NULL,
    [ClientId]            UNIQUEIDENTIFIER NOT NULL,
    [ExcludedDatesString] NVARCHAR (MAX)   NULL,
    [DateDeleted]         DATETIME         NULL,
    CONSTRAINT [PK_RecurringServices] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_RecurringServiceClient] FOREIGN KEY ([ClientId]) REFERENCES [dbo].[Clients] ([Id]),
    CONSTRAINT [FK_RecurringServiceRepeat] FOREIGN KEY ([Id]) REFERENCES [dbo].[Repeats] ([Id]),
    CONSTRAINT [FK_RecurringServiceServiceTemplate] FOREIGN KEY ([Id]) REFERENCES [dbo].[ServiceTemplates] ([Id]) ON DELETE CASCADE
);






GO
CREATE NONCLUSTERED INDEX [IX_FK_RecurringServiceClient]
    ON [dbo].[RecurringServices]([ClientId] ASC);

