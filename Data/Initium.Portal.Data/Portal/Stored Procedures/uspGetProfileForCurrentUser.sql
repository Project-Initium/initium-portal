CREATE PROCEDURE [Portal].[uspGetProfileForCurrentUser]
	@userId UNIQUEIDENTIFIER
AS
BEGIN
	SELECT 
			FirstName
		,	LastName
	FROM [Identity].[Profile]
	WHERE UserId = @userId
END

