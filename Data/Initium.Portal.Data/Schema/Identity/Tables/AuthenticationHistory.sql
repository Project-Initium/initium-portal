CREATE TABLE [Identity].[AuthenticationHistory]
(
	    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY
	,   [UserId] UNIQUEIDENTIFIER NOT NULL
    ,   [WhenHappened] DATETIME2 NOT NULL 
    ,   [AuthenticationHistoryType] INT NOT NULL
    ,   [TenantId] UNIQUEIDENTIFIER NOT NULL 
    ,   CONSTRAINT [FK_AuthenticationHistory_Identity_User] FOREIGN KEY ([UserId]) REFERENCES [Identity].[User]([Id])
    ,   CONSTRAINT [FK_AuthenticationHistory_Tenant] FOREIGN KEY ([TenantId]) REFERENCES [Admin].[Tenant]([Id])
)

GO

CREATE INDEX [IX_AuthenticationHistory_UserId] ON [Identity].[AuthenticationHistory] ([UserId])
GO

CREATE INDEX [IX_AuthenticationHistory_TenantId] ON [Identity].[AuthenticationHistory] ([TenantId])
