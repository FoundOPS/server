--This procedure deletes a Business Account
CREATE PROCEDURE dbo.DeleteLocationBasedOnId
		(@locationId uniqueidentifier)

	AS
	BEGIN

	DELETE FROM RouteTasks
	WHERE LocationId = @locationId

	DELETE FROM SubLocations
	WHERE LocationId = @locationId

	DELETE ContactInfoSet
	WHERE LocationId = @locationId

	DELETE FROM RouteDestinations
	WHERE LocationId = @locationId

	DELETE FROM Locations
	WHERE Id = @locationId	

	END
	RETURN