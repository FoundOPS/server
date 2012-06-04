SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Use Core
GO
IF OBJECT_ID(N'[dbo].[GetServicesToBeCreatedForDate]', N'FN') IS NOT NULL
DROP FUNCTION [dbo].[GetServicesToBeCreatedForDate]
GO
/*****************************************************************************************************************************************************************************************************************************
* FUNCTION GetServicesToBeCreatedForDate will take the context provided and find all the services that are scheduled for that day, but have not yet been created
** Input Parameters **
* @ServiceProviderId - The BusinessAccount context
* @serviceDate - The date in which you are looking for new services to create
** Output Parameters: **
* @NewServices - Ex. below
* Id		| ClientId	| ServiceProviderId	| RecurringServiceId|	ServiceDate
* ----------------------------------------------------------------------------
* {GUID}	| {GUID}    | {GUID}			| {GUID}			|	{DateTime}
* {GUID}	| {GUID}    | {GUID}			| {GUID}			|	{DateTime}	
* {GUID}	| {GUID}	| {GUID}			| {GUID}			|	{DateTime} 
****************************************************************************************************************************************************************************************************************************/
CREATE FUNCTION [dbo].[GetServicesToBeCreatedForDate]
(@ServiceProviderId uniqueidentifier,
@serviceDate date)
RETURNS @NewServices TABLE
	(
		Id uniqueidentifier,
		ClientId uniqueidentifier,
		ServiceProviderId uniqueidentifier,
		RecurringServiceId uniqueidentifier,
		ServiceDate date
	)
AS
BEGIN

	--Stores the Recurring Services that are associated with the lowest context provided
	DECLARE @RepeatsTableWithNextOccurrence TABLE
	(Id uniqueidentifier,
     EndDate date,
     EndAfterTimes int,
     RepeatEveryTimes int,
     FrequencyInt int,
     FrequencyDetailInt int,
     StartDate date,
	 NextDate date,
	 ServiceName nvarchar(max))

--------------------------------------------------------------------------------------------------------------------------------------------------------
--Get all Services scheduled for today, find out if there is already an existing Service and Create it if it doesnt exists already
--------------------------------------------------------------------------------------------------------------------------------------------------------

	INSERT INTO @RepeatsTableWithNextOccurrence (Id, EndDate, EndAfterTimes, RepeatEveryTimes, FrequencyInt, FrequencyDetailInt, StartDate, ServiceName)
	--This is a Semi-Join between the Clients table created above and the RecurringServices Table
	--Semi-Join simply means that it has all the same logic as a normal join, but it doesnt actually join the tables
	--In this case, it finds all the RecurringServices that correspond to a Client with a vendorId = @ServiceProviderId
	--This actually is a double semi-join, so it tasks all the RecurringServices found and finds all their Repeat schedules as well as the ServiceName
	SELECT DISTINCT	t1.Id, t1.EndDate, t1.EndAfterTimes, t1.RepeatEveryTimes, t1.FrequencyInt, t1.FrequencyDetailInt, t1.StartDate, t2.Name
	FROM		Repeats t1, ServiceTemplates t2
	WHERE		EXISTS
	(
		SELECT DISTINCT	*
		FROM	Clients
		WHERE	EXISTS
		(
		SELECT DISTINCT	*
		FROM	RecurringServices
		WHERE	RecurringServices.ClientId = Clients.Id 
				AND Clients.VendorId = @ServiceProviderId 
				AND RecurringServices.Id = t1.Id
				AND RecurringServices.Id = t2.Id
		)
	)

	--Finds the next scheduled date for the service
	UPDATE @RepeatsTableWithNextOccurrence
	SET NextDate = (SELECT dbo.GetNextOccurence(@serviceDate, StartDate,  EndDate, EndAfterTimes, FrequencyInt, RepeatEveryTimes, FrequencyDetailInt))
	FROM @RepeatsTableWithNextOccurrence

	--Remove any rows that do not have a NextOccurrence on the OnOrAfterDate
	DELETE FROM @RepeatsTableWithNextOccurrence
	WHERE NextDate IS NULL OR NextDate <> @serviceDate
	
	--Will store all the Services after the OnOrAfterDate
	DECLARE @GeneratedServices TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)

	--Keeps all the Services that could be generated for the date. 
	INSERT INTO @GeneratedServices (RecurringServiceId, OccurDate, ServiceName)
	SELECT Id, NextDate, ServiceName
	FROM @RepeatsTableWithNextOccurrence

	--This table will store all Existing Services on the OnOrAfterDate
	DECLARE @ExistingServices TABLE
	(RecurringServiceId uniqueidentifier,
	ServiceId uniqueidentifier,
	OccurDate date,
	ServiceName nvarchar(max))

	--Fills @ExistingServices with Existing Services on the SeedDate
	INSERT INTO @ExistingServices (RecurringServiceId, ServiceId, OccurDate, ServiceName)
		SELECT  t1.RecurringServiceId, t1.Id, t1.ServiceDate, t2.Name
		FROM	Services t1, ServiceTemplates t2
		WHERE	t1.ServiceProviderId = @ServiceProviderId 
				AND t1.ServiceDate = @serviceDate
				AND t1.Id = t2.Id

	--This table will store all Existing Services on the OnOrAfterDate
	DECLARE @tempServicesForDate TABLE
	(RecurringServiceId uniqueidentifier,
	ServiceId uniqueidentifier,
	OccurDate date,
	ServiceName nvarchar(max))

	INSERT INTO @tempServicesForDate
	SELECT * FROM @GeneratedServices
	
	INSERT INTO @tempServicesForDate
	SELECT * FROM @ExistingServices

	--This table will temporarily store the RecurringServiceId's of all rows that appear more than once in @TempTable
	DECLARE @DuplicateIdTable TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceDate nvarchar(max)
	)

	INSERT INTO @DuplicateIdTable
	SELECT t1.RecurringServiceId, t2.ServiceId, t2.OccurDate, t2.ServiceName FROM (SELECT OccurDate, RecurringServiceId, ServiceId FROM @tempServicesForDate AS t2 WHERE ServiceId IS NOT NULL GROUP BY OccurDate, RecurringServiceId, ServiceId) t1
	JOIN @tempServicesForDate as t2 on t1.RecurringServiceId = t2.RecurringServiceId AND t1.OccurDate = t2.OccurDate AND t2.ServiceId IS NULL
	
	--Will hold all Schedules that still need to have a Service created for them
	DECLARE @ServicesForDate TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)

	--Filtered down from the table above so that no duplicates are created
	INSERT INTO @ServicesForDate
	SELECT * FROM @tempServicesForDate
	EXCEPT
	SELECT * FROM @DuplicateIdTable

	DECLARE @PreRoutedServices TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)
	
	--Finds all the Schedules that have already been put into a Route
	INSERT INTO @PreRoutedServices (RecurringServiceId, ServiceId, OccurDate, ServiceName)
	SELECT t1.RecurringServiceId, t1.Id, t1.ServiceDate, t2.Name
	FROM Services t1, ServiceTemplates t2
	WHERE	t1.ServiceProviderId = @ServiceProviderId 
				AND t1.ServiceDate = @serviceDate
				AND t1.Id = t2.Id
				AND EXISTS	( 
								SELECT *
								FROM  RouteTasks t3
								WHERE EXISTS
								(
									SELECT *
									FROM RouteDestinations t4
									WHERE EXISTS
									(
										SELECT * FROM Routes t5
										WHERE t4.RouteId = t5.Id AND t3.RouteDestinationId = t4.Id AND t1.Id = t3.ServiceId AND t5.Date = @serviceDate
									)
								)
							)

	DECLARE @ServicesToBeCreatedForDate TABLE
	(
		RecurringServiceId uniqueidentifier,
		ServiceId uniqueidentifier,
		OccurDate date,
		ServiceName nvarchar(max)
	)

	--Cuts down the list of Schedules to only those that need a Service to be Created
	INSERT INTO @ServicesToBeCreatedForDate
	SELECT DISTINCT * FROM @ServicesForDate
	EXCEPT
	SELECT DISTINCT * FROM @PreRoutedServices

	-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	-- Now that we have all of the services that need to be Created for today, Create them
	-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	INSERT INTO @NewServices (Id, RecurringServiceId, ServiceDate)
	SELECT ServiceId, RecurringServiceId, OccurDate
	FROM @ServicesToBeCreatedForDate

	UPDATE @NewServices
	SET Id = (SELECT NEWID()) WHERE Id IS NULL

	UPDATE @NewServices
	SET ServiceProviderId = @ServiceProviderId
		
	UPDATE @NewServices
	SET ClientId = (
					SELECT DISTINCT t1.Id
					FROM	Parties_Business t1
					WHERE	EXISTS
					(
						SELECT  t2.ClientId
						FROM	RecurringServices t2
						WHERE	RecurringServiceId = t2.Id 
						AND		t2.ClientId = t1.Id
					)
				   )

RETURN 
END