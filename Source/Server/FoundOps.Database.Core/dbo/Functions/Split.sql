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
CREATE FUNCTION dbo.Split(
	@Id			uniqueidentifier --RecurringServiceId
  , @sInputList VARCHAR(8000) -- List of delimited ExcludedDates
  , @sDelimiter VARCHAR(8000) = ',' -- Delimiter that separates ExcludedDates
) RETURNS @List TABLE (Id uniqueidentifier, ExcludedDate VARCHAR(8000))

BEGIN
DECLARE @sItem VARCHAR(8000)
WHILE CHARINDEX(@sDelimiter,@sInputList,0) <> 0
 BEGIN
 SELECT
  @sItem=RTRIM(LTRIM(SUBSTRING(@sInputList,1,CHARINDEX(@sDelimiter,@sInputList,0)-1))),
  @sInputList=RTRIM(LTRIM(SUBSTRING(@sInputList,CHARINDEX(@sDelimiter,@sInputList,0)+LEN(@sDelimiter),LEN(@sInputList))))
 
 IF LEN(@sItem) > 0
 BEGIN
  INSERT INTO @List (ExcludedDate) SELECT @sItem

  UPDATE @List
  SET Id = @Id
 END
 END

IF LEN(@sInputList) > 0
BEGIN
 INSERT INTO @List (ExcludedDate) SELECT @sInputList -- Put the last ExcludedDate in

 UPDATE @List
 SET Id = @Id	
END
RETURN
END