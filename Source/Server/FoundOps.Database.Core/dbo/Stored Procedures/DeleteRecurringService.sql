--This procedure deletes a Business Account
CREATE PROCEDURE [dbo].[DeleteRecurringService]
		(@recurringServiceId uniqueidentifier)

	AS
	BEGIN

	DELETE 
	FROM	Services 
	WHERE	RecurringServiceId = @recurringServiceId

	DECLARE @serviceTemplateId uniqueidentifier

	SET		@serviceTemplateId = (SELECT Id FROM ServiceTemplates WHERE Id = @recurringServiceId)

	EXEC	[dbo].[DeleteServiceTemplateAndChildrenBasedOnServiceTemplateId]	@parentTemplateId = @serviceTemplateId

	DELETE
	FROM	Repeats 
	WHERE	Id = @recurringServiceId

	DELETE 
	FROM	RecurringServices 
	WHERE	Id = @recurringServiceId

	END
	RETURN