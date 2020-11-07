CREATE TABLE [Messaging].[SystemNotificationType]
(
		[Id]				    UNIQUEIDENTIFIER NOT NULL PRIMARY KEY
	,   [Name]				    NVARCHAR(100) NOT NULL
    ,   [NormalizedName]	    NVARCHAR(100) NOT NULL
    ,   [IsSubscribable]        BIT NOT NULL
    ,   [IsUserSubscribable]    BIT NOT NULL
)
