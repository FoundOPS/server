
CREATE PROCEDURE [dbo].[GetFieldsInJavaScriptFormat]
(
	@businessAccountId UNIQUEIDENTIFIER,
	@serviceType NVARCHAR(MAX)
)
AS
BEGIN
	DECLARE @fields TABLE
	(
		Id UNIQUEIDENTIFIER,
		[Name] NVARCHAR(Max),
		[Type] NVARCHAR(Max)  
	)

	DECLARE @serviceTemplateId UNIQUEIDENTIFIER
	SET @serviceTemplateId = (SELECT Id FROM dbo.ServiceTemplates WHERE OwnerServiceProviderId = @businessAccountId AND Name = @serviceType AND LevelInt = 1)

	INSERT INTO @fields (Id, Name)
	SELECT Id, REPLACE(Name, ' ', '_') FROM dbo.Fields WHERE ServiceTemplateId = @serviceTemplateId

	UPDATE @fields
	SET [Type] = 'string'
	WHERE [@fields].Id NOT IN (SELECT Id FROM dbo.Fields_DateTimeField) 
	AND [@fields].Id NOT IN (SELECT Id FROM dbo.Fields_NumericField) 

	UPDATE @fields
	SET [Type] = 'number'
	WHERE [@fields].Id IN (SELECT Id FROM dbo.Fields_NumericField) 

	UPDATE @fields
	SET [Type] = 'dateTime'
	WHERE [@fields].Id IN (SELECT Id FROM dbo.Fields_DateTimeField WHERE TypeInt = 0)

	UPDATE @fields
	SET [Type] = 'time'
	WHERE [@fields].Id IN (SELECT Id FROM dbo.Fields_DateTimeField WHERE TypeInt = 1)

	UPDATE @fields
	SET [Type] = 'date'
	WHERE [@fields].Id IN (SELECT Id FROM dbo.Fields_DateTimeField WHERE TypeInt = 2)

	SELECT Name, [Type] FROM @fields

RETURN  
END