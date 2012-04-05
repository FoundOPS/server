IF OBJECT_ID(N'[dbo].[DeleteBasicPartyBasedOnId]', N'FN') IS NOT NULL
DROP PROCEDURE [dbo].DeleteBasicPartyBasedOnId
GO
--This procedure deletes a Business Account
CREATE PROCEDURE dbo.DeleteBasicPartyBasedOnId
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


	END
	RETURN