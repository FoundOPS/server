CREATE VIEW [dbo].[PartiesWithName]
AS
SELECT        dbo.Parties.Id, ISNULL(dbo.Parties_Person.LastName, '') + ' ' +  ISNULL(dbo.Parties_Person.FirstName, '') +' ' +  ISNULL(dbo.Parties_Person.MiddleInitial, '')  AS 'ChildName'
FROM            dbo.Parties INNER JOIN
                         dbo.Parties_Person ON dbo.Parties.Id = dbo.Parties_Person.Id
UNION
SELECT        dbo.Parties.Id, dbo.Parties_Business.Name AS 'ChildName'
FROM            dbo.Parties INNER JOIN
                         dbo.Parties_Business ON dbo.Parties.Id = dbo.Parties_Business.Id

GO