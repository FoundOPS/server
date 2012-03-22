Create FUNCTION [dbo].[GetFirstDayOfMonth]
    (@Date datetime)       
    returns Datetime       
    as       
    begin  
		return dateadd(day,1-day(@Date),@date)   
    end
GO
Create FUNCTION [dbo].[GetLastDayOfMonth]
    (@Date datetime)       
    returns Datetime       
    as       
    begin  
		return dateadd(day,-1,dateadd(month,1,dbo.GetFirstDayOfMonth(@date)))   
    end
Go
Create FUNCTION [dbo].[GetFirstOfDayOfWeekInMonth]
    (@Date datetime,@DoW int)       
    returns Datetime       
    as       
    begin  
		declare @FirstDate datetime, @DateDif int
		set @FirstDate = dbo.GetFirstDayOfMonth(@date)
		set @DateDif = @DoW - datepart(dw,@FirstDate)
		if @DateDif < 0
			set @DateDif = @DateDif + 7
		return dateadd(d,@DateDif,@FirstDate)
    end
Go
Create FUNCTION [dbo].[GetLastOfDayOfWeekInMonth]
    (@Date datetime,@DoW int)       
    returns Datetime       
    as       
    begin  
		declare @LastDate datetime, @DateDif int
		set @LastDate = dbo.GetLastDayOfMonth(@Date)
		set @DateDif = @DoW - datepart(dw,@LastDate)
		if @DateDif > 0
			set @DateDif = @DateDif - 7
		return dateadd(d,@DateDif,@LastDate)		
    end
go
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