USE [Core]
GO
/****** Object:  StoredProcedure [dbo].[PropagateNameChange]    Script Date: 6/19/2012 1:04:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

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