CREATE TABLE [Identity].[Profile]
(
	    [UserId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY
    ,   [FirstName] NVARCHAR(300) NULL
    ,   [LastName] NVARCHAR(300) NULL
    ,   [TenantId] UNIQUEIDENTIFIER NOT NULL
    ,   CONSTRAINT [FK_Profile_User] FOREIGN KEY ([UserId]) REFERENCES [Identity].[User]([Id])
    ,   CONSTRAINT [FK_Profile_Tenant] FOREIGN KEY ([TenantId]) REFERENCES [Admin].[Tenant]([Id])
)
GO

CREATE INDEX [IX_Profile_TenantId] ON [Identity].[Profile] ([TenantId])
