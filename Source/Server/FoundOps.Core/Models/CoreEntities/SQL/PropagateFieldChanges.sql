USE Core
GO
/****** Object:  StoredProcedure [dbo].[PropagateNewFields]    Script Date: 6/19/2012 1:04:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--This procedure deletes all the basic info held on a Party (Locations, Contacts, ContactInfoSet, Roles, Vehicles and Files)
ALTER PROCEDURE [dbo].[PropagateNewFields]
		(@FieldId uniqueidentifier)
	AS
	BEGIN
		BEGIN -- declaring variables to be used to copy fields
			DECLARE @FieldRowCount int
			DECLARE @fieldName NVARCHAR(max)
			DECLARE @fieldRequired BIT
			DECLARE @fieldToolTip NVARCHAR(MAX)
			DECLARE @fieldServiceTemplateId UNIQUEIDENTIFIER
			DECLARE @serviceTemplateCount INT 
			DECLARE @currentServiceTemplateId UNIQUEIDENTIFIER
			DECLARE @newFieldId UNIQUEIDENTIFIER      
			DECLARE @optionsFieldId UNIQUEIDENTIFIER --Will only be used for copying OptionsFields and for copying the Options
			DECLARE @serviceTemplateId UNIQUEIDENTIFIER
		END

		BEGIN --Declaring tables to use for copying fields 
			DECLARE @CopyOfNewServiceTemplates TABLE
			(
				Id UNIQUEIDENTIFIER
			)    
			  
		END

		BEGIN --Sets all variables to copy basic field data
			SET @fieldName = (SELECT Name FROM Fields WHERE Id = @FieldId)
			SET @fieldRequired = (SELECT [Required] FROM Fields WHERE Id = @FieldId)
			SET @fieldToolTip = (SELECT ToolTip FROM Fields WHERE Id = @FieldId)
			SET @serviceTemplateId = (SELECT ServiceTemplateId FROM dbo.Fields WHERE Id = @FieldId)
		END;
  
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

		INSERT INTO @CopyOfNewServiceTemplates (Id)
		SELECT	TemplateRecurs.Id
		FROM	TemplateRecurs		      
		
		DELETE FROM @CopyOfNewServiceTemplates
		WHERE Id = @serviceTemplateId

		--Track the number of Service Templates left to be created
		SET @serviceTemplateCount = (SELECT COUNT(*) FROM @CopyOfNewServiceTemplates)
        
		WHILE @serviceTemplateCount > 0
		BEGIN     
			SET @currentServiceTemplateId = (SELECT MIN(Id) FROM @CopyOfNewServiceTemplates)				
			
			SET @newFieldId = NEWID()

			INSERT INTO Fields --Add a copy of the old field to the Fields table
				    ( Id ,
				        Name ,
				        [Required] ,
				        ToolTip ,
				        ParentFieldId ,
				        ServiceTemplateId
				    )
			VALUES  ( @newFieldId , -- Id - uniqueidentifier
				        @fieldName , -- Name - nvarchar(max)
				        @fieldRequired , -- Required - bit
				        @fieldToolTip , -- ToolTip - nvarchar(max)
				        @FieldId , -- ParentFieldId - uniqueidentifier
				        @currentServiceTemplateId  -- ServiceTemplateId - uniqueidentifier
				    ) 
				
			BEGIN --Copy field to its appropriate inherited table
				IF @FieldId IN (SELECT Id FROM dbo.Fields_DateTimeField) --Copy the DateTime field, set new Id's
				BEGIN 
					INSERT INTO Fields_DateTimeField
							( Earliest ,
								Latest ,
								TypeInt ,
								VALUE ,
								Id
							)
					VALUES  ( (SELECT Earliest FROM Fields_DateTimeField WHERE Id = @FieldId) , -- Earliest - datetime
								(SELECT Latest FROM Fields_DateTimeField WHERE Id = @FieldId) , -- Latest - datetime
								(SELECT TypeInt FROM Fields_DateTimeField WHERE Id = @FieldId) , -- TypeInt - smallint
								(SELECT Value FROM Fields_DateTimeField WHERE Id = @FieldId) , -- Value - datetime
								@newFieldId  -- Id - uniqueidentifier
							)
				END
    
				IF @FieldId IN (SELECT Id FROM dbo.Fields_LocationField) --Copy Location field, set new Id's
				BEGIN   
			 
					INSERT INTO Fields_LocationField
							( LocationId ,
								LocationFieldTypeInt ,
								Id
							)
					VALUES  ( (SELECT LocationId FROM Fields_LocationField WHERE Id = @FieldId) , --LocationId - uniqueidenetifier
								(SELECT LocationFieldTypeInt FROM Fields_LocationField WHERE Id = @FieldId) , --LocationFieldTypeInt - int
								@newFieldId  -- Id - uniqueidentifier
							)
				END

				IF @FieldId IN (SELECT Id FROM dbo.Fields_NumericField) --Copy Numeric field, set new Id's 
				BEGIN
					INSERT INTO Fields_NumericField
							( Mask ,
								DecimalPlaces ,
								Minimum ,
								Maximum ,
								Value ,
								Id
							)
					VALUES  ( (SELECT Mask FROM Fields_NumericField WHERE Id = @FieldId) , -- Mask - nvarchar(max)
								(SELECT DecimalPlaces FROM Fields_NumericField WHERE Id = @FieldId) , -- DecimalPlaces - int
								(SELECT Minimum FROM Fields_NumericField WHERE Id = @FieldId) , -- Minimum - decimal
								(SELECT Maximum FROM Fields_NumericField WHERE Id = @FieldId) , -- Maximum - decimal
								(SELECT Value FROM Fields_NumericField WHERE Id = @FieldId) , -- Value - decimal
								@newFieldId  -- Id - uniqueidentifier
							)

				END

				IF @FieldId IN (SELECT Id FROM dbo.Fields_TextBoxField) --Copy TextBox field, set new Id's
				BEGIN
					INSERT INTO Fields_TextBoxField
							( IsMultiline, 
								Value, 
								Id )
					VALUES  ( (SELECT IsMultiline FROM dbo.Fields_TextBoxField WHERE Id = @FieldId), -- IsMultiline - bit
								(SELECT Value FROM dbo.Fields_TextBoxField WHERE Id = @FieldId), -- Value - nvarchar(max)
								@newFieldId  -- Id - uniqueidentifier
								)	    
				END

				IF @FieldId IN (SELECT Id FROM dbo.Fields_OptionsField) --Copy the Options field, set new Id's 
				BEGIN
					
					INSERT INTO dbo.Fields_OptionsField
							( AllowMultipleSelection ,
							  TypeInt ,
							  Value ,
							  OptionsString ,
							  Id
							)
					VALUES	( (SELECT AllowMultipleSelection FROM dbo.Fields_OptionsField WHERE Id = @FieldId) , -- AllowMultipleSelection - bit
							  (SELECT TypeInt FROM dbo.Fields_OptionsField WHERE Id = @FieldId), -- TypeInt - smallint
							  (SELECT Value FROM dbo.Fields_OptionsField WHERE Id = @FieldId) , -- Value - nvarchar(max)
							  (SELECT OptionsString FROM dbo.Fields_OptionsField WHERE Id = @FieldId) , -- OptionsString - nvarchar(max)
							  @newFieldId  -- Id - uniqueidentifier
							)
				END  		
			END
			      
			DELETE FROM @CopyOfNewServiceTemplates
			WHERE Id = @currentServiceTemplateId

			SET @serviceTemplateCount = @serviceTemplateCount - 1
		END	
	
	END
	RETURN