CREATE TABLE [dbo].[LineItems] (
    [Id]          UNIQUEIDENTIFIER NOT NULL,
    [InvoiceId]   UNIQUEIDENTIFIER NOT NULL,
    [Description] NVARCHAR (MAX)   NULL,
    [Amount]      NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_LineItems] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_LineItemInvoice] FOREIGN KEY ([InvoiceId]) REFERENCES [dbo].[Invoices] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
);


GO
CREATE NONCLUSTERED INDEX [IX_FK_LineItemInvoice]
    ON [dbo].[LineItems]([InvoiceId] ASC);

