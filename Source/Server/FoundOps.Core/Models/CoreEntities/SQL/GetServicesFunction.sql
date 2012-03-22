SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
USE Core;
GO

   /****************************************************************************************************************************************************
	* FUNCTION GetServicesBasedOnProvider will take the context provided and find all the service dates
	* for RecurringServices and existing Services with that context. The function will return past, future or both past and future Services.
	* For RecurringServices, if there are existing services for dates it will return those. Otherwise it will generate a date for the instances.
	* You can distinguish generated services because they will not have a ServiceId *
	** Input Parameters **
	* @serviceProviderIdContext - The BusinessAccount context or NULL 
	* @clientIdContext - The Client context or NULL 
	* @recurringServiceIdContext - The RecurringService context or NULL
	* @firstDateToLookForServices - The reference date to look for services before or after. Also known as the onOrBeforeDate and the onOrAfterDate
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
    ALTER FUNCTION [dbo].[GetServicesBasedOnProvider]
    (@serviceProviderIdContext uniqueidentifier,
    @clientIdContext uniqueidentifier,
    @recurringServiceIdContext uniqueidentifier,
    @firstDateToLookForServices date,
    @numberOfOccurrences int,
	@getPrevious bit,
	@getNext bit)
	--TODO RENAME TempTable to ...
	--TempTable is where we will put all the Services and their corresponding dates
	RETURNS @ServicesTableToReturn TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date
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
	 NextDate date)

	--Insert all (the single) RecurringService associated with the @recurringServiceIdContext to @TempGenServiceTable
	IF @recurringServiceIdContext IS NOT NULL
	BEGIN		
	INSERT INTO @TempGenServiceTable (Id, EndDate, EndAfterTimes, RepeatEveryTimes, FrequencyInt, FrequencyDetailInt, StartDate)
	SELECT	*
	FROM	Repeats 
	WHERE	Id = @recurringServiceIdContext
		END
	
	--Insert all (the single) RecurringServices associated with the @clientIdContext to @TempGenServiceTable
	ELSE IF @clientIdContext IS NOT NULL
	BEGIN
	INSERT INTO @TempGenServiceTable (Id, EndDate, EndAfterTimes, RepeatEveryTimes, FrequencyInt, FrequencyDetailInt, StartDate)
	--This is a Semi-Join between the Repeats table created above and the RecurringServices Table
	--Semi-Join simply means that it has all the same logic as a normal join, but it doesnt actually join the tables
	--In this case, it finds all the Repeats that correspond to a RecurringService with a ClientId = @clientIdContext
	SELECT *
	FROM		Repeats
	WHERE		EXISTS
	(
		SELECT	* 
		FROM	RecurringServices
		WHERE	ClientId = @clientIdContext
	)
	END
	
	--Insert all (the single) RecurringServices associated with the @serviceProviderIdContext to @TempGenServiceTable
	ELSE
	BEGIN
	INSERT INTO @TempGenServiceTable (Id, EndDate, EndAfterTimes, RepeatEveryTimes, FrequencyInt, FrequencyDetailInt, StartDate)
	--This is a Semi-Join between the Clients table created above and the RecurringServices Table
	--Semi-Join simply means that it has all the same logic as a normal join, but it doesnt actually join the tables
	--In this case, it finds all the RecurringServices that correspond to a Client with a vendorId = @serviceProviderIdContext
	SELECT	* 
	FROM		Repeats
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
				AND RecurringServices.Id = Repeats.Id
		)
	)
	END

	--Stores all the Recurring Services from @TempGenServiceTable and their next occurrence date
	DECLARE @TempNextDateTable TABLE
	(Id uniqueidentifier,
	NextDate date)

	--Takes the Table formed above and will find the next occurrence for all those recurring services
	INSERT INTO @TempNextDateTable (Id, NextDate)
	SELECT Id, dbo.GetNextOccurence(@firstDateToLookForServices, StartDate,  EndDate, EndAfterTimes, FrequencyInt, RepeatEveryTimes, FrequencyDetailInt)
	FROM   @TempGenServiceTable

	--Stores all the Recurring Services from @TempGenServiceTable and their previous occurrence date
	DECLARE @TempPreviousDateTable TABLE
	(Id uniqueidentifier,
	PreviousDate date)

	--Takes the Table formed above and will find the previous occurrence for all those recurring services
	INSERT INTO @TempPreviousDateTable (Id, PreviousDate)
	SELECT Id, dbo.GetPreviousOccurrence(@firstDateToLookForServices, StartDate,  EndDate, EndAfterTimes, FrequencyInt, RepeatEveryTimes, FrequencyDetailInt)
	FROM   @TempGenServiceTable

	--This table is simply the result of merging @TempGenServiceTable and @TempNextDateTable
	DECLARE @TempGenServiceTableWithNextOccurrence TABLE
	(Id uniqueidentifier,
     EndDate date,
     EndAfterTimes int,
     RepeatEveryTimes int,
     FrequencyInt int,
     FrequencyDetailInt int,
     StartDate date,
	 NextDate date)
	
	--Merges @TempGenServiceTable amd @TempNextDateTable created above based on their Id into @TempGenServiceTableWithNextOccurrence
	INSERT INTO @TempGenServiceTableWithNextOccurrence (Id, EndDate, EndAfterTimes, RepeatEveryTimes, FrequencyInt, FrequencyDetailInt, StartDate, NextDate)
	SELECT	t1.Id, t1.EndDate, t1.EndAfterTimes, t1.RepeatEveryTimes, t1.FrequencyInt, t1.FrequencyDetailInt, t1.StartDate, t2.NextDate
	FROM	@TempGenServiceTable t1
	FULL JOIN @TempNextDateTable t2
	ON t1.Id =t2.Id

	--Remove any rows that do not have a NextOccurrence past or on the OnOrAfterDate
	DELETE FROM @TempGenServiceTableWithNextOccurrence
	WHERE NextDate IS NULL

	--This table is simply the result of merging @TempGenServiceTable and @TempPreviousDateTable
	DECLARE @TempGenServiceTableWithPreviousOccurrence TABLE
	(Id uniqueidentifier,
     EndDate date,
     EndAfterTimes int,
     RepeatEveryTimes int,
     FrequencyInt int,
     FrequencyDetailInt int,
     StartDate date,
	 PreviousDate date)
	
	--Merges @TempGenServiceTable amd @TempPreviousDateTable created above based on their Id into @TempGenServiceTableWithNextOccurrence
	INSERT INTO @TempGenServiceTableWithPreviousOccurrence (Id, EndDate, EndAfterTimes, RepeatEveryTimes, FrequencyInt, FrequencyDetailInt, StartDate, PreviousDate)
	SELECT	t1.Id, t1.EndDate, t1.EndAfterTimes, t1.RepeatEveryTimes, t1.FrequencyInt, t1.FrequencyDetailInt, t1.StartDate, t2.PreviousDate
	FROM	@TempGenServiceTable t1
	FULL JOIN @TempPreviousDateTable t2
	ON t1.Id =t2.Id

	--Remove any rows that do not have a PreviousOccurrence on or before the OnOrBeforeDate
	DELETE FROM @TempGenServiceTableWithPreviousOccurrence
	WHERE PreviousDate IS NULL
	------------------------------------------------------------------
	--Here we will take the Recurring Services that are in @TempGenServiceTableWithNextOccurrence and find all occurrences in date order until
	--we get to the number specified by @numberOfOccurrences
	--We will also do the same thing with @TempGenServiceTableWithPreviousOccurrence except in reverse date order
	
	DECLARE @minVal date
	DECLARE @lastDateToLookFor date
	DECLARE @firstDateToLookFor date
	DECLARE @ServiceCount int
	DECLARE @NextDay date
	DECLARE @RowCountForRecurringServiceOccurrenceTable int
	DECLARE @RowCountForTempGenServiceTableWithNextOccurrence int
	DECLARE @RowCountForTempGenServiceTableWithPreviousOccurrence int
	DECLARE @splitNumberToGet int

	--Table to temporarily store all the Recurring Services dates in the future
	DECLARE @NextRecurringServiceOccurrenceTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		OccurDate date
	)

	--Table to temporarily store all the Recurring Services dates in the past
	DECLARE @PreviousRecurringServiceOccurrenceTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		OccurDate date
	)

	--Set up @splitNumberToGet so that it is half the size of what the function is looking to output
	IF @GetNext = 1 AND @GetPrevious = 1
	BEGIN
		SET @splitNumberToGet = @numberOfOccurrences / 2
	END
	ELSE
	BEGIN
		SET @splitNumberToGet = @numberOfOccurrences
	END

	SET		@RowCountForRecurringServiceOccurrenceTable = 0
	--Preset to a value above 0 just so it makes it into the loop. Actual value is calculated there
	SET		@RowCountForTempGenServiceTableWithPreviousOccurrence = 1

	IF		@GetPrevious = 1
	BEGIN
	--Will loop until it finds @splitNumberToGet services in the future
	WHILE	@RowCountForRecurringServiceOccurrenceTable <= @splitNumberToGet
	BEGIN
		SET		@minVal = (SELECT MAX(PreviousDate) FROM @TempGenServiceTableWithPreviousOccurrence)
		SET		@NextDay = DATEADD(day, -1, @minVal)
		
		SET		@RowCountForTempGenServiceTableWithPreviousOccurrence = (SELECT COUNT(*) FROM @TempGenServiceTableWithPreviousOccurrence)

		--Checks to be sure that there are still rows to look at in @TempGenServiceTableWithNextOccurrence
		IF NOT	@RowCountForTempGenServiceTableWithPreviousOccurrence > 0
		BEGIN
				BREAK
		END
		
		--Inserts all Services with the lowest date to @PreviousRecurringServiceOccurrenceTable
		INSERT INTO @PreviousRecurringServiceOccurrenceTable
		SELECT	Id, PreviousDate
		FROM	@TempGenServiceTableWithPreviousOccurrence
		WHERE	PreviousDate = @minVal

		--Updates all Services that were just put into a new table to show their previous occurrence date
		UPDATE	@TempGenServiceTableWithPreviousOccurrence
		SET		PreviousDate = (SELECT dbo.GetPreviousOccurrence(@NextDay, StartDate,  EndDate, EndAfterTimes, FrequencyInt, RepeatEveryTimes, FrequencyDetailInt))
		FROM	@TempGenServiceTableWithPreviousOccurrence

		--If any of those Services updated above do not have a previous occurrence, they will be deleted from the table
		DELETE FROM @TempGenServiceTableWithPreviousOccurrence
		WHERE	PreviousDate IS NULL

		--Sets up row count so that when the loop returns to the top, we will know if there are any rows remaining
		SET		@RowCountForRecurringServiceOccurrenceTable = (SELECT COUNT(*) FROM @PreviousRecurringServiceOccurrenceTable)

	END
	END

	SET		@RowCountForRecurringServiceOccurrenceTable = 0
	--Preset to a value above 0 just so it makes it into the loop. Actual value is calculated there
	SET		@RowCountForTempGenServiceTableWithNextOccurrence = 1

	IF		@GetNext = 1
	BEGIN
	--Will loop until it fills in remaining number of Services to get to @numberOfOccurrences
	WHILE	@RowCountForRecurringServiceOccurrenceTable <= @numberOfOccurrences
	BEGIN
		SET		@minVal = (SELECT MIN(NextDate) FROM @TempGenServiceTableWithNextOccurrence)
		SET		@NextDay = DATEADD(day, 1, @minVal)

		SET		@RowCountForTempGenServiceTableWithNextOccurrence = (SELECT COUNT(*) FROM @TempGenServiceTableWithNextOccurrence)

		--Checks to be sure that there are still rows to look at in @TempGenServiceTableWithNextOccurrence
		IF NOT	@RowCountForTempGenServiceTableWithNextOccurrence > 0
		BEGIN
				BREAK
		END
		
		--Inserts all Services with the lowest date to @NextRecurringServiceOccurrenceTable
		INSERT INTO @NextRecurringServiceOccurrenceTable
		SELECT	Id, NextDate
		FROM	@TempGenServiceTableWithNextOccurrence
		WHERE	NextDate = @minVal

		--Updates all Services that were just put into a new table to show their next occurrence date
		UPDATE	@TempGenServiceTableWithNextOccurrence
		SET		NextDate = (SELECT dbo.GetNextOccurence(@NextDay, StartDate,  EndDate, EndAfterTimes, FrequencyInt, RepeatEveryTimes, FrequencyDetailInt))
		FROM	@TempGenServiceTableWithNextOccurrence

		--If any of those Services updated above do not have a next occurrence, they will be deleted from the table
		DELETE FROM @TempGenServiceTableWithNextOccurrence
		WHERE	NextDate IS NULL

		--Sets up row count so that when the loop returns to the top, we will know if there are any rows remaining
		SET		@RowCountForRecurringServiceOccurrenceTable = (SELECT COUNT(*) FROM @NextRecurringServiceOccurrenceTable)

	END
	END

	SET		@lastDateToLookFor = (SELECT MAX(OccurDate) FROM @NextRecurringServiceOccurrenceTable)
	SET		@firstDateToLookFor = (SELECT MIN(OccurDate) FROM @PreviousRecurringServiceOccurrenceTable)
	-----------------------------------------------------------------------
	
	--Here we will add Existing Services to the table of their own

	--This table will store all Existing Services prior to the OnOrAfterDate
	DECLARE @TempPreviousExistingServiceTable TABLE
	(ServiceId uniqueidentifier,
	RecurringServiceId uniqueidentifier,
	OccurDate date)

	--This table will store all Existing Services after to the OnOrAfterDate
	DECLARE @TempNextExistingServiceTable TABLE
	(ServiceId uniqueidentifier,
	RecurringServiceId uniqueidentifier,
	OccurDate date)

	IF @recurringServiceIdContext IS NOT NULL
	BEGIN
	--Inserts all Existing Services with a service date after the OnOrAfterDate or before the OnOrBeforeDate to a temp table
	INSERT INTO @TempPreviousExistingServiceTable (ServiceId, RecurringServiceId, OccurDate)
	SELECT TOP(@numberOfOccurrences) Id, RecurringServiceId, ServiceDate
	FROM Services
	WHERE ServiceProviderId = @recurringServiceIdContext AND ServiceDate BETWEEN @firstDateToLookFor AND @firstDateToLookForServices
	ORDER BY ServiceDate

	INSERT INTO @TempPreviousExistingServiceTable (ServiceId, RecurringServiceId, OccurDate)
	SELECT TOP(@numberOfOccurrences) Id, RecurringServiceId, ServiceDate
	FROM Services
	WHERE ServiceProviderId = @recurringServiceIdContext AND ServiceDate BETWEEN @firstDateToLookForServices AND @lastDateToLookFor
	ORDER BY ServiceDate
	END

	ELSE IF @clientIdContext IS NOT NULL
	BEGIN
	--Inserts all Existing Services with a service date after the OnOrAfterDate or before the OnOrBeforeDate to a temp table
	INSERT INTO @TempPreviousExistingServiceTable (ServiceId, RecurringServiceId, OccurDate)
	SELECT TOP(@numberOfOccurrences) Id, RecurringServiceId, ServiceDate
	FROM Services
	WHERE ServiceProviderId = @clientIdContext AND ServiceDate BETWEEN @firstDateToLookFor AND @firstDateToLookForServices
	ORDER BY ServiceDate

	INSERT INTO @TempPreviousExistingServiceTable (ServiceId, RecurringServiceId, OccurDate)
	SELECT TOP(@numberOfOccurrences) Id, RecurringServiceId, ServiceDate
	FROM Services
	WHERE ServiceProviderId = @clientIdContext AND ServiceDate BETWEEN @firstDateToLookForServices AND @lastDateToLookFor
	ORDER BY ServiceDate
	END

	ELSE IF @serviceProviderIdContext IS NOT NULL
	BEGIN 
	--Inserts all Existing Services with a service date after the OnOrAfterDate or before the OnOrBeforeDate to a temp table
	INSERT INTO @TempPreviousExistingServiceTable (ServiceId, RecurringServiceId, OccurDate)
	SELECT TOP(@numberOfOccurrences) Id, RecurringServiceId, ServiceDate
	FROM Services
	WHERE ServiceProviderId = @serviceProviderIdContext AND ServiceDate BETWEEN @firstDateToLookFor AND @firstDateToLookForServices
	ORDER BY ServiceDate

	INSERT INTO @TempPreviousExistingServiceTable (ServiceId, RecurringServiceId, OccurDate)
	SELECT TOP(@numberOfOccurrences) Id, RecurringServiceId, ServiceDate
	FROM Services
	WHERE ServiceProviderId = @serviceProviderIdContext AND ServiceDate BETWEEN @firstDateToLookForServices AND @lastDateToLookFor
	ORDER BY ServiceDate
	END
------------------------------------------------------------------------------------------
	--Will store all the Services before the OnOrAfterDate
	DECLARE @PreviousIntermediateTempTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date
	)

	--Will store all the Services after the OnOrAfterDate
	DECLARE @NextIntermediateTempTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date
	)

	--Here we add all the previous occurrences to the final temporary table
	INSERT INTO @PreviousIntermediateTempTable (RecurringServiceId, OccurDate)
	SELECT *
	FROM @PreviousRecurringServiceOccurrenceTable

	INSERT INTO @PreviousIntermediateTempTable (ServiceId, OccurDate)
	SELECT ServiceId, OccurDate
	FROM @TempPreviousExistingServiceTable

	--Here we add the next occurrences to the final temporary table
	INSERT INTO @NextIntermediateTempTable (RecurringServiceId, OccurDate)
	SELECT *
	FROM @NextRecurringServiceOccurrenceTable

	INSERT INTO @NextIntermediateTempTable (ServiceId, OccurDate)
	SELECT ServiceId, OccurDate
	FROM @TempNextExistingServiceTable

	--This is just a copy of the output table that allows for more than the specified number of Services to be added
	DECLARE @TempTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date
	)	
	--Here we take the two final temporary tables and combine them into a table that resembles the final table
	INSERT INTO @TempTable
	SELECT TOP(@numberOfOccurrences/2) * 
	FROM @PreviousIntermediateTempTable
	ORDER BY OccurDate

	INSERT INTO @TempTable
	SELECT TOP(@numberOfOccurrences) * 
	FROM @NextIntermediateTempTable
	ORDER BY OccurDate

	--This table will temporarily store the RecurringServiceId's of all rows that appear more than once in @TempTable
	DECLARE @DuplicateIdTable TABLE
	(RecurringServiceId uniqueidentifier)

	--Add the duplicate Id's to the table created above
	INSERT INTO @DuplicateIdTable
	SELECT RecurringServiceId FROM @TempTable t1
	WHERE 1 < 
	(
		SELECT COUNT (*) 
		FROM @TempTable t2
		WHERE t1.RecurringServiceId = t2.RecurringServiceId
	)

	--Delete matching Services between @IdTable and @TempTable. This will delete the Generated Service and will keep the Existing Service
	DELETE FROM @TempTable
	WHERE EXISTS
	(
		SELECT * FROM @DuplicateIdTable t1
		WHERE	RecurringServiceId = t1.RecurringServiceId
		AND		ServiceId IS NOT NULL
	)

	--Filtered down from the table above so that only the correct number of Services is returned from the function
	INSERT INTO @ServicesTableToReturn
	SELECT DISTINCT TOP (@numberOfOccurrences) *
	FROM @TempTable
	ORDER BY OccurDate ASC
	
	RETURN
	END