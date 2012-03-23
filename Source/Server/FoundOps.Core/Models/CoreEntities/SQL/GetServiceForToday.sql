SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF OBJECT_ID(N'[dbo].[GetServicesForToday]', N'FN') IS NOT NULL
DROP FUNCTION [dbo].[GetServicesForToday]
GO
/****************************************************************************************************************************************************
* FUNCTION GetServicesForToday will take the context provided and find all the services that are scheduled for that day
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
CREATE FUNCTION [dbo].[GetServicesForToday]
(@serviceProviderIdContext uniqueidentifier,
@serviceDate date)
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

	--Stores all the Recurring Services from @TempGenServiceTable and their next occurrence date
	DECLARE @TempNextDateTable TABLE
	(Id uniqueidentifier,
	NextDate date)

	--Takes the Table formed above and will find the next occurrence for all those recurring services
	INSERT INTO @TempNextDateTable (Id, NextDate)
	SELECT Id, dbo.GetNextOccurence(@serviceDate, StartDate,  EndDate, EndAfterTimes, FrequencyInt, RepeatEveryTimes, FrequencyDetailInt)
	FROM   @TempGenServiceTable

	--SELECT * FROM @TempNextDateTable

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
	WHERE NextDate IS NULL OR NextDate > @serviceDate

	--SELECT * FROM @TempGenServiceTableWithNextOccurrence

	--This table will store all Existing Services after to the OnOrAfterDate
	DECLARE @TempNextExistingServiceTable TABLE
	(ServiceId uniqueidentifier,
	RecurringServiceId uniqueidentifier,
	OccurDate date)

	INSERT INTO @TempNextExistingServiceTable (ServiceId, RecurringServiceId, OccurDate)
	SELECT Id, RecurringServiceId, ServiceDate
	FROM Services
	WHERE ServiceProviderId = @serviceProviderIdContext AND ServiceDate = @serviceDate
	ORDER BY ServiceDate

	--Will store all the Services after the OnOrAfterDate
	DECLARE @ServicesForDayTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date
	)

	--Here we add the next occurrences to the final temporary table
	INSERT INTO @ServicesForDayTable (RecurringServiceId, OccurDate)
	SELECT Id, NextDate
	FROM @TempGenServiceTableWithNextOccurrence

	INSERT INTO @ServicesForDayTable (ServiceId, OccurDate)
	SELECT ServiceId, OccurDate
	FROM @TempNextExistingServiceTable

	--This table will temporarily store the RecurringServiceId's of all rows that appear more than once in @TempTable
	DECLARE @DuplicateIdTable TABLE
	(RecurringServiceId uniqueidentifier)

	--Add the duplicate Id's to the table created above
	INSERT INTO @DuplicateIdTable
	SELECT RecurringServiceId FROM @ServicesForDayTable t1
	WHERE 1 < 
	(
		SELECT COUNT (*) 
		FROM @ServicesForDayTable t2
		WHERE t1.RecurringServiceId = t2.RecurringServiceId
	)

	--Delete matching Services between @IdTable and @TempTable. This will delete the Generated Service and will keep the Existing Service
	DELETE FROM @ServicesForDayTable
	WHERE EXISTS
	(
		SELECT * FROM @DuplicateIdTable t1
		WHERE	RecurringServiceId = t1.RecurringServiceId
		AND		ServiceId IS NOT NULL
	)

	INSERT INTO @ServicesTableToReturn
	SELECT DISTINCT *
	FROM @ServicesForDayTable
	ORDER BY OccurDate ASC

RETURN 
END