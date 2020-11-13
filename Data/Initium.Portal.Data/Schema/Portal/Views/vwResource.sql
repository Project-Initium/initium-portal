CREATE VIEW [Portal].[vwResource]
AS
SELECT 
        r.Id
    ,   r.Name
    ,   r.NormalizedName
    ,   r.ParentResourceId
FROM AccessProtection.Resource r;