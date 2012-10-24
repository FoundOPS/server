USE Core
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

DECLARE @serviceTemplateId UNIQUEIDENTIFIER

SET @serviceTemplateId = 'C8E89404-3547-4A75-9E90-0B3BF48B04F1'

DECLARE @newName NVARCHAR(max)

SET @newName = 'Direct'

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

	UPDATE dbo.ServiceTemplates
	SET Name = @newName
	WHERE Id IN
	(
		SELECT Id
		FROM #TempTable
	)    

	SELECT * FROM dbo.ServiceTemplates WHERE Id IN (SELECT Id FROM #TempTable)

	DROP TABLE  #TempTable