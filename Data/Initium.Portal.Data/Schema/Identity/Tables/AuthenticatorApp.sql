CREATE TABLE [Identity].[AuthenticatorApp]
(
	    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY
    ,   [Key] VARCHAR(500) NOT NULL
    ,   [WhenEnrolled] DATETIME2 NOT NULL
    ,   [WhenRevoked] DATETIME2 NULL
    ,   [UserId] UNIQUEIDENTIFIER NOT NULL
    ,   [TenantId] UNIQUEIDENTIFIER NOT NULL
    ,   CONSTRAINT [FK_AuthenticatorApp_User] FOREIGN KEY ([UserId]) REFERENCES [Identity].[User]([Id])
    ,   CONSTRAINT [FK_AuthenticatorApp_Tenant] FOREIGN KEY ([TenantId]) REFERENCES [Admin].[Tenant]([Id]) 
)
GO

CREATE INDEX [IX_AuthenticatorApp_UserId] ON [Identity].[AuthenticatorApp] ([UserId])

GO

CREATE INDEX [IX_AuthenticatorApp_TenantId] ON [Identity].[AuthenticatorApp] ([TenantId])
