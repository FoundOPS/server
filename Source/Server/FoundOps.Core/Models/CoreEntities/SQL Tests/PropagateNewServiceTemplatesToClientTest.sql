USE Core
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

DECLARE @serviceTemplateId UNIQUEIDENTIFIER

SET @serviceTemplateId = '623B11B0-ECC9-4BD8-94D0-163F68BFF73D'

DECLARE @serviceProviderId UNIQUEIDENTIFIER
SET @serviceProviderId = (SELECT OwnerServiceProviderId FROM dbo.ServiceTemplates WHERE Id = @serviceTemplateId)

BEGIN --Propagate new Service Template to all Clients Avaliable Services
	--find all clients for the business account
	DECLARE @ClientsTable TABLE
	(
		Id UNIQUEIDENTIFIER
	)

	--Table that will hold the new Service Templates created
	DECLARE @newServiceTemplateTable TABLE
	(
		Id UNIQUEIDENTIFIER,
		OwnerServiceProviderId UNIQUEIDENTIFIER,
		OwnerClientId UNIQUEIDENTIFIER,
		OwnerServiceTemplateId UNIQUEIDENTIFIER,
		LevelInt INT,
		Name NVARCHAR(max)
	)

	--Find all clients for the Service Provider
	INSERT INTO @ClientsTable
	SELECT Id FROM dbo.Clients
	WHERE BusinessAccountId = @serviceProviderId

	DECLARE @ClientRowCount int
	SET @ClientRowCount = (SELECT COUNT(*) FROM @ClientsTable)

	--Stores the Name of the Service Templates to be created
	DECLARE @serviceTemplateName NVARCHAR(max)
	--Stores the Id of the parent Service Template
	DECLARE @ownerServiceTemplateId UNIQUEIDENTIFIER

	SET @serviceTemplateName = (SELECT Name FROM dbo.ServiceTemplates WHERE Id = @serviceTemplateId)
	SET @ownerServiceTemplateId = @serviceTemplateId

	DECLARE @currentId UNIQUEIDENTIFIER

	WHILE @ClientRowCount > 0 --Iterate through all clients, while making a new service template and assigning it to the client
	BEGIN 

		SET @currentId = (SELECT MIN(Id) FROM @ClientsTable)

		--Create the new Service Template and add it to the table to be added to Service Template table later
		INSERT INTO @newServiceTemplateTable
				( Id ,
				  OwnerServiceProviderId ,
				  OwnerClientId ,
				  OwnerServiceTemplateId ,
				  LevelInt ,
				  Name
				)
		VALUES  ( NEWID() , -- Id - uniqueidentifier
				  @serviceProviderId , -- OwnerServiceProviderId - uniqueidentifier
				  @currentId , -- OwnerClientId - uniqueidentifier
				  @ownerServiceTemplateId , -- OwnerServiceTemplateId - uniqueidentifier
				  3 , -- LevelInt - int
				  @serviceTemplateName  -- Name - nvarchar(max)
				)

				--Once the client has been used, remove it from the list
				DELETE FROM @ClientsTable
				WHERE Id = @currentId

				SET @ClientRowCount = @ClientRowCount - 1
	END

	--Add all the newly created Service Tempalte to the database
	INSERT INTO dbo.ServiceTemplates
	SELECT * FROM @newServiceTemplateTable
END 

-------------------------------------------------------------------------------------------------------------------------------
--Now that we have all the new Service Templates created, we need to add all the appropriate fields to them
-------------------------------------------------------------------------------------------------------------------------------

BEGIN --Propagate all the fields down to the new Service Templates created above
	
	DECLARE @FieldsTable TABLE --Will be used to store Fields that were pulled off of the parent Service Template
	(
		Id UNIQUEIDENTIFIER,
		Name NVARCHAR(MAX),
		[Group] nvarchar(MAX),
		[Required] BIT,
		ToolTip NVARCHAR(MAX),
		ParentFieldId UNIQUEIDENTIFIER,
		ServiceTemplateId UNIQUEIDENTIFIER
	) 

	INSERT INTO @FieldsTable
	SELECT * FROM dbo.Fields t1
	WHERE t1.ServiceTemplateId = @ownerServiceTemplateId 

	BEGIN -- declaring variables to be used to copy fields
		DECLARE @FieldRowCount int
		DECLARE @fieldName NVARCHAR(max)
		DECLARE @fieldGroup NVARCHAR(max)
		DECLARE @fieldRequired BIT
		DECLARE @fieldToolTip NVARCHAR(MAX)
		DECLARE @fieldServiceTemplateId UNIQUEIDENTIFIER
		DECLARE @serviceTemplateCount INT 
		DECLARE @currentServiceTemplateId UNIQUEIDENTIFIER
		DECLARE @newFieldId UNIQUEIDENTIFIER      
		DECLARE @optionsFieldId UNIQUEIDENTIFIER --Will only be used for copying OptionsFields and for copying the Options
	END

	BEGIN --Declaring tables to use for copying fields 
		DECLARE @CopyOfNewServiceTemplates TABLE
		(
			Id UNIQUEIDENTIFIER,
			OwnerServiceProviderId UNIQUEIDENTIFIER,
			OwnerClientId UNIQUEIDENTIFIER,
			OwnerServiceTemplateId UNIQUEIDENTIFIER,
			LevelInt INT,
			Name NVARCHAR(max)
		)
		DECLARE @OptionsCopies TABLE
		(
			Id UNIQUEIDENTIFIER,
			NAME NVARCHAR(MAX),
			IsChecked BIT,
			OptionsFieldId UNIQUEIDENTIFIER,
			[Index] INT,
			ToolTip NVARCHAR(MAX)
		)      
	END

	SET @FieldRowCount = (SELECT COUNT(*) FROM @FieldsTable)

	WHILE @FieldRowCount > 0
	BEGIN
		SET @currentId = (SELECT MIN(Id) FROM @FieldsTable)
    
		--Make sure that @CopyOfNewServiceTemplates is empty so it can be repopulated
		DELETE FROM @CopyOfNewServiceTemplates

		--Re-populate
		INSERT INTO @CopyOfNewServiceTemplates
		SELECT * FROM @newServiceTemplateTable

		BEGIN --Sets all variables to copy basic field data
			SET @fieldName = (SELECT Name FROM @FieldsTable WHERE Id = @currentId)
			SET @fieldGroup = (SELECT [Group] FROM @FieldsTable WHERE Id = @currentId)
			SET @fieldRequired = (SELECT [Required] FROM @FieldsTable WHERE Id = @currentId)
			SET @fieldToolTip = (SELECT ToolTip FROM @FieldsTable WHERE Id = @currentId)
		END
		
		--Track the number of Service Templates left to be created
		SET @serviceTemplateCount = (SELECT COUNT(*) FROM @CopyOfNewServiceTemplates)
        
		WHILE @serviceTemplateCount > 0
		BEGIN     
			SET @currentServiceTemplateId = (SELECT MIN(Id) FROM @CopyOfNewServiceTemplates)				
			
			SET @newFieldId = NEWID()

			INSERT INTO Fields --Add a copy of the old field to the Fields table
				        ( Id ,
				          Name ,
				          [Group] ,
				          [Required] ,
				          ToolTip ,
				          ParentFieldId ,
				          ServiceTemplateId
				        )
				VALUES  ( @newFieldId , -- Id - uniqueidentifier
				          @fieldName , -- Name - nvarchar(max)
				          @fieldGroup , -- Group - nvarchar(max)
				          @fieldRequired , -- Required - bit
				          @fieldToolTip , -- ToolTip - nvarchar(max)
				          @currentId , -- ParentFieldId - uniqueidentifier
				          @currentServiceTemplateId  -- ServiceTemplateId - uniqueidentifier
				        ) 
			
			BEGIN --Copy field to its appropriate inherited table
				
			IF @currentId IN (SELECT Id FROM dbo.Fields_DateTimeField) --Copy the DateTime field, set new Id's
			BEGIN 
				INSERT INTO Fields_DateTimeField
				        ( Earliest ,
				          Latest ,
				          TypeInt ,
				          VALUE ,
				          Id
				        )
				VALUES  ( (SELECT Earliest FROM Fields_DateTimeField WHERE Id = @currentId) , -- Earliest - datetime
				          (SELECT Latest FROM Fields_DateTimeField WHERE Id = @currentId) , -- Latest - datetime
				          (SELECT TypeInt FROM Fields_DateTimeField WHERE Id = @currentId) , -- TypeInt - smallint
				          (SELECT Value FROM Fields_DateTimeField WHERE Id = @currentId) , -- Value - datetime
				         @newFieldId  -- Id - uniqueidentifier
				        )
			END
    
			IF @currentId IN (SELECT Id FROM dbo.Fields_LocationField) --Copy Location field, set new Id's
			BEGIN   
			 
				INSERT INTO Fields_LocationField
						( LocationId ,
						  LocationFieldTypeInt ,
						  Id
						)
				VALUES  ( (SELECT LocationId FROM Fields_LocationField WHERE Id = @currentId) , --LocationId - uniqueidenetifier
						  (SELECT LocationFieldTypeInt FROM Fields_LocationField WHERE Id = @currentId) , --LocationFieldTypeInt - int
						  @newFieldId  -- Id - uniqueidentifier
						)
			END

			IF @currentId IN (SELECT Id FROM dbo.Fields_NumericField) --Copy Numeric field, set new Id's 
			BEGIN
				INSERT INTO Fields_NumericField
				        ( Mask ,
				          DecimalPlaces ,
				          Minimum ,
				          Maximum ,
				          Value ,
				          Id
				        )
				VALUES  ( (SELECT Mask FROM Fields_NumericField WHERE Id = @currentId) , -- Mask - nvarchar(max)
				          (SELECT DecimalPlaces FROM Fields_NumericField WHERE Id = @currentId) , -- DecimalPlaces - int
				          (SELECT Minimum FROM Fields_NumericField WHERE Id = @currentId) , -- Minimum - decimal
				          (SELECT Maximum FROM Fields_NumericField WHERE Id = @currentId) , -- Maximum - decimal
				          (SELECT Value FROM Fields_NumericField WHERE Id = @currentId) , -- Value - decimal
				          @newFieldId  -- Id - uniqueidentifier
				        )

			END

			IF @currentId IN (SELECT Id FROM dbo.Fields_TextBoxField) --Copy TextBox field, set new Id's
			BEGIN
				INSERT INTO Fields_TextBoxField
				        ( IsMultiline, 
						  Value, 
						  Id )
				VALUES  ( (SELECT IsMultiline FROM dbo.Fields_TextBoxField WHERE Id = @currentId), -- IsMultiline - bit
				          (SELECT Value FROM dbo.Fields_TextBoxField WHERE Id = @currentId), -- Value - nvarchar(max)
				          @newFieldId  -- Id - uniqueidentifier
				          )	    
			END

			IF @currentId IN (SELECT Id FROM dbo.Fields_SignatureField) --Copy Signature field, set new Id's
			BEGIN
				INSERT INTO Fields_SignatureField
				        ( Value, 
						  Signed,
						  Id )
				VALUES  ( (SELECT Value FROM dbo.Fields_SignatureField WHERE Id = @currentId), -- Value - nvarchar(max)
						  (SELECT Signed FROM dbo.Fields_SignatureField WHERE Id = @currentId), -- Value - nvarchar(max)
				          @newFieldId  -- Id - uniqueidentifier
				          )	    
			END

			IF @currentId IN (SELECT Id FROM dbo.Fields_OptionsField) --Copy the Options field, set new Id's 
			BEGIN
				INSERT INTO Fields_OptionsField
				        ( AllowMultipleSelection ,
				          TypeInt ,
				          Id
				        )
				VALUES  ( (SELECT AllowMultipleSelection FROM dbo.Fields_OptionsField WHERE Id = @currentId) , -- AllowMultipleSelection - bit
				          (SELECT TypeInt FROM dbo.Fields_OptionsField WHERE Id = @currentId) , -- TypeInt - smallint
				          @newFieldId  -- Id - uniqueidentifier
				        )
						
				INSERT INTO @OptionsCopies
				SELECT * FROM Options
				WHERE OptionsFieldId = @currentId

				UPDATE @OptionsCopies
				SET Id = NEWID(),
					OptionsFieldId = @newFieldId

				INSERT INTO Options
				SELECT * FROM @OptionsCopies

				DELETE FROM @OptionsCopies

			END  
			
			END
			      
			DELETE FROM @CopyOfNewServiceTemplates
			WHERE Id = @currentServiceTemplateId

			SET @serviceTemplateCount = @serviceTemplateCount - 1
		END	

		DELETE FROM @FieldsTable
		WHERE Id = @currentId

		SET @FieldRowCount = @FieldRowCount - 1
	END
END