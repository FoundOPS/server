--This procedure deletes all service templates and child service templates
CREATE PROCEDURE dbo.DeleteServiceTemplatesAndChildrenBasedOnBusinessAccountId
		(@providerId uniqueidentifier)
	AS
	BEGIN

	CREATE TABLE #TempTable
	(
		Id uniqueidentifier
	);
	
	--This is the CTE for SetviceTemplates and their Children for the specified businessaccount (service provider)
	WITH		ServiceTemplateRecurs as
	(
    SELECT		ServiceTemplates.Id, ServiceTemplates.OwnerServiceTemplateId
    FROM		ServiceTemplates
    WHERE		ServiceTemplates.OwnerServiceProviderId = @providerId
    UNION		ALL
    SELECT		ServiceTemplates.Id, ServiceTemplates.OwnerServiceTemplateId
    FROM		ServiceTemplates
    JOIN		ServiceTemplateRecurs
    ON			ServiceTemplateRecurs.id = ServiceTemplates.OwnerServiceTemplateId
	)
	
	INSERT INTO #TempTable (Id)
	SELECT	ServiceTemplateRecurs.Id
	FROM	ServiceTemplateRecurs
	
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