use Core
GO
	DECLARE @TempTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date
	)

	DECLARE @TempGenServiceTable TABLE
	(Id uniqueidentifier,
     EndDate date,
     EndAfterTimes int,
     RepeatEveryTimes int,
     FrequencyInt int,
     FrequencyDetailInt int,
     StartDate date)

	 DECLARE @GetNext bit
	 DECLARE @GetPrevious bit
	 DECLARE @NumberToGet int

	 SET @GetPrevious = 1
	 SET @GetNext = 1
	 SET @NumberToGet = 1000


	 --This will get all Recurring Services that are associated with the Business Account and put them in @TempGenServiceTable
	INSERT INTO @TempGenServiceTable (Id, EndDate, EndAfterTimes, RepeatEveryTimes, FrequencyInt, FrequencyDetailInt, StartDate)
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
				AND Clients.VendorId = 'A6CF1180-28B8-4FEC-BAAD-0C8939312686'
				AND RecurringServices.Id = Repeats.Id
		)
	)

	DECLARE @TempNextDateTable TABLE
	(Id uniqueidentifier,
	NextDate date)

	--Takes the Table formed above and will find the next occurrence for all those recurring services
	INSERT INTO @TempNextDateTable (Id, NextDate)
	SELECT Id, dbo.GetNextOccurence('9-15-2012', StartDate,  EndDate, EndAfterTimes, FrequencyInt, RepeatEveryTimes, FrequencyDetailInt)
	FROM   @TempGenServiceTable

	DECLARE @TempPreviousDateTable TABLE
	(Id uniqueidentifier,
	PreviousDate date)

	--Takes the Table formed above and will find the next occurrence for all those recurring services
	INSERT INTO @TempPreviousDateTable (Id, PreviousDate)
	SELECT Id, dbo.GetPreviousOccurrence('9-15-2012', StartDate,  EndDate, EndAfterTimes, FrequencyInt, RepeatEveryTimes, FrequencyDetailInt)
	FROM   @TempGenServiceTable

	--SELECT * FROM @TempPreviousDateTable


	DECLARE @TempGenServiceTableWithNextOccurrence TABLE
	(Id uniqueidentifier,
     EndDate date,
     EndAfterTimes int,
     RepeatEveryTimes int,
     FrequencyInt int,
     FrequencyDetailInt int,
     StartDate date,
	 NextDate date)
	
	--Merges @TempGenServiceTable amd @TempNextDateTable created above based on their Id
	INSERT INTO @TempGenServiceTableWithNextOccurrence (Id, EndDate, EndAfterTimes, RepeatEveryTimes, FrequencyInt, FrequencyDetailInt, StartDate, NextDate)
	SELECT	t1.Id, t1.EndDate, t1.EndAfterTimes, t1.RepeatEveryTimes, t1.FrequencyInt, t1.FrequencyDetailInt, t1.StartDate, t2.NextDate
	FROM	@TempGenServiceTable t1
	FULL JOIN @TempNextDateTable t2
	ON t1.Id =t2.Id

	--Remove any rows that do not have a NextOccurrence past the OnOrAfterDate
	DELETE FROM @TempGenServiceTableWithNextOccurrence
	WHERE NextDate IS NULL

	DECLARE @TempGenServiceTableWithPreviousOccurrence TABLE
	(Id uniqueidentifier,
     EndDate date,
     EndAfterTimes int,
     RepeatEveryTimes int,
     FrequencyInt int,
     FrequencyDetailInt int,
     StartDate date,
	 PreviousDate date)
	
	--Merges @TempGenServiceTable amd @TempNextDateTable created above based on their Id
	INSERT INTO @TempGenServiceTableWithPreviousOccurrence (Id, EndDate, EndAfterTimes, RepeatEveryTimes, FrequencyInt, FrequencyDetailInt, StartDate, PreviousDate)
	SELECT	t1.Id, t1.EndDate, t1.EndAfterTimes, t1.RepeatEveryTimes, t1.FrequencyInt, t1.FrequencyDetailInt, t1.StartDate, t2.PreviousDate
	FROM	@TempGenServiceTable t1
	FULL JOIN @TempPreviousDateTable t2
	ON t1.Id =t2.Id

	--Remove any rows that do not have a NextOccurrence past the OnOrAfterDate
	DELETE FROM @TempGenServiceTableWithPreviousOccurrence
	WHERE PreviousDate IS NULL

	--SELECT * FROM @TempGenServiceTableWithPreviousOccurrence
	------------------------------------------------------------------

	DECLARE @minVal date
	DECLARE @lastDateToLookFor date
	DECLARE @firstDateToLookFor date
	DECLARE @ServiceCount int
	DECLARE @NextDay date
	DECLARE @RowCountForRecurringServiceOccurrenceTable int
	DECLARE @RowCountForTempGenServiceTableWithNextOccurrence int
	DECLARE @RowCountForTempGenServiceTableWithPreviousOccurrence int
	DECLARE @splitNumberToGet int


	DECLARE @NextRecurringServiceOccurrenceTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		OccurDate datetime
	)

	DECLARE @PreviousRecurringServiceOccurrenceTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		OccurDate datetime
	)

	IF @GetNext = 1 AND @GetPrevious = 1
	BEGIN
		SET @splitNumberToGet = @NumberToGet / 2
	END
	ELSE
	BEGIN
		SET @splitNumberToGet = @NumberToGet
	END

	SET		@RowCountForRecurringServiceOccurrenceTable = 0
	--Preset to a value above 0 just so it makes it into the loop. Actual value is calculated there
	SET		@RowCountForTempGenServiceTableWithPreviousOccurrence = 1

	IF		@GetPrevious = 1
	BEGIN
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
		
		INSERT INTO @PreviousRecurringServiceOccurrenceTable
		SELECT	Id, PreviousDate
		FROM	@TempGenServiceTableWithPreviousOccurrence
		WHERE	PreviousDate = @minVal

		UPDATE	@TempGenServiceTableWithPreviousOccurrence
		SET		PreviousDate = (SELECT dbo.GetPreviousOccurrence(@NextDay, StartDate,  EndDate, EndAfterTimes, FrequencyInt, RepeatEveryTimes, FrequencyDetailInt))
		FROM	@TempGenServiceTableWithPreviousOccurrence

		DELETE FROM @TempGenServiceTableWithPreviousOccurrence
		WHERE	PreviousDate IS NULL

		SET		@RowCountForRecurringServiceOccurrenceTable = (SELECT COUNT(*) FROM @PreviousRecurringServiceOccurrenceTable)

	END
	END

	--SELECT * FROM @RecurringServiceOccurrenceTable
	--ORDER BY OccurDate

	SET		@RowCountForRecurringServiceOccurrenceTable = 0
	--Preset to a value above 0 just so it makes it into the loop. Actual value is calculated there
	SET		@RowCountForTempGenServiceTableWithNextOccurrence = 1

	IF		@GetNext = 1
	BEGIN
	WHILE	@RowCountForRecurringServiceOccurrenceTable <= @NumberToGet
	BEGIN
		SET		@minVal = (SELECT MIN(NextDate) FROM @TempGenServiceTableWithNextOccurrence)
		SET		@NextDay = DATEADD(day, 1, @minVal)

		SET		@RowCountForTempGenServiceTableWithNextOccurrence = (SELECT COUNT(*) FROM @TempGenServiceTableWithNextOccurrence)

		--Checks to be sure that there are still rows to look at in @TempGenServiceTableWithNextOccurrence
		IF NOT	@RowCountForTempGenServiceTableWithNextOccurrence > 0
		BEGIN
				BREAK
		END
		
		INSERT INTO @NextRecurringServiceOccurrenceTable
		SELECT	Id, NextDate
		FROM	@TempGenServiceTableWithNextOccurrence
		WHERE	NextDate = @minVal

		UPDATE	@TempGenServiceTableWithNextOccurrence
		SET		NextDate = (SELECT dbo.GetNextOccurence(@NextDay, StartDate,  EndDate, EndAfterTimes, FrequencyInt, RepeatEveryTimes, FrequencyDetailInt))
		FROM	@TempGenServiceTableWithNextOccurrence

		DELETE FROM @TempGenServiceTableWithNextOccurrence
		WHERE	NextDate IS NULL

		SET		@RowCountForRecurringServiceOccurrenceTable = (SELECT COUNT(*) FROM @NextRecurringServiceOccurrenceTable)

	END
	END

	SET		@lastDateToLookFor = (SELECT MAX(OccurDate) FROM @NextRecurringServiceOccurrenceTable)
	SET		@firstDateToLookFor = (SELECT MIN(OccurDate) FROM @PreviousRecurringServiceOccurrenceTable)

	--SELECT @RowCount
		--SELECT * FROM @RecurringServiceOccurrenceTable
		--ORDER BY OccurDate
	----------------------------------------------------------------------------
	DECLARE @TempPreviousExistingServiceTable TABLE
	(ServiceId uniqueidentifier,
	OccurDate date)

	DECLARE @TempNextExistingServiceTable TABLE
	(ServiceId uniqueidentifier,
	OccurDate date)


	INSERT INTO @TempPreviousExistingServiceTable (ServiceId, OccurDate)
	SELECT TOP(@NumberToGet) Id, ServiceDate
	FROM Services
	WHERE ServiceProviderId = 'A6CF1180-28B8-4FEC-BAAD-0C8939312686' AND ServiceDate BETWEEN @firstDateToLookFor AND '9-15-2012'

	INSERT INTO @TempPreviousExistingServiceTable (ServiceId, OccurDate)
	SELECT TOP(@NumberToGet) Id, ServiceDate
	FROM Services
	WHERE ServiceProviderId = 'A6CF1180-28B8-4FEC-BAAD-0C8939312686' AND ServiceDate BETWEEN '9-15-2012' AND @lastDateToLookFor


	--SELECT * FROM @TempExistingServiceTable

	DECLARE @PreviousIntermediateTempTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate datetime
	)

	DECLARE @NextIntermediateTempTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate datetime
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
	
	--Here we take the two final temporary tables and combine them into the final table to be returned.
	INSERT INTO @TempTable
	SELECT TOP(@NumberToGet/2) * 
	FROM @PreviousIntermediateTempTable
	ORDER BY OccurDate

	INSERT INTO @TempTable
	SELECT TOP(@NumberToGet) * 
	FROM @NextIntermediateTempTable
	ORDER BY OccurDate

	SELECT TOP(@NumberToGet) * FROM @TempTable
	ORDER BY OccurDate
