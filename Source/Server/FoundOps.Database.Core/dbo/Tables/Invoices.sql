CREATE TABLE [dbo].[Invoices] (
    [Id]                     UNIQUEIDENTIFIER NOT NULL,
    [LocationId]             UNIQUEIDENTIFIER NULL,
    [FixedScheduleOptionInt] INT              NULL,
    [RelativeScheduleDays]   INT              NULL,
    [ScheduleModeInt]        INT              NOT NULL,
    [SalesTermId]            UNIQUEIDENTIFIER NULL,
    [Memo]                   NVARCHAR (MAX)   NULL,
    [DueDate]                DATETIME         NULL,
    [SyncToken]              NVARCHAR (MAX)   NULL,
    [CustomerId]             NVARCHAR (MAX)   NULL,
    [CreateTime]             NVARCHAR (MAX)   NULL,
    [LastUpdatedTime]        NVARCHAR (MAX)   NULL,
    [BusinessAccountId]      UNIQUEIDENTIFIER NULL,
    [ClientId]               UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Invoices] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_BusinessAccountInvoice] FOREIGN KEY ([BusinessAccountId]) REFERENCES [dbo].[Parties_BusinessAccount] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_ClientInvoice] FOREIGN KEY ([ClientId]) REFERENCES [dbo].[Clients] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_InvoiceLocation] FOREIGN KEY ([LocationId]) REFERENCES [dbo].[Locations] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_InvoiceSalesTerm] FOREIGN KEY ([SalesTermId]) REFERENCES [dbo].[SalesTerms] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_InvoiceServiceTemplate] FOREIGN KEY ([Id]) REFERENCES [dbo].[ServiceTemplates] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
);




GO
CREATE NONCLUSTERED INDEX [IX_FK_InvoiceSalesTerm]
    ON [dbo].[Invoices]([SalesTermId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FK_InvoiceLocation]
    ON [dbo].[Invoices]([LocationId] ASC);

