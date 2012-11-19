
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
	SELECT Id, Name FROM dbo.Fields WHERE ServiceTemplateId = @serviceTemplateId

	UPDATE @fields
	SET [Type] = 'string'
	WHERE [@fields].Id NOT IN (SELECT Id FROM dbo.Fields_NumericField) 
	AND [@fields].Id NOT IN (SELECT Id FROM dbo.Fields_SignatureField) 

	UPDATE @fields
	SET [Type] = 'number'
	WHERE [@fields].Id IN (SELECT Id FROM dbo.Fields_NumericField) 

	UPDATE @fields
	SET [Type] = 'signature'
	WHERE [@fields].Id IN (SELECT Id FROM dbo.Fields_SignatureField)

	INSERT INTO @fields
			( Id, Name, Type )
	VALUES	( NEWID(), -- Id - uniqueidentifier
			  'Service Status', -- Name - nvarchar(max)
			  'string'  -- Type - nvarchar(max)
			  )

	SELECT Name, [Type] FROM @fields

RETURN  
END