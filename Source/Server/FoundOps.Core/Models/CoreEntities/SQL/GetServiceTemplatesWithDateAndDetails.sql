USE Core
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
sp_configure 'Show Advanced Options', 1 
GO
RECONFIGURE
GO
sp_configure 'Ad Hoc Distributed Queries', 1 
GO
RECONFIGURE
GO
-- =============================================
-- Author:		Zach Bright
-- Create date: 9/7/2012
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[GetServiceTemplatesWithDateAndDetails]
	(
	  @serviceProviderIdContext UNIQUEIDENTIFIER ,
	  @clientIdContext UNIQUEIDENTIFIER ,
	  @recurringServiceIdContext UNIQUEIDENTIFIER ,
	  @firstDate DATE ,
	  @lastDate DATE ,
	  @serviceTypeContext NVARCHAR(MAX)
	)
AS 
	BEGIN

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
		SET NOCOUNT ON;

		CREATE TABLE #ServiceHolders
			(
			  RecurringServiceId UNIQUEIDENTIFIER ,
			  ServiceId UNIQUEIDENTIFIER ,
			  OccurDate DATE ,
			  ServiceName NVARCHAR(MAX) ,
			  ClientName NVARCHAR(MAX)
			)

		INSERT	INTO #ServiceHolders (RecurringServiceId, ServiceId, OccurDate, ServiceName, ClientName)
				EXEC [dbo].[GetServiceHolders] @serviceProviderIdContext,
					@clientIdContext, @recurringServiceIdContext, @firstDate,
					@lastDate, @serviceTypeContext, 1
		
		CREATE TABLE #ServiceTemplateIds
		(
			Id UNIQUEIDENTIFIER
		)
		CREATE TABLE #FieldIds
		(
			Id UNIQUEIDENTIFIER
		)

		IF @serviceTypeContext IS NOT NULL
		BEGIN
			INSERT INTO #ServiceTemplateIds
			SELECT RecurringServiceId FROM #ServiceHolders WHERE ServiceId IS NULL AND ServiceName = @serviceTypeContext
  
			INSERT INTO #ServiceTemplateIds
			SELECT ServiceId FROM #ServiceHolders WHERE ServiceId IS NOT NULL AND ServiceName = @serviceTypeContext
		END  
		ELSE
		BEGIN
			INSERT INTO #ServiceTemplateIds
			SELECT RecurringServiceId FROM #ServiceHolders WHERE ServiceId IS NULL
  
			INSERT INTO #ServiceTemplateIds
			SELECT ServiceId FROM #ServiceHolders WHERE ServiceId IS NOT NULL
		END    

		
		INSERT INTO #FieldIds
		SELECT Id FROM dbo.Fields 
		WHERE ServiceTemplateId IN (SELECT Id FROM #ServiceTemplateIds)

		SELECT * FROM #ServiceHolders 
		ORDER BY OccurDate, ServiceId, RecurringServiceId
		
		SELECT t2.Id, t2.ServiceTemplateId, t2.Name, t1.Value 
		FROM dbo.Fields_DateTimeField t1
		JOIN dbo.Fields t2
		ON t2.Id = t1.Id AND t2.Id IN (SELECT Id FROM #FieldIds)
		ORDER BY t2.ServiceTemplateId

		SELECT t2.Id, t2.ServiceTemplateId, t2.Name, t1.Value 
		FROM dbo.Fields_NumericField t1
		JOIN dbo.Fields t2
		ON t2.Id = t1.Id AND t2.Id IN (SELECT Id FROM #FieldIds)
		ORDER BY t2.ServiceTemplateId

		SELECT t2.Id, t2.ServiceTemplateId, t2.Name, t1.Value 
		FROM dbo.Fields_TextBoxField t1
		JOIN dbo.Fields t2
		ON t2.Id = t1.Id AND t2.Id IN (SELECT Id FROM #FieldIds)
		ORDER BY t2.ServiceTemplateId

		SELECT t2.Id, t2.ServiceTemplateId, t2.Name, t1.Value 
		FROM dbo.Fields_SignatureField t1
		JOIN dbo.Fields t2
		ON t2.Id = t1.Id AND t2.Id IN (SELECT Id FROM #FieldIds)
		ORDER BY t2.ServiceTemplateId

		SELECT t2.Id, t2.ServiceTemplateId, t2.Name, t1.OptionsString, t1.Value
		FROM dbo.Fields_OptionsField t1
		JOIN dbo.Fields t2
		ON t2.Id = t1.Id AND t2.Id IN (SELECT Id FROM #FieldIds)
		ORDER BY t2.ServiceTemplateId

		SELECT t2.Id, t2.ServiceTemplateId, t2.Name, t3.Name + ', ' + t3.AddressLineOne + ' ' + t3.AddressLineTwo AS 'Value'
		FROM dbo.Fields_LocationField t1
		JOIN dbo.Fields t2
		ON t2.Id = t1.Id
		JOIN dbo.Locations t3
		ON t1.LocationId = t3.Id AND t2.Id IN (SELECT Id FROM #FieldIds)
		ORDER BY t2.ServiceTemplateId

		DROP TABLE #FieldIds
		DROP TABLE #ServiceTemplateIds
		DROP TABLE #ServiceHolders

		RETURN
	END
    