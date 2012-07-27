USE Core
GO

SELECT        Parties_Business.Name, Roles.Id AS 'RoleId', Parties_Business.Id AS 'Id'
FROM          Parties_Business INNER JOIN Roles ON Parties_Business.Id = Roles.OwnerPartyId
WHERE        (Parties_Business.Name = N'GotGrease?') AND Roles.Name = 'Administrator'