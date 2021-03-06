CREATE VIEW [Portal].[vwUserRole]
AS 
SELECT 
		ur.RoleId
	,	ur.UserId
	,	r.Name as RoleName
	,	ur.TenantId
FROM [Identity].[UserRole] ur
JOIN [AccessProtection].Role r
	ON Ur.RoleId = r.Id