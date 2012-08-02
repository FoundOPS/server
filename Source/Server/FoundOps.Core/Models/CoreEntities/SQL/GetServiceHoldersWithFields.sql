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
-- Create date: 6/28/2012
-- Description:	
-- =============================================
CREATE PROCEDURE GetServiceHoldersWithFields
	(
	  @serviceProviderIdContext UNIQUEIDENTIFIER ,
	  @clientIdContext UNIQUEIDENTIFIER ,
	  @recurringServiceIdContext UNIQUEIDENTIFIER ,
	  @firstDate DATE ,
	  @lastDate DATE
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
					@lastDate
	
		DECLARE @ServiceTemplateIds TABLE 
		(
			Id UNIQUEIDENTIFIER
		)

		INSERT INTO @ServiceTemplateIds
		SELECT Id FROM dbo.Services
		WHERE Id IN 
		(
			SELECT ServiceId
			FROM #ServiceHolders
			WHERE ServiceId IS NOT NULL
		)

		INSERT INTO @ServiceTemplateIds
		SELECT Id FROM dbo.RecurringServices
		WHERE Id IN 
		(
			SELECT RecurringServiceId
			FROM #ServiceHolders
			WHERE ServiceId IS NULL
		)

		BEGIN --Declaring Fields Tables

			CREATE TABLE #DateTimeFields
				(
				  Earliest DATETIME ,
				  Latest DATETIME ,
				  TypeInt SMALLINT ,
				  [Value] DATETIME ,
				  Id UNIQUEIDENTIFIER,
				  FieldType NVARCHAR(MAX) ,
				  ServiceTemplateId UNIQUEIDENTIFIER ,
				  FieldName NVARCHAR(MAX)
				)  

			CREATE TABLE #LocationFields
				(
				  LocationId UNIQUEIDENTIFIER ,
				  LocationFieldTypeInt SMALLINT ,
				  Id UNIQUEIDENTIFIER,
				  FieldType NVARCHAR(MAX) ,
				  ServiceTemplateId UNIQUEIDENTIFIER ,
				  FieldName NVARCHAR(MAX)
				)  

			CREATE TABLE #NumericFields
				(
				  Mask NVARCHAR(MAX) ,
				  DecimalPlaces INT ,
				  Minimum DECIMAL(16, 6) ,
				  Maximum DECIMAL(16, 6) ,
				  [Value] DECIMAL(16, 6) ,
				  Id UNIQUEIDENTIFIER,
				  FieldType NVARCHAR(MAX) ,
				  ServiceTemplateId UNIQUEIDENTIFIER ,
				  FieldName NVARCHAR(MAX)
				)  
		
			CREATE TABLE #OptionsFields 
				(
				  AllowMultipleSelection BIT ,
				  TypeInt SMALLINT ,
				  Id UNIQUEIDENTIFIER,
				  FieldType NVARCHAR(MAX) ,
				  ServiceTemplateId UNIQUEIDENTIFIER ,
				  FieldName NVARCHAR(MAX),
				  [Value] NVARCHAR(MAX)
				)    
		
			DECLARE	@Options TABLE
				(
				  Id UNIQUEIDENTIFIER ,
				  Name NVARCHAR(MAX) ,
				  IsChecked BIT ,
				  OptionsFieldId UNIQUEIDENTIFIER ,
				  [Index] INT ,
				  Tooltip NVARCHAR(MAX)
				)

			CREATE TABLE #TextBoxFields
				(
				  IsMultiLine BIT ,
				  [Value] NVARCHAR(MAX) ,
				  Id UNIQUEIDENTIFIER,
				  FieldType NVARCHAR(MAX) ,
				  ServiceTemplateId UNIQUEIDENTIFIER ,
				  FieldName NVARCHAR(MAX)
				)  
			
		END   
	
		BEGIN --Insert into all Fields Tables

			INSERT INTO	#DateTimeFields (Earliest, Latest, TypeInt, Value, Id)
			SELECT * FROM dbo.Fields_DateTimeField
			WHERE Id IN 
			(
				SELECT Id 
				FROM dbo.Fields
				WHERE ServiceTemplateId IN
				(
					SELECT Id FROM @ServiceTemplateIds
				)          
			)

			INSERT INTO	#LocationFields (LocationId, LocationFieldTypeInt, Id)
			SELECT * FROM dbo.Fields_LocationField
			WHERE Id IN 
			(
				SELECT Id 
				FROM dbo.Fields
				WHERE ServiceTemplateId IN
				(
					SELECT Id FROM @ServiceTemplateIds
				)          
			)

			INSERT INTO	#NumericFields (Mask, DecimalPlaces, Minimum, Maximum, Value, Id)
			SELECT * FROM dbo.Fields_NumericField
			WHERE Id IN 
			(
				SELECT Id 
				FROM dbo.Fields
				WHERE ServiceTemplateId IN
				(
					SELECT Id FROM @ServiceTemplateIds
				)          
			)

			INSERT INTO #OptionsFields (AllowMultipleSelection, TypeInt, Id)
			SELECT * FROM dbo.Fields_OptionsField
			WHERE Id IN 
			(
				SELECT Id 
				FROM dbo.Fields
				WHERE ServiceTemplateId IN
				(
					SELECT Id FROM @ServiceTemplateIds
				)          
			)

			DECLARE @CheckedOptions TABLE 
			(
				OptionsFieldId UNIQUEIDENTIFIER ,
				[Name] NVARCHAR(MAX)
			)

			INSERT INTO @CheckedOptions
			SELECT OptionsFieldId, Name FROM dbo.Options --OptionsFieldId, Name
			WHERE IsChecked = 1 
			AND OptionsFieldId IN (SELECT Id FROM #OptionsFields)
			
			--Concatinates all Options that were checked as follows: op1, op2, op3,
			UPDATE #OptionsFields
			SET Value = (SELECT Name + ', ' AS 'data()'
			FROM @CheckedOptions 
			WHERE OptionsFieldId = [#OptionsFields].Id
			FOR XML PATH(''))

			--Removes the extra comma inserted at the end of the Value string because of concatination
			UPDATE #OptionsFields
			SET Value = LEFT(Value, LEN(Value) -1)

			INSERT INTO	#TextBoxFields (IsMultiLine, Value, Id)
			SELECT * FROM dbo.Fields_TextBoxField
			WHERE Id IN 
			(
				SELECT Id 
				FROM dbo.Fields
				WHERE ServiceTemplateId IN
				(
					SELECT Id FROM @ServiceTemplateIds
				)          
			)

		END

		BEGIN --Set all ServiceTemplateId's and Field Names
			
			UPDATE #DateTimeFields
			SET ServiceTemplateId = (SELECT ServiceTemplateId FROM dbo.Fields WHERE Id = [#DateTimeFields].Id),
				FieldName = (SELECT Name FROM dbo.Fields WHERE Id = [#DateTimeFields].Id)

			UPDATE #LocationFields
			SET ServiceTemplateId = (SELECT ServiceTemplateId FROM dbo.Fields WHERE Id = [#LocationFields].Id),
				FieldName = (SELECT Name FROM dbo.Fields WHERE Id = [#LocationFields].Id)

			UPDATE #NumericFields
			SET ServiceTemplateId = (SELECT ServiceTemplateId FROM dbo.Fields WHERE Id = [#NumericFields].Id),
				FieldName = (SELECT Name FROM dbo.Fields WHERE Id = [#NumericFields].Id)

			UPDATE #OptionsFields
			SET ServiceTemplateId = (SELECT ServiceTemplateId FROM dbo.Fields WHERE Id = [#OptionsFields].Id),
				FieldName = (SELECT Name FROM dbo.Fields WHERE Id = [#OptionsFields].Id)

			UPDATE #TextBoxFields
			SET ServiceTemplateId = (SELECT ServiceTemplateId FROM dbo.Fields WHERE Id = [#TextBoxFields].Id),
				FieldName = (SELECT Name FROM dbo.Fields WHERE Id = [#TextBoxFields].Id)
		END    

		BEGIN --Set all FieldTypes to their appropriate values
			UPDATE #NumericFields
			SET FieldType = Replace(FieldName, ' ', '_')
		
			UPDATE #LocationFields
			SET FieldType =  Replace(FieldName, ' ', '_')

			UPDATE #DateTimeFields
			SET FieldType = Replace(FieldName, ' ', '_')

			UPDATE #OptionsFields
			SET FieldType = Replace(FieldName, ' ', '_')
		
			UPDATE #TextBoxFields
			SET FieldType = Replace(FieldName, ' ', '_')
		END

		BEGIN --Add fields to the #ServiceHolders table in their own columns
			DECLARE @RowCount INT
			DECLARE @FieldType NVARCHAR(MAX)
			DECLARE @cmd nvarchar(MAX)

			--DateTime Fields
			SET @RowCount = (SELECT COUNT(*) FROM #DateTimeFields)
			IF @RowCount > 0
			BEGIN
				WHILE @RowCount > 0
				BEGIN
				SET @FieldType = (SELECT Max(FieldType) FROM #DateTimeFields)

				SET @cmd = 'ALTER TABLE #ServiceHolders ADD [' + @FieldType + '] DATETIME'
				EXEC(@cmd)

				SET @cmd = 
				'UPDATE #ServiceHolders
				SET [' + @FieldType + '] = (
				SELECT t1.Value
				FROM [#DateTimeFields] t1
				WHERE ([#ServiceHolders].ServiceId = t1.ServiceTemplateId OR ([#ServiceHolders].RecurringServiceId = t1.ServiceTemplateId AND [#ServiceHolders].ServiceId IS NULL))
				AND @FieldType = t1.FieldType)'

				EXECUTE sp_executesql @cmd , N'@FieldType NVARCHAR(MAX)', @FieldType

				DELETE FROM #DateTimeFields
				WHERE @FieldType = FieldType 
		
				SET @RowCount = (SELECT COUNT(*) FROM #DateTimeFields)
				END            
			END
        
			--Numeric Fields
			SET @RowCount = (SELECT COUNT(*) FROM #NumericFields)
			IF @RowCount > 0
			BEGIN
				WHILE @RowCount > 0
				BEGIN
				SET @FieldType = (SELECT Max(FieldType) FROM #NumericFields)

				SET @cmd = 'ALTER TABLE #ServiceHolders ADD [' + @FieldType + '] DECIMAL(16, 6)'
				EXEC(@cmd)

				SET @cmd = 
				'UPDATE #ServiceHolders
				SET [' + @FieldType + '] = (
				SELECT t1.Value
				FROM [#NumericFields] t1
				WHERE ([#ServiceHolders].ServiceId = t1.ServiceTemplateId OR ([#ServiceHolders].RecurringServiceId = t1.ServiceTemplateId AND [#ServiceHolders].ServiceId IS NULL))
				AND @FieldType = t1.FieldType)'

				EXECUTE sp_executesql @cmd , N'@FieldType NVARCHAR(MAX)', @FieldType

				DELETE FROM #NumericFields
				WHERE @FieldType = FieldType 
		
				SET @RowCount = (SELECT COUNT(*) FROM #NumericFields)
				END            
			END
		      
			--Location Fields
			SET @RowCount = (SELECT COUNT(*) FROM #LocationFields)
			IF @RowCount > 0
			BEGIN
				WHILE @RowCount > 0
				BEGIN
				SET @FieldType = (SELECT Max(FieldType) FROM #LocationFields)

				SET @cmd = 'ALTER TABLE #ServiceHolders ADD [' + @FieldType + '] NVARCHAR(MAX)'
				EXEC(@cmd)

				SET @cmd = 
				'UPDATE #ServiceHolders
				SET [' + @FieldType + '] = ( SELECT Name FROM dbo.Locations WHERE Id =
				(SELECT t1.LocationId
				FROM [#LocationFields] t1
				WHERE ([#ServiceHolders].ServiceId = t1.ServiceTemplateId OR ([#ServiceHolders].RecurringServiceId = t1.ServiceTemplateId AND [#ServiceHolders].ServiceId IS NULL))
				AND @FieldType = t1.FieldType))'

				EXECUTE sp_executesql @cmd , N'@FieldType NVARCHAR(MAX)', @FieldType

				DELETE FROM #LocationFields
				WHERE @FieldType = FieldType 
		
				SET @RowCount = (SELECT COUNT(*) FROM #LocationFields)
				END            
			END

			--Options Fields

			SET @RowCount = (SELECT COUNT(*) FROM #OptionsFields)
			IF @RowCount > 0
			BEGIN
				WHILE @RowCount > 0
				BEGIN			
			  			  	
				SET @FieldType = (SELECT MIN(FieldType) FROM #OptionsFields)

				SET @cmd = 'ALTER TABLE #ServiceHolders ADD [' + @FieldType + '] NVARCHAR(MAX)'
				EXEC(@cmd)

				SET @cmd = 
				'UPDATE #ServiceHolders
				SET [' + @FieldType + '] = (
				SELECT t1.Value
				FROM [#OptionsFields] t1
				WHERE ([#ServiceHolders].ServiceId = t1.ServiceTemplateId OR ([#ServiceHolders].RecurringServiceId = t1.ServiceTemplateId AND [#ServiceHolders].ServiceId IS NULL))
				AND @FieldType = t1.FieldType)'

				EXECUTE sp_executesql @cmd , N'@FieldType NVARCHAR(MAX)', @FieldType

				DELETE FROM #OptionsFields
				WHERE FieldType = @FieldType 
		
				SET @RowCount = (SELECT COUNT(*) FROM #OptionsFields)
				END            
			END      

			--TextBox Fields
			SET @RowCount = (SELECT COUNT(*) FROM #TextBoxFields)
			IF @RowCount > 0
			BEGIN
				WHILE @RowCount > 0
				BEGIN 	
				SET @FieldType = (SELECT Max(FieldType) FROM #TextBoxFields)

				SET @cmd = 'ALTER TABLE #ServiceHolders ADD [' + @FieldType + '] NVARCHAR(MAX)'
				EXEC(@cmd)

				SET @cmd = 
				'UPDATE #ServiceHolders
				SET [' + @FieldType + '] = (
				SELECT t1.Value
				FROM [#TextBoxFields] t1
				WHERE ([#ServiceHolders].ServiceId = t1.ServiceTemplateId OR ([#ServiceHolders].RecurringServiceId = t1.ServiceTemplateId AND [#ServiceHolders].ServiceId IS NULL))
				AND @FieldType = t1.FieldType)'

				EXECUTE sp_executesql @cmd , N'@FieldType NVARCHAR(MAX)', @FieldType

				DELETE FROM #TextBoxFields
				WHERE @FieldType = FieldType 
		
				SET @RowCount = (SELECT COUNT(*) FROM #TextBoxFields)
				END            
			END    
		END

		SELECT * FROM #ServiceHolders ORDER BY ServiceName, OccurDate

		DROP TABLE #NumericFields
		DROP TABLE #TextBoxFields
		DROP TABLE #LocationFields
		DROP TABLE #DateTimeFields
		DROP TABLE #OptionsFields
		DROP TABLE #ServiceHolders
		RETURN 
        
	END
GO