CREATE TABLE [Messaging].[Notification]
(
	    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY
    ,   [Subject] NVARCHAR(MAX) NOT NULL
    ,   [Message] NVARCHAR(MAX) NOT NULL
    ,   [Type] INT NOT NULL
    ,   [SerializedEventData] NVARCHAR(MAX) NOT NULL
    ,   [WhenNotified] DATETIME2 NOT NULL
    ,   [TenantId] UNIQUEIDENTIFIER NOT NULL
    ,   CONSTRAINT [FK_Notification_Tenant] FOREIGN KEY ([TenantId]) REFERENCES [Admin].[Tenant]([Id])
)
GO

CREATE INDEX [IX_Notification_TenantId] ON [Messaging].[Notification] ([TenantId])