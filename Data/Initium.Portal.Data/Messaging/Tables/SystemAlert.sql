CREATE TABLE [Messaging].[SystemAlert]
(
	    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY
    ,   [Message] NVARCHAR(MAX) NOT NULL
    ,   [Type] INT NOT NULL
    ,   [WhenToShow] DATETIME2 NULL
    ,   [WhenToHide] DATETIME2 NULL
)
