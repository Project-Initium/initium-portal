CREATE PROCEDURE [Portal].[uspCheckForPresenceOfUserByEmailAddress]
	@emailAddress nvarchar(320)
AS
BEGIN
	SELECT TOP 1 Id
	FROM [Identity].[User]
	WHERE EmailAddress = @emailAddress
END
