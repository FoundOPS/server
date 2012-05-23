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
* @serviceDate - The date the you want to get Resource and Locations for
** Output Parameters: **
* @ServicesTableToReturn - Ex. below
* EmployeeId		| VehicleId | EntityName			| CompassHeading | Latitude	| Longitude	| LastTimeStamp	| Speed	| TrackSource	| RouteId
* ---------------------------------------------------------------------------------------------------------------------------------------------------
* {GUID}			|           | Bob Black <- Employee	| 186			 | 47.456	| -86.166	| DateTime		| 46.32	| iPhone		| {GUID}
* {GUID}			|           | Jane Doe <- Employee	| 45			 | 43.265	| -89.254	| DateTime		| 25.13	| Android		| {GUID}
* {GUID}			| {GUID}	| 372 0925 <- Vehicle	| 321			 | 44.165	| -79.365	| Datetime		| 32.89	| Windows Phone	| {GUID}
*****************************************************************************************************************************************************/

CREATE FUNCTION [dbo].[GetResourcesWithLatestPoint]
(@serviceProviderId uniqueidentifier)
RETURNS @EmployeeVehicleTableToReturn TABLE
	(
		EmployeeId uniqueidentifier,
		VehicleId uniqueidentifier,
		EntityName nvarchar(max),
		CompassHeading int,
		Latitude decimal(18,8),
		Longitude decimal(18,8),
		LastTimeStamp datetime,
		Speed decimal(18,8),
		TrackSource nvarchar(max),
		RouteId uniqueidentifier,
		Accuracy int
	) 
AS
BEGIN

	DECLARE @serviceDate datetime
	SET @serviceDate = CONVERT (date, GETUTCDATE())

	DECLARE @RoutesForDate TABLE
	(
		RouteId uniqueidentifier
	)

	--Finds all Routes for the ServiceProvider on the given date
	INSERT INTO @RoutesForDate
	SELECT Id FROM Routes
	WHERE OwnerBusinessAccountId = @serviceProviderId AND Date = @serviceDate

	DECLARE @EmployeesForRoutesForDate TABLE
	(
		EmployeeId uniqueidentifier,
		EmployeeName nvarchar(max),
		RouteId uniqueidentifier
	)

	DECLARE @VehiclesForRoutesForDate TABLE
	(
		VehicleId uniqueidentifier,
		RouteId uniqueidentifier
	)

	--Pull all employees that are in a Route for the specified day. Keep the EmployeeId and RouteId
	INSERT INTO @EmployeesForRoutesForDate (EmployeeId, RouteId)
	SELECT t1.Technicians_Id, t2.RouteId FROM RouteEmployee t1, @RoutesForDate t2
	WHERE t1.Routes_Id = t2.RouteId

	--Fill in the Employee Name based on the Id
	UPDATE @EmployeesForRoutesForDate
	SET EmployeeName = (SELECT FirstName + ' ' + LastName FROM Parties_Person WHERE Id = EmployeeId)
	FROM @EmployeesForRoutesForDate


	--Pull all vehicles that are in a Route for the specified day. Keep the VehicleId and RouteId
	INSERT INTO @VehiclesForRoutesForDate
	SELECT t1.Vehicles_Id, t2.RouteId FROM RouteVehicle t1, @RoutesForDate t2
	WHERE t1.Routes_Id = t2.RouteId

----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--Combine @EmployeesForRoutesForDate and @VehiclesForRoutesForDate into the final output table
--Most of the data for the output table needs to be pulled from either the Employees or Vehicles tables, this requires a simple combination of INSERT, SELECT and WHERE
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	INSERT INTO @EmployeeVehicleTableToReturn (VehicleId, EntityName, CompassHeading, Latitude, Longitude, LastTimeStamp, Speed, TrackSource, RouteId, Accuracy)
	SELECT t1.Id, t1.VehicleId, t1.LastCompassDirection, t1.LastLatitude, t1.LastLongitude, t1.LastTimeStamp, t1.LastSpeed, t1.LastSource, t2.RouteId, t1.Accuracy FROM Vehicles t1, @VehiclesForRoutesForDate t2 
	WHERE t1.Id = t2.VehicleId

	INSERT INTO @EmployeeVehicleTableToReturn (EmployeeId, EntityName, CompassHeading, Latitude, Longitude, LastTimeStamp, Speed, TrackSource, RouteId, Accuracy)
	SELECT t1.Id, t2.EmployeeName, t1.LastCompassDirection, t1.LastLatitude, t1.LastLongitude, t1.LastTimeStamp, t1.LastSpeed, t1.LastSource, t2.RouteId, t1.Accuracy FROM Employees t1, @EmployeesForRoutesForDate t2 
	WHERE t1.Id = t2.EmployeeId

RETURN 
END

