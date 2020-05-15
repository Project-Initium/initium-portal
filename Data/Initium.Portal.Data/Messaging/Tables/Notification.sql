CREATE TABLE [Messaging].[Notification]
(
	    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY
    ,   [Subject] NVARCHAR(MAX) NOT NULL
    ,   [Message] NVARCHAR(MAX) NOT NULL
    ,   [Type] INT NOT NULL
    ,   [Event] INT NOT NULL
    ,   [WhenNotified] DATETIME2 NOT NULL
)