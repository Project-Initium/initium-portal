CREATE TABLE [AccessProtection].[Resource]
(
	    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY
    ,   [Name] NVARCHAR(100) NOT NULL
    ,   [NormalizedName] NVARCHAR(100) NOT NULL
    ,   [ParentResourceId] UNIQUEIDENTIFIER NULL
    ,   [FeatureCode] NVARCHAR(50) NULL
    ,   CONSTRAINT [FK_Resource_Resource] FOREIGN KEY ([ParentResourceId]) REFERENCES [AccessProtection].[Resource]([Id])
)
GO

CREATE INDEX [IX_Resource_ParentResourceId] ON [AccessProtection].[Resource] ([ParentResourceId])
