CREATE TABLE [Identity].[SecurityTokenMapping]
(
		[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY
	,	[UserId] UNIQUEIDENTIFIER NOT NULL
	,	[Purpose] INT NOT NULL
	,	[WhenCreated] DATETIME2 NOT NULL
	,	[WhenExpires] DATETIME2 NOT NULL
	,	[WhenUsed] DATETIME2 NULL	
	,   [TenantId] UNIQUEIDENTIFIER NOT NULL
	,   CONSTRAINT [FK_SecurityTokenMapping_User] FOREIGN KEY ([UserId]) REFERENCES [Identity].[User]([Id])
	,	CONSTRAINT [FK_SecurityTokenMapping_Tenant] FOREIGN KEY ([TenantId]) REFERENCES [Admin].[Tenant]([Id])
)
GO

CREATE INDEX [IX_SecurityTokenMapping_UserId] ON [Identity].[SecurityTokenMapping] ([UserId])

GO

CREATE INDEX [IX_SecurityTokenMapping_TenantId] ON [Identity].[SecurityTokenMapping] ([TenantId])
