SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID(N'[dbo].[GetPreviousOccurrence]', N'FN') IS NOT NULL
DROP FUNCTION [dbo].[GetPreviousOccurrence]
GO
    Create FUNCTION [dbo].[GetPreviousOccurrence]
    (@OnOrBeforeDate datetime,
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
		
		--No meeting after end date
		if @endDate is not null and @OnOrBeforeDate > @endDate
			set @OnOrBeforeDate	 = @endDate
		-- No meeting before start date
		if @OnOrBeforeDate < @startDate
			return null
		
		if @frequencyInt = 1
			set @ReturnDate = @startDate
			
		else if @frequencyInt = 2
			begin	
				DECLARE @DateDif int			
				set @DateDif = DATEDIFF ( d , @startDate , @OnOrBeforeDate )
				if @endAfterTimes is not null and @OnOrBeforeDate >= dateadd(d,(@endAfterTimes -1)*@repeatEveryTimes,@startDate) 
					set @ReturnDate = dateadd(d,(@endAfterTimes -1)*@repeatEveryTimes,@startDate)					
				else
					begin
						declare @RemainDays int
						set @RemainDays = @DateDif%@repeatEveryTimes
						if @RemainDays = 0
							set @ReturnDate = @OnOrBeforeDate
						else
							set @ReturnDate = dateadd(d,0 - @RemainDays,@OnOrBeforeDate)
					end
			end
		
		else if @frequencyInt = 3
			begin			
				declare @DoW char(7),@MeetingInWeek int, @FirstDate int, @LastDate int, @Weeks int,@LastWeekRemains int,
						@TempDate datetime,@DoWOnOrBeforeDate int, @MeetingDate int,@dwStartDate int
				set @DoW = convert(char(7),@frequencyDetailInt)
				set @MeetingInWeek = len(@DoW)
				set @FirstDate = convert(int,substring(@DoW,1,1))
				set @LastDate = convert(int,substring(@DoW,@MeetingInWeek,1))
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
								set @TempDate = dateadd(week,(@Weeks - 1)*@repeatEveryTimes,@startDate)
								set @TempDate = dateadd(day,@LastDate - @FirstDate,@TempDate)
							end
						else
							begin								
								set @TempDate = dateadd(week,@Weeks*@repeatEveryTimes,@startDate)
								set @TempDate = dateadd(day,convert(int,substring(@DoW,@LastWeekRemains,1)) - @FirstDate,@TempDate)
							end
						if @OnOrBeforeDate >= @TempDate
							return @TempDate					
					end						
				set @Weeks = datediff(week,@startDate,@OnOrBeforeDate)
				if @Weeks%@repeatEveryTimes>0 --in the odd week, get the last meeting in previous week
					begin						
						set @ReturnDate = dateadd(week,((@Weeks/@repeatEveryTimes))*@repeatEveryTimes,@startDate)
						set @ReturnDate = dateadd(day,@LastDate - @FirstDate,@ReturnDate)
					end
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
						set @DoWOnOrBeforeDate = datepart(dw,@OnOrBeforeDate)
						
						select top 1 @MeetingDate = DoW from @WeekDay where Meeting = 1 and DoW <= @DoWOnOrBeforeDate order by DoW DESC
						if @MeetingDate is not null
							set @ReturnDate = dateadd(day, @MeetingDate - @DoWOnOrBeforeDate, @OnOrBeforeDate)
						else 
							begin								
								set @ReturnDate = dateadd(day, @LastDate - @DoWOnOrBeforeDate, @OnOrBeforeDate)	
								set @ReturnDate = dateadd(week, 0-@repeatEveryTimes, @ReturnDate)									
							end 
					end
					 
			end
			
		else if @frequencyInt = 4
			begin
				declare @Times int, @MonthDiff int
				set @MonthDiff = DATEDIFF(month, @startDate, @OnOrBeforeDate)
				set @Times = @MonthDiff/@repeatEveryTimes
				set @Remains= @MonthDiff%@repeatEveryTimes 								
				if @frequencyDetailInt = 10
					begin																			
						set @ReturnDate = dbo.GetLastDayOfMonth(dateadd(month,@Times*@repeatEveryTimes,@startDate))
						if @ReturnDate > @OnOrBeforeDate
							set @ReturnDate = dbo.GetLastDayOfMonth(dateadd(month,0-@repeatEveryTimes,@ReturnDate))
						if @endAfterTimes is not null and DATEDIFF(month, @startDate, @ReturnDate)/@repeatEveryTimes > @endAfterTimes-1
							set @ReturnDate = 	dbo.GetLastDayOfMonth(dateadd(month,(@endAfterTimes-1)*@repeatEveryTimes,@startDate))							
					end
				else if @frequencyDetailInt = 11
					begin
						set @ReturnDate = dbo.GetFirstOfDayOfWeekInMonth(dateadd(month,@Times*@repeatEveryTimes,@startDate),datepart(dw,@startDate))
						if @OnOrBeforeDate < @ReturnDate
							set @ReturnDate = dbo.GetFirstOfDayOfWeekInMonth(dateadd(month,0-@repeatEveryTimes,@ReturnDate),datepart(dw,@startDate))
						if @endAfterTimes is not null and DATEDIFF(month, @startDate, @ReturnDate)/@repeatEveryTimes > @endAfterTimes-1
							set @ReturnDate =	dbo.GetFirstOfDayOfWeekInMonth(dateadd(month,(@endAfterTimes-1)*@repeatEveryTimes,@startDate),datepart(dw,@startDate))							
					end
				else if @frequencyDetailInt = 12
					begin 
						set @ReturnDate = dbo.GetFirstOfDayOfWeekInMonth(dateadd(month,@Times*@repeatEveryTimes,@startDate),datepart(dw,@startDate))
						set @ReturnDate = dateadd(week,1,@ReturnDate)
						if @OnOrBeforeDate < @ReturnDate
							begin
								set @ReturnDate = dbo.GetFirstOfDayOfWeekInMonth(dateadd(month,0-@repeatEveryTimes,@ReturnDate),datepart(dw,@startDate))
								set @ReturnDate = dateadd(week,1,@ReturnDate)
							end
						if @endAfterTimes is not null and (DATEDIFF(month, @startDate, @ReturnDate)/@repeatEveryTimes) > (@endAfterTimes-1)
							begin
								set @ReturnDate =	dbo.GetFirstOfDayOfWeekInMonth(dateadd(month,(@endAfterTimes-1)*@repeatEveryTimes,@startDate),datepart(dw,@startDate))
								set @ReturnDate = dateadd(week,1,@ReturnDate)
							end														
					end
				else if @frequencyDetailInt = 13
					begin 
					set @ReturnDate = dbo.GetFirstOfDayOfWeekInMonth(dateadd(month,@Times*@repeatEveryTimes,@startDate),datepart(dw,@startDate))
						set @ReturnDate = dateadd(week,2,@ReturnDate)
						if @OnOrBeforeDate < @ReturnDate
							begin
								set @ReturnDate = dbo.GetFirstOfDayOfWeekInMonth(dateadd(month,0-@repeatEveryTimes,@ReturnDate),datepart(dw,@startDate))
								set @ReturnDate = dateadd(week,2,@ReturnDate)
							end
						if @endAfterTimes is not null and (DATEDIFF(month, @startDate, @ReturnDate)/@repeatEveryTimes) > (@endAfterTimes-1)
							begin
								set @ReturnDate =	dbo.GetFirstOfDayOfWeekInMonth(dateadd(month,(@endAfterTimes-1)*@repeatEveryTimes,@startDate),datepart(dw,@startDate))
								set @ReturnDate = dateadd(week,2,@ReturnDate)
							end							
					end
				else if @frequencyDetailInt = 14
					begin
						set @ReturnDate = dbo.GetLastOfDayOfWeekInMonth(dateadd(month,@Times*@repeatEveryTimes,@startDate),datepart(dw,@startDate))
						if @OnOrBeforeDate < @ReturnDate
							set @ReturnDate = dbo.GetLastOfDayOfWeekInMonth(dateadd(month,0-@repeatEveryTimes,@ReturnDate),datepart(dw,@startDate))
						if @endAfterTimes is not null and DATEDIFF(month, @startDate, @ReturnDate)/@repeatEveryTimes > @endAfterTimes-1
							set @ReturnDate = dbo.GetLastOfDayOfWeekInMonth(dateadd(month,(@endAfterTimes-1)*@repeatEveryTimes,@startDate),datepart(dw,@startDate))								
					end
				else if @frequencyDetailInt = 8
					begin
						if day(@startdate) in (29,30,31)
							set @ReturnDate = dbo.GetPreviousDayInMonth(@startdate,@OnOrBeforeDate,@repeatEveryTimes,@endAfterTimes)
						else
							begin						
								set @ReturnDate = dateadd(month,@Times*@repeatEveryTimes,@startDate)
								if @OnOrBeforeDate < @ReturnDate
									set @ReturnDate = dateadd(month,0-@repeatEveryTimes,@ReturnDate)
								if @endAfterTimes is not null and DATEDIFF(month, @startDate, @ReturnDate)/@repeatEveryTimes > @endAfterTimes-1
									set @ReturnDate = dateadd(month,(@endAfterTimes-1)*@repeatEveryTimes,@startDate)
							end							
					end
					
			end		
		else if @frequencyInt = 5
			begin
				DECLARE @tmpdate datetime,@years int, @months int, @days int,@TimesInt int
				set @tmpdate = @startDate
				SELECT @years = DATEDIFF(yy, @tmpdate, @OnOrBeforeDate) - CASE WHEN (MONTH(@startDate) > MONTH(@OnOrBeforeDate)) OR (MONTH(@startDate) = MONTH(@OnOrBeforeDate) AND DAY(@startDate) > DAY(@OnOrBeforeDate)) THEN 1 ELSE 0 END
				SELECT @tmpdate = DATEADD(yy, @years, @tmpdate)
				SELECT @months = DATEDIFF(m, @tmpdate, @OnOrBeforeDate) - CASE WHEN DAY(@startDate) > DAY(@OnOrBeforeDate) THEN 1 ELSE 0 END
				SELECT @tmpdate = DATEADD(m, @months, @tmpdate)
				SELECT @days = DATEDIFF(d, @tmpdate, @OnOrBeforeDate)
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
					set @returndate = @OnOrBeforeDate
				else
					begin								
						set @returndate = DATEADD(year, (@TimesInt)*@repeatEveryTimes, @startDate)
					end 
				if @endAfterTimes is not null and DATEDIFF(year, @startDate, @ReturnDate)/@repeatEveryTimes > @endAfterTimes-1 
					set @returndate = DATEADD(year, (@endAfterTimes-1)*@repeatEveryTimes, @startDate)
				--return @returndate
					
			end	
			
		return @ReturnDate
	end

GO
IF OBJECT_ID(N'[dbo].[GetPreviousDayInMonth]', N'FN') IS NOT NULL
DROP FUNCTION [dbo].[GetPreviousDayInMonth]
GO
Create FUNCTION [dbo].[GetPreviousDayInMonth]
    (@StarDate datetime,@EndDate datetime, @repeatEveryTimes int, @endAfterTime int)       
    returns Datetime       
    as       
    begin  
		declare @Times int,@TempDate datetime, @TimeOccur int, @ReturnDate datetime
		set @Times = 0
		set @TimeOccur = 0
		set @TempDate = @StarDate
		
		while @TempDate < @EndDate and (@endAfterTime is null or @TimeOccur < @endAfterTime)
			begin
				set @TempDate = dateadd(month,@repeatEveryTimes*@Times,@StarDate)
				if day(@StarDate) = day(@TempDate) and @TempDate <= @EndDate
					begin
						set @TimeOccur = @TimeOccur + 1
						set @ReturnDate = @TempDate
					end
				set @Times = @Times + 1
			end		
		return @ReturnDate
    end
    
 GO
