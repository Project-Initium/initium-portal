CREATE TABLE [Messaging].[SystemNotification]
(
	    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY
    ,   [Subject] NVARCHAR(MAX) NOT NULL
    ,   [Message] NVARCHAR(MAX) NOT NULL
    ,   [SystemNotificationTypeId] UNIQUEIDENTIFIER NOT NULL
    ,   [SerializedEventData] NVARCHAR(MAX) NOT NULL
    ,   [WhenNotified] DATETIME2 NOT NULL
    ,   [TenantId] UNIQUEIDENTIFIER NOT NULL
    ,   CONSTRAINT [FK_Notification_Tenant] FOREIGN KEY ([TenantId]) REFERENCES [Admin].[Tenant]([Id])
    ,   CONSTRAINT [FK_SystemNotification_SystemNotification] FOREIGN KEY ([SystemNotificationTypeId]) REFERENCES [Messaging].[SystemNotificationType]([Id])
)
GO

CREATE INDEX [IX_SystemNotification_TenantId] ON [Messaging].[SystemNotification] ([TenantId])

GO

CREATE INDEX [IX_SystemNotification_SystemNotificationTypeId] ON [Messaging].[SystemNotification] ([SystemNotificationTypeId])
