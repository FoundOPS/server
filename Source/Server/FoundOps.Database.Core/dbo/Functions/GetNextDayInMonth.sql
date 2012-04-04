-- only use for check 29th, 30th, 31st with 8 = OnDayInMonth
Create FUNCTION [dbo].[GetNextDayInMonth]
    (@StarDate datetime,@EndDate datetime, @repeatEveryTimes int, @endAfterTime int)       
    returns Datetime       
    as       
    begin  
		declare @Times int,@TempDate datetime, @TimeOccur datetime
		set @Times = 1
		set @TimeOccur = 1
		set @TempDate = @StarDate
		
		while not (day(@StarDate) = day(@TempDate) and @TempDate > @EndDate)
			begin
				set @TempDate = dateadd(month,@repeatEveryTimes*@Times,@StarDate)
				if day(@StarDate) = day(@TempDate)
					set @TimeOccur = @TimeOccur + 1
				set @Times = @Times + 1
			end
		if @endAfterTime is not null and @TimeOccur > @endAfterTime
			return null
		return @TempDate
    end