SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[CheckServiceTemplateForChildren]
(
	-- Add the parameters for the function here
	@serviceTemplateId uniqueidentifier
)
RETURNS  INT
AS
BEGIN
	DECLARE @TempTable TABLE
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

	INSERT INTO @TempTable (Id)
	SELECT	TemplateRecurs.Id
	FROM	TemplateRecurs

	IF (SELECT COUNT(*) FROM @TempTable) > 1
		RETURN 1

	RETURN 0

END
GO

