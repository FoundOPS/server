Create FUNCTION [dbo].[GetLastDayOfMonth]
    (@Date datetime)       
    returns Datetime       
    as       
    begin  
		return dateadd(day,-1,dateadd(month,1,dbo.GetFirstDayOfMonth(@date)))   
    end