/****************************************************************************************************************************************************
* FUNCTION DeleteLocationBasedOnId will delete a Location and all entities associated with it
* Follows the following progression to delete: RouteTasks, SubLocations, ContactInfoSet, RouteDestinations and finally the Location itself
** Input Parameters **
* @locationId - The Location Id to be deleted
***************************************************************************************************************************************************/
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