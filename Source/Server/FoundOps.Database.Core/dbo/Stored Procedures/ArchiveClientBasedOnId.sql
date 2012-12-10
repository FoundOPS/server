/****************************************************************************************************************************************************
* FUNCTION ArchiveClientBasedOnId will archive a Client and all entities associated with it
* Follows the following progression to delete: RouteDestinations, RouteTasks, Services, ServiceTemplates, RecurringServices, Locations 
* ClientTitles, Parties_Business and finally the Client itself
** Input Parameters **
* @clientId - The Client Id to be deleted
***************************************************************************************************************************************************/
CREATE PROCEDURE dbo.ArchiveClientBasedOnId
		(@clientId uniqueidentifier, @userId UNIQUEIDENTIFIER)

	AS
	BEGIN
	DECLARE @date DATE
	SET @date = GETUTCDATE()  
	
	UPDATE dbo.Clients
	SET DateDeleted = @date,
		LastModified = @date,
		LastModifyingUserId = @userId
	WHERE Id = @clientId
	  
-------------------------------------------------------------------------------------------------------------------------
--Delete Locations for Client
-------------------------------------------------------------------------------------------------------------------------
	DECLARE @LocationId uniqueidentifier
	
	DECLARE @LocationIdsForClient TABLE
	(
		LocationId uniqueidentifier
	)

	--Finds all Locations that are associated with the Client
	INSERT INTO @LocationIdsForClient
	SELECT Id FROM Locations
	WHERE	ClientId = @clientId

	DECLARE @RowCount int
	SET @RowCount = (SELECT COUNT(*) FROM @LocationIdsForClient)

	--Iterates through @LocationIdsForClient and calls DeleteLocationBasedOnId on each
	WHILE @RowCount > 0
	BEGIN
			SET @LocationId = (SELECT MIN(LocationId) FROM @LocationIdsForClient)

			EXEC dbo.ArchiveLocationBasedOnId @locationId = @LocationId, @date = @date, @userId = @userId

			DELETE FROM @LocationIdsForClient
			WHERE LocationId = @LocationId

			SET @RowCount = (SELECT COUNT(*) FROM @LocationIdsForClient)
	END
-------------------------------------------------------------------------------------------------------------------------
	END
	RETURN