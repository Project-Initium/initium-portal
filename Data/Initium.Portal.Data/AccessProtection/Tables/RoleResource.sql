CREATE TABLE [AccessProtection].[RoleResource]
(
	    [RoleId] UNIQUEIDENTIFIER NOT NULL
    ,   [ResourceId] UNIQUEIDENTIFIER NOT NULL
    ,   [TenantId] UNIQUEIDENTIFIER NOT NULL
    ,   PRIMARY KEY ([RoleId], [ResourceId])
    ,   CONSTRAINT [FK_RoleResource_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AccessProtection].[Role]([Id])
    ,   CONSTRAINT [FK_RoleResource_ResourceId] FOREIGN KEY ([ResourceId]) REFERENCES [AccessProtection].[Resource]([Id])
    ,   CONSTRAINT [FK_RoleResource_Tenant] FOREIGN KEY ([TenantId]) REFERENCES [Admin].[Tenant]([Id])
)
GO

CREATE INDEX [IX_RoleResource_TenantId] ON [AccessProtection].[RoleResource] ([TenantId])
