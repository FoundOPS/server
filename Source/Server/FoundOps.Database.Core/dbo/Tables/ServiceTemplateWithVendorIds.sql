CREATE TABLE [dbo].[ServiceTemplateWithVendorIds] (
    [ServiceTemplateId] UNIQUEIDENTIFIER NOT NULL,
    [BusinessAccountId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_ServiceTemplateWithVendorIds] PRIMARY KEY CLUSTERED ([ServiceTemplateId] ASC, [BusinessAccountId] ASC)
);

