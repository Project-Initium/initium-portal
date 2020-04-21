CREATE TABLE [Identity].[AuthenticatorDevice]
(
	    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY
    ,   [UserId] UNIQUEIDENTIFIER NOT NULL
    ,   [Name] NVARCHAR(200) NOT NULL
    ,   [WhenEnrolled] DATETIME2 NOT NULL
    ,   [WhenLastUsed] DATETIME2 NULL
    ,   [WhenRevoked] DATETIME2 NULL
    ,   [CredentialId] VARBINARY(MAX) NOT NULL
    ,   [PublicKey] VARBINARY(MAX) NOT NULL
    ,   [Aaguid] UNIQUEIDENTIFIER NOT NULL
    ,   [Counter] INT NOT NULL
    ,   [CredType] NVARCHAR(200) NOT NULL
    ,   CONSTRAINT [FK_AuthenticatorDevice_User] FOREIGN KEY ([UserId]) REFERENCES [Identity].[User]([Id])
)
