USE Core 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetServiceTemplatesAndFields]
(
	@serviceProviderContextId UNIQUEIDENTIFIER,
	@clientContextId UNIQUEIDENTIFIER,
	@serviceTemplateContextId UNIQUEIDENTIFIER,
	@levelInt INT
)
AS
BEGIN
	
	DECLARE @fieldIds TABLE 
	(
		Id UNIQUEIDENTIFIER
	)

	IF @serviceProviderContextId IS NOT NULL
	BEGIN

		IF @levelInt IS NOT NULL
		BEGIN
			SELECT * FROM dbo.ServiceTemplates WHERE OwnerServiceProviderId = @serviceProviderContextId AND LevelInt = @levelInt	

			INSERT INTO @fieldIds
			SELECT Id FROM dbo.Fields 
			WHERE ServiceTemplateId IN (SELECT Id 
										FROM dbo.ServiceTemplates 
										WHERE OwnerServiceProviderId = @serviceProviderContextId
										AND LevelInt = @levelInt)
		END
		ELSE
		BEGIN
			SELECT * FROM dbo.ServiceTemplates WHERE OwnerServiceProviderId = @serviceProviderContextId	
		
			INSERT INTO @fieldIds
			SELECT Id FROM dbo.Fields 
			WHERE ServiceTemplateId IN (SELECT Id 
										FROM dbo.ServiceTemplates 
										WHERE OwnerServiceProviderId = @serviceProviderContextId)	
		END  
	END
  
	IF @clientContextId IS NOT NULL
	BEGIN
  		
		IF @levelInt IS NOT NULL
		BEGIN
			SELECT * FROM dbo.ServiceTemplates WHERE OwnerClientId = @clientContextId AND LevelInt = @levelInt
		
			INSERT INTO @fieldIds
			SELECT Id FROM dbo.Fields 
			WHERE ServiceTemplateId IN (SELECT Id 
										FROM dbo.ServiceTemplates 
										WHERE OwnerClientId = @clientContextId
										AND LevelInt = @levelInt)		
		END
		ELSE
		BEGIN
			SELECT * FROM dbo.ServiceTemplates WHERE OwnerClientId = @clientContextId	
		
			INSERT INTO @fieldIds
			SELECT Id FROM dbo.Fields 
			WHERE ServiceTemplateId IN (SELECT Id 
										FROM dbo.ServiceTemplates 
										WHERE OwnerClientId = @clientContextId)	
		END 
	END
  
	IF @serviceTemplateContextId IS NOT NULL
	BEGIN

		IF @levelInt IS NOT NULL
		BEGIN
			SELECT * FROM dbo.ServiceTemplates WHERE Id = @serviceTemplateContextId AND LevelInt = @levelInt	

			INSERT INTO @fieldIds
			SELECT Id FROM dbo.Fields WHERE ServiceTemplateId IN (	SELECT Id 
																	FROM dbo.ServiceTemplates 
																	WHERE Id = @serviceTemplateContextId
																	AND LevelInt = @levelInt)	
		END
		ELSE
		BEGIN
			SELECT * FROM dbo.ServiceTemplates WHERE Id = @serviceTemplateContextId	
			
			INSERT INTO @fieldIds
			SELECT Id FROM dbo.Fields WHERE ServiceTemplateId IN (	SELECT Id 
																	FROM dbo.ServiceTemplates 
																	WHERE Id = @serviceTemplateContextId)	
		END 

		

	END  

	SELECT * FROM dbo.Fields WHERE Id IN (SELECT Id FROM @fieldIds)

  	SELECT t1.*, t2.* FROM dbo.Fields t1 
	JOIN dbo.Fields_DateTimeField t2 
	ON t1.Id = t2.Id and t1.Id IN (SELECT Id FROM @fieldIds)

	SELECT t1.*, t2.* FROM dbo.Fields t1 
	JOIN dbo.Fields_NumericField t2 
	ON t1.Id = t2.Id and t1.Id IN (SELECT Id FROM @fieldIds)

	SELECT t1.*, t2.* FROM dbo.Fields t1 
	JOIN dbo.Fields_TextBoxField t2 
	ON t1.Id = t2.Id and t1.Id IN (SELECT Id FROM @fieldIds)

	SELECT t1.*, t2.* FROM dbo.Fields t1 
	JOIN dbo.Fields_OptionsField t2 
	ON t1.Id = t2.Id and t1.Id IN (SELECT Id FROM @fieldIds)

	SELECT * FROM dbo.Options WHERE OptionsFieldId IN (SELECT Id FROM dbo.Fields_OptionsField WHERE Id IN (SELECT Id FROM @fieldIds))

	SELECT t1.*, t2.* FROM dbo.Fields t1 
	JOIN dbo.Fields_LocationField t2 
	ON t1.Id = t2.Id and t1.Id IN (SELECT Id FROM @fieldIds)

	SELECT * FROM dbo.Locations WHERE Id IN (SELECT LocationId FROM dbo.Fields_LocationField WHERE Id IN (SELECT Id FROM @fieldIds))

RETURN  
END