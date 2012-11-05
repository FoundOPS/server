/****************************************************************************************************************************************************
* FUNCTION TestFieldPropagationSuccess will check a ServiceTemplate to see if its children ServiceTemplates have had a field propagated successfully
** Input Parameters **
* @serviceTemplateId - The Id of the Service Template that you want to check 
* @fieldName - The name of the field that was propagated
** Output Parameters: **
* INT - Will be '0' if propagation was not successful and will be '1' if it was
***************************************************************************************************************************************************/
CREATE PROCEDURE [dbo].[TestFieldPropagationSuccess]
(
	@serviceTemplateId UNIQUEIDENTIFIER,
	@fieldName NVARCHAR(MAX)
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

	CREATE TABLE #FieldIds
	(
		Id UNIQUEIDENTIFIER
	)

	INSERT INTO #FieldIds
	SELECT Id FROM dbo.Fields WHERE ServiceTemplateId IN (SELECT Id FROM #TemplateIds) AND Name = @fieldName

	IF (SELECT COUNT(*) FROM #FieldIds) <> (SELECT COUNT(*) FROM #TemplateIds)
		SELECT 1
	
	SELECT 0

	DROP TABLE #TemplateIds
	DROP TABLE #FieldIds
END