USE [Core]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF OBJECT_ID(N'[dbo].[GetResourcesWithLatestPoint]', N'FN') IS NOT NULL
DROP FUNCTION [dbo].[GetResourcesWithLatestPoint]
GO
/****************************************************************************************************************************************************
* FUNCTION GetResourcesWithLatestPoint will take a BusinessAccount Id and find all Employees and Vehicles associated with it that are on Routes for the today.
* It will then find all the required information from those Employees and Vehicles and return them in a table as depicted below.
** Input Parameters **
* @serviceProviderId - The BusinessAccount Id that will be used to find all Employees and Vehicles
* @serviceDateUtc - The date (in UTC) you want to get Resource and Locations for
** Output Parameters: **
* @ServicesTableToReturn - Ex. below
* EmployeeId		| VehicleId | EntityName			| CompassHeading | Latitude	| Longitude	| LastTimeStamp	| Speed	| TrackSource	| RouteId
* ---------------------------------------------------------------------------------------------------------------------------------------------------
* {GUID}			|           | Bob Black <- Employee	| 186			 | 47.456	| -86.166	| DateTime		| 46.32	| iPhone		| {GUID}
* {GUID}			|           | Jane Doe <- Employee	| 45			 | 43.265	| -89.254	| DateTime		| 25.13	| Android		| {GUID}
* {GUID}			| {GUID}	| 372 0925 <- Vehicle	| 321			 | 44.165	| -79.365	| Datetime		| 32.89	| Windows Phone	| {GUID}
*****************************************************************************************************************************************************/

CREATE FUNCTION [dbo].[GetResourcesWithLatestPoint]
(@serviceProviderId uniqueidentifier, @serviceDateUtc date)
RETURNS @EmployeeVehicleTableToReturn TABLE
	(
		EmployeeId uniqueidentifier,
		VehicleId uniqueidentifier,
		EntityName nvarchar(max),
		Heading int,
		Latitude decimal(18,8),
		Longitude decimal(18,8),
		CollectedTimeStamp datetime,
		Speed decimal(18,8),
		Source nvarchar(max),
		RouteId uniqueidentifier,
		Accuracy int
	) 
AS
BEGIN

	INSERT INTO @EmployeeVehicleTableToReturn (EmployeeId, RouteId)
	SELECT Employees_Id, Routes_Id FROM dbo.RouteEmployee WHERE Routes_Id IN (SELECT Id FROM dbo.Routes WHERE Date = @serviceDateUtc AND OwnerBusinessAccountId = @serviceProviderId)

	UPDATE @EmployeeVehicleTableToReturn
	SET EntityName = FirstName + ' ' + LastName,
		Heading = LastCompassDirection,
		Latitude = LastLatitude,
		Longitude = LastLongitude,
		CollectedTimeStamp = LastTimeStamp,
		Speed = LastSpeed,
		[Source] = LastSource,
		Accuracy = LastAccuracy
	FROM dbo.Employees t1 WHERE EmployeeId = t1.Id

	DELETE FROM @EmployeeVehicleTableToReturn
	WHERE CollectedTimeStamp < @serviceDateUtc OR CollectedTimeStamp IS NULL

RETURN 
END

