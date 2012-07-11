CREATE VIEW [dbo].[PartiesWithName]
AS
SELECT        dbo.Parties.Id, ISNULL(dbo.Parties_UserAccount.LastName, '') + ' ' +  ISNULL(dbo.Parties_UserAccount.FirstName, '') +' ' +  ISNULL(dbo.Parties_UserAccount.MiddleInitial, '')  AS 'ChildName'
FROM            dbo.Parties INNER JOIN
                         dbo.Parties_UserAccount ON dbo.Parties.Id = dbo.Parties_UserAccount.Id
UNION
SELECT        dbo.Parties.Id, dbo.Parties_BusinessAccount.Name AS 'ChildName'
FROM            dbo.Parties INNER JOIN
                         dbo.Parties_BusinessAccount ON dbo.Parties.Id = dbo.Parties_BusinessAccount.Id

GO