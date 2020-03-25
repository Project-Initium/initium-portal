CREATE PROCEDURE [Portal].[uspCheckForPresenceOfAnyUser]	
AS
BEGIN
	SELECT TOP 1 Id
	FROM [identity].[user]
END
