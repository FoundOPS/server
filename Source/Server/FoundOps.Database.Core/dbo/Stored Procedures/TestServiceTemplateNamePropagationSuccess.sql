/****************************************************************************************************************************************************
* FUNCTION TestServiceTemplateNamePropagationSuccess will check a ServiceTemplate to see if its children ServiceTemplates have had a name change propagated successfully
** Input Parameters **
* @serviceTemplateId - The Id of the Service Template that you want to check 
* @oldName - The previous name of the service template that was propagated
** Output Parameters: **
* INT - Will be '0' if propagation was successful and will be '1' if it was not
***************************************************************************************************************************************************/
CREATE PROCEDURE [dbo].[TestServiceTemplateNamePropagationSuccess]
(
	@serviceTemplateId UNIQUEIDENTIFIER,
	@oldName NVARCHAR(MAX)
)
AS
BEGIN
	CREATE TABLE #TemplateIds
	(
		Id uniqueidentifier
	);

	WITH TemplateRecurs AS
	(
    --Select all the Fields from a ServiceTemplate whose OwnerServiceProvider = providerId
    SELECT	ServiceTemplates.Id, ServiceTemplates.OwnerServiceTemplateId
    FROM	ServiceTemplates 
    WHERE	ServiceTemplates.Id = @serviceTemplateId
    --Recursively select the children
    UNION	ALL
    SELECT	ServiceTemplates.Id, ServiceTemplates.OwnerServiceTemplateId
    FROM	ServiceTemplates 
    JOIN	TemplateRecurs 
    ON		ServiceTemplates.OwnerServiceTemplateId = TemplateRecurs.Id
	)

	INSERT INTO #TemplateIds (Id)
	SELECT	TemplateRecurs.Id
	FROM	TemplateRecurs

	--@failedServiceTemplateId will only be non-null if one of the children service templates did not get re-named properly
	DECLARE @failedServiceTemplateId UNIQUEIDENTIFIER = NULL  
	SET @failedServiceTemplateId = (SELECT TOP 1 Id FROM dbo.ServiceTemplates WHERE Id IN (SELECT Id FROM #TemplateIds) AND Name = @oldName)
	
	IF @failedServiceTemplateId IS NOT NULL  
		SELECT 1
	
	--@failedRoutesId will only be non-null if there is a Route with the old Service Templates name
	DECLARE @failedRoutesId UNIQUEIDENTIFIER = NULL
	SET @failedRoutesId = (SELECT TOP 1 Id FROM dbo.[Routes] WHERE Name = @oldName)
	
	IF @failedRoutesId IS NOT NULL
		SELECT 1  

	SELECT 0

	DROP TABLE #TemplateIds
END