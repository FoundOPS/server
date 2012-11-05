/****************************************************************************************************************************************************
* FUNCTION ArchiveLocationBasedOnId will delete a Location and all entities associated with it
* Follows the following progression to delete: RouteTasks, SubLocations, ContactInfoSet, RouteDestinations and finally the Location itself
** Input Parameters **
* @locationId - The Location Id to be deleted
***************************************************************************************************************************************************/
CREATE PROCEDURE dbo.ArchiveLocationBasedOnId
		(@locationId UNIQUEIDENTIFIER, @date DATE)

	AS
	BEGIN
  
	UPDATE	dbo.Locations
	SET		DateDeleted = @date
	WHERE	Id = @locationId   

	CREATE TABLE #RecurringService (Id UNIQUEIDENTIFIER)

	INSERT INTO #RecurringService
	SELECT Id FROM dbo.RecurringServices
	WHERE	Id IN (
			SELECT	Id
			FROM	dbo.ServiceTemplates
			WHERE	Id IN ( SELECT	ServiceTemplateId
							FROM	dbo.Fields
							WHERE	Id IN ( SELECT	Id
											FROM	dbo.Fields_LocationField
											WHERE	LocationId = @locationId ) ) )

	
	UPDATE	dbo.RecurringServices
	SET		DateDeleted = @date
	WHERE Id IN (SELECT Id FROM #RecurringService)

	UPDATE dbo.Repeats
	SET EndDate = @date
	WHERE Id IN (SELECT Id FROM #RecurringService)

	CREATE TABLE #RouteTasksToDelete
		(
		  Id UNIQUEIDENTIFIER ,
		  RouteDestinationId UNIQUEIDENTIFIER
		)

	INSERT	INTO #RouteTasksToDelete
			SELECT	Id ,
					RouteDestinationId
			FROM	dbo.RouteTasks
			WHERE	LocationId = @locationId
					AND [Date] > @date

	DELETE	FROM RouteTasks
	WHERE	Id IN ( SELECT	Id
					FROM	#RouteTasksToDelete )

	DELETE	FROM RouteDestinations
	WHERE	Id IN ( SELECT	RouteDestinationId
					FROM	#RouteTasksToDelete )
	
	CREATE TABLE #ServicesToDelete ( Id UNIQUEIDENTIFIER )
	
	INSERT	INTO #ServicesToDelete
			SELECT	Id
			FROM	dbo.Services
			WHERE	Id IN (
					SELECT	Id
					FROM	dbo.ServiceTemplates
					WHERE	Id IN (
							SELECT	ServiceTemplateId
							FROM	dbo.Fields
							WHERE	Id IN ( SELECT	Id
											FROM	dbo.Fields_LocationField
											WHERE	LocationId = @locationId ) ) 
					AND ServiceDate >= @Date
					AND RecurringServiceId IS NULL
					AND LevelInt = 5)
	
	DELETE FROM dbo.Services
	WHERE Id IN (SELECT Id FROM #ServicesToDelete)

	DECLARE @RowCount INT
	DECLARE @RowId UNIQUEIDENTIFIER

	SET @RowCount = (SELECT COUNT(*) FROM #ServicesToDelete)

	WHILE @RowCount > 0
	BEGIN
	
		SET @RowId = (SELECT TOP(1) Id FROM #ServicesToDelete ORDER BY Id)

		EXECUTE [dbo].[DeleteServiceTemplateAndChildrenBasedOnServiceTemplateId] @RowId

		DELETE FROM #ServicesToDelete WHERE Id = @RowId
	
		SET @RowCount = (SELECT COUNT(*) FROM #ServicesToDelete)          
	END
	
	END
	RETURN