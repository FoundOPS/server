--IF OBJECT_ID(N'[dbo].[GetUnroutedServicesForDate]', N'FN') IS NOT NULL
--DROP FUNCTION [dbo].[GetUnroutedServicesForDate]
--GO
/*****************************************************************************************************************************************************************************************************************************
* FUNCTION GetUnroutedServicesForDate will take the context provided and find all the services that are scheduled for that day
** Input Parameters **
* @serviceProviderIdContext - The BusinessAccount context
** Output Parameters: **
* @ServicesTableToReturn - Ex. below
* RecurringServiceId| ServiceId | OccurDate									| ServiceName | ClientName		| ClientId  | RegionName | LocationName    | LocationId | AddressLine   | Latitude	| Longitude	| StatusInt
* ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
* {GUID}			|           | 1/1/2012 <-- Generated service			| Oil		  | Seltzer Factory | {GUID}	| South		 | Seltzer Factory | {GUID}		| 123 Fake St	| 47.456	| -86.166	| 3	
* {GUID}			|           | 1/1/2012 <-- Existing service				| Direct	  |	GotGrease?		| {GUID}	| North		 | GotGrease?	   | {GUID} 	| 6789 Help Ln	| 43.265	| -89.254	| 2	
* {GUID}			| {GUID}	| 1/2/2012 <-- Existing service w/ RS parent| Regular     | AB Couriers		| {GUID}	| West		 | AB Couriers	   | {GUID}		| 4953 Joe Way	| 44.165	| -79.365	| 4	
****************************************************************************************************************************************************************************************************************************/
CREATE PROCEDURE [dbo].[sp_GetUnroutedServicesForDate]
(@serviceProviderIdContext uniqueidentifier,
@serviceDate date)
AS
BEGIN

	--Stores the Recurring Services that are associated with the lowest context provided
	DECLARE @TempGenServiceTable TABLE
	(Id uniqueidentifier,
     EndDate date,
     EndAfterTimes int,
     RepeatEveryTimes int,
     FrequencyInt int,
     FrequencyDetailInt int,
     StartDate date,
	 NextDate date,
	 ServiceName nvarchar(max))


	 --Delete all RouteTasks in the TaskBoard so they can be regenerated
	--This is necessary 
	DELETE FROM dbo.RouteTasks 
	WHERE RouteDestinationId IS NULL 
	AND BusinessAccountId = @serviceProviderIdContext 
	AND (Date = @serviceDate AND (OriginalDate = @serviceDate OR OriginalDate IS NULL))

	INSERT INTO @TempGenServiceTable (Id, EndDate, EndAfterTimes, RepeatEveryTimes, FrequencyInt, FrequencyDetailInt, StartDate, ServiceName)
	--This is a Semi-Join between the Clients table created above and the RecurringServices Table
	--Semi-Join simply means that it has all the same logic as a normal join, but it doesnt actually join the tables
	--In this case, it finds all the RecurringServices that correspond to a Client with a vendorId = @serviceProviderIdContext
	SELECT	t1.Id, t1.EndDate, t1.EndAfterTimes, t1.RepeatEveryTimes, t1.FrequencyInt, t1.FrequencyDetailInt, t1.StartDate, t2.Name
	FROM		Repeats t1, ServiceTemplates t2
	WHERE		EXISTS
	(
		SELECT	*
		FROM	Clients
		WHERE	EXISTS
		(
		SELECT	*
		FROM	RecurringServices
		WHERE	RecurringServices.ClientId = Clients.Id 
				AND Clients.BusinessAccountId = @serviceProviderIdContext 
				AND RecurringServices.Id = t1.Id
				AND RecurringServices.Id = t2.Id
		)
	)
	
	--This table is simply the result of merging @TempGenServiceTable and @TempNextDateTable
	DECLARE @TempGenServiceTableWithNextOccurrence TABLE
	(Id uniqueidentifier,
     EndDate date,
     EndAfterTimes int,
     RepeatEveryTimes int,
     FrequencyInt int,
     FrequencyDetailInt int,
     StartDate date,
	 NextDate date,
	 ServiceName nvarchar(max))
	
	--Merges @TempGenServiceTable amd @TempNextDateTable created above based on their Id into @TempGenServiceTableWithNextOccurrence
	INSERT INTO @TempGenServiceTableWithNextOccurrence (Id, EndDate, EndAfterTimes, RepeatEveryTimes, FrequencyInt, FrequencyDetailInt, StartDate, ServiceName)
	SELECT	t1.Id, t1.EndDate, t1.EndAfterTimes, t1.RepeatEveryTimes, t1.FrequencyInt, t1.FrequencyDetailInt, t1.StartDate, t1.ServiceName
	FROM	@TempGenServiceTable t1

	UPDATE @TempGenServiceTableWithNextOccurrence
	SET NextDate = (SELECT dbo.GetNextOccurence(@serviceDate, StartDate,  EndDate, EndAfterTimes, FrequencyInt, RepeatEveryTimes, FrequencyDetailInt))
	FROM @TempGenServiceTableWithNextOccurrence

	--Remove any rows that do not have a NextOccurrence past or on the OnOrAfterDate
	DELETE FROM @TempGenServiceTableWithNextOccurrence
	WHERE NextDate IS NULL OR NextDate <> @serviceDate

	--This table will store all Existing Services after the OnOrAfterDate
	DECLARE @TempNextExistingServiceTable TABLE
	(ServiceId uniqueidentifier,
	RecurringServiceId uniqueidentifier,
	OccurDate date,
	ServiceName nvarchar(max))

	--Fills @ServicesForToday with Existing Services on the SeedDate
	INSERT INTO @TempNextExistingServiceTable (RecurringServiceId, ServiceId, OccurDate, ServiceName)
		SELECT  t1.RecurringServiceId, t1.Id, t1.ServiceDate, t2.Name
		FROM	Services t1, ServiceTemplates t2
		WHERE	t1.ServiceProviderId = @serviceProviderIdContext 
				AND t1.ServiceDate = @serviceDate
				AND t1.Id = t2.Id

	--Will store all the Services after the OnOrAfterDate
	DECLARE @tempHolderTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)

	--Here we add the next occurrences to the final temporary table
	INSERT INTO @tempHolderTable (RecurringServiceId, OccurDate, ServiceName)
	SELECT Id, NextDate, ServiceName
	FROM @TempGenServiceTableWithNextOccurrence

	INSERT INTO @tempHolderTable (RecurringServiceId, ServiceId, OccurDate, ServiceName)
	SELECT RecurringServiceId, ServiceId, OccurDate, ServiceName
	FROM @TempNextExistingServiceTable

	--This table will temporarily store the RecurringServiceId's of all rows that appear more than once in @TempTable
	DECLARE @DuplicateIdTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceDate nvarchar(max)
	)

	INSERT INTO @DuplicateIdTable
	SELECT t1.RecurringServiceId, t2.ServiceId, t2.OccurDate, t2.ServiceName FROM (SELECT OccurDate, RecurringServiceId, ServiceId FROM @tempHolderTable AS t2 WHERE ServiceId IS NOT NULL GROUP BY OccurDate, RecurringServiceId, ServiceId) t1
	JOIN @tempHolderTable as t2 on t1.RecurringServiceId = t2.RecurringServiceId AND t1.OccurDate = t2.OccurDate AND t2.ServiceId IS NULL

	DECLARE @serviceForDayTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)

	--Filtered down from the table above so that only the correct number of Services is returned from the function
	INSERT INTO @serviceForDayTable
	SELECT * FROM @tempHolderTable
	EXCEPT
	SELECT * FROM @DuplicateIdTable

	DECLARE @PreRoutedServices TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)

	--Selects all those RouteTasks that are in a RouteDestination that is in a Route that is on the specified day
	INSERT INTO @PreRoutedServices (RecurringServiceId, ServiceId, OccurDate, ServiceName)
	SELECT t1.RecurringServiceId, t1.ServiceId, t1.OccurDate, t1.ServiceName
	FROM @serviceForDayTable  t1
	WHERE EXISTS
	( 
		SELECT *
		FROM  RouteTasks t2
		WHERE EXISTS
		(
			SELECT *
			FROM RouteDestinations t3
			WHERE EXISTS
			(
				SELECT * FROM Routes t4
				WHERE t3.RouteId = t4.Id AND t2.RouteDestinationId = t3.Id AND t1.RecurringServiceId = RecurringServiceId AND t4.Date = @serviceDate
			)
		)
	)

	INSERT INTO @PreRoutedServices (RecurringServiceId, ServiceId, OccurDate, ServiceName)
	SELECT t1.RecurringServiceId, t1.Id, t1.ServiceDate, t2.Name
	FROM Services t1, ServiceTemplates t2
	WHERE	t1.ServiceProviderId = @serviceProviderIdContext 
				AND t1.ServiceDate = @serviceDate
				AND t1.Id = t2.Id
				AND EXISTS	( 
								SELECT *
								FROM  RouteTasks t2
								WHERE EXISTS
								(
									SELECT *
									FROM RouteDestinations t3
									WHERE EXISTS
									(
										SELECT * FROM Routes t4
										WHERE t3.RouteId = t4.Id AND t2.RouteDestinationId = t3.Id AND t1.Id = ServiceId AND t4.Date = @serviceDate
									)
								)
							)
						
	
	INSERT INTO @PreRoutedServices
	SELECT RecurringServiceId, ServiceId, [Date], Name
	FROM dbo.RouteTasks
	WHERE Date = @serviceDate AND BusinessAccountId = @serviceProviderIdContext AND RouteDestinationId IS NULL

	DECLARE @UnroutedOrUncompletedServices TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max),
		ClientName nvarchar(max),
		ClientId uniqueidentifier,
		LocationId uniqueidentifier,
		RegionName nvarchar(max),
		LocationName nvarchar(max),
		AddressLine nvarchar(max),
		Latitude float,
		Longitude float,
		StatusName nvarchar(max)
	) 

	INSERT INTO @UnroutedOrUncompletedServices (RecurringServiceId, ServiceId, OccurDate, ServiceName)
	SELECT * FROM @serviceForDayTable
	EXCEPT
	SELECT * FROM @PreRoutedServices

	UPDATE @UnroutedOrUncompletedServices
	SET LocationId =	(
							SELECT	TOP 1 LocationId
							FROM	Fields_LocationField t1
							WHERE	EXISTS
							(
								SELECT TOP 1	Id
								FROM	Fields t2
								WHERE	t2.Id = t1.Id 
								AND (t2.ServiceTemplateId = [@UnroutedOrUncompletedServices].RecurringServiceId OR t2.ServiceTemplateId = [@UnroutedOrUncompletedServices].ServiceId)
								AND LocationFieldTypeInt = 0
							)
						)

	UPDATE @UnroutedOrUncompletedServices
	SET RegionName =	(
							SELECT	DISTINCT Name
							FROM	Regions t1
							WHERE	EXISTS
							(
								SELECT	RegionId
								FROM	Locations t2
								WHERE	t2.RegionId = t1.Id 
								AND		LocationId = t2.Id
							)
						)


	UPDATE	@UnroutedOrUncompletedServices
	SET		AddressLine = t1.AddressLineOne, 
			LocationName = t1.Name,
			LocationId = t1.Id,
			Latitude = t1.Latitude, 
			Longitude = t1.Longitude
	FROM	Locations t1
	WHERE	t1.Id = LocationId

	UPDATE	@UnroutedOrUncompletedServices
	SET		ClientName =	(
							SELECT DISTINCT t1.Name
							FROM	dbo.Clients t1
							WHERE	t1.Id IN 
							(
								SELECT  t2.ClientId
								FROM	RecurringServices t2
								WHERE	RecurringServiceId = t2.Id 
								AND		t2.ClientId = t1.Id
							)
							)

	UPDATE	@UnroutedOrUncompletedServices
	SET		ClientName =	(
							SELECT DISTINCT  t1.Name
							FROM	dbo.Clients t1
							WHERE	t1.Id IN 
							(
								SELECT  t2.ClientId
								FROM	Services t2
								WHERE	ServiceId = t2.Id 
								AND		t2.ClientId = t1.Id
							)
							)
	WHERE ClientName IS NULL							

	UPDATE	@UnroutedOrUncompletedServices
	SET		ClientId =		(
							SELECT	DISTINCT ClientId
							FROM	RecurringServices t1
							WHERE	RecurringServiceId = t1.Id
							)

	UPDATE	@UnroutedOrUncompletedServices
	SET		ClientId =		(
							SELECT	DISTINCT ClientId
							FROM	Services t1
							WHERE	ServiceId = t1.Id
							)
	WHERE ClientId IS NULL

	DECLARE @ServicesForDateTable TABLE
		(
			RecurringServiceId uniqueidentifier,
			ServiceId uniqueidentifier,
			OccurDate date,
			ServiceName nvarchar(max),
			ClientName nvarchar(max),
			ClientId uniqueidentifier,
			RegionName nvarchar(max),
			LocationName nvarchar(max),
			LocationId uniqueidentifier,
			AddressLine nvarchar(max),
			Latitude decimal(18,8),
			Longitude decimal(18,8),
			StatusName nvarchar(max)
		) 

	--This will be a complete table of all services that should have been scheduled for the date provided
	--This does not take into account dates that have been excluded
	INSERT @ServicesForDateTable (RecurringServiceId, ServiceId, OccurDate, ServiceName, ClientName, ClientId, RegionName, LocationName, LocationId, AddressLine, Latitude, Longitude, StatusName)
	SELECT RecurringServiceId, ServiceId, OccurDate, ServiceName, ClientName, ClientId, RegionName, LocationName, LocationId, AddressLine, Latitude, Longitude, StatusName FROM @UnroutedOrUncompletedServices

-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--Now that we have all the services that would have been on the date provided, we will take ExcludedDates into account
-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	--Table will hold all Ids and ExcludedDatesStrings
	--Only those RecurringServices that have been scheduled for the date provided and have an ExcludedDatesString will appear
	DECLARE @RecurringServicesWithExcludedDates TABLE
	(
		Id uniqueidentifier,
		ExcludedDatesString nvarchar(max)
	)

	INSERT INTO @RecurringServicesWithExcludedDates
	SELECT t1.Id, t1.ExcludedDatesString FROM RecurringServices t1, @ServicesForDateTable t2
	WHERE t1.Id = t2.RecurringServiceId AND t1.ExcludedDatesString IS NOT NULL

	DECLARE @RecurringServicesWithExcludedDatesSplit TABLE
	(
		Id uniqueidentifier,
		ExcludedDate nvarchar(max)
	)

	DECLARE @RowCount int --Row count for @RecurringServicesWithExcludedDates (We delete from this table as we input into @RecurringServicesWithExcludedDatesSplit)
	DECLARE @RowId  uniqueidentifier --RecurringServiceId of the current row
	DECLARE @RowExcludedDateString nvarchar(max) --ExcludedDatesString for the current row

	SET @RowCount = (SELECT COUNT(*) FROM @RecurringServicesWithExcludedDates)

	WHILE @RowCount > 0
	BEGIN 
		SET @RowId = (SELECT TOP(1) Id FROM @RecurringServicesWithExcludedDates ORDER BY Id) --Find the RowId of the top row sorted by Id
		SET @RowExcludedDateString = (SELECT ExcludedDatesString FROM @RecurringServicesWithExcludedDates WHERE Id = @RowId) --Find the ExcludedDatesString of the top row found above

		--Converts the ExcludedDateString to a Table(See example below for more information)
		/****************************************************************************************************************************************************
		* FUNCTION Split will convert the comma separated string of dates ()
		** Input Parameters **
		* @Id - RecurringServiceId
		* @sInputList - List of delimited ExcludedDates
		* @sDelimiter - -- Delimiter that separates ExcludedDates
		** Output Parameters: **
		*  @List TABLE (Id uniqueidentifier, ExcludedDate VARCHAR(8000)) - Ex. below
		* Id                                     | ExcludedDate
		* -----------------------------------------------------------------------------------------
		* {036BD670-39A5-478F-BFA3-AD312E3F7F47} | 1/1/2012
		* {B30A43AD-655A-449C-BD4E-951F8F988718} | 1/1/2012
		* {03DB9F9B-2FF6-4398-B984-533FB3E19C50} | 1/2/2012
		***************************************************************************************************************************************************/
		INSERT INTO @RecurringServicesWithExcludedDatesSplit
		SELECT * FROM [dbo].[Split] (
								@RowId,
								@RowExcludedDateString,
								','
								)
		
		--Now that we have converted this row, remove it from @RecurringServicesWithExcludedDates
		DELETE FROM @RecurringServicesWithExcludedDates
		WHERE Id = @RowId

		--Reset @RowCount for the loop condition
		SET @RowCount = (SELECT COUNT(*) FROM @RecurringServicesWithExcludedDates)
	END

	--Table that will hold all information about Services that have been excluded
	DECLARE @SevicesThatHaveBeenExcluded TABLE
		(
			RecurringServiceId uniqueidentifier,
			ServiceId uniqueidentifier,
			OccurDate date,
			ServiceName nvarchar(max),
			ClientName nvarchar(max),
			ClientId uniqueidentifier,
			RegionName nvarchar(max),
			LocationName nvarchar(max),
			LocationId uniqueidentifier,
			AddressLine nvarchar(max),
			Latitude decimal(18,8),
			Longitude decimal(18,8),
			StatusName nvarchar(max)
		) 

	--Find all ExcludedServices from @ServicesForDateTable
	INSERT INTO @SevicesThatHaveBeenExcluded
	SELECT t1.* FROM @ServicesForDateTable t1, @RecurringServicesWithExcludedDatesSplit t2
	WHERE t1.RecurringServiceId = t2.Id AND t1.OccurDate = t2.ExcludedDate

	CREATE TABLE #ServicesTableToReturn
(
		Id UNIQUEIDENTIFIER,
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max),
		ClientName nvarchar(max),
		ClientId UNIQUEIDENTIFIER ,
		RegionName nvarchar(max),
		LocationName nvarchar(max),
		LocationId uniqueidentifier,
		AddressLine nvarchar(max),
		Latitude decimal(18,8),
		Longitude decimal(18,8),
		StatusName nvarchar(max)
	) 

	--Add all services that have not been excluded to the output table
	INSERT INTO #ServicesTableToReturn(RecurringServiceId, ServiceId, OccurDate, ServiceName, ClientName, ClientId, RegionName, LocationName, LocationId, AddressLine, Latitude, Longitude, StatusName)
	SELECT * FROM @ServicesForDateTable
	EXCEPT
	SELECT * FROM @SevicesThatHaveBeenExcluded

	UPDATE #ServicesTableToReturn
	SET Id = NEWID()

	CREATE TABLE #RouteTasks
			( Id UNIQUEIDENTIFIER,
			  LocationId UNIQUEIDENTIFIER,
			  RouteDestinationId UNIQUEIDENTIFIER,
			  ClientId UNIQUEIDENTIFIER,
			  ServiceId UNIQUEIDENTIFIER,
			  BusinessAccountId UNIQUEIDENTIFIER,
			  EstimatedDuration TIME,
			  Name NVARCHAR(MAX),
			  StatusInt INT,
			  [Date] DATETIME,
			  OrderInRouteDestination INT,
			  RecurringServiceId UNIQUEIDENTIFIER,
			  DelayedChildId UNIQUEIDENTIFIER,
			  TaskStatusId UNIQUEIDENTIFIER,
			  CreatedDate DATETIME
			)

	INSERT INTO #RouteTasks(Id, LocationId, ClientId, ServiceId, Name, [Date], RecurringServiceId)
	SELECT Id, LocationId, ClientId, ServiceId, ServiceName, OccurDate, RecurringServiceId
	FROM #ServicesTableToReturn

	UPDATE #RouteTasks
	SET BusinessAccountId = @serviceProviderIdContext,
		EstimatedDuration = '0:0:16.00',
		OrderInRouteDestination = 0,
		TaskStatusId = (SELECT TOP 1 Id FROM dbo.TaskStatuses WHERE BusinessAccountId = @serviceProviderIdContext AND DefaultTypeInt = 1),
		StatusInt = 0,
		CreatedDate = GETUTCDATE()

	INSERT INTO dbo.RouteTasks (Id, LocationId, RouteDestinationId, ClientId, ServiceId, BusinessAccountId, EstimatedDuration, Name, StatusInt, [Date], OrderInRouteDestination, RecurringServiceId, DelayedChildId, TaskStatusId, CreatedDate)
	SELECT * FROM #RouteTasks

	SELECT * FROM dbo.RouteTasks
	LEFT JOIN dbo.Locations
	ON dbo.RouteTasks.LocationId = dbo.Locations.Id
	LEFT JOIN dbo.Regions
	ON dbo.Locations.RegionId = dbo.Regions.Id
	LEFT JOIN dbo.Clients
	ON dbo.RouteTasks.ClientId = dbo.Clients.Id  
	LEFT JOIN dbo.TaskStatuses
	ON dbo.RouteTasks.TaskStatusId = dbo.TaskStatuses.Id
	WHERE dbo.RouteTasks.BusinessAccountId = @serviceProviderIdContext AND [Date] = @serviceDate AND RouteDestinationId IS NULL 

	DROP TABLE #ServicesTableToReturn
	DROP TABLE #RouteTasks
RETURN 
END