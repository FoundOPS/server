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