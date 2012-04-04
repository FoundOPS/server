/****************************************************************************************************************************************************
* FUNCTION GetUnroutedServicesForDate will take the context provided and find all the services that are scheduled for that day
** Input Parameters **
* @serviceProviderIdContext - The BusinessAccount context
* @firstDateToLookForServices - The reference date to look for services
** Output Parameters: **
* @ServicesTableToReturn - Ex. below
* RecurringServiceId                     | ServiceId                              | OccurDate
* -----------------------------------------------------------------------------------------
* {036BD670-39A5-478F-BFA3-AD312E3F7F47} |                                        | 1/1/2012 <-- Generated service
* {B30A43AD-655A-449C-BD4E-951F8F988718} |                                        | 1/1/2012 <-- Existing service
* {03DB9F9B-2FF6-4398-B984-533FB3E19C50} | {FC222C74-EFEA-4B45-93FB-B042E6D6DB0D} | 1/2/2012 <-- Existing service with a RecurringService parent **
***************************************************************************************************************************************************/
CREATE FUNCTION [dbo].[GetUnroutedServicesForDate]
(@serviceProviderIdContext uniqueidentifier,
@serviceDate date)
RETURNS @ServicesTableToReturn TABLE
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
		Longitude decimal(18,8)
	) 
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
				AND Clients.VendorId = @serviceProviderIdContext 
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

	--This table will store all Existing Services after to the OnOrAfterDate
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

	DECLARE @UnroutedServices TABLE
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
		Longitude float
	) 

	INSERT INTO @UnroutedServices (RecurringServiceId, ServiceId, OccurDate, ServiceName)
	SELECT * FROM @serviceForDayTable
	EXCEPT
	SELECT * FROM @PreRoutedServices

	UPDATE @UnroutedServices
	SET LocationId =	(
							SELECT	LocationId
							FROM	Fields_LocationField t1
							WHERE	EXISTS
							(
								SELECT	Id
								FROM	Fields t2
								WHERE	t2.Id = t1.Id 
								AND t2.ServiceTemplateId = RecurringServiceId 
								AND LocationFieldTypeInt = 0
							)
						)

	UPDATE @UnroutedServices
	SET RegionName =	(
							SELECT	Name
							FROM	Regions t1
							WHERE	EXISTS
							(
								SELECT	RegionId
								FROM	Locations t2
								WHERE	t2.RegionId = t1.Id 
								AND		LocationId = t2.Id
							)
						)


	UPDATE	@UnroutedServices
	SET		AddressLine = t1.AddressLineOne, 
			LocationName = t1.Name,
			LocationId = t1.Id,
			Latitude = t1.Latitude, 
			Longitude = t1.Longitude
	FROM	Locations t1
	WHERE	t1.Id = LocationId

	UPDATE	@UnroutedServices
	SET		ClientName =	(
							SELECT t1.Name
							FROM	Parties_Business t1
							WHERE	EXISTS
							(
								SELECT  ClientId
								FROM	RecurringServices t2
								WHERE	RecurringServiceId = t2.Id AND t2.ClientId = t1.Id
							)
							)
	UPDATE	@UnroutedServices
	SET		ClientId =		(
							SELECT	ClientId
							FROM	RecurringServices t1
							WHERE	RecurringServiceId = t1.Id
							)

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
			Longitude decimal(18,8)
		) 

	--This will be a complete table of all services that should have been scheduled for the date provided
	--This does not take into account dates that have been excluded
	INSERT @ServicesForDateTable (RecurringServiceId, ServiceId, OccurDate, ServiceName, ClientName, ClientId, RegionName, LocationName, LocationId, AddressLine, Latitude, Longitude)
	SELECT RecurringServiceId, ServiceId, OccurDate, ServiceName, ClientName, ClientId, RegionName, LocationName, LocationId, AddressLine, Latitude, Longitude FROM @UnroutedServices

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
			Longitude decimal(18,8)
		) 

	--Find all ExcludedServices from @ServicesForDateTable
	INSERT INTO @SevicesThatHaveBeenExcluded
	SELECT t1.* FROM @ServicesForDateTable t1, @RecurringServicesWithExcludedDatesSplit t2
	WHERE t1.RecurringServiceId = t2.Id AND t1.OccurDate = t2.ExcludedDate

	--Add all services that have not been excluded to the output table
	INSERT INTO @ServicesTableToReturn
	SELECT * FROM @ServicesForDateTable
	EXCEPT
	SELECT * FROM @SevicesThatHaveBeenExcluded

RETURN 
END