CREATE TABLE [Identity].[AuthenticatorDevice]
(
	    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY
    ,   [UserId] UNIQUEIDENTIFIER NOT NULL
    ,   [Name] NVARCHAR(200) NOT NULL
    ,   [WhenEnrolled] DATETIME2 NOT NULL
    ,   [WhenLastUsed] DATETIME2 NULL
    ,   [WhenRevoked] DATETIME2 NULL
    ,   [CredentialId] VARBINARY(MAX) NOT NULL
    ,   [PublicKey] VARBINARY(MAX) NOT NULL
    ,   [Aaguid] UNIQUEIDENTIFIER NOT NULL
    ,   [Counter] INT NOT NULL
    ,   [CredType] NVARCHAR(200) NOT NULL    
    ,   [TenantId] UNIQUEIDENTIFIER NOT NULL
    ,   CONSTRAINT [FK_AuthenticatorDevice_User] FOREIGN KEY ([UserId]) REFERENCES [Identity].[User]([Id])
    ,   CONSTRAINT [FK_AuthenticatorDevice_Tenant] FOREIGN KEY ([TenantId]) REFERENCES [Admin].[Tenant]([Id])
)
GO

CREATE INDEX [IX_AuthenticatorDevice_UserId] ON [Identity].[AuthenticatorDevice] ([UserId])

GO

CREATE INDEX [IX_AuthenticatorDevice_TenantId] ON [Identity].[AuthenticatorDevice] ([TenantId])
