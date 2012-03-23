IF OBJECT_ID(N'[dbo].[DeleteServiceTemplatesAndChildrenBasedOnContextId]', N'FN') IS NOT NULL
DROP PROCEDURE [dbo].[DeleteServiceTemplatesAndChildrenBasedOnContextId]
GO
--This procedure deletes all service templates and child service templates
CREATE PROCEDURE dbo.DeleteServiceTemplatesAndChildrenBasedOnContextId
		(@serviceProviderId uniqueidentifier,
		@ownerClientId uniqueidentifier)
	AS
	BEGIN

	CREATE TABLE #TempTable
	(
		Id uniqueidentifier
	);
	
	IF @serviceProviderId IS NOT NULL
	BEGIN
	--This is the CTE for SetviceTemplates and their Children for the specified businessaccount (service provider)
	WITH		ServiceTemplateRecurs as
	(
    SELECT		ServiceTemplates.Id, ServiceTemplates.OwnerServiceTemplateId
    FROM		ServiceTemplates
    WHERE		ServiceTemplates.OwnerServiceProviderId = @serviceProviderId
    UNION		ALL
    SELECT		ServiceTemplates.Id, ServiceTemplates.OwnerServiceTemplateId
    FROM		ServiceTemplates
    JOIN		ServiceTemplateRecurs
    ON			ServiceTemplateRecurs.id = ServiceTemplates.OwnerServiceTemplateId
	)
	
	INSERT INTO #TempTable (Id)
	SELECT	ServiceTemplateRecurs.Id
	FROM	ServiceTemplateRecurs
	END

	ELSE IF @ownerClientId IS NOT NULL
	BEGIN
	WITH		ServiceTemplateRecurs as
	(
    SELECT		ServiceTemplates.Id, ServiceTemplates.OwnerServiceTemplateId
    FROM		ServiceTemplates
    WHERE		ServiceTemplates.OwnerClientId = @ownerClientId
    UNION		ALL
    SELECT		ServiceTemplates.Id, ServiceTemplates.OwnerServiceTemplateId
    FROM		ServiceTemplates
    JOIN		ServiceTemplateRecurs
    ON			ServiceTemplateRecurs.id = ServiceTemplates.OwnerServiceTemplateId
	)
	INSERT INTO #TempTable (Id)
	SELECT	ServiceTemplateRecurs.Id
	FROM	ServiceTemplateRecurs
	END


	DELETE 
	FROM		Services
	--This is a Semi-Join between the ServiceTemplateRecurs table created above and the Services Table
	--Semi-Join simply means that it has all the same logic as a normal join, but it doesnt actually join the tables
	--In this case, it finds all the rows on Services that correspond to a row in ServiceTemplateRecurs
	WHERE		EXISTS
	(
	SELECT		#TempTable.Id
	FROM		#TempTable
	WHERE		#TempTable.Id = Services.Id
	)

	DELETE 
	FROM		RecurringServices
	--This is a Semi-Join between the ServiceTemplateRecurs table created above and the RecurringServices Table
	--Semi-Join simply means that it has all the same logic as a normal join, but it doesnt actually join the tables
	--In this case, it finds all the rows on RecurringServices that correspond to a row in ServiceTemplateRecurs
	WHERE		EXISTS
	(
	SELECT		#TempTable.Id
	FROM		#TempTable
	WHERE		#TempTable.Id = RecurringServices.Id
	)

	DELETE 
	FROM		ServiceTemplates
	--This is a Semi-Join between the ServiceTemplateRecurs table created above and the ServiceTemplates Table
	--Semi-Join simply means that it has all the same logic as a normal join, but it doesnt actually join the tables
	--In this case, it finds all the rows on ServiceTemplates that correspond to a row in ServiceTemplateRecurs
	WHERE		EXISTS
	(
	SELECT		#TempTable.Id
	FROM		#TempTable
	WHERE		#TempTable.Id = ServiceTemplates.Id
	OR			#TempTable.Id = ServiceTemplates.OwnerServiceTemplateId
	)
	
	--Drop the table that was created to store the CTE
	DROP TABLE #TempTable
	

	END
	RETURN