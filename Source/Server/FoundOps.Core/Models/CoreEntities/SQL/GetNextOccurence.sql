SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Use Core
Go
IF OBJECT_ID(N'[dbo].[GetNextOccurence]', N'FN') IS NOT NULL
DROP FUNCTION [dbo].[GetNextOccurence]
GO
/****************************************************************************************************************************************************
* FUNCTION GetNextOccurence will take in a Service schedule and return the next date that that Service is scheduled to occur that is on or after the OnOrAfterDate
** Input Parameters **
* @onOrAfterDate - The first date the can be accepted as a response
* @startDate - The StartDate of the Repeat
* @endDate - The EndDate of the Repeat
* @endAfterTimes - The number of times the Service is scheduled to occur before it ends
* @frequencyInt - Corresponds to the type of schedule (Once, Daily, Weekly, Monthly or Yearly)
* @repeatEveryTimes - Corresponds to how often the Service is sceduled to repeat (ex. a value of 2 would mean that it repeats every 2 days, weeks, months or years)
* @FrequencyDetailInt - Corresponds to weekly and monthly schedules only. For Weekly it is equal to a list of numbers that correspond to days of the week (ex. 1237 would mean that the service happens on Monday, Tuesday, Wednesday and Sunday)
*						For Monthly it corresponds to when during the month the Service is schedule (ex. LastDayOfMonth, FirstOfDayOfMonth, ThirdOfDayOfMonth, etc.)
** Output Parameters  **
*  DateTime - The date of the next scheduled Service occurrence
***************************************************************************************************************************************************/
    Create FUNCTION [dbo].[GetNextOccurence]
    (@onOrAfterDate datetime,
    @startDate datetime,
    @endDate datetime,
    @endAfterTimes int,
    @frequencyInt int,
    @repeatEveryTimes int,
    @frequencyDetailInt int
    )       
    returns Datetime       
    as       
    begin  		
		declare @ReturnDate datetime,				
				@LastMeeting datetime, @Remains int				
		
		if @endDate is not null and @onOrAfterDate > @endDate
			return null	
		
		if @frequencyInt = 1 
			begin
				if @onOrAfterDate > @startDate
					return null
				else 
					set @ReturnDate = @startDate
			end
		
		else if @frequencyInt = 2
			begin	
				DECLARE @DateDif int			
				set @DateDif = DATEDIFF ( d , @startDate , @onOrAfterDate )
				if @DateDif <=0
					set @ReturnDate = @startDate
				else if @endAfterTimes is not null and @onOrAfterDate > dateadd(d,(@endAfterTimes -1)*@repeatEveryTimes,@startDate) 
					return null					
				else
					begin
						declare @RemainDays int
						set @RemainDays = @DateDif%@repeatEveryTimes
						if @RemainDays = 0
							set @ReturnDate = @onOrAfterDate
						else
							set @ReturnDate = dateadd(d,@repeatEveryTimes - @RemainDays,@onOrAfterDate)
					end
			end
		
		else if @frequencyInt = 3
			begin
				if @onOrAfterDate <= @startDate
					set @ReturnDate = @startDate
				else
					begin
						declare @DoW char(7),@MeetingInWeek int, @FirstDate int, @LastDate int, @Weeks int,@LastWeekRemains int,
								@TempDate datetime,@DoWonOrAfterDate int, @MeetingDate int,@dwStartDate int
						set @DoW = convert(char(7),@frequencyDetailInt)
						set @MeetingInWeek = len(@DoW)
						set @FirstDate = convert(int,substring(@DoW,1,1))
						-- change start date to begin of the week
						set @dwStartDate = datepart(dw,@startDate)
						set @startDate = dateadd(day,@FirstDate - @dwStartDate,@startDate)				
						
						-- get the date of last meeting if @endAfterTimes is not null
						if @endAfterTimes is not null
							begin		
								set @endAfterTimes = @endAfterTimes + charindex(convert(char(1),@dwStartDate),@DoW)-1				
								set @Weeks = @endAfterTimes / @MeetingInWeek
								set @LastWeekRemains = @endAfterTimes % @MeetingInWeek
								if @LastWeekRemains = 0 
									begin
										set @LastDate = convert(int,substring(@DoW,@MeetingInWeek,1))
										set @TempDate = dateadd(week,(@Weeks - 1)*@repeatEveryTimes,@startDate)
										set @TempDate = dateadd(day,@LastDate - @FirstDate,@TempDate)
									end
								else
									begin
										set @LastDate = convert(int,substring(@DoW,@LastWeekRemains,1))
										set @TempDate = dateadd(week,@Weeks*@repeatEveryTimes,@startDate)
										set @TempDate = dateadd(day,@LastDate - @FirstDate,@TempDate)
									end
								if @onOrAfterDate > @TempDate
									return null						
							end						
						set @Weeks = datediff(week,@startDate,@onOrAfterDate)
						if @Weeks%@repeatEveryTimes>0
							set @ReturnDate = dateadd(week,((@Weeks/@repeatEveryTimes)+1)*@repeatEveryTimes,@startDate)
						else
							begin
								declare @WeekDay table
								(
									DoW int,
									Meeting bit
								)
								insert into @WeekDay values (1,case when charindex('1',@DoW)>0 then 1 else 0 end )
								insert into @WeekDay values (2,case when charindex('2',@DoW)>0 then 1 else 0 end )
								insert into @WeekDay values (3,case when charindex('3',@DoW)>0 then 1 else 0 end )
								insert into @WeekDay values (4,case when charindex('4',@DoW)>0 then 1 else 0 end )
								insert into @WeekDay values (5,case when charindex('5',@DoW)>0 then 1 else 0 end )
								insert into @WeekDay values (6,case when charindex('6',@DoW)>0 then 1 else 0 end )
								insert into @WeekDay values (7,case when charindex('7',@DoW)>0 then 1 else 0 end )
								set @DoWonOrAfterDate = datepart(dw,@onOrAfterDate)
								
								select top 1 @MeetingDate = DoW from @WeekDay where Meeting = 1 and DoW >=@DoWonOrAfterDate
								if @MeetingDate is not null
									set @ReturnDate = dateadd(day, @MeetingDate - @DoWonOrAfterDate, @onOrAfterDate)
								else 
									begin
										set @TempDate = dateadd(day, @FirstDate - @DoWonOrAfterDate, @onOrAfterDate)
										set @ReturnDate = dateadd(week,@repeatEveryTimes,@TempDate)
									end 
							end
					end	 
			end
			
		else if @frequencyInt = 4
			begin				
				if @onOrAfterDate <= @startDate
					set @ReturnDate = @startDate
				else
					begin
						declare @Times int, @MonthDiff int
						set @MonthDiff = DATEDIFF(month, @startDate, @onOrAfterDate)
						set @Times = @MonthDiff/@repeatEveryTimes
						set @Remains= @MonthDiff%@repeatEveryTimes 								
						if @frequencyDetailInt = 10
							begin	
								if @Remains>0
									set @Times = @Times+1											
								set @ReturnDate = dbo.GetLastDayOfMonth(dateadd(month,@Times*@repeatEveryTimes,@startDate))
								if @endAfterTimes is not null and DATEDIFF(month, @startDate, @ReturnDate)/@repeatEveryTimes > @endAfterTimes-1
									return null								
							end
						else if @frequencyDetailInt = 11
							begin
								if @Remains>0
									set @Times = @Times+1
								set @ReturnDate = dbo.GetFirstOfDayOfWeekInMonth(dateadd(month,@Times*@repeatEveryTimes,@startDate),datepart(dw,@startDate))
								if @onOrAfterDate > @ReturnDate
									set @ReturnDate = dbo.GetFirstOfDayOfWeekInMonth(dateadd(month,@repeatEveryTimes,@ReturnDate),datepart(dw,@startDate))
								if @endAfterTimes is not null and DATEDIFF(month, @startDate, @ReturnDate)/@repeatEveryTimes > @endAfterTimes-1
									return null								
							end
						else if @frequencyDetailInt = 12
							begin 
								if @Remains>0
									set @Times = @Times+1
								set @ReturnDate = dbo.GetFirstOfDayOfWeekInMonth(dateadd(month,@Times*@repeatEveryTimes,@startDate),datepart(dw,@startDate))
								set @ReturnDate = dateadd(week,1,@ReturnDate)
								if @onOrAfterDate > @ReturnDate
									begin
										set @ReturnDate = dbo.GetFirstOfDayOfWeekInMonth(dateadd(month,@repeatEveryTimes,@ReturnDate),datepart(dw,@startDate))
										set @ReturnDate = dateadd(week,1,@ReturnDate)
									end
								if @endAfterTimes is not null and (DATEDIFF(month, @startDate, @ReturnDate)/@repeatEveryTimes) > (@endAfterTimes-1)
									return null								
							end
						else if @frequencyDetailInt = 13
							begin 
								if @Remains>0
									set @Times = @Times+1
								set @ReturnDate = dbo.GetFirstOfDayOfWeekInMonth(dateadd(month,@Times*@repeatEveryTimes,@startDate),datepart(dw,@startDate))
								set @ReturnDate = dateadd(week,2,@ReturnDate)
								if @onOrAfterDate > @ReturnDate
									begin
										set @ReturnDate = dbo.GetFirstOfDayOfWeekInMonth(dateadd(month,@repeatEveryTimes,@ReturnDate),datepart(dw,@startDate))
										set @ReturnDate = dateadd(week,2,@ReturnDate)
									end
								if @endAfterTimes is not null and DATEDIFF(month, @startDate, @ReturnDate)/@repeatEveryTimes > @endAfterTimes-1
									return null								
							end
						else if @frequencyDetailInt = 14
							begin
								if @Remains>0
									set @Times = @Times+1
								set @ReturnDate = dbo.GetLastOfDayOfWeekInMonth(dateadd(month,@Times*@repeatEveryTimes,@startDate),datepart(dw,@startDate))
								if @onOrAfterDate > @ReturnDate
									set @ReturnDate = dbo.GetLastOfDayOfWeekInMonth(dateadd(month,@repeatEveryTimes,@ReturnDate),datepart(dw,@startDate))
								if @endAfterTimes is not null and DATEDIFF(month, @startDate, @ReturnDate)/@repeatEveryTimes > @endAfterTimes-1
									return null								
							end
						else if @frequencyDetailInt = 8
							begin
								if day(@startdate) in (29,30,31)
									set @ReturnDate = dbo.GetNextDayInMonth(@startdate,@onOrAfterDate,@repeatEveryTimes,@endAfterTimes)
								else
									begin
										if @Remains>0
											set @Times = @Times+1						
										set @ReturnDate = dateadd(month,@Times*@repeatEveryTimes,@startDate)
										if @onOrAfterDate > @ReturnDate
											set @ReturnDate = dateadd(month,@repeatEveryTimes,@ReturnDate)
										if @endAfterTimes is not null and DATEDIFF(month, @startDate, @ReturnDate)/@repeatEveryTimes > @endAfterTimes-1
											return null	
									end							
							end
					end
			end		
		else if @frequencyInt = 5
			begin
				if @onOrAfterDate <= @startDate
					set @ReturnDate = @startDate
				--else if @endAfterTimes is not null and @onOrAfterDate > dateadd(year,(@endAfterTimes -1)*@repeatEveryTimes,@startDate)
				--	return null
				else 
					begin
						DECLARE @tmpdate datetime,@years int, @months int, @days int,@TimesInt int
						set @tmpdate = @startDate
						SELECT @years = DATEDIFF(yy, @tmpdate, @onOrAfterDate) - CASE WHEN (MONTH(@startDate) > MONTH(@onOrAfterDate)) OR (MONTH(@startDate) = MONTH(@onOrAfterDate) AND DAY(@startDate) > DAY(@onOrAfterDate)) THEN 1 ELSE 0 END
						SELECT @tmpdate = DATEADD(yy, @years, @tmpdate)
						SELECT @months = DATEDIFF(m, @tmpdate, @onOrAfterDate) - CASE WHEN DAY(@startDate) > DAY(@onOrAfterDate) THEN 1 ELSE 0 END
						SELECT @tmpdate = DATEADD(m, @months, @tmpdate)
						SELECT @days = DATEDIFF(d, @tmpdate, @onOrAfterDate)
						if DAY(@startDate) = 29 and MONTH(@startDate) =2 and @repeatEveryTimes%4 > 0 
							begin
								if @repeatEveryTimes % 2 = 0
									set @repeatEveryTimes = @repeatEveryTimes*2
								else
									set @repeatEveryTimes = @repeatEveryTimes*4									
							end
						
						set @Remains = @years%@repeatEveryTimes
						set @TimesInt = @years/@repeatEveryTimes
						if @Remains = 0 and @months = 0 and @days=0
							set @returndate = @onOrAfterDate
						else
							begin								
								set @returndate = DATEADD(year, (@TimesInt+1)*@repeatEveryTimes, @startDate)
							end 
						if @endAfterTimes is not null and DATEDIFF(year, @startDate, @ReturnDate)/@repeatEveryTimes > @endAfterTimes-1 
							return null
						--return @returndate
					end
			end			
		if @endDate is not null and @ReturnDate > @endDate
			return null				
		return @ReturnDate
	end

