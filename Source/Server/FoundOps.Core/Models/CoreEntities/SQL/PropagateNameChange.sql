USE [Core]
GO
/****** Object:  StoredProcedure [dbo].[PropagateNameChange]    Script Date: 6/19/2012 1:04:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PropagateNameChange] 
	(@serviceTemplateId UNIQUEIDENTIFIER,
	 @userId UNIQUEIDENTIFIER)

AS
BEGIN
	DECLARE @newName NVARCHAR(max)
	DECLARE @oldName NVARCHAR(max)
	
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

	SET @newName = (SELECT Name 
					FROM dbo.ServiceTemplates 
					WHERE Id = @serviceTemplateId)

	--This will take the first row from the results of a query where the name of the ServiceTemplate is not the new name to be assigned
	--From there it will only select the Name column from that row
	SET @oldName = (SELECT TOP 1 Name 
					FROM dbo.ServiceTemplates 
					WHERE Id IN 
					(
						SELECT Id 
						FROM #TempTable
					) AND Name <> @newName)

	UPDATE dbo.ServiceTemplates
	SET Name = @newName,
		LastModified = GETUTCDATE(),
		LastModifyingUserId = @userId
	WHERE Id IN
	(
		SELECT Id
		FROM #TempTable
	) 

	UPDATE dbo.[Routes] 
	SET RouteType = @newName,
		LastModified = GETUTCDATE(),
		LastModifyingUserId = @userId
	WHERE RouteType = @oldName   

	DROP TABLE  #TempTable

END
RETURN