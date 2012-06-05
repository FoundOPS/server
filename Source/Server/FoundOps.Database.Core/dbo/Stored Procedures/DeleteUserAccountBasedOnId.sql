/****************************************************************************************************************************************************
* FUNCTION DeleteUserAccountBasedOnId will delete a UserAccount and all entities associated with it
* Follows the following progression to delete: Locations, Contacts, ContactInfoSet, Roles, Vehicles, Files, UserAccountLog and finally the UserAccount itself 
** Input Parameters **
* @providerId - The UserAccount Id to be deleted
***************************************************************************************************************************************************/
CREATE PROCEDURE dbo.DeleteUserAccountBasedOnId
		(@providerId uniqueidentifier)

	AS
	BEGIN

-------------------------------------------------------------------------------------------------------------------------
--Delete Locations for ServiceProvider
-------------------------------------------------------------------------------------------------------------------------	
	DECLARE @LocationId uniqueidentifier

	DECLARE @LocationIdsForServiceProvider TABLE
	(
		LocationId uniqueidentifier
	)

	--Finds all Locations that are associated with the UserAccount
	INSERT INTO @LocationIdsForServiceProvider
	SELECT Id FROM Locations
	WHERE	OwnerPartyId = @providerId OR PartyId = @providerId

	DECLARE @LocationRowCount int
	SET @LocationRowCount = (SELECT COUNT(*) FROM @LocationIdsForServiceProvider)

	--Iterates through @LocationIdsForServiceProvider and calls DeleteLocationBasedOnId on each
	WHILE @LocationRowCount > 0
	BEGIN
			SET @LocationId = (SELECT MIN(LocationId) FROM @LocationIdsForServiceProvider)

			EXEC dbo.DeleteLocationBasedOnId @locationId = @LocationRowCount

			DELETE FROM @LocationIdsForServiceProvider
			WHERE LocationId = @LocationId

			SET @LocationRowCount = (SELECT COUNT(*) FROM @LocationIdsForServiceProvider)
	END
-------------------------------------------------------------------------------------------------------------------------

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

	DELETE FROM UserAccountLog
	WHERE UserAccountId = @providerId

	DELETE FROM Parties_UserAccount
	WHERE Id = @providerId

	END
	RETURN