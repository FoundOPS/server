
CREATE PROCEDURE [dbo].[PropagateNameChange] 
	(@serviceTemplateId uniqueidentifier)

AS
BEGIN
	DECLARE @newName NVARCHAR(max)
	
	CREATE TABLE #TempTable
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

	INSERT INTO #TempTable (Id)
	SELECT	TemplateRecurs.Id
	FROM	TemplateRecurs

	SET @newName = (SELECT Name FROM dbo.ServiceTemplates WHERE Id = @serviceTemplateId)

	UPDATE dbo.ServiceTemplates
	SET Name = @newName
	WHERE Id IN
	(
		SELECT Id
		FROM #TempTable
	)    

	SELECT * FROM dbo.ServiceTemplates WHERE Id IN (SELECT Id FROM #TempTable)

	DROP TABLE  #TempTable

END
RETURN