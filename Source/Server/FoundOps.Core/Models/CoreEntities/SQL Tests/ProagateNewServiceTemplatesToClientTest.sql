USE Core
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

DECLARE @serviceProviderId UNIQUEIDENTIFIER
DECLARE @serviceTemplateId UNIQUEIDENTIFIER

SET @serviceTemplateId = '49A1FAA4-3255-4859-9FF3-25845D986C6E'
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
	SET @ownerServiceTemplateId = (SELECT OwnerServiceTemplateId FROM dbo.ServiceTemplates WHERE Id = @serviceTemplateId)

	DECLARE @currentId UNIQUEIDENTIFIER

	--Iterate through all cliets, while making a new service template and assigning it to the client
	WHILE @ClientRowCount > 0
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

	--Will be used to store Fields that were pulled off of the parent Service Template
	DECLARE @FieldsTable TABLE
	(
		Id UNIQUEIDENTIFIER NOT NULL,
		Name NVARCHAR(MAX) NOT NULL,
		[Group] nvarchar(MAX) NOT NULL,
		[Required] BIT NOT NULL,
		ToolTip NVARCHAR(MAX) NOT NULL,
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
	END

	BEGIN --Declaring tables to use for copying fields
		DECLARE @newFieldsTable TABLE
		(
			Id UNIQUEIDENTIFIER NOT NULL,
			Name NVARCHAR(MAX) NOT NULL,
			[Group] nvarchar(MAX) NOT NULL,
			[Required] BIT NOT NULL,
			ToolTip NVARCHAR(MAX) NOT NULL,
			ParentFieldId UNIQUEIDENTIFIER,
			ServiceTemplateId UNIQUEIDENTIFIER
		)
		DECLARE @DateTimeTable TABLE
		(
			Earliest DATETIME NOT NULL,
			Latest DATETIME NOT NULL,
			TypeInt SMALLINT NOT NULL,
			Value DATETIME NOT NULL,
			Id UNIQUEIDENTIFIER
		) 
		DECLARE @LocationTable TABLE
		(
			LocationId UNIQUEIDENTIFIER,
			LocationFieldTypeInt SMALLINT,
			Id UNIQUEIDENTIFIER          
		)
		DECLARE @NumericTable TABLE
		(
			Mask NVARCHAR(MAX),
			DeciamalPlaces INT,
			Minimum DECIMAL(16,6),
			Maximum DECIMAL(16,6),
			Value DECIMAL(16,6)     
		) 
		DECLARE @TextBoxTable TABLE
		(
			IsMultiLine BIT,
			Value NVARCHAR(max),
			Id UNIQUEIDENTIFIER   
		)  
		DECLARE @CopyOfNewServiceTemplates TABLE
		(
			Id UNIQUEIDENTIFIER,
			OwnerServiceProviderId UNIQUEIDENTIFIER,
			OwnerClientId UNIQUEIDENTIFIER,
			OwnerServiceTemplateId UNIQUEIDENTIFIER,
			LevelInt INT,
			Name NVARCHAR(max)
		)
	END

	SET @FieldRowCount = (SELECT COUNT(*) FROM @FieldsTable)

	WHILE @FieldRowCount > 0
	BEGIN
	
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

		SET @currentId = (SELECT MIN(Id) FROM @FieldsTable)
        
		WHILE @serviceTemplateCount > 0
		BEGIN
  
			SET @currentServiceTemplateId = (SELECT MIN(Id) FROM @CopyOfNewServiceTemplates)				
			      
			IF @currentId IN (SELECT Id FROM dbo.Fields_DateTimeField)
			BEGIN
				--Copy the Field and the DateTime field, set new Id's
				INSERT INTO @newFieldsTable
				        ( Id ,
				          Name ,
				          [Group] ,
				          Required ,
				          ToolTip ,
				          ParentFieldId ,
				          ServiceTemplateId
				        )
				VALUES  ( NEWID() , -- Id - uniqueidentifier
				          @fieldName , -- Name - nvarchar(max)
				          @fieldGroup , -- Group - nvarchar(max)
				          @fieldRequired , -- Required - bit
				          @fieldToolTip , -- ToolTip - nvarchar(max)
				          @currentId , -- ParentFieldId - uniqueidentifier
				          @currentServiceTemplateId  -- ServiceTemplateId - uniqueidentifier
				        ) 
				
				INSERT INTO @DateTimeTable
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
				          NEWID()  -- Id - uniqueidentifier
				        )    
			END
    
			IF @currentId IN (SELECT Id FROM dbo.Fields_LocationField)
			BEGIN
				--Copy the Field and the Location field, set new Id's   
			END

			IF @currentId IN (SELECT Id FROM dbo.Fields_NumericField)
			BEGIN
				--Copy the Field and the Numeric field, set new Id's 
			END

			IF @currentId IN (SELECT Id FROM dbo.Fields_TextBoxField)
			BEGIN
				--Copy the Field and the TextBox field, set new Id's \    
			END

			IF @currentId IN (SELECT Id FROM dbo.Fields_OptionsField)
			BEGIN
				--Copy the Field and the Options field, set new Id's 
				--This is not set up yet...cause its a bitchy bitch
			END  
	
			DELETE FROM @CopyOfNewServiceTemplates
			WHERE Id = @currentServiceTemplateId

			SET @serviceTemplateCount = @serviceTemplateCount - 1
		END	

		DELETE FROM @FieldsTable
		WHERE Id = @currentId



	END
END