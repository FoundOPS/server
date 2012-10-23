SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
USE Core
GO
/****************************************************************************************************************************************************
* FUNCTION TestNewServiceTemplatePropagationSuccess will check if a new Service Template was propagated successfully to all clients
** Input Parameters **
* @serviceTemplateId - The Id of the Service Template that you want to check 
* @oldName - The previous name of the service template that was propagated
** Output Parameters: **
* INT - Will be '0' if propagation was successful and will be '1' if it was not
***************************************************************************************************************************************************/
CREATE PROCEDURE [dbo].[TestNewServiceTemplatePropagationSuccess]
(
	@serviceTemplateId UNIQUEIDENTIFIER
)
AS
BEGIN
	DECLARE @templateName NVARCHAR(MAX)
	SET @templateName = (SELECT Name FROM dbo.ServiceTemplates WHERE Id = @serviceTemplateId)

	DECLARE @businessAccountId UNIQUEIDENTIFIER
	SET @businessAccountId = (SELECT OwnerServiceProviderId FROM dbo.ServiceTemplates WHERE Id = @serviceTemplateId)  

	CREATE TABLE #ClientIds
	(
		Id UNIQUEIDENTIFIER
	)

	--Insert all client Id's for the business account
	INSERT INTO #ClientIds
	SELECT Id FROM dbo.Clients WHERE BusinessAccountId = @businessAccountId

	CREATE TABLE #TemplateIds
	(
		Id uniqueidentifier
	)

	--Insert all service templates that are on the client level and have the correct name
	INSERT INTO #TemplateIds
	SELECT Id FROM dbo.ServiceTemplates WHERE OwnerClientId IN (SELECT Id FROM #ClientIds) AND Name = @templateName

	--If there is a different number of clients than Service Templates, return an error
	IF (SELECT COUNT(*) FROM #ClientIds) <> (SELECT COUNT(*) FROM #TemplateIds)
		SELECT 1

	CREATE TABLE #Fields
	(
		Id UNIQUEIDENTIFIER
	)

	--Inserts all fields from the the service templates in #TemplateIds
	INSERT INTO #Fields
	SELECT Id FROM dbo.Fields WHERE ServiceTemplateId IN (SELECT Id FROM #TemplateIds)

	DECLARE @originalFieldCount INT
	SET @originalFieldCount = (SELECT COUNT(*) FROM dbo.Fields WHERE ServiceTemplateId = @serviceTemplateId)  

	--If there is a different number of fields than was expected, return an error
	IF (SELECT COUNT(*) FROM #TemplateIds) * @originalFieldCount <> (SELECT COUNT(*) FROM #Fields)
		SELECT 1

	SELECT 0

	DROP TABLE #TemplateIds
	DROP TABLE #ClientIds
	DROP TABLE #Fields
END