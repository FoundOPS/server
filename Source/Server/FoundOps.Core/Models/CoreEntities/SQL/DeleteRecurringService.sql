USE Core
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF OBJECT_ID(N'[dbo].[DeleteRecurringService]', N'FN') IS NOT NULL
DROP PROCEDURE [dbo].[DeleteRecurringService]
GO
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