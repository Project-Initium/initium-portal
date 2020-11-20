CREATE VIEW [Portal].[vwResource]
AS
SELECT 
        r.Id
    ,   r.Name
    ,   r.NormalizedName
    ,   r.ParentResourceId
    ,   r.FeatureCode
FROM AccessProtection.Resource r;