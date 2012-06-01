﻿	USE Core
	GO
	SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
	CREATE PROCEDURE [dbo].[DeleteFieldAndChildrenBasedOnFieldId]
		(@parentFieldId uniqueidentifier)
	AS

	BEGIN

	CREATE TABLE #TempTable
	(
		Id uniqueidentifier
	);
	
	WITH FieldRecurs AS
	(
    --Select all the Fields from a ServiceTemplate whose OwnerServiceProvider = providerId
    SELECT	Fields.Id, Fields.ParentFieldId
    FROM	Fields 
    WHERE	Fields.Id = @parentFieldId
    --Recursively select the children
    UNION	ALL
    SELECT	Fields.Id, Fields.ParentFieldId
    FROM	Fields 
    JOIN	FieldRecurs 
    ON		Fields.ParentFieldId = FieldRecurs.Id
	)

	INSERT INTO #TempTable (Id)
	SELECT	FieldRecurs.Id
	FROM	FieldRecurs

	DELETE 
	FROM		Fields
	--This is a Semi-Join between the FieldRecurs table created above and the Fields Table
	--Semi-Join simply means that it has all the same logic as a normal join, but it doesnt actually join the tables
	--In this case, it finds all the rows on Fields that correspond to a row in FieldRecurs
	WHERE		EXISTS
	(
	SELECT		#TempTable.Id
	FROM		#TempTable
	WHERE		#TempTable.Id = Fields.Id
	OR			#TempTable.Id = Fields.ParentFieldId
	)

	--Drop the table that was created to store the CTE
	DROP TABLE #TempTable
	
	END
	RETURN