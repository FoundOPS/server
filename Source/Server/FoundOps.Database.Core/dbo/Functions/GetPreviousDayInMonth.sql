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