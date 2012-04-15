--This is not used

--CREATE PROCEDURE dbo.DeleteFieldsAndChildren
--		(@parentTemplateId uniqueidentifier)
--	AS

--	BEGIN

--	WITH FieldRecurs AS
--	(
--    --Select all the Fields from a ServiceTemplate whose OwnerServiceProvider = providerId
--    SELECT		Fields.Id, Fields.ParentFieldId, Fields.ServiceTemplateId
--    FROM		Fields 
--	INNER JOIN	ServiceTemplates 
--    ON			Fields.ServiceTemplateId = ServiceTemplates.Id
--    WHERE		ServiceTemplates.Id = @parentTemplateId
--    --Recursively select the children
--    UNION		ALL
--    SELECT		Fields.Id, Fields.ParentFieldId, Fields.ServiceTemplateId
--    FROM		Fields 
--    JOIN		FieldRecurs 
--    ON			Fields.ParentFieldId = FieldRecurs.Id
--	)

--	--SELECT DISTINCT * FROM FieldRecurs
--	DELETE FROM Fields
--	--This is a Semi-Join between the FieldRecurs table created above and the Fields Table
--	--Semi-Join simply means that it has all the same logic as a normal join, but it doesnt actually join the tables
--	--In this case, it finds all the rows on Fields that correspond to a row in FieldRecurs
--	WHERE		EXISTS
--	(
--	SELECT		FieldRecurs.Id
--	FROM		FieldRecurs
--	WHERE		FieldRecurs.Id = Fields.Id
--	OR			FieldRecurs.Id = Fields.ParentFieldId
--	)

--	END