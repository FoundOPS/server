
Create FUNCTION [dbo].[GetFirstDayOfMonth]
    (@Date datetime)       
    returns Datetime       
    as       
    begin  
		return dateadd(day,1-day(@Date),@date)   
    end