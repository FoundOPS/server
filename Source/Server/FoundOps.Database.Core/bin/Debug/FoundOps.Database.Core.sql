/*
Deployment script for FoundOps.Database.Core
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "FoundOps.Database.Core"
:setvar DefaultDataPath "C:\FoundOps\Agile5\Source-DEV\Server\FoundOps.Database.Core\Sandbox"
:setvar DefaultLogPath "C:\FoundOps\Agile5\Source-DEV\Server\FoundOps.Database.Core\Sandbox"

GO
USE [master];


GO
:on error exit
GO
USE [$(DatabaseName)];


GO
PRINT N'Update complete.'
GO
