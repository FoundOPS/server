USE [Core]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF OBJECT_ID(N'[dbo].[GetResourcesWithLatestPoint]', N'FN') IS NOT NULL
DROP FUNCTION [dbo].[GetResourcesWithLatestPoint]
GO
CREATE FUNCTION [dbo].[GetResourcesWithLatestPoint]
(@serviceProviderId uniqueidentifier,
@serviceDate date)
RETURNS @EmployeeVehicleTableToReturn TABLE
	(
		EmployeeId uniqueidentifier,
		VehicleId uniqueidentifier,
		EntityName nvarchar(max),
		CompassHeading int,
		Latitude float(7),
		Longitude float(7),
		LastTimeStamp datetime,
		Speed float(7),
		TrackSource nvarchar(max)
	) 
AS
BEGIN

	DECLARE @RoutesForDate TABLE
	(
		RouteId uniqueidentifier
	)

	--Find all Routes for the ServiceProvider on the given date
	INSERT INTO @RoutesForDate
	SELECT Id FROM Routes
	WHERE OwnerBusinessAccountId = @serviceProviderId

	DECLARE @EmployeesForRoutesForDate TABLE
	(
		EmployeeId uniqueidentifier,
		EmployeeName nvarchar(max)
	)

	DECLARE @VehiclesForRoutesForDate TABLE
	(
		VehicleId uniqueidentifier
	)

	--Find all Employees on the Routes found above 
	INSERT INTO @EmployeesForRoutesForDate (EmployeeId)
	SELECT Technicians_Id FROM RouteEmployee
	WHERE EXISTS
	(
		SELECT	RouteId
		FROM	@RoutesForDate
		WHERE	RouteId = Routes_Id
	)

	--Update @EmployeesForRoutesForDate wuth the Employee names
	UPDATE @EmployeesForRoutesForDate
	SET EmployeeName = (SELECT FirstName + ' ' + LastName FROM Parties_Person WHERE Id = EmployeeId)
	FROM @EmployeesForRoutesForDate

	--Find all Vehicles associated with the Routes found above
	INSERT INTO @VehiclesForRoutesForDate
	SELECT Vehicles_Id FROM RouteVehicle
	WHERE EXISTS
	(
		SELECT	RouteId
		FROM	@RoutesForDate
		WHERE	RouteId = Routes_Id
	)

	--Add Vehicles and all associated data to the final table to be returned
	INSERT INTO @EmployeeVehicleTableToReturn (VehicleId, EntityName, CompassHeading, Latitude, Longitude, LastTimeStamp, Speed, TrackSource)
	SELECT t1.Id, t1.VehicleId, t1.LastCompassDirection, t1.LastLatitude, t1.LastLongitude, t1.LastTimeStamp, t1.LastSpeed, t1.LastSource FROM Vehicles t1, @VehiclesForRoutesForDate t2 
	WHERE t1.Id = t2.VehicleId

	--Add Employees and all associated data to the final table to be returned
	INSERT INTO @EmployeeVehicleTableToReturn (EmployeeId, EntityName, CompassHeading, Latitude, Longitude, LastTimeStamp, Speed, TrackSource)
	SELECT t1.Id, t2.EmployeeName, t1.LastCompassDirection, t1.LastLatitude, t1.LastLongitude, t1.LastTimeStamp, t1.LastSpeed, t1.LastSource FROM Employees t1, @EmployeesForRoutesForDate t2 
	WHERE t1.Id = t2.EmployeeId

RETURN 
END

