CREATE VIEW [ReadAggregation].[vwUserRole]
AS 
SELECT 
		ur.RoleId
	,	ur.UserId
	,	r.Name as RoleName
FROM [Identity].[UserRole] ur
JOIN [AccessProtection].Role r
	ON Ur.RoleId = r.Id
