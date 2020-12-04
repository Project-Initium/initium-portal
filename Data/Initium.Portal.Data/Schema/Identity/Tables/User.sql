CREATE TABLE [Identity].[User]
(
	    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY
    ,   [EmailAddress] NVARCHAR(320) NOT NULL
    ,   [PasswordHash] VARCHAR(100) NOT NULL
    ,   [WhenCreated] DATETIME2 NOT NULL
    ,   [WhenLastAuthenticated] DATETIME2 NULL
    ,   [IsLockable] BIT NOT NULL DEFAULT 1
    ,   [WhenLocked] DATETIME2 NULL
    ,   [AttemptsSinceLastAuthentication] INT NOT NULL DEFAULT 0
    ,   [SecurityStamp] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID()
    ,   [IsAdmin] BIT NOT NULL DEFAULT 0
    ,   [WhenVerified] DATETIME2 NULL
    ,   [WhenDisabled] DATETIME2 NULL
    ,   [TenantId] UNIQUEIDENTIFIER NOT NULL
    ,   CONSTRAINT [FK_User_Tenant] FOREIGN KEY ([TenantId]) REFERENCES [Admin].[Tenant]([Id])
)
GO

CREATE INDEX [IX_User_TenantId] ON [Identity].[User] ([TenantId])
