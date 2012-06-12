CREATE VIEW [dbo].[ServiceTemplatesWithVendorId]
as
SELECT ServiceTemplates.Id as 'ServiceTemplateId', Parties_BusinessAccount.Id as 'BusinessAccountId'
FROM ServiceTemplates, Parties_BusinessAccount
WHERE ServiceTemplates.OwnerServiceProviderId = Parties_BusinessAccount.Id
UNION ALL
SELECT ServiceTemplates.Id as 'ServiceTemplateId', Clients.BusinessAccountId as 'BusinessAccountId'
FROM ServiceTemplates, Clients
WHERE Clients.Id = ServiceTemplates.OwnerClientId
UNION ALL
SELECT ServiceTemplates.Id as 'ServiceTemplateId', Services.ServiceProviderId as 'BusinessAccountId'
FROM ServiceTemplates, Services
WHERE ServiceTemplates.Id = Services.Id
UNION ALL
SELECT ServiceTemplates.Id as 'ServiceTemplateId', Clients.BusinessAccountId as 'BusinessAccountId'
FROM ServiceTemplates, RecurringServices, Clients
WHERE ServiceTemplates.Id = RecurringServices.Id AND RecurringServices.ClientId = Clients.Id

GO