USE Core
GO

--This procedure deletes a Business Account
CREATE PROCEDURE dbo.DeleteClientBasedOnId
		(@clientId uniqueidentifier)

	AS
	BEGIN

	DELETE FROM Services
	WHERE ClientId = @clientId

	EXEC dbo.DeleteServiceTemplatesAndChildrenBasedOnContextId @serviceProviderId = NULL, @ownerClientId = @clientId

	DELETE FROM RouteTasks
	WHERE ClientId = @clientId

	DELETE FROM RouteDestinations
	WHERE ClientId = @clientId

	DELETE FROM RecurringServices
	WHERE ClientId = @clientId

	DELETE FROM ClientTitles
	WHERE ClientId = @clientId

	DELETE FROM Clients
	WHERE Id = @clientId

	END
	RETURN