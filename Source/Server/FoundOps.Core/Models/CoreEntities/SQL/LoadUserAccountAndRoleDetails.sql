USE [Core]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[LoadUserAccountAndRoleDetails]
		(@emailAddress NVARCHAR(MAX))

	AS
	BEGIN
    
	SELECT ua.*, r.*, ba.Id, ba.MaxRoutes, ba.Name, ba.QuickBooksAccessToken, ba.QuickBooksAccessTokenSecret, ba.QuickBooksEnabled, ba.QuickBooksSessionXml, ba.RouteManifestSettings, b.* 
	FROM dbo.Parties_UserAccount ua
    INNER JOIN dbo.Roles r
    ON r.Id IN (
						    SELECT RoleMembership_Id FROM dbo.PartyRole 
						    WHERE MemberParties_Id = (SELECT Id FROM dbo.Parties_UserAccount WHERE EmailAddress = @emailAddress)
				        ) 
    AND ua.EmailAddress = @emailAddress
    INNER JOIN dbo.Parties_BusinessAccount ba
    ON r.OwnerBusinessAccountId = ba.Id
    INNER JOIN dbo.Blocks b
    ON b.Id IN (
						    SELECT Blocks_Id FROM dbo.RoleBlock 
						    WHERE Roles_Id IN (
											    SELECT RoleMembership_Id FROM dbo.PartyRole 
											    WHERE MemberParties_Id = (SELECT Id FROM dbo.Parties_UserAccount WHERE EmailAddress = @emailAddress)
										        ) 
						    AND r.Id = dbo.RoleBlock.Roles_Id
					    )
    AND r.Id IN (
						    SELECT RoleMembership_Id FROM dbo.PartyRole 
						    WHERE MemberParties_Id = (SELECT Id FROM dbo.Parties_UserAccount WHERE EmailAddress = @emailAddress)
					    )

	END
	RETURN

