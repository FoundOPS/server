USE Core
GO
IF OBJECT_ID(N'[dbo].[DeleteBusinessAccountBasedOnId]', N'FN') IS NOT NULL
DROP PROCEDURE [dbo].DeleteBusinessAccountBasedOnId
GO
/****************************************************************************************************************************************************
* FUNCTION DeleteBusinessAccountBasedOnId will delete a BusinessAccount and all entities associated with it
* Follows the following progression to delete: RouteEmployee, RouteVehicle, Routes, RouteTasks, Services, ServiceTemplates, Clients, Locations
* Regions, Contacts, ContactInfoSet, Roles, Vehicles, Files, Employees and finally it deletes the BusinessAccount itself 
** Input Parameters **
* @providerId - The BusinessAccount Id
***************************************************************************************************************************************************/
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

	DELETE FROM Locations
	WHERE BusinessAccountIdIfDepot = @providerId

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

	--Finds all Clients that are associated with the BusinessAccount
	INSERT INTO @ClientIdsForServiceProvider
	SELECT Id FROM Clients
	WHERE	BusinessAccountId = @providerId

	DECLARE @ClientRowCount int
	SET @ClientRowCount = (SELECT COUNT(*) FROM @ClientIdsForServiceProvider)

	--Iterates through @ClientIdsForServiceProvider and calls DeleteClientBasedOnId on each
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

	--Finds all Locations that are associated with the BusinessAccount
	INSERT INTO @LocationIdsForServiceProvider
	SELECT Id FROM Locations
	WHERE	BusinessAccountId = @providerId OR BusinessAccountIdIfDepot = @providerId

	DECLARE @LocationRowCount int
	SET @LocationRowCount = (SELECT COUNT(*) FROM @LocationIdsForServiceProvider)

	--Iterates through @LocationIdsForServiceProvider and calls DeleteLocationBasedOnId on each
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