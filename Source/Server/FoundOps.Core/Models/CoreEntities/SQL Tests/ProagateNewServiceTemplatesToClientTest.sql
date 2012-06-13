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

--find all clients for the business account
DECLARE @ClientsTable TABLE
(
	Id UNIQUEIDENTIFIER
)

DECLARE @newServiceTemplateTable TABLE
(
	Id UNIQUEIDENTIFIER,
	OwnerServiceProviderId UNIQUEIDENTIFIER,
    OwnerClientId UNIQUEIDENTIFIER,
    OwnerServiceTemplateId UNIQUEIDENTIFIER,
    LevelInt INT,
	Name NVARCHAR(max)
)

INSERT INTO @ClientsTable
SELECT Id FROM dbo.Clients
WHERE BusinessAccountId = @serviceProviderId

DECLARE @ClientRowCount int
SET @ClientRowCount = (SELECT COUNT(*) FROM @ClientsTable)

DECLARE @serviceTemplateName NVARCHAR(max)
DECLARE @ownerserviceTemplateId UNIQUEIDENTIFIER
DECLARE @levelInt INT

SET @serviceTemplateName = (SELECT Name FROM dbo.ServiceTemplates WHERE Id = @serviceTemplateId)
SET @ownerserviceTemplateId = (SELECT OwnerServiceTemplateId FROM dbo.ServiceTemplates WHERE Id = @serviceTemplateId)
SET @levelInt = 3

DECLARE @currentId UNIQUEIDENTIFIER

WHILE @ClientRowCount > 0
BEGIN

	SET @currentId = (SELECT MIN(Id) FROM @ClientsTable)

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
	          @ownerserviceTemplateId , -- OwnerServiceTemplateId - uniqueidentifier
	          @levelInt , -- LevelInt - int
	          @serviceTemplateName  -- Name - nvarchar(max)
	        )

			DELETE FROM @ClientsTable
			WHERE Id = @currentId

			SET @ClientRowCount = @ClientRowCount - 1
END

INSERT INTO dbo.ServiceTemplates
SELECT * FROM @newServiceTemplateTable


--iterate through all cliets, while making a new service template and assigning it to the client

--level = 3 --> Client Defined