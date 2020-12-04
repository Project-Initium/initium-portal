CREATE TABLE [AccessProtection].[Role]
(
		[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY
	,	[Name] NVARCHAR(100) NOT NULL
    ,   [TenantId] UNIQUEIDENTIFIER NOT NULL
    ,   CONSTRAINT [FK_Role_Tenant] FOREIGN KEY ([TenantId]) REFERENCES [Admin].[Tenant]([Id])
)
GO

CREATE INDEX [IX_Role_TenantId] ON [AccessProtection].[Role] ([TenantId])
