USE Core
GO

--This procedure deletes a Business Account
CREATE PROCEDURE dbo.DeleteBusinessAccountBasedOnId
		(@providerId uniqueidentifier)

	AS
	BEGIN

	DELETE FROM Services
	WHERE ServiceProviderId = @providerId

	EXEC dbo.DeleteServiceTemplatesAndChildrenBasedOnBusinessAccountId @providerId

	DELETE FROM RouteTasks
	WHERE BusinessAccountId = @providerId

	DELETE FROM Routes
	WHERE OwnerBusinessAccountId = @providerId

	DELETE FROM Clients
	WHERE VendorId = @providerId

	DELETE FROM Locations
	WHERE		PartyId = @providerId
	OR			OwnerPartyId = @providerId

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