USE Core
GO

SELECT        Parties_Business.Name, Roles.Id AS 'RoleId'
FROM          Parties_Business INNER JOIN Roles ON Parties_Business.Id = Roles.OwnerPartyId
WHERE        (Parties_Business.Name = N'GotGrease?')