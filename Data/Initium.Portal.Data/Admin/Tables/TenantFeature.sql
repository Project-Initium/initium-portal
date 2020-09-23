CREATE TABLE [Admin].[TenantFeature]
(
	    [TenantId] UNIQUEIDENTIFIER NOT NULL
    ,   [FeatureId] UNIQUEIDENTIFIER NOT NULL
    ,   PRIMARY KEY ([TenantId], [FeatureId])
)
