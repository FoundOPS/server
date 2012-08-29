USE [Core]
GO
/****** Object:  StoredProcedure [dbo].[GetServiceHolders]    Script Date: 8/10/2012 12:43:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetServiceHolders]
--ALTER PROCEDURE [dbo].[GetServiceHolders]
	(
	@serviceProviderIdContext UNIQUEIDENTIFIER, 
	@clientIdContext UNIQUEIDENTIFIER, 
	@recurringServiceIdContext UNIQUEIDENTIFIER, 
	@firstDate DATE, 
	@lastDate DATE,
	@serviceTypeContext NVARCHAR(MAX),
	@withFields BIT
	)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

BEGIN	--Here we find all of the Services for the correct Context and store them in @TempGenServiceTable

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
END

BEGIN	--This section will find the first occurrence of all the RecurringServices on or after the SeedDate
	
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

	--Update the NextService date for all Services in the context to the first date on or after @firstDate
	UPDATE @NextGenServices
	SET NextDate = (SELECT dbo.GetNextOccurence(@firstDate, StartDate,  EndDate, EndAfterTimes, FrequencyInt, RepeatEveryTimes, FrequencyDetailInt))
	FROM @NextGenServices

END

BEGIN	--Find all RecurringServices where the ExcludedDateString is not null

	DECLARE @RecurringServicesWithExcludedDates TABLE
	(
		Id uniqueidentifier,
		ExcludedDatesString nvarchar(max)
	)

	INSERT INTO @RecurringServicesWithExcludedDates
	SELECT DISTINCT t1.Id, t1.ExcludedDatesString FROM RecurringServices t1, @NextGenServices t2
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

END

BEGIN	--Cycle through all of the RecurringServices until we get all from @firstDate through @lastDate
	BEGIN	--Declare variables and tables used in this section
		DECLARE @minVal date
		DECLARE @maxVal date
		DECLARE @NextDay date
		DECLARE @RowCountForTempGenServiceTable int
		DECLARE @RowCountForRecurringServiceOccurrenceTable int

		--Collection of all services that were temporarily stored in @TempNextRecurringServiceOccurrenceTable
		DECLARE @NextRecurringServiceOccurrenceTable TABLE
		(
			RecurringServiceId uniqueidentifier,
			OccurDate date,
			ServiceName nvarchar(max)
		)

		--Table to temporarily store all the Recurring Services dates in the future, deleted after each iteration of the loop
		DECLARE @TempNextRecurringServiceOccurrenceTable TABLE
		(
			RecurringServiceId uniqueidentifier,
			OccurDate date,
			ServiceName nvarchar(max)
		)
	END

	SET		@RowCountForTempGenServiceTable = (SELECT COUNT(*) FROM @NextGenServices)

	WHILE	@RowCountForTempGenServiceTable <> 0
	BEGIN
		SET		@minVal = (SELECT MIN(NextDate) FROM @NextGenServices)
		SET		@NextDay = DATEADD(day, 1, @minVal)
		SET		@maxVal = (SELECT MAX(NextDate) FROM @NextGenServices)

		--Checks to be sure that there are still rows to look at in @NextGenServices
		IF NOT	@RowCountForTempGenServiceTable > 0
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
		WHERE	NextDate IS NULL OR NextDate > @lastDate
		
		DELETE FROM @NextServicesToRemove
		DELETE FROM @TempNextShouldAddTable
		DELETE FROM @TempNextRecurringServiceOccurrenceTable

		SET		@RowCountForTempGenServiceTable = (SELECT COUNT(*) FROM @NextGenServices)
	END

END

BEGIN	--Put all ExistingServices from @firstDate through @lastDate into a table to be added to the list of RecurringServices

	--This table will store all Existing Services after to the OnOrAfterDate
	DECLARE @NextExistingServiceTable TABLE
	(
		ServiceId uniqueidentifier,
		RecurringServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)
	
	IF @recurringServiceIdContext IS NOT NULL
	BEGIN
		--Fills @NextExistingServiceTable with existing Services in the future
		INSERT INTO @NextExistingServiceTable (ServiceId, RecurringServiceId, OccurDate, ServiceName)
		SELECT  t1.Id, t1.RecurringServiceId, t1.ServiceDate, t2.Name
		FROM	Services t1, ServiceTemplates t2
		WHERE	t1.RecurringServiceId = @recurringServiceIdContext 
				AND t1.ServiceDate BETWEEN @firstDate AND @lastDate
				AND t1.Id = t2.Id
	END

	ELSE IF @clientIdContext IS NOT NULL
	BEGIN
		--Fills @NextExistingServiceTable with existing Services in the future
		INSERT INTO @NextExistingServiceTable (ServiceId, RecurringServiceId, OccurDate, ServiceName)
		SELECT  t1.Id, t1.RecurringServiceId, t1.ServiceDate, t2.Name
		FROM	Services t1, ServiceTemplates t2
		WHERE	t1.ClientId = @clientIdContext 
				AND t1.ServiceDate BETWEEN @firstDate AND @lastDate
				AND t1.Id = t2.Id
	END

	ELSE IF @serviceProviderIdContext IS NOT NULL
	BEGIN 
		--Fills @NextExistingServiceTable with existing Services in the future
		INSERT INTO @NextExistingServiceTable (ServiceId, RecurringServiceId, OccurDate, ServiceName)
		SELECT  t1.Id, t1.RecurringServiceId, t1.ServiceDate, t2.Name
		FROM	Services t1, ServiceTemplates t2
		WHERE	t1.ServiceProviderId = @serviceProviderIdContext 
				AND t1.ServiceDate BETWEEN @firstDate AND @lastDate
				AND t1.Id = t2.Id
	END

END

BEGIN	--Combine the RecurringServices table with the ExistingServices table, remove duplicates

	DECLARE @CombinedNextServices TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max),
		ClientName NVARCHAR(MAX)
	)

	--Here we add the next occurrences to the final temporary table
	INSERT INTO @CombinedNextServices (RecurringServiceId, OccurDate, ServiceName)
	SELECT DISTINCT RecurringServiceId, OccurDate, ServiceName
	FROM @NextRecurringServiceOccurrenceTable

	INSERT INTO @CombinedNextServices (RecurringServiceId, ServiceId, OccurDate, ServiceName)
	SELECT RecurringServiceId, ServiceId, OccurDate, ServiceName
	FROM @NextExistingServiceTable

	DELETE FROM @CombinedNextServices
	WHERE ServiceId IS NULL
	AND RecurringServiceId IN
	(
		SELECT RecurringServiceId
		FROM @NextExistingServiceTable
		WHERE OccurDate = [@CombinedNextServices].OccurDate
	)

	UPDATE @CombinedNextServices
	SET ClientName = (SELECT Name FROM dbo.Clients WHERE Id = (SELECT ClientId FROM dbo.[Services] WHERE Id = [@CombinedNextServices].ServiceId))
	WHERE ServiceId IS NOT NULL
  
	UPDATE @CombinedNextServices
	SET ClientName = (SELECT Name FROM dbo.Clients WHERE Id = (SELECT ClientId FROM dbo.RecurringServices WHERE Id = [@CombinedNextServices].RecurringServiceId))
	WHERE ServiceId IS NULL  
END

IF @withFields = 0
	BEGIN
		SELECT RecurringServiceId ,
				ServiceId ,
				OccurDate ,
				ServiceName FROM @CombinedNextServices
		ORDER BY OccurDate
	END
ELSE IF @serviceTypeContext IS NOT NULL
	BEGIN
	SELECT * from @CombinedNextServices
	WHERE ServiceName =  @serviceTypeContext
	ORDER BY OccurDate
	END
ELSE
	BEGIN
	SELECT * from @CombinedNextServices 
	ORDER BY OccurDate
	END

RETURN
END