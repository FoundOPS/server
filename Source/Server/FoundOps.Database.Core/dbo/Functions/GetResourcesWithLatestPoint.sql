CREATE FUNCTION [dbo].[GetResourcesWithLatestPoint]
(@serviceProviderId uniqueidentifier,
@serviceDate date)
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
		RouteId uniqueidentifier
	) 
AS
BEGIN

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
	INSERT INTO @EmployeeVehicleTableToReturn (VehicleId, EntityName, CompassHeading, Latitude, Longitude, LastTimeStamp, Speed, TrackSource, RouteId)
	SELECT t1.Id, t1.VehicleId, t1.LastCompassDirection, t1.LastLatitude, t1.LastLongitude, t1.LastTimeStamp, t1.LastSpeed, t1.LastSource, t2.RouteId FROM Vehicles t1, @VehiclesForRoutesForDate t2 
	WHERE t1.Id = t2.VehicleId

	INSERT INTO @EmployeeVehicleTableToReturn (EmployeeId, EntityName, CompassHeading, Latitude, Longitude, LastTimeStamp, Speed, TrackSource, RouteId)
	SELECT t1.Id, t2.EmployeeName, t1.LastCompassDirection, t1.LastLatitude, t1.LastLongitude, t1.LastTimeStamp, t1.LastSpeed, t1.LastSource, t2.RouteId FROM Employees t1, @EmployeesForRoutesForDate t2 
	WHERE t1.Id = t2.EmployeeId

RETURN 
END