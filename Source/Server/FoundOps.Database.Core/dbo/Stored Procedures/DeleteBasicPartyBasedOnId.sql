--This procedure deletes all the basic info held on a Party (Locations, Contacts, ContactInfoSet, Roles, Vehicles and Files)
CREATE PROCEDURE dbo.DeleteBasicPartyBasedOnId
		(@providerId uniqueidentifier)
	AS
	BEGIN

	DELETE FROM Files
	WHERE	PartyId = @providerId

	DELETE FROM dbo.Parties
	WHERE	Id = @providerId
	
	END
	RETURN