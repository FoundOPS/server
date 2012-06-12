SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
USE Core
GO
IF OBJECT_ID(N'[dbo].[DeleteUserAccountBasedOnId]', N'FN') IS NOT NULL
DROP PROCEDURE [dbo].[DeleteUserAccountBasedOnId]
GO
/****************************************************************************************************************************************************
* FUNCTION DeleteUserAccountBasedOnId will delete a UserAccount and all entities associated with it
* Follows the following progression to delete: Locations, Contacts, ContactInfoSet, Roles, Vehicles, Files, UserAccountLog and finally the UserAccount itself 
** Input Parameters **
* @providerId - The UserAccount Id to be deleted
***************************************************************************************************************************************************/
CREATE PROCEDURE dbo.DeleteUserAccountBasedOnId
		(@providerId uniqueidentifier)

	AS
	BEGIN

	DELETE FROM ContactInfoSet
	WHERE		PartyId = @providerId

	DELETE FROM Roles
	WHERE		OwnerPartyId = @providerId

	DELETE FROM Vehicles 
	WHERE		OwnerPartyId = @providerId

	DELETE FROM Files
	WHERE		PartyId = @providerId

	DELETE FROM UserAccountLog
	WHERE UserAccountId = @providerId

	DELETE FROM Parties_UserAccount
	WHERE Id = @providerId

	END
	RETURN