USE Master
GO

IF EXISTS(SELECT 1 FROM master.dbo.sysdatabases WHERE name ='Core')
BEGIN
ALTER DATABASE Core
SET SINGLE_USER

DROP DATABASE Core
END

CREATE DATABASE Core
GO