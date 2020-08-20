CREATE TABLE [AccessProtection].[RoleResource]
(
	    [RoleId] UNIQUEIDENTIFIER NOT NULL
    ,   [ResourceId] UNIQUEIDENTIFIER NOT NULL
    ,   PRIMARY KEY ([RoleId], [ResourceId])
    ,   CONSTRAINT [FK_RoleResource_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AccessProtection].[Role]([Id])
    ,   CONSTRAINT [FK_RoleResource_ResourceId] FOREIGN KEY ([ResourceId]) REFERENCES [AccessProtection].[Resource]([Id])
)
