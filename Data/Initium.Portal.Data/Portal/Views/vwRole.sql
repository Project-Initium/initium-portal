CREATE VIEW [Portal].[vwRole]
AS
SELECT 
        r.Id
    ,   r.Name
    ,   count(rR.RoleId) as ResourceCount
    ,   count(uR.RoleId) as UserCount
    ,   r.TenantId
FROM [AccessProtection].[Role] r
LEFT JOIN [AccessProtection].[RoleResource] rR
    ON rR.RoleId = r.Id
LEFT JOIN [Identity].[UserRole] uR
    ON uR.RoleId = r.Id
GROUP BY 
        r.Id
    ,   r.Name
    ,   r.TenantId
