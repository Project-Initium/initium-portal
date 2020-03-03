CREATE VIEW [ReadAggregation].[vwRole]
AS
SELECT 
        r.Id
    ,   r.Name
    ,   count(rR.RoleId) as ResourceCount
FROM [AccessProtection].[Role] r
LEFT JOIN [AccessProtection].[RoleResource] rR
    ON rR.RoleId = r.Id
GROUP BY 
        r.Id
    ,   r.Name
