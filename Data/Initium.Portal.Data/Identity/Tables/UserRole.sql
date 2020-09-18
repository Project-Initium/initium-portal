CREATE TABLE [Identity].[UserRole]
(
	    [UserId] UNIQUEIDENTIFIER NOT NULL
    ,   [RoleId] UNIQUEIDENTIFIER NOT NULL
    ,   [TenantId] UNIQUEIDENTIFIER NOT NULL
    ,   PRIMARY KEY ([UserId], [RoleId])
    ,   CONSTRAINT [FK_UserRole_User] FOREIGN KEY ([UserId]) REFERENCES [Identity].[User]([Id])
    ,   CONSTRAINT [FK_UserRole_Role] FOREIGN KEY ([RoleId]) REFERENCES [AccessProtection].[Role]([Id])
)
