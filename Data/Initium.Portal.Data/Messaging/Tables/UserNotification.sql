CREATE TABLE [Messaging].[UserNotification]
(
	    [NotificationId] UNIQUEIDENTIFIER NOT NULL
    ,   [UserId] UNIQUEIDENTIFIER NOT NULL
    ,   [WhenViewed] DATETIME2 NULL
    ,   [WhenDismissed] DATETIME2 NULL
    ,   [TenantId] UNIQUEIDENTIFIER NOT NULL
    ,   PRIMARY KEY ([NotificationId], [UserId])
    ,   CONSTRAINT [FK_UserNotification_Notification] FOREIGN KEY ([NotificationId]) REFERENCES [Messaging].[Notification]([Id])
    ,   CONSTRAINT [FK_UserNotification_User] FOREIGN KEY ([UserId]) REFERENCES [Identity].[User]([Id])
)