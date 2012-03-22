DECLARE @datetime datetime

-- @onOrAfterDate datetime,@startDate datetime,@endDate datetime,@endAfterTimes int,@frequencyInt int,@repeatEveryTimes int,@frequencyDetailInt char(7)

print('His test for NextRepeatDateOnOrAfterDate')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-3-2011','5-3-2011',null,null,1,null,null),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-3-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-4-2011','5-3-2011',null,null,1,null,null),'1-1-1900'))
select DATEDIFF(day, @datetime, '1-1-1900')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-1-2011','5-3-2011',null,null,1,null,null),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-3-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('3-31-2012','4-1-2012',null,null,1,null,null),'1-1-1900'))
select DATEDIFF(day, @datetime, '4-1-2012')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('3-31-2012','4-1-2012',null,null,1,null,null),'1-1-1900'))
select DATEDIFF(day, @datetime, '4-1-2012')

print('NextRepeatDateOnOrAfterDateDailyTest')

SET @datetime =  convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-2-2011','5-3-2011',null,10,2,3,null),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-3-2011')

SET @datetime =  convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-5-2011','5-3-2011',null,10,2,3,null),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-6-2011')

SET @datetime =  convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-7-2011','5-3-2011',null,10,2,3,null),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-9-2011')

SET @datetime =  convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-12-2011','5-3-2011',null,10,2,3,null),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-12-2011')

SET @datetime =  convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-30-2011','5-3-2011',null,10,2,3,null),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-30-2011')

SET @datetime =  convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-31-2011','5-3-2011',null,10,2,3,null),'1-1-1900'))
select DATEDIFF(day, @datetime, '1-1-1900')

SET @datetime =  convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-29-2011','5-3-2011',null,10,2,3,null),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-30-2011')

SET @datetime =  convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-28-2011','5-3-2011',null,10,2,3,null),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-30-2011')

SET @datetime =  convert(nvarchar(50), isnull(dbo.GetNextOccurence('6-30-2011','5-3-2011',null,10,2,3,null),'1-1-1900'))
select DATEDIFF(day, @datetime, '1-1-1900')

SET @datetime =  convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-28-2011','5-3-2011','5-29-2011',10,2,3,null),'1-1-1900'))
select DATEDIFF(day, @datetime, '1-1-1900')


print 'NextRepeatDateOnOrAfterDateYearlyTest'
SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-4-2011','5-3-2011','10-20-2020',null,5,2,null),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-3-2013')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-3-2011','5-3-2011','10-20-2020',null,5,2,null),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-3-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-31-2011','5-3-2011','10-20-2020',null,5,2,null),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-3-2013')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-1-2011','5-3-2011','10-20-2020',null,5,2,null),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-3-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-1-2012','5-3-2011','10-20-2020',null,5,2,null),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-3-2013')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-1-2013','5-3-2011','10-20-2020',null,5,2,null),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-3-2013')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-2-2013','5-3-2011','10-20-2020',null,5,2,null),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-3-2013')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-3-2013','5-3-2011','10-20-2020',null,5,2,null),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-3-2013')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-4-2013','5-3-2011','10-20-2020',null,5,2,null),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-3-2015')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-4-2013','2-29-2012',null,null,5,3,null),'1-1-1900'))
select DATEDIFF(day, @datetime, '2-29-2024')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('2-4-2013','2-27-2012','10-20-2020',null,5,1,null),'1-1-1900'))
select DATEDIFF(day, @datetime, '2-27-2013')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('2-4-2013','2-14-2012','10-20-2020',null,5,1,null),'1-1-1900'))
select DATEDIFF(day, @datetime, '2-14-2013')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('1-4-2013','1-31-2012','10-20-2020',null,5,1,null),'1-1-1900'))
select DATEDIFF(day, @datetime, '1-31-2013')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('1-4-2013','1-31-2012','1-5-2013',null,5,1,null),'1-1-1900'))
select DATEDIFF(day, @datetime, '1-1-1900')

print 'NextRepeatDateOnOrAfterDateWeeklyTest'
SET @datetime =  convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-1-2011','5-1-2011',null,10,3,2,15),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-1-2011')

SET @datetime =  convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-2-2011','5-1-2011',null,10,3,2,15),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-5-2011')

SET @datetime =  convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-3-2011','5-1-2011',null,10,3,2,15),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-5-2011')

SET @datetime =  convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-4-2011','5-1-2011',null,10,3,2,15),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-5-2011')

SET @datetime =  convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-5-2011','5-1-2011',null,10,3,2,15),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-5-2011')

SET @datetime =  convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-6-2011','5-1-2011',null,10,3,2,15),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-15-2011')

SET @datetime =  convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-7-2011','5-1-2011',null,10,3,2,15),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-15-2011')

SET @datetime =  convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-8-2011','5-1-2011',null,10,3,2,15),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-15-2011')

SET @datetime =  convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-9-2011','5-1-2011',null,10,3,2,15),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-15-2011')

SET @datetime =  convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-10-2011','5-1-2011',null,10,3,2,15),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-15-2011')

SET @datetime =  convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-11-2011','5-1-2011',null,10,3,2,15),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-15-2011')

SET @datetime =  convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-12-2011','5-1-2011',null,10,3,2,15),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-15-2011')

SET @datetime =  convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-13-2011','5-1-2011',null,10,3,2,15),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-15-2011')

SET @datetime =  convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-14-2011','5-1-2011',null,10,3,2,15),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-15-2011')

SET @datetime =  convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-15-2011','5-1-2011',null,10,3,2,15),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-15-2011')

SET @datetime =  convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-16-2011','5-1-2011',null,10,3,2,15),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-19-2011')

SET @datetime =  convert(nvarchar(50), isnull(dbo.GetNextOccurence('7-1-2011','5-1-2011',null,10,3,2,15),'1-1-1900'))
select DATEDIFF(day, @datetime, '1-1-1900')

SET @datetime =  convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-1-2011','5-1-2011',null,10,3,1,15),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-1-2011')

SET @datetime =  convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-2-2011','5-1-2011',null,10,3,1,15),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-5-2011')

SET @datetime =  convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-3-2011','5-1-2011',null,10,3,1,15),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-5-2011')

SET @datetime =   convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-4-2011','5-1-2011',null,10,3,1,15),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-5-2011')

SET @datetime =   convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-5-2011','5-1-2011',null,10,3,1,15),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-5-2011')

SET @datetime =   convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-6-2011','5-1-2011',null,10,3,1,15),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-8-2011')

SET @datetime =   convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-7-2011','5-1-2011',null,10,3,1,15),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-8-2011')

SET @datetime =   convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-8-2011','5-1-2011',null,10,3,1,15),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-8-2011')

SET @datetime =   convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-9-2011','5-1-2011',null,10,3,1,15),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-12-2011')

SET @datetime =   convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-10-2011','5-1-2011',null,10,3,1,15),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-12-2011')

SET @datetime =   convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-11-2011','5-1-2011',null,10,3,1,15),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-12-2011')

SET @datetime =   convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-12-2011','5-1-2011',null,10,3,1,15),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-12-2011')

SET @datetime =   convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-13-2011','5-1-2011',null,10,3,1,15),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-15-2011')

SET @datetime =   convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-14-2011','5-1-2011',null,10,3,1,15),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-15-2011')

SET @datetime =   convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-15-2011','5-1-2011',null,10,3,1,15),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-15-2011')

SET @datetime =   convert(nvarchar(50), isnull(dbo.GetNextOccurence('6-3-2011','5-1-2011',null,10,3,1,15),'1-1-1900'))
select DATEDIFF(day, @datetime, '1-1-1900')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('6-4-2011','6-3-2011',null,100,3,1,2346),'1-1-1900'))
select DATEDIFF(day, @datetime, '6-6-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('6-7-2011','6-3-2011',null,100,3,1,2346),'1-1-1900'))
select DATEDIFF(day, @datetime, '6-7-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('6-4-2011','6-3-2011',null,100,3,1,36),'1-1-1900'))
select DATEDIFF(day, @datetime, '6-7-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-1-2011','5-1-2011',null,100,3,2,1234567),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-1-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-2-2011','5-1-2011',null,100,3,2,1234567),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-2-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-3-2011','5-1-2011',null,100,3,2,1234567),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-3-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-4-2011','5-1-2011',null,100,3,2,1234567),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-4-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-5-2011','5-1-2011',null,100,3,2,1234567),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-5-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-6-2011','5-1-2011',null,100,3,2,1234567),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-6-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-7-2011','5-1-2011',null,100,3,2,1234567),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-7-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-8-2011','5-1-2011',null,100,3,2,1234567),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-15-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-9-2011','5-1-2011',null,100,3,2,1234567),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-15-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-10-2011','5-1-2011',null,100,3,2,1234567),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-15-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-11-2011','5-1-2011',null,100,3,2,1234567),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-15-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-12-2011','5-1-2011',null,100,3,2,1234567),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-15-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-13-2011','5-1-2011',null,100,3,2,1234567),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-15-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-14-2011','5-1-2011',null,100,3,2,1234567),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-15-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-15-2011','5-1-2011',null,100,3,2,1234567),'1-1-1900'))
select DATEDIFF(day, @datetime, '5-15-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-18-2011','5-1-2011',null,10,3,2,1234567),'1-1-1900'))
select DATEDIFF(day, @datetime, '1-1-1900')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('6-24-2011','6-23-2011',null,null,3,2,5),'1-1-1900'))
select DATEDIFF(day, @datetime, '7-7-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('6-24-2011','6-23-2011','7-1-2011',null,3,2,5),'1-1-1900'))
select DATEDIFF(day, @datetime, '1-1-1900')


print 'NextRepeatDateOnOrAfterDateMonthlyTest'
SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-4-2011','5-3-2011',null,null,4,2,8),'1-1-1900'))
select DATEDIFF(day, @datetime, '7-3-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-4-2011','5-3-2011',null,null,4,2,11),'1-1-1900'))
select DATEDIFF(day, @datetime, '7-5-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-11-2011','5-10-2011',null,null,4,2,12),'1-1-1900'))
select DATEDIFF(day, @datetime, '7-12-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-18-2011','5-17-2011',null,null,4,2,13),'1-1-1900'))
select DATEDIFF(day, @datetime, '7-19-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('6-1-2011','5-31-2011',null,null,4,2,14),'1-1-1900'))
select DATEDIFF(day, @datetime, '7-26-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-5-2011','5-3-2011',null,null,4,2,8),'1-1-1900'))
select DATEDIFF(day, @datetime, '7-3-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-5-2011','5-3-2011',null,null,4,2,11),'1-1-1900'))
select DATEDIFF(day, @datetime, '7-5-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-12-2011','5-10-2011',null,10,4,2,12),'1-1-1900'))
select DATEDIFF(day, @datetime, '7-12-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-19-2011','5-17-2011',null,10,4,2,13),'1-1-1900'))
select DATEDIFF(day, @datetime, '7-19-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('6-2-2011','5-31-2011',null,10,4,2,14),'1-1-1900'))
select DATEDIFF(day, @datetime, '7-26-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-6-2011','5-3-2011',null,10,4,2,8),'1-1-1900'))
select DATEDIFF(day, @datetime, '7-3-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-6-2011','5-3-2011',null,null,4,2,11),'1-1-1900'))
select DATEDIFF(day, @datetime, '7-5-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-13-2011','5-10-2011',null,10,4,2,12),'1-1-1900'))
select DATEDIFF(day, @datetime, '7-12-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-20-2011','5-17-2011',null,10,4,2,13),'1-1-1900'))
select DATEDIFF(day, @datetime, '7-19-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('6-3-2011','5-31-2011',null,null,4,2,14),'1-1-1900'))
select DATEDIFF(day, @datetime, '7-26-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-7-2011','5-3-2011',null,null,4,2,8),'1-1-1900'))
select DATEDIFF(day, @datetime, '7-3-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-7-2011','5-3-2011',null,null,4,2,11),'1-1-1900'))
select DATEDIFF(day, @datetime, '7-5-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-13-2011','5-10-2011',null,null,4,2,12),'1-1-1900'))
select DATEDIFF(day, @datetime, '7-12-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-21-2011','5-17-2011',null,null,4,2,13),'1-1-1900'))
select DATEDIFF(day, @datetime, '7-19-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('6-3-2011','5-31-2011',null,null,4,2,14),'1-1-1900'))
select DATEDIFF(day, @datetime, '7-26-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-8-2011','5-3-2011',null,null,4,2,8),'1-1-1900'))
select DATEDIFF(day, @datetime, '7-3-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-8-2011','5-3-2011',null,null,4,2,11),'1-1-1900'))--
select DATEDIFF(day, @datetime, '7-5-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-15-2011','5-10-2011',null,null,4,2,12),'1-1-1900'))--
select DATEDIFF(day, @datetime, '7-12-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-22-2011','5-17-2011',null,null,4,2,13),'1-1-1900'))--
select DATEDIFF(day, @datetime, '7-19-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('6-5-2011','5-31-2011',null,null,4,2,14),'1-1-1900'))--
select DATEDIFF(day, @datetime, '7-26-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-9-2011','5-3-2011',null,null,4,2,8),'1-1-1900'))--
select DATEDIFF(day, @datetime, '7-3-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-9-2011','5-3-2011',null,null,4,2,11),'1-1-1900'))--
select DATEDIFF(day, @datetime, '7-5-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-16-2011','5-10-2011',null,null,4,2,12),'1-1-1900'))--
select DATEDIFF(day, @datetime, '7-12-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-23-2011','5-17-2011',null,null,4,2,13),'1-1-1900'))--
select DATEDIFF(day, @datetime, '7-19-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('6-6-2011','5-31-2011',null,null,4,2,14),'1-1-1900'))--
select DATEDIFF(day, @datetime, '7-26-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-10-2011','5-3-2011',null,null,4,2,8),'1-1-1900'))--
select DATEDIFF(day, @datetime, '7-3-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-10-2011','5-3-2011',null,null,4,2,11),'1-1-1900'))--
select DATEDIFF(day, @datetime, '7-5-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-17-2011','5-10-2011',null,null,4,2,12),'1-1-1900'))--
select DATEDIFF(day, @datetime, '7-12-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('5-24-2011','5-17-2011',null,null,4,2,13),'1-1-1900'))--
select DATEDIFF(day, @datetime, '7-19-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('6-7-2011','5-31-2011',null,null,4,2,14),'1-1-1900'))--
select DATEDIFF(day, @datetime, '7-26-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('8-13-2011','8-13-2011',null,null,4,2,12),'1-1-1900'))
select DATEDIFF(day, @datetime, '8-13-2011')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('8-1-2011','7-31-2011','1-25-2012',null,4,2,8),'1-1-1900'))
select DATEDIFF(day, @datetime, '1-1-1900')

SET @datetime = convert(nvarchar(50), isnull(dbo.GetNextOccurence('12-30-2012','10-29-2011',null,null,4,2,8),'1-1-1900'))
select DATEDIFF(day, @datetime, '4-29-2013')


--select DATEDIFF(month, '5-10-2011', '11/3/2012')/2



--SELECT DATEDIFF(month, '2005-12-31 23:59:59.9999999', '2006-01-01 00:00:00.0000000');
--select dateadd(day,-30,'3-30-2012')
--select dateadd(month,1,'1-31-2012')

--select dbo.GetFirstDayOfMonth(getdate())
--select dbo.GetLastDayOfMonth(getdate())
--select dbo.GetFirstOfDayOfWeekInMonth(GETDATE(),1)
--select dbo.GetLastOfDayOfWeekInMonth(GETDATE(),1)

--select dbo.GetNextOccurrence('7/31/2011','8/1/2011',2,4)
--select dbo.GetNextOccurrence('7/31/2011','4/1/2012',2,4)
--select dbo.GetNextDayInMonth('7/31/2011','6/1/2012',2,4)

--select dbo.GetNextDayInMonth('2/29/2012','6/1/2012',12,4)


--DECLARE @date datetime, @tmpdate datetime, @years int, @months int, @days int
--SELECT @date = '6/29/04'

--SELECT @tmpdate = @date

--SELECT @years = DATEDIFF(yy, @tmpdate, GETDATE()) - CASE WHEN (MONTH(@date) > MONTH(GETDATE())) OR (MONTH(@date) = MONTH(GETDATE()) AND DAY(@date) > DAY(GETDATE())) THEN 1 ELSE 0 END
--SELECT @tmpdate = DATEADD(yy, @years, @tmpdate)
--SELECT @months = DATEDIFF(m, @tmpdate, GETDATE()) - CASE WHEN DAY(@date) > DAY(GETDATE()) THEN 1 ELSE 0 END
--SELECT @tmpdate = DATEADD(m, @months, @tmpdate)
--SELECT @days = DATEDIFF(d, @tmpdate, GETDATE())

--SELECT @years, @months, @days

--select charindex('1','123')

--select len(convert(char(7),123))

--declare @DoW char(7),@MeetingDate int
--declare @WeekDay table
--					(
--						DoW int,
--						Meeting bit
--					)
--					set @DoW = '1246'
--					insert into @WeekDay values (1,case when charindex('1',@DoW)>0 then 1 else 0 end )
--					insert into @WeekDay values (2,case when charindex('2',@DoW)>0 then 1 else 0 end )
--					insert into @WeekDay values (3,case when charindex('3',@DoW)>0 then 1 else 0 end )
--					insert into @WeekDay values (4,case when charindex('4',@DoW)>0 then 1 else 0 end )
--					insert into @WeekDay values (5,case when charindex('5',@DoW)>0 then 1 else 0 end )
--					insert into @WeekDay values (6,case when charindex('6',@DoW)>0 then 1 else 0 end )
--					insert into @WeekDay values (7,case when charindex('7',@DoW)>0 then 1 else 0 end )
					
--					select top 1 @MeetingDate = DoW from @WeekDay where Meeting = 1 and DoW >=3
--					select @MeetingDate

--with yearlist as 
--(
--    select 2004 as year
--    union all
--    select yl.year + 1 as year
--    from yearlist yl
--    where yl.year + 1 <= YEAR(GetDate())
--)

--select year from yearlist order by year desc;