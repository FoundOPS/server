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