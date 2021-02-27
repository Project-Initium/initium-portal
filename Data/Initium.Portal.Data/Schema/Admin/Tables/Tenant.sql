CREATE TABLE [Admin].[Tenant]
(
	    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY
    ,   [Identifier] NVARCHAR(MAX) NOT NULL
    ,   [Name] NVARCHAR(500) NOT NULL
    ,   [ConnectionString] NVARCHAR(MAX) NOT NULL
    ,   [WhenDisabled] DATETIME2 NULL
    ,   [EnabledFeatureJson] NVARCHAR(200) NULL
    ,   [SystemFeaturesJson] NVARCHAR(200) NULL
    ,   CONSTRAINT [UC_Tenant] UNIQUE ([Name])
)
