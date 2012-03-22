--This procedure deletes a User Account
IF OBJECT_ID(N'[dbo].[DeleteUserAccountBasedOnId]', N'FN') IS NOT NULL
DROP PROCEDURE [dbo].[DeleteUserAccountBasedOnId]
GO
CREATE PROCEDURE dbo.DeleteUserAccountBasedOnId
		(@providerId uniqueidentifier)

	AS
	BEGIN

	DELETE FROM Locations
	WHERE		OwnerPartyId = @providerId
	OR			PartyId = @providerId

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
	WHERE LinkedUserAccountId = @providerId

	DELETE FROM TrackPoints
	WHERE UserAccountId = @providerId

	DELETE FROM UserAccountLog
	WHERE UserAccountId = @providerId

	DELETE FROM Parties_UserAccount
	WHERE Id = @providerId

	END
	RETURN