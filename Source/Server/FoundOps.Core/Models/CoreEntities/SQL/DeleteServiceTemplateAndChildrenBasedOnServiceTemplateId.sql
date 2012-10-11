SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
USE Core
GO
IF OBJECT_ID(N'[dbo].[DeleteServiceTemplateAndChildrenBasedOnServiceTemplateId]', N'FN') IS NOT NULL
DROP PROCEDURE [dbo].DeleteServiceTemplateAndChildrenBasedOnServiceTemplateId
GO
/****************************************************************************************************************************************************
* FUNCTION DeleteServiceTemplateAndChildrenBasedOnServiceTemplateId will delete a ServiceTemplate and all child ServiceTemplates associated with it
* Begins by using a CTE(Common Table Expression) to find all chlid ServiceTemplates. It does this by making the CTE self referencing so it becomes recursive
* Since the CTE will only last for one operation (it is not actually stored as an abject) we save it into a teemporary table for later use.
* After we have this table, we do a series of Semi-Joins to find RouteTasks, Services, RecurringServices and ServiceTemplates that have an Id that exists in the table
* Finally we drop the temp table so it can be recreated later.
** Input Parameters **
* @parentTemplateId - The ServiceTemplalate Id to use to find children ServiceTemplates
***************************************************************************************************************************************************/
CREATE PROCEDURE dbo.DeleteServiceTemplateAndChildrenBasedOnServiceTemplateId
		(@parentTemplateId uniqueidentifier)
	AS

	BEGIN

		CREATE TABLE #TempTable
	(
		Id uniqueidentifier
	);

	WITH TemplateRecurs AS
	(
    --Select all the Fields from a ServiceTemplate whose OwnerServiceProvider = providerId
    SELECT	ServiceTemplates.Id, ServiceTemplates.OwnerServiceTemplateId
    FROM	ServiceTemplates 
    WHERE	ServiceTemplates.Id = @parentTemplateId
    --Recursively select the children
    UNION	ALL
    SELECT	ServiceTemplates.Id, ServiceTemplates.OwnerServiceTemplateId
    FROM	ServiceTemplates 
    JOIN	TemplateRecurs 
    ON		ServiceTemplates.OwnerServiceTemplateId = TemplateRecurs.Id
	)

	INSERT INTO #TempTable (Id)
	SELECT	TemplateRecurs.Id
	FROM	TemplateRecurs
	
	DELETE
	FROM		RouteTasks
	--This is a Semi-Join between the ServiceTemplateRecurs table created above and the RouteTasks Table
	--Semi-Join simply means that it has all the same logic as a normal join, but it doesnt actually join the tables
	--In this case, it finds all the rows on RouteTasks that correspond to a row in ServiceTemplateRecurs
	WHERE EXISTS
	(
	SELECT		#TempTable.Id
	FROM		#TempTable
	WHERE		#TempTable.Id = RouteTasks.ServiceId
	)

	DELETE 
	FROM		Services
	--This is a Semi-Join between the ServiceTemplateRecurs table created above and the Services Table
	--Semi-Join simply means that it has all the same logic as a normal join, but it doesnt actually join the tables
	--In this case, it finds all the rows on Services that correspond to a row in ServiceTemplateRecurs
	WHERE		EXISTS
	(
	SELECT		#TempTable.Id
	FROM		#TempTable
	WHERE		#TempTable.Id = Services.Id OR #TempTable.Id = Services.RecurringServiceId
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