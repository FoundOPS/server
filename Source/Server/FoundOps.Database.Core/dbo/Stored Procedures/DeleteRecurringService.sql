/****************************************************************************************************************************************************
* FUNCTION DeleteRecurringService will delete a RecurringService and all entities associated with it
* Follows the following progression to delete: Services, ServiceTemplates, Repeats and finally the RecurringService itself
** Input Parameters **
* @recurringServiceId - The RecurringService Id to be deleted
***************************************************************************************************************************************************/
CREATE PROCEDURE [dbo].[DeleteRecurringService]
		(@recurringServiceId uniqueidentifier)

	AS
	BEGIN

	DECLARE @serviceTemplateId uniqueidentifier

	--Find the ServiceTemplate that correspods to the Recurring Service  
	SET		@serviceTemplateId = (SELECT Id FROM ServiceTemplates WHERE Id = @recurringServiceId)

	--Delete the ServiceTemplate found above
	EXEC	[dbo].[DeleteServiceTemplateAndChildrenBasedOnServiceTemplateId]	@parentTemplateId = @serviceTemplateId

	DELETE
	FROM	Repeats 
	WHERE	Id = @recurringServiceId

	DELETE 
	FROM	RecurringServices 
	WHERE	Id = @recurringServiceId

	END
	RETURN