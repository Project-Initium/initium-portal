CREATE VIEW [Portal].[vwAuthenticatorApp]
AS
SELECT aa.Id,
       aa.UserId,
       aa.[Key],
       aa.WhenEnrolled
FROM [Identity].AuthenticatorApp aa
WHERE aa.WhenRevoked IS NULL;
