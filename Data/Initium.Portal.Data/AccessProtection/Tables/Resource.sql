CREATE TABLE [AccessProtection].[Resource]
(
	    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY
    ,   [Name] NVARCHAR(100) NOT NULL
    ,   [NormalizedName] NVARCHAR(100) NOT NULL
    ,   [ParentResourceId] UNIQUEIDENTIFIER NULL
    ,   CONSTRAINT [FK_Resource_Resource] FOREIGN KEY ([ParentResourceId]) REFERENCES [AccessProtection].[Resource]([Id])
)
