USE Core
GO
IF OBJECT_ID(N'[dbo].[DeleteClientBasedOnId]', N'FN') IS NOT NULL
DROP PROCEDURE [dbo].DeleteClientBasedOnId
GO
--This procedure deletes a Business Account
CREATE PROCEDURE dbo.DeleteClientBasedOnId
		(@clientId uniqueidentifier)

	AS
	BEGIN

	DELETE FROM RouteDestinations
	WHERE ClientId = @clientId
	
	DELETE FROM RouteTasks
	WHERE ClientId = @clientId

	DELETE FROM Services
	WHERE ClientId = @clientId

	EXEC dbo.DeleteServiceTemplatesAndChildrenBasedOnContextId @serviceProviderId = NULL, @ownerClientId = @clientId

	DELETE FROM RecurringServices
	WHERE ClientId = @clientId

	DECLARE @LocationId uniqueidentifier
	
	DECLARE @LocationIdsForClient TABLE
	(
		LocationId uniqueidentifier
	)

	INSERT INTO @LocationIdsForClient
	SELECT Id FROM Locations
	WHERE	PartyId = @clientId

	DECLARE @RowCount int
	SET @RowCount = (SELECT COUNT(*) FROM @LocationIdsForClient)

	WHILE @RowCount > 0
	BEGIN
			SET @LocationId = (SELECT MIN(LocationId) FROM @LocationIdsForClient)

			EXEC dbo.DeleteLocationBasedOnId @locationId = @LocationId

			DELETE FROM @LocationIdsForClient
			WHERE LocationId = @LocationId

			SET @RowCount = (SELECT COUNT(*) FROM @LocationIdsForClient)
	END

	DELETE FROM ClientTitles
	WHERE ClientId = @clientId

	DELETE FROM Parties_Business
	WHERE Id = @clientId

	DELETE FROM Clients
	WHERE Id = @clientId

	END
	RETURN