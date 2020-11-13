CREATE TABLE [Admin].[TenantFeature]
(
	    [TenantId] UNIQUEIDENTIFIER NOT NULL
    ,   [FeatureId] UNIQUEIDENTIFIER NOT NULL
    ,   PRIMARY KEY ([TenantId], [FeatureId])
    ,   CONSTRAINT [FK_TenantFeature_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [Admin].[Tenant]([Id])
)

GO
