CREATE TABLE [Identity].[User]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [EmailAddress] NVARCHAR(320) NOT NULL, 
    [PasswordHash] VARCHAR(100) NOT NULL, 
    [WhenCreated] DATETIME2 NOT NULL,
    [WhenLastAuthenticated] DATETIME2 NULL
)
