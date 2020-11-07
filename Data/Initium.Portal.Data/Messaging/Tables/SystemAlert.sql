CREATE TABLE [Messaging].[SystemAlert]
(
	    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY
    ,   [Name] NVARCHAR(MAX) NOT NULL DEFAULT ''
    ,   [Message] NVARCHAR(MAX) NOT NULL
    ,   [Type] INT NOT NULL
    ,   [WhenToShow] DATETIME2 NULL
    ,   [WhenToHide] DATETIME2 NULL
    ,   [TenantId] UNIQUEIDENTIFIER NOT NULL
    ,   CONSTRAINT [FK_SystemAlert_Tenant] FOREIGN KEY ([TenantId]) REFERENCES [Admin].[Tenant]([Id])
)
GO

CREATE INDEX [IX_SystemAlert_TenantId] ON [Messaging].[SystemAlert] ([TenantId])
