USE Core
GO
IF OBJECT_ID(N'[dbo].[DeleteBusinessAccountBasedOnId]', N'FN') IS NOT NULL
DROP PROCEDURE [dbo].DeleteBusinessAccountBasedOnId
GO
--This procedure deletes a Business Account
CREATE PROCEDURE dbo.DeleteBusinessAccountBasedOnId
		(@providerId uniqueidentifier)

	AS
	BEGIN

	DELETE FROM RouteEmployee
	WHERE EXISTS
	(
		SELECT Id
		FROM Routes
		WHERE OwnerBusinessAccountId = @providerId
	)

	DELETE FROM RouteVehicle
	WHERE EXISTS
	(
		SELECT Id
		FROM Routes
		WHERE OwnerBusinessAccountId = @providerId
	)

	DELETE FROM Routes
	WHERE OwnerBusinessAccountId = @providerId

	DELETE FROM RouteTasks
	WHERE BusinessAccountId = @providerId

	DELETE FROM Services
	WHERE ServiceProviderId = @providerId

	EXEC dbo.DeleteServiceTemplatesAndChildrenBasedOnContextId @serviceProviderId = @providerId, @ownerClientId = null
-------------------------------------------------------------------------------------------------------------------------
--Delete Clients for ServiceProvider
-------------------------------------------------------------------------------------------------------------------------	
	DECLARE @ClientId uniqueidentifier

	DECLARE @ClientIdsForServiceProvider TABLE
	(
		ClientId uniqueidentifier
	)

	INSERT INTO @ClientIdsForServiceProvider
	SELECT Id FROM Clients
	WHERE	VendorId = @providerId

	DECLARE @ClientRowCount int
	SET @ClientRowCount = (SELECT COUNT(*) FROM @ClientIdsForServiceProvider)

	WHILE @ClientRowCount > 0
	BEGIN
			SET @ClientId = (SELECT MIN(ClientId) FROM @ClientIdsForServiceProvider)

			EXEC dbo.DeleteClientBasedOnId @clientId = @ClientId

			DELETE FROM @ClientIdsForServiceProvider
			WHERE ClientId = @ClientId

			SET @ClientRowCount = (SELECT COUNT(*) FROM @ClientIdsForServiceProvider)
	END
-------------------------------------------------------------------------------------------------------------------------
--Delete Locations for ServiceProvider
-------------------------------------------------------------------------------------------------------------------------	
	DECLARE @LocationId uniqueidentifier

	DECLARE @LocationIdsForServiceProvider TABLE
	(
		LocationId uniqueidentifier
	)

	INSERT INTO @LocationIdsForServiceProvider
	SELECT Id FROM Locations
	WHERE	OwnerPartyId = @providerId OR PartyId = @providerId

	DECLARE @LocationRowCount int
	SET @LocationRowCount = (SELECT COUNT(*) FROM @LocationIdsForServiceProvider)

	WHILE @LocationRowCount > 0
	BEGIN
			SET @LocationId = (SELECT MIN(LocationId) FROM @LocationIdsForServiceProvider)

			EXEC dbo.DeleteLocationBasedOnId @locationId = @LocationId

			DELETE FROM @LocationIdsForServiceProvider
			WHERE LocationId = @LocationId

			SET @LocationRowCount = (SELECT COUNT(*) FROM @LocationIdsForServiceProvider)
	END
-------------------------------------------------------------------------------------------------------------------------

	DELETE FROM Regions
	WHERE BusinessAccountId = @providerId

	DELETE FROM Contacts
	WHERE		OwnerPartyId = @providerId

	DELETE FROM ContactInfoSet
	WHERE		PartyId = @providerId

	DELETE FROM Roles
	WHERE		OwnerPartyId = @providerId

	DELETE FROM Vehicles 
	WHERE		OwnerPartyId = @providerId

	DELETE FROM Files
	WHERE		PartyId = @providerId

	DELETE FROM Employees
	WHERE EmployerId = @providerId
	
	DELETE FROM Parties_BusinessAccount
	WHERE Id = @providerId

	END
	RETURN