/****************************************************************************************************************************************************
* FUNCTION will take the context provided and find all the service dates
* for RecurringServices and existing Services with that context. The function will return past, future or both past and future Services.
* For RecurringServices, if there are existing services for dates it will return those. Otherwise it will generate a date for the instances.
* You can distinguish generated services because they will not have a ServiceId *
** Input Parameters **
* @serviceProviderIdContext - The BusinessAccount context or NULL 
* @clientIdContext - The Client context or NULL 
* @recurringServiceIdContext - The RecurringService context or NULL
* @seedDate - The reference date to look for services before or after. Also known as the onOrBeforeDate and the onOrAfterDate
* @numberOfOccurrences - The number of occurrences to return
* @getPrevious - If set to 1, this will return previous services
* @getNext - If set to 1, this will return future services
** Output Parameters: **
* @ServicesTableToReturn - Ex. below
* RecurringServiceId                     | ServiceId                              | OccurDate
* -----------------------------------------------------------------------------------------
* {036BD670-39A5-478F-BFA3-AD312E3F7F47} |                                        | 1/1/2012 <-- Generated service
* {B30A43AD-655A-449C-BD4E-951F8F988718} |                                        | 1/1/2012 <-- Existing service
* {03DB9F9B-2FF6-4398-B984-533FB3E19C50} | {FC222C74-EFEA-4B45-93FB-B042E6D6DB0D} | 1/2/2012 <-- Existing service with a RecurringService parent **
***************************************************************************************************************************************************/
CREATE FUNCTION [dbo].[GetServiceHolders]
(@serviceProviderIdContext uniqueidentifier,
@clientIdContext uniqueidentifier,
@recurringServiceIdContext uniqueidentifier,
@seedDate date,
@frontBackMinimum int,
@getPrevious bit,
@getNext bit)
--TODO RENAME TempTable to ...
--TempTable is where we will put all the Services and their corresponding dates
RETURNS @ServicesTableToReturn TABLE
(
	RecurringServiceId uniqueidentifier,
	ServiceId uniqueidentifier,
	OccurDate date,
	ServiceName nvarchar(max)
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

	 --Insert all (the single) RecurringService associated with the @recurringServiceIdContext to @TempGenServiceTable
	IF @recurringServiceIdContext IS NOT NULL
	BEGIN		
	INSERT INTO @TempGenServiceTable (Id, EndDate, EndAfterTimes, RepeatEveryTimes, FrequencyInt, FrequencyDetailInt, StartDate, ServiceName)
	SELECT	t1.Id, t1.EndDate, t1.EndAfterTimes, t1.RepeatEveryTimes, t1.FrequencyInt, t1.FrequencyDetailInt, t1.StartDate, t2.Name
	FROM	Repeats t1, ServiceTemplates t2
	WHERE	@recurringServiceIdContext = t1.Id
			AND @recurringServiceIdContext = t2.Id
	END
	--Insert all (the single) RecurringServices associated with the @clientIdContext to @TempGenServiceTable
	ELSE IF @clientIdContext IS NOT NULL
	BEGIN
	INSERT INTO @TempGenServiceTable (Id, EndDate, EndAfterTimes, RepeatEveryTimes, FrequencyInt, FrequencyDetailInt, StartDate, ServiceName)
	--This is a Semi-Join between the Repeats table created above and the RecurringServices Table
	--Semi-Join simply means that it has all the same logic as a normal join, but it doesnt actually join the tables
	--In this case, it finds all the Repeats that correspond to a RecurringService with a ClientId = @clientIdContext
	SELECT t1.Id, t1.EndDate, t1.EndAfterTimes, t1.RepeatEveryTimes, t1.FrequencyInt, t1.FrequencyDetailInt, t1.StartDate, t2.Name
	FROM		Repeats t1, ServiceTemplates t2
	WHERE		EXISTS
	(
		SELECT	* 
		FROM	RecurringServices
		WHERE	ClientId = @clientIdContext
				AND RecurringServices.Id = t1.Id
				AND RecurringServices.Id = t2.Id
	)
	END
	
	--Insert all (the single) RecurringServices associated with the @serviceProviderIdContext to @TempGenServiceTable
	ELSE
	BEGIN
	INSERT INTO @TempGenServiceTable (Id, EndDate, EndAfterTimes, RepeatEveryTimes, FrequencyInt, FrequencyDetailInt, StartDate, ServiceName)
	--This is a Semi-Join between the Clients table created above and the RecurringServices Table
	--Semi-Join simply means that it has all the same logic as a normal join, but it doesnt actually join the tables
	--In this case, it finds all the RecurringServices that correspond to a Client with a BusinessAccountId = @serviceProviderIdContext
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
	END

	--This table will hold the Recurring Services and Existing Services that occur on @seedDate
	DECLARE @ServicesForToday TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)

------------------------------------------------------------------------------------------------------------------------------------------
--This section will find the first occurrence of all the RecurringServices on or after the SeedDate
------------------------------------------------------------------------------------------------------------------------------------------
	IF @getNext = 1
	BEGIN
		--This table is simply the result of merging @TempGenServiceTable and @TempNextDateTable
		DECLARE @NextGenServices TABLE
		(Id uniqueidentifier,
		 EndDate date,
		 EndAfterTimes int,
		 RepeatEveryTimes int,
		 FrequencyInt int,
		 FrequencyDetailInt int,
		 StartDate date,
		 NextDate date,
		 ServiceName nvarchar(max))
	 
		--Merges @TempGenServiceTable amd @TempNextDateTable created above based on their Id into @NextGenServices
		INSERT INTO @NextGenServices (Id, EndDate, EndAfterTimes, RepeatEveryTimes, FrequencyInt, FrequencyDetailInt, StartDate, ServiceName)
		SELECT	t1.Id, t1.EndDate, t1.EndAfterTimes, t1.RepeatEveryTimes, t1.FrequencyInt, t1.FrequencyDetailInt, t1.StartDate, t1.ServiceName
		FROM	@TempGenServiceTable t1

		UPDATE @NextGenServices
		SET NextDate = (SELECT dbo.GetNextOccurence(@seedDate, StartDate,  EndDate, EndAfterTimes, FrequencyInt, RepeatEveryTimes, FrequencyDetailInt))
		FROM @NextGenServices

		--Puts all the Generated Services that occur today in a separate table
		INSERT INTO @ServicesForToday (RecurringServiceId, OccurDate, ServiceName)
		SELECT t1.Id, t1.NextDate, t1.ServiceName
		FROM @NextGenServices t1
		WHERE t1.NextDate = @seedDate

		UPDATE @NextGenServices
		SET NextDate = (SELECT dbo.GetNextOccurence(DATEADD(day, 1, @seedDate), StartDate,  EndDate, EndAfterTimes, FrequencyInt, RepeatEveryTimes, FrequencyDetailInt))
		FROM @NextGenServices
		WHERE NextDate = @seedDate

		--Remove any rows that do not have a NextOccurrence past or on the OnOrAfterDate
		DELETE FROM @NextGenServices
		WHERE NextDate IS NULL

	END
------------------------------------------------------------------------------------------------------------------------------------------
--This section will find the first occurrence of all the RecurringServices on or before the SeedDate
------------------------------------------------------------------------------------------------------------------------------------------
	IF @getPrevious = 1
	BEGIN
		DECLARE @previousDayToLookForServices date

		--Subtracts one day to @seedDate if you are looking for both Next and Previous
		--If we were to omit this, the GetPreviousOccurrence and GetNextOccurrence functions
		--would both include the original @seedDate
		--Thus throwing off the output of this function
		IF @getNext = 1 AND @getPrevious = 1
			SET @previousDayToLookForServices = DATEADD (day, -1, @seedDate)
		ELSE
			SET @previousDayToLookForServices = @seedDate
							
		--This table is simply the result of merging @TempGenServiceTable and @TempPreviousDateTable
		DECLARE @PreviousGenServices TABLE
		(Id uniqueidentifier,
		 EndDate date,
		 EndAfterTimes int,
		 RepeatEveryTimes int,
		 FrequencyInt int,
		 FrequencyDetailInt int,
		 StartDate date,
		 PreviousDate date,
		 ServiceName nvarchar(max))
	
		--Merges @TempGenServiceTable amd @TempPreviousDateTable created above based on their Id into @NextGenServices
		INSERT INTO @PreviousGenServices (Id, EndDate, EndAfterTimes, RepeatEveryTimes, FrequencyInt, FrequencyDetailInt, StartDate, ServiceName)
		SELECT	t1.Id, t1.EndDate, t1.EndAfterTimes, t1.RepeatEveryTimes, t1.FrequencyInt, t1.FrequencyDetailInt, t1.StartDate, t1.ServiceName
		FROM	@TempGenServiceTable t1

		UPDATE @PreviousGenServices
		SET PreviousDate = (SELECT dbo.GetPreviousOccurrence(@previousDayToLookForServices, StartDate,  EndDate, EndAfterTimes, FrequencyInt, RepeatEveryTimes, FrequencyDetailInt))
		FROM @PreviousGenServices

		IF @getNext = 0
		BEGIN
			--Puts all the Generated Services that occur today in a separate table
			INSERT INTO @ServicesForToday (RecurringServiceId, OccurDate, ServiceName)
			SELECT t1.Id, t1.PreviousDate, t1.ServiceName
			FROM @PreviousGenServices t1
			WHERE t1.PreviousDate = @seedDate

			UPDATE @PreviousGenServices
			SET PreviousDate = (SELECT dbo.GetNextOccurence(DATEADD(day, -1, @seedDate), StartDate,  EndDate, EndAfterTimes, FrequencyInt, RepeatEveryTimes, FrequencyDetailInt))
			FROM @PreviousGenServices
			WHERE PreviousDate = @seedDate

			--Remove any rows that do not have a NextOccurrence past or on the OnOrAfterDate
			DELETE FROM @PreviousGenServices
			WHERE PreviousDate IS NULL
		END
		ELSE
		--Remove any rows that do not have a PreviousOccurrence on or before the OnOrBeforeDate
		DELETE FROM @PreviousGenServices
		WHERE PreviousDate IS NULL
	END
------------------------------------------------------------------------------------------------------------------------------------------
	DECLARE @CombinedGenServices TABLE
		(Id uniqueidentifier,
		 EndDate date,
		 EndAfterTimes int,
		 RepeatEveryTimes int,
		 FrequencyInt int,
		 FrequencyDetailInt int,
		 StartDate date,
		 OccurDate date,
		 ServiceName nvarchar(max))

	INSERT INTO @CombinedGenServices (Id, OccurDate, ServiceName)
	SELECT Id, PreviousDate, ServiceName  FROM @PreviousGenServices
	
	INSERT INTO @CombinedGenServices (Id, OccurDate, ServiceName)
	SELECT Id, NextDate, ServiceName  FROM @NextGenServices

	--Table will hold all Ids and ExcludedDatesStrings
	--Only those RecurringServices that have been scheduled for the date provided and have an ExcludedDatesString will appear
	DECLARE @RecurringServicesWithExcludedDates TABLE
	(
		Id uniqueidentifier,
		ExcludedDatesString nvarchar(max)
	)

	INSERT INTO @RecurringServicesWithExcludedDates
	SELECT DISTINCT t1.Id, t1.ExcludedDatesString FROM RecurringServices t1, @CombinedGenServices t2
	WHERE t1.Id = t2.Id AND t1.ExcludedDatesString IS NOT NULL	

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

------------------------------------------------------------------------------------------------------------------------------------------
--Here we will take the Recurring Services that are in @NextGenServices and find all occurrences in date order until
--we get to the number specified by @frontBackMinimum in the future
--We will also do the same thing with @PreviousGenServices except in reverse date order
------------------------------------------------------------------------------------------------------------------------------------------
	DECLARE @minVal date
	DECLARE @lastDateToLookFor date
	DECLARE @firstDateToLookFor date
	DECLARE @ServiceCount int
	DECLARE @NextDay date
	DECLARE @RowCountForRecurringServiceOccurrenceTable int
	DECLARE @RowCountForTempGenServiceTableWithNextOccurrence int
	DECLARE @RowCountForTempGenServiceTableWithPreviousOccurrence int
	DECLARE @remainingNumberOfServicesToFind int

	--Table to temporarily store all the Recurring Services dates in the future, deleted after each iteration of the loop
	DECLARE @TempNextRecurringServiceOccurrenceTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)

	--Table to temporarily store all the Recurring Services dates in the past, deleted after each iteration of the loop
	DECLARE @TempPreviousRecurringServiceOccurrenceTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)

	--Collection of all services that were temporarily stored in @TempNextRecurringServiceOccurrenceTable
	DECLARE @NextRecurringServiceOccurrenceTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)

	--Collection of all services that were temporarily stored in @TempPreviousRecurringServiceOccurrenceTable
	DECLARE @PreviousRecurringServiceOccurrenceTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)

	--Preset to a value above 0 just so it makes it into the loop. Actual value is calculated there
	SET		@RowCountForTempGenServiceTableWithPreviousOccurrence = 1
	SET		@RowCountForRecurringServiceOccurrenceTable = 0

	--Preset to @seedDate
	SET		@NextDay = @seedDate

	IF		@GetPrevious = 1
	BEGIN
	--Will loop until it finds @@frontBackMinimum services in the past
	WHILE	@RowCountForRecurringServiceOccurrenceTable <= @frontBackMinimum
	BEGIN
		SET		@minVal = (SELECT MAX(PreviousDate) FROM @PreviousGenServices)

		SET		@NextDay = DATEADD(day, -1, @minVal)

		SET		@RowCountForTempGenServiceTableWithPreviousOccurrence = (SELECT COUNT(*) FROM @PreviousGenServices)

		--Checks to be sure that there are still rows to look at in @NextGenServices
		IF NOT	@RowCountForTempGenServiceTableWithPreviousOccurrence > 0
		BEGIN
				BREAK
		END
		
		DECLARE @TempPreviousShouldAddTable TABLE
		(
			RecurringServiceId uniqueidentifier,
			OccurDate date,
			ServiceName nvarchar(max)
		)

		--Inserts all Services with the lowest date to @PreviousRecurringServiceOccurrenceTable
		INSERT INTO @TempPreviousRecurringServiceOccurrenceTable
		SELECT DISTINCT	t1.Id, t1.PreviousDate, t1.ServiceName
		FROM	@PreviousGenServices t1
		WHERE	t1.PreviousDate = @minVal

		--Will hold all the services that need to be removed from the final list based on the RecurringService's ExcludedDates
		DECLARE @PreviousServicesToRemove TABLE
		(
				RecurringServiceId uniqueidentifier,
				OccurDate date,
				ServiceName nvarchar(max)
		)

		--Inserts all Services that need to be removed to @PreviousServicesToRemove
		INSERT INTO @PreviousServicesToRemove
		SELECT DISTINCT * FROM @TempPreviousShouldAddTable t3
		WHERE EXISTS
		(
			SELECT DISTINCT t1.Id
			FROM @RecurringServicesWithExcludedDatesSplit t1
			WHERE EXISTS
			(
				SELECT DISTINCT t2.RecurringServiceId
				FROM @TempPreviousShouldAddTable t2
				WHERE (t1.Id = t2.RecurringServiceId AND t1.ExcludedDate = t2.OccurDate)
			) AND t1.ExcludedDate = t3.OccurDate AND t1.Id = t3.RecurringServiceId
		) 

		--Takes all ExcludedDate services out of the list of Next Services to return
		INSERT INTO @TempPreviousRecurringServiceOccurrenceTable
		SELECT DISTINCT * FROM @TempPreviousShouldAddTable
		EXCEPT
		SELECT DISTINCT * FROM @PreviousServicesToRemove

		--Add the temporary list of @TempPreviousRecurringServiceOccurrenceTable to the final list of @PreviousRecurringServiceOccurrenceTable
		INSERT INTO @PreviousRecurringServiceOccurrenceTable
		SELECT DISTINCT * FROM @TempPreviousRecurringServiceOccurrenceTable

		--Updates all Services that were just put into a new table to show their previous occurrence date
		UPDATE	@PreviousGenServices
		SET		PreviousDate = (SELECT dbo.GetPreviousOccurrence(@NextDay, StartDate,  EndDate, EndAfterTimes, FrequencyInt, RepeatEveryTimes, FrequencyDetailInt))
		FROM	@PreviousGenServices
		WHERE	PreviousDate = @minVal

		--If any of those Services updated above do not have a previous occurrence, they will be deleted from the table
		DELETE FROM @PreviousGenServices
		WHERE	PreviousDate IS NULL

		--Sets up row count so that when the loop returns to the top, we will know if there are any rows remaining
		SET		@RowCountForRecurringServiceOccurrenceTable = (SELECT COUNT(*) FROM @PreviousRecurringServiceOccurrenceTable)

		DELETE FROM @PreviousServicesToRemove
		DELETE FROM @TempPreviousShouldAddTable
		DELETE FROM @TempPreviousRecurringServiceOccurrenceTable
	END
	END

	--Preset to a value above 0 just so it makes it into the loop. Actual value is calculated there
	SET		@RowCountForTempGenServiceTableWithNextOccurrence = 1
	SET		@RowCountForRecurringServiceOccurrenceTable = 0

	--Preset to @seedDate
	SET		@NextDay = @seedDate

	IF		@GetNext = 1
	BEGIN
	--Will loop until it fills in remaining number of Services to get to @numberOfOccurrences
	WHILE	@RowCountForRecurringServiceOccurrenceTable <= @frontBackMinimum
	BEGIN

		SET		@minVal = (SELECT MIN(NextDate) FROM @NextGenServices)
		SET		@NextDay = DATEADD(day, 1, @minVal)

		SET		@RowCountForTempGenServiceTableWithNextOccurrence = (SELECT COUNT(*) FROM @NextGenServices)

		--Checks to be sure that there are still rows to look at in @NextGenServices
		IF NOT	@RowCountForTempGenServiceTableWithNextOccurrence > 0
		BEGIN
			BREAK
		END

		DECLARE @TempNextShouldAddTable TABLE
		(
			RecurringServiceId uniqueidentifier,
			OccurDate date,
			ServiceName nvarchar(max)
		)

		--Inserts all  Services with the lowest date to @TempNextShouldAddTable
		INSERT INTO @TempNextShouldAddTable
		SELECT DISTINCT	t1.Id, t1.NextDate, t1.ServiceName
		FROM	@NextGenServices t1
		WHERE	NextDate = @minVal 

		--Will hold all the services that need to be removed from the final list based on the RecurringService's ExcludedDates
		DECLARE @NextServicesToRemove TABLE
		(
				RecurringServiceId uniqueidentifier,
				OccurDate date,
				ServiceName nvarchar(max)
		)

		--Inserts all Services that need to be removed to @NextServicesToRemove
		INSERT INTO @NextServicesToRemove
		SELECT DISTINCT * FROM @TempNextShouldAddTable t3
		WHERE EXISTS
		(
			SELECT DISTINCT t1.Id
			FROM @RecurringServicesWithExcludedDatesSplit t1
			WHERE EXISTS
			(
				SELECT DISTINCT t2.RecurringServiceId
				FROM @TempNextShouldAddTable t2
				WHERE (t1.Id = t2.RecurringServiceId AND t1.ExcludedDate = t2.OccurDate)
			) AND t1.ExcludedDate = t3.OccurDate AND t1.Id = t3.RecurringServiceId
		)

		--Takes all ExcludedDate services out of the list of Next Services to return
		INSERT INTO @TempNextRecurringServiceOccurrenceTable
		SELECT DISTINCT * FROM @TempNextShouldAddTable
		EXCEPT
		SELECT DISTINCT * FROM @NextServicesToRemove

		--Add the temporary list of @TempNextRecurringServiceOccurrenceTable to the final list of @NextRecurringServiceOccurrenceTable
		INSERT INTO @NextRecurringServiceOccurrenceTable
		SELECT DISTINCT * FROM @TempNextRecurringServiceOccurrenceTable

		--Updates all Services that were just put into a new table to show their next occurrence date
		UPDATE	@NextGenServices
		SET		NextDate = (SELECT dbo.GetNextOccurence(@NextDay, StartDate,  EndDate, EndAfterTimes, FrequencyInt, RepeatEveryTimes, FrequencyDetailInt))
		FROM	@NextGenServices
		WHERE	NextDate = @minVal

		--If any of those Services updated above do not have a next occurrence, they will be deleted from the table
		DELETE FROM @NextGenServices
		WHERE	NextDate IS NULL

		--Sets up row count so that when the loop returns to the top, we will know if there are any rows remaining
		SET		@RowCountForRecurringServiceOccurrenceTable = (SELECT COUNT(*) FROM @NextRecurringServiceOccurrenceTable)
		
		DELETE FROM @NextServicesToRemove
		DELETE FROM @TempNextShouldAddTable
		DELETE FROM @TempNextRecurringServiceOccurrenceTable
	END
	END

	SET		@lastDateToLookFor = (SELECT MAX(OccurDate) FROM @NextRecurringServiceOccurrenceTable)
	SET		@firstDateToLookFor = (SELECT MIN(OccurDate) FROM @PreviousRecurringServiceOccurrenceTable)
------------------------------------------------------------------------------------------------------------------------------------------
--Remove all ExcludedDates where the date is @seedDate
------------------------------------------------------------------------------------------------------------------------------------------
	DECLARE @ServicesForTodayExcluded TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)

	INSERT INTO @ServicesForTodayExcluded
	SELECT t1.* FROM @ServicesForToday t1, @RecurringServicesWithExcludedDatesSplit t2
	WHERE t1.RecurringServiceId = t2.Id AND t1.OccurDate = t2.ExcludedDate

	DECLARE @ServicesForTodayWithoutExcluded TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)

	INSERT INTO @ServicesForTodayWithoutExcluded
	SELECT * FROM @ServicesForToday
	EXCEPT
	SELECT * FROM @ServicesForTodayExcluded
------------------------------------------------------------------------------------------------------------------------------------------
--Here we will add Existing Services to tables of their own
--Just as in the RecurringServiceTables, we will have separate tables for Previous, Next, and SeedDate
------------------------------------------------------------------------------------------------------------------------------------------
	--This table will store all Existing Services prior to the OnOrAfterDate
	DECLARE @PreviousExistingServiceTable TABLE
	(ServiceId uniqueidentifier,
	RecurringServiceId uniqueidentifier,
	OccurDate date,
	ServiceName nvarchar(max))

	--This table will store all Existing Services after to the OnOrAfterDate
	DECLARE @NextExistingServiceTable TABLE
	(ServiceId uniqueidentifier,
	RecurringServiceId uniqueidentifier,
	OccurDate date,
	ServiceName nvarchar(max))

	DECLARE @dayBeforeSeedDate date
	DECLARE @dayAfterSeedDate date
	SET @dayBeforeSeedDate = DATEADD(Day, -1, @seedDate)
	SET @dayAfterSeedDate = DATEADD(Day, 1, @seedDate)

	IF @recurringServiceIdContext IS NOT NULL
	BEGIN
		--Fills @PreviousExistingServiceTable with existing Services in the past
		INSERT INTO @PreviousExistingServiceTable (ServiceId, RecurringServiceId, OccurDate, ServiceName)
		SELECT  t1.Id, t1.RecurringServiceId, t1.ServiceDate, t2.Name
		FROM	Services t1, ServiceTemplates t2
		WHERE	t1.RecurringServiceId = @recurringServiceIdContext 
				AND t1.ServiceDate BETWEEN @firstDateToLookFor AND @dayBeforeSeedDate
				AND t1.Id = t2.Id

		--Fills @NextExistingServiceTable with existing Services in the future
		INSERT INTO @NextExistingServiceTable (ServiceId, RecurringServiceId, OccurDate, ServiceName)
		SELECT  t1.Id, t1.RecurringServiceId, t1.ServiceDate, t2.Name
		FROM	Services t1, ServiceTemplates t2
		WHERE	t1.RecurringServiceId = @recurringServiceIdContext 
				AND t1.ServiceDate BETWEEN @dayAfterSeedDate AND @lastDateToLookFor
				AND t1.Id = t2.Id

		--Fills @ServicesForTodayWithoutExcluded will Existing Services on the SeedDate
		INSERT INTO @ServicesForTodayWithoutExcluded (RecurringServiceId, ServiceId, OccurDate, ServiceName)
		SELECT  t1.RecurringServiceId, t1.Id, t1.ServiceDate, t2.Name
		FROM	Services t1, ServiceTemplates t2
		WHERE	t1.RecurringServiceId = @recurringServiceIdContext 
				AND t1.ServiceDate = @seedDate
				AND t1.Id = t2.Id
	END

	ELSE IF @clientIdContext IS NOT NULL
	BEGIN
		--Fills @PreviousExistingServiceTable with existing Services in the past
		INSERT INTO @PreviousExistingServiceTable (ServiceId, RecurringServiceId, OccurDate, ServiceName)
		SELECT  t1.Id, t1.RecurringServiceId, t1.ServiceDate, t2.Name
		FROM	Services t1, ServiceTemplates t2
		WHERE	t1.ClientId = @clientIdContext 
				AND t1.ServiceDate BETWEEN @firstDateToLookFor AND @dayBeforeSeedDate
				AND t1.Id = t2.Id

		--Fills @NextExistingServiceTable with existing Services in the future
		INSERT INTO @NextExistingServiceTable (ServiceId, RecurringServiceId, OccurDate, ServiceName)
		SELECT  t1.Id, t1.RecurringServiceId, t1.ServiceDate, t2.Name
		FROM	Services t1, ServiceTemplates t2
		WHERE	t1.ClientId = @clientIdContext 
				AND t1.ServiceDate BETWEEN @dayAfterSeedDate AND @lastDateToLookFor
				AND t1.Id = t2.Id

		--Fills @ServicesForTodayWithoutExcluded will Existing Services on the SeedDate
		INSERT INTO @ServicesForTodayWithoutExcluded (ServiceId, RecurringServiceId, OccurDate, ServiceName)
		SELECT  t1.Id, t1.RecurringServiceId, t1.ServiceDate, t2.Name
		FROM	Services t1, ServiceTemplates t2
		WHERE	t1.ClientId = @clientIdContext 
				AND t1.ServiceDate = @seedDate
				AND t1.Id = t2.Id
	END

	ELSE IF @serviceProviderIdContext IS NOT NULL
	BEGIN 
		--Fills @PreviousExistingServiceTable with existing Services in the past
		INSERT INTO @PreviousExistingServiceTable (ServiceId, RecurringServiceId, OccurDate, ServiceName)
		SELECT  t1.Id, t1.RecurringServiceId, t1.ServiceDate, t2.Name
		FROM	Services t1, ServiceTemplates t2
		WHERE	t1.ServiceProviderId = @serviceProviderIdContext 
				AND t1.ServiceDate BETWEEN @firstDateToLookFor AND @dayBeforeSeedDate
				AND t1.Id = t2.Id

		--Fills @NextExistingServiceTable with existing Services in the future
		INSERT INTO @NextExistingServiceTable (ServiceId, RecurringServiceId, OccurDate, ServiceName)
		SELECT  t1.Id, t1.RecurringServiceId, t1.ServiceDate, t2.Name
		FROM	Services t1, ServiceTemplates t2
		WHERE	t1.ServiceProviderId = @serviceProviderIdContext 
				AND t1.ServiceDate BETWEEN @dayAfterSeedDate AND @lastDateToLookFor
				AND t1.Id = t2.Id
	
		--Fills @ServicesForTodayWithoutExcluded will Existing Services on the SeedDate
		INSERT INTO @ServicesForTodayWithoutExcluded (ServiceId, RecurringServiceId, OccurDate, ServiceName)
		SELECT  t1.Id, t1.RecurringServiceId, t1.ServiceDate, t2.Name
		FROM	Services t1, ServiceTemplates t2
		WHERE	t1.ServiceProviderId = @serviceProviderIdContext 
				AND t1.ServiceDate = @seedDate
				AND t1.Id = t2.Id
	END
------------------------------------------------------------------------------------------------------------------------------------------
--Here we will combine the Previous RecurringServices table with the Previous ExistingServices table
--It will then look for duplicate Id's and remove the RecurringService from the combined table
--Following this, it will find the first day (becasue this is the previous section) that a service should be on according @frontBackMinimum
--Then we will remove all Services that with an occur date before the date calculated above
------------------------------------------------------------------------------------------------------------------------------------------
	--Will store all the Services before the OnOrAfterDate
	DECLARE @CombinedPreviousServices TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)

	--Here we add all the previous occurrences to the final temporary table
	INSERT INTO @CombinedPreviousServices (RecurringServiceId, OccurDate, ServiceName)
	SELECT RecurringServiceId, OccurDate, ServiceName
	FROM @PreviousRecurringServiceOccurrenceTable

	INSERT INTO @CombinedPreviousServices (RecurringServiceId, ServiceId, OccurDate, ServiceName)
	SELECT RecurringServiceId, ServiceId, OccurDate, ServiceName
	FROM @PreviousExistingServiceTable

	--This table will temporarily store the RecurringServiceId's of all rows that appear more than once in @TempTable
	DECLARE @PreviousDuplicateIdTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)

	INSERT INTO @PreviousDuplicateIdTable
	SELECT t1.RecurringServiceId, t2.ServiceId, t2.OccurDate, t2.ServiceName FROM (SELECT OccurDate, RecurringServiceId, ServiceId FROM @CombinedPreviousServices AS t2 WHERE ServiceId IS NOT NULL GROUP BY OccurDate, RecurringServiceId, ServiceId) t1
	JOIN @CombinedPreviousServices as t2 on t1.RecurringServiceId = t2.RecurringServiceId AND t1.OccurDate = t2.OccurDate AND t2.ServiceId IS NULL

	DECLARE @previousHolderTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)

	--Filtered down from the table above so that only the correct number of Services is returned from the function
	INSERT TOP(@frontBackMinimum) INTO @previousHolderTable
	SELECT * FROM @CombinedPreviousServices
	EXCEPT
	SELECT * FROM @PreviousDuplicateIdTable
	ORDER BY OccurDate ASC

	Declare @previousLastDate date

	SET @previousLastDate = (SELECT MIN(OccurDate) FROM @previousHolderTable)

	DECLARE @finalPreviousServiceTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)	

	INSERT INTO @finalPreviousServiceTable
	SELECT * FROM @CombinedPreviousServices
	EXCEPT
	SELECT * FROM @PreviousDuplicateIdTable

	DELETE FROM @finalPreviousServiceTable
	WHERE OccurDate < @previousLastDate
------------------------------------------------------------------------------------------------------------------------------------------
--Here we will combine the Next RecurringServices table with the Next ExistingServices table
--It will then look for duplicate Id's and remove the RecurringService from the combined table
--Following this, it will find the last day (becasue this is the next section) that a service should be on according @frontBackMinimum
--Then we will remove all Services that with an occur date after the date calculated above
------------------------------------------------------------------------------------------------------------------------------------------
	--Will store all the Services after the OnOrAfterDate
	DECLARE @CombinedNextServices TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)

	--Here we add the next occurrences to the final temporary table
	INSERT INTO @CombinedNextServices (RecurringServiceId, OccurDate, ServiceName)
	SELECT RecurringServiceId, OccurDate, ServiceName
	FROM @NextRecurringServiceOccurrenceTable

	INSERT INTO @CombinedNextServices (RecurringServiceId, ServiceId, OccurDate, ServiceName)
	SELECT RecurringServiceId, ServiceId, OccurDate, ServiceName
	FROM @NextExistingServiceTable

	--This table will temporarily store the RecurringServiceId's of all rows that appear more than once in @TempTable
	DECLARE @NextDuplicateIdTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)

	INSERT INTO @NextDuplicateIdTable
	SELECT t1.RecurringServiceId, t2.ServiceId, t2.OccurDate, t2.ServiceName FROM (SELECT OccurDate, RecurringServiceId, ServiceId FROM @CombinedNextServices AS t2 WHERE ServiceId IS NOT NULL GROUP BY OccurDate, RecurringServiceId, ServiceId) t1
	JOIN @CombinedNextServices as t2 on t1.RecurringServiceId = t2.RecurringServiceId AND t1.OccurDate = t2.OccurDate AND t2.ServiceId IS NULL

	DECLARE @nextHolderTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)	

	--Filtered down from the table above so that only the correct number of Services is returned from the function
	INSERT TOP(@frontBackMinimum) INTO @nextHolderTable
	SELECT * FROM @CombinedNextServices
	EXCEPT
	SELECT * FROM @NextDuplicateIdTable
	ORDER BY OccurDate ASC

	Declare @nextLastDate date

	SET @nextLastDate = (SELECT MAX(OccurDate) FROM @nextHolderTable)
	
	DECLARE @finalNextServiceTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)	

	INSERT INTO @finalNextServiceTable
	SELECT * FROM @CombinedNextServices
	EXCEPT
	SELECT * FROM @NextDuplicateIdTable

	DELETE FROM @finalNextServiceTable
	WHERE OccurDate > @nextLastDate
	
------------------------------------------------------------------------------------------------------------------------------------------
--Finally, we combine the three tables (previous, onSeedDay, next) into one master table
--This master table is then returned from the function
------------------------------------------------------------------------------------------------------------------------------------------
	--This table will temporarily store the RecurringServiceId's of all rows that appear more than once in @TempTable
	DECLARE @SeedDateDuplicateIdTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)

	INSERT INTO @SeedDateDuplicateIdTable
	SELECT t1.RecurringServiceId, t2.ServiceId, t2.OccurDate, t2.ServiceName FROM (SELECT OccurDate, RecurringServiceId, ServiceId FROM @ServicesForTodayWithoutExcluded AS t2 WHERE ServiceId IS NOT NULL GROUP BY OccurDate, RecurringServiceId, ServiceId) t1
	JOIN @ServicesForTodayWithoutExcluded as t2 on t1.RecurringServiceId = t2.RecurringServiceId AND t1.OccurDate = t2.OccurDate AND t2.ServiceId IS NULL

	DECLARE @seedDateFinalTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)	

	--Filtered down from the table above so that only the correct number of Services is returned from the function
	INSERT INTO @seedDateFinalTable
	SELECT * FROM @ServicesForTodayWithoutExcluded
	EXCEPT
	SELECT * FROM @SeedDateDuplicateIdTable
	ORDER BY OccurDate ASC

------------------------------------------------------------------------------------------------------------------------------------------
--Finally, we combine the three tables (previous, onSeedDay, next) into one master table
--This master table is then returned from the function
------------------------------------------------------------------------------------------------------------------------------------------
	INSERT INTO @ServicesTableToReturn
	SELECT * FROM @finalPreviousServiceTable

	INSERT INTO @ServicesTableToReturn
	SELECT * FROM @seedDateFinalTable

	INSERT INTO @ServicesTableToReturn
	SELECT * FROM @finalNextServiceTable

RETURN
END