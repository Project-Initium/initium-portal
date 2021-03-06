CREATE TABLE [Identity].[PasswordHistory]
(
	    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY
    ,   [Hash] VARCHAR(100) NOT NULL
    ,   [WhenUsed] DATETIME2 NOT NULL
    ,   [UserId] UNIQUEIDENTIFIER NOT NULL
    ,   [TenantId] UNIQUEIDENTIFIER NOT NULL
    ,   CONSTRAINT [FK_PasswordHistory_User] FOREIGN KEY ([UserId]) REFERENCES [Identity].[User]([Id])
    ,   CONSTRAINT [FK_PasswordHistory_Tenant] FOREIGN KEY ([TenantId]) REFERENCES [Admin].[Tenant]([Id])
)
GO

CREATE INDEX [IX_PasswordHistory_UserId] ON [Identity].[PasswordHistory] ([UserId])

GO

CREATE INDEX [IX_PasswordHistory_TenantId] ON [Identity].[PasswordHistory] ([TenantId])
