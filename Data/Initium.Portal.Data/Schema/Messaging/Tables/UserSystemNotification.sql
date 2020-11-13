CREATE TABLE [Messaging].[UserSystemNotification]
(
	    [NotificationId] UNIQUEIDENTIFIER NOT NULL
    ,   [UserId] UNIQUEIDENTIFIER NOT NULL
    ,   [WhenViewed] DATETIME2 NULL
    ,   [WhenDismissed] DATETIME2 NULL
    ,   [TenantId] UNIQUEIDENTIFIER NOT NULL
    ,   PRIMARY KEY ([NotificationId], [UserId])
    ,   CONSTRAINT [FK_UserSystemNotification_Notification] FOREIGN KEY ([NotificationId]) REFERENCES [Messaging].[SystemNotification]([Id])
    ,   CONSTRAINT [FK_UserSystemNotification_User] FOREIGN KEY ([UserId]) REFERENCES [Identity].[User]([Id])
    ,   CONSTRAINT [FK_UserSystemNotification_Tenant] FOREIGN KEY ([TenantId]) REFERENCES [Admin].[Tenant]([Id])
)
GO

CREATE INDEX [IX_UserSystemNotification_TenantId] ON [Messaging].[UserSystemNotification] ([TenantId])
