CREATE PROCEDURE [Portal].[uspCheckForPresenceOfAuthAppForCurrentUser]
	@userId UNIQUEIDENTIFIER
AS
BEGIN
	SELECT TOP 1 Id
	FROM [Identity].AuthenticatorApp
	WHERE
			UserId = @userId
		AND WhenRevoked IS NULL
END